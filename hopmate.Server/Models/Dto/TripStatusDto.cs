using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.DTOs
{
    public class TripStatusDto
    {
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = null!;
    }
}