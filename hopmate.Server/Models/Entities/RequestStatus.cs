using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class RequestStatus
    {
        [Key]
        public int Id { get; set; }

        public required string Status { get; set; }

        public ICollection<PassengerTrip> PassengerTrips { get; set; } = new List<PassengerTrip>();

        public static implicit operator RequestStatus(int v)
        {
            throw new NotImplementedException();
        }
    }
}