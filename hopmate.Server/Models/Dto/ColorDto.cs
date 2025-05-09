using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Dto
{
    public class ColorDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "The color name cannot exceed 50 characters.")]
        public string Name { get; set; } = null!;
    }
}
