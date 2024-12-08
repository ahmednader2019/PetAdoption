using System.ComponentModel.DataAnnotations;

namespace Pet_Adoption_App.Dtos
{
    public class LoginDto
    {

        [Required]
        public string Username { get; set; }

        [Required]

        public string Password { get; set; }
    }
}
