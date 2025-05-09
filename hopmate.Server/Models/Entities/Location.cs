using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class Location
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "Código postal inválido.")]
        public string PostalCode { get; set; } = string.Empty;

        public virtual ICollection<TripLocation> TripLocations { get; set; } = new List<TripLocation>();
    }

}