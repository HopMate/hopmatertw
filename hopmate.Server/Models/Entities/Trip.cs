using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hopmate.Server.Models.Entities
{
    public class Trip
    {
        [Key]
        public Guid Id { get; set; }

        public DateTimeOffset DtDeparture { get; set; }
        public DateTimeOffset DtArrival { get; set; }
        public int AvailableSeats { get; set; }

        public Guid IdDriver { get; set; }
        public Guid IdVehicle { get; set; }
        public int IdStatusTrip { get; set; }

        public virtual Driver? Driver { get; set; }
        public virtual Vehicle? Vehicle { get; set; }
        public virtual TripStatus? TripStatus { get; set; }

        public ICollection<PassengerTrip> PassengerTrips { get; set; } = new List<PassengerTrip>();
        public ICollection<TripLocation> TripLocations { get; set; } = new List<TripLocation>();
    }
}
