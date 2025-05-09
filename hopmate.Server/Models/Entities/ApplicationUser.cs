using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace hopmate.Server.Models.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Range(0, int.MaxValue)]
        public int Points { get; set; }

        [Range(0, int.MaxValue)]
        public int Hops { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        public string? ImageFilePath { get; set; }

        [Required]
        public bool HasDrivingLicense { get; set; }

        public virtual ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();

        public virtual ICollection<UserVoucher> MemberVouchers { get; set; } = new List<UserVoucher>();
    }
}