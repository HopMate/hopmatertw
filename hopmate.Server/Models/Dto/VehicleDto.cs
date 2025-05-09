using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Dto
{
    public class VehicleDto
    {
        [Required]
        public string Brand { get; set; } = null!;

        [Required]
        public string Model { get; set; } = null!;

        [Required]
        public string Plate { get; set; } = null!;

        [Required]
        public int Seats { get; set; }

        [Required]
        public string ImageFilePath { get; set; } = null!;

        [Required]
        public Guid IdDriver { get; set; }

        [Required]
        public int IdColor { get; set; }
    }

}
