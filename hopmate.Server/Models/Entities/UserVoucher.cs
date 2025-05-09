using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class UserVoucher
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime? DateRedeemed { get; set; }

        public Guid IdUser { get; set; }
        public Guid IdVoucher { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Voucher? Voucher { get; set; }
    }
}
