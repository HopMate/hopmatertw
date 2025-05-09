using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Dto
{
    public class RegisterDriverDto
    {
        public Guid IdUser { get; set; }

        [Required]
        public required string DrivingLicense { get; set; }
    }
}
