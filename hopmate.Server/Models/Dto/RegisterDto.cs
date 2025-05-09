using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Dto
{
    public class RegisterDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public bool HasDrivingLicense { get; set; }
    }
}
