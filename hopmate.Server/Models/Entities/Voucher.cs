using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class Voucher
    {
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }
        public int HopsCost { get; set; }
        public DateTime ExpiracyDate { get; set; }

        public Guid IdSponsor { get; set; }
        public Guid IdVoucherStatus { get; set; }
        public Guid IdImage { get; set; }

        public virtual Sponsor? Sponsor { get; set; }
        public virtual VoucherStatus? VoucherStatus { get; set; }
        public virtual Image? Image { get; set; }

        public virtual ICollection<UserVoucher> MemberVouchers { get; set; } = new List<UserVoucher>();
    }
}