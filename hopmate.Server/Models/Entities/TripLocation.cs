using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hopmate.Server.Models.Entities
{
    public class TripLocation
    {
        [Key]
        public Guid Id { get; set; }

        public Guid IdTrip { get; set; }
        public Guid IdLocation { get; set; }

        public virtual Trip? Trip { get; set; }
        public virtual Location? Location { get; set; }
        public bool IsStart { get; set; }
    }
}
