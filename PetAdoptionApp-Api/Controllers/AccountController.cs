using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Adoption_App.Data;
using Pet_Adoption_App.Dtos;
using Pet_Adoption_App.Interfaces;
using Pet_Adoption_App.Models;
using Pet_Adoption_App.Services;
using System.Security.Cryptography;
using System.Text;

namespace Pet_Adoption_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _dBContext;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(ApplicationDbContext dBContext,ITokenService tokenService,IMapper mapper) 
        {
           _dBContext = dBContext;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username Is taken ");
            using var hmac = new HMACSHA512();
            var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;
            //using var hmac = new HMACSHA512();

            //var user = new AppUser
            //{
            //    UserName = registerDto.Username.ToLower(),
            //    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),  
            //    PasswordSalt = hmac.Key
            //};
            //_dBContext.Users.Add(user);
            //await _dBContext.SaveChangesAsync();

            //return new UserDto
            //{
            //    Username = user.UserName,
            //    Token =  _tokenService.CreateToken(user),

            //};
            _dBContext.Users.Add(user);
            await _dBContext.SaveChangesAsync();
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };
        }
    
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>>Login(LoginDto loginDto)
        {
            var user = await _dBContext.Users
                       .Include(p => p.Photos)
                           .FirstOrDefaultAsync(x =>
                               x.UserName == loginDto.Username.ToLower()); if (user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i = 0; i < ComputedHash.Length;  i++)
            {
                if (ComputedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid Password");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
        }
        private async Task<bool> UserExists(string username)
        {
            return await _dBContext.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }

    }
}
