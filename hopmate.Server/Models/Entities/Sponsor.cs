using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class Sponsor
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
    }
}
