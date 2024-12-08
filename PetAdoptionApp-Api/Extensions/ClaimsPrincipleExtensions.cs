using System.Security.Claims;

namespace Pet_Adoption_App.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user,string username)
        {
            //var username = user.FindFirstValue(ClaimTypes.NameIdentifier)
            //    ?? throw new Exception("Cannot get username from token");

            // var username = "lisa";
            return username;
        }
    }
}
