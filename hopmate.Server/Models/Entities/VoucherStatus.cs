using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class VoucherStatus
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Status { get; set; }

        public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
    }
}
