using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class Driver
    {
        [Key, ForeignKey(nameof(User))]
        public Guid IdUser { get; set; }

        [Required]
        public required ApplicationUser User { get; set; }

        [Required]
        public string DrivingLicense { get; set; } = string.Empty;

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public virtual ICollection<Reward> Rewards { get; set; } = new List<Reward>();

        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}