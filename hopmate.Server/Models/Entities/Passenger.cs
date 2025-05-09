using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class Passenger
    {
        [Key, ForeignKey(nameof(ApplicationUser))]
        public Guid IdUser { get; set; }

        public virtual required ApplicationUser User { get; set; }

        public virtual ICollection<PassengerTrip> PassengerTrips { get; set; } = new List<PassengerTrip>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}