using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class TripStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Status { get; set; }

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
