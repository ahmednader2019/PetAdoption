using Pet_Adoption_App.Models;

namespace Pet_Adoption_App.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
