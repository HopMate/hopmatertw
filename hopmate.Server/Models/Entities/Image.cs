using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class Image
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
    }
}