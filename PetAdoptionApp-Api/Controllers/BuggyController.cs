using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pet_Adoption_App.Data;
using Pet_Adoption_App.Models;

namespace Pet_Adoption_App.Controllers
{
    public class BuggyController : baseApiController
    {
        private readonly ApplicationDbContext _dBContext;

        public BuggyController(ApplicationDbContext dBContext)
        {
            _dBContext = dBContext;
        }

        [HttpGet("auth")]
        public ActionResult<string> GetAuth()
        {
            return "secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = _dBContext.Users.Find(-1);

            if (thing == null) return NotFound();

            return thing;
        }

        [HttpGet("server-error")]
        public ActionResult<AppUser> GetServerError()
        {
            var thing = _dBContext.Users.Find(-1) ?? throw new Exception("A bad thing has happened");

            return thing;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was not a good request");
        }
    }
}
