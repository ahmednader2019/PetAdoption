using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Adoption_App.Data;
using Pet_Adoption_App.Dtos;
using Pet_Adoption_App.Extensions;
using Pet_Adoption_App.Interfaces;
using Pet_Adoption_App.Models;
using Pet_Adoption_App.Services;
using System.Security.Claims;

namespace Pet_Adoption_App.Controllers
{
    public class UserController : baseApiController
    {
        private readonly ApplicationDbContext _dBContext;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UserController(ApplicationDbContext dBContext, IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _dBContext = dBContext;
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();

            return Ok(users);
        }

        [HttpGet("{username}")]  // /api/users/2
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername(username));
            if (user == null) return NotFound();

            return Ok(user);

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null) return BadRequest("No username found in token");
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null) return BadRequest("Could not find user");
            _mapper.Map(memberUpdateDto, user);
            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update the user");
        }

        [HttpPost("add-photo/{username}/{photoId:int}")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file,int photoId,string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername(username));
           // var user = await _userRepository.GetUserByIdAsync(Id);
            if (user == null) return BadRequest("Cannot update user");
            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);
            if (await _userRepository.SaveAllAsync())
                return CreatedAtAction(nameof(GetUser),
                    new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            return BadRequest("Problem adding photo");

        }
        [HttpPut("set-main-photo/{username}/{photoId:int}")]
        public async Task<ActionResult> SetMainPhoto(int photoId,string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername(username));
            // var user = await _userRepository.GetUserByIdAsync(photoId);

            if (user == null) return BadRequest("Could not find user");
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null || photo.IsMain) return BadRequest("Cannot use this as main photo");
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Problem setting main photo");
        }
        [HttpDelete("delete-photo/{username}/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int photoId,string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername(username));
          //  var user = await _userRepository.GetUserByIdAsync(photoId);

            if (user == null) return BadRequest("User not found");
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null || photo.IsMain) return BadRequest("This photo cannot be deleted");
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if (await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Problem deleting photo");
        }
    }

}