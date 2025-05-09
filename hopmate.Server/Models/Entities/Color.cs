using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class Color
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The color name cannot exceed 50 characters.")]
        public string Name { get; set; } = null!;

        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
