using Pet_Adoption_App.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Pet_Adoption_App.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        public byte[]? PasswordHash { get; set; }


        public byte[]? PasswordSalt { get; set; }
        public DateOnly DateOfBirth { get; set; }
      
        [Required]

        public  string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        
         [Required]
         public  string Gender { get; set; }
        public string? Introduction { get; set; }
        public string? Interests { get; set; }
        public string? LookingFor { get; set; }
        [Required]
        public  string City { get; set; }
        [Required]
        public  string Country { get; set; }
        public List<Photo> Photos { get; set; } = [];

        public int GetAge()
        {
            return DateOfBirth.CalculateAge();
        }

    }
}
