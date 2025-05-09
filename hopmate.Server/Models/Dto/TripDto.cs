using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Dto
{
    public class TripDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset DtDeparture { get; set; }

        public DateTimeOffset DtArrival { get; set; }

        [Required]
        public int AvailableSeats { get; set; }

        [Required]
        public Guid IdDriver { get; set; }

        [Required]
        public Guid IdVehicle { get; set; }

        [Required]
        public int IdStatusTrip { get; set; }
    }
}
