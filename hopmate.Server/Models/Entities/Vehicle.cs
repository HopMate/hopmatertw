using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace hopmate.Server.Models.Entities
{
    public class Vehicle
    {
        [Key]
        public Guid Id { get; set; }

        public required string Brand { get; set; }
        public required string Model { get; set; }
        public required string Plate { get; set; }
        public required int Seats { get; set; }
        public string ImageFilePath { get; set; } = null!;

        public Guid IdDriver { get; set; }
        public int IdColor { get; set; }

        public virtual Color? Color { get; set; }
        [JsonIgnore]
        public virtual Driver? Driver { get; set; }
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
