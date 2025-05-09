using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class PassengerTrip
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Passenger))]
        public Guid IdPassenger { get; set; }

        [ForeignKey(nameof(Trip))]
        public Guid IdTrip { get; set; }

        [ForeignKey(nameof(Location))]
        public Guid IdLocation { get; set; }

        [ForeignKey(nameof(RequestStatus))]
        public int IdRequestStatus { get; set; }

        public string LocationName { get; set; }

        public DateTime DateRequest { get; set; } = DateTime.UtcNow;

        public string? Reason { get; set; }

        public virtual Passenger? Passenger { get; set; }
        public virtual Trip? Trip { get; set; }
        public virtual Location? Location { get; set; }
        public virtual RequestStatus? RequestStatus { get; set; }
    }
}