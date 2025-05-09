using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class Review
    {
        [Key]
        public Guid Id { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public required string Comment { get; set; }

        public DateTime DateReview { get; set; } = DateTime.UtcNow;

        public Guid IdDriver { get; set; }
        public Guid IdPassenger { get; set; }

        public virtual Driver? Driver { get; set; }
        public virtual Passenger? Passenger { get; set; }
    }
}