using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class Penalty
    {
        [Key]
        public Guid Id { get; set; }

        public int Hops { get; set; }
        public int Points { get; set; }
        public string? Description { get; set; }
        public Guid IdUser { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}