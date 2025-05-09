using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Entities
{
    public class Reward
    {
        [Key]
        public Guid Id { get; set; }

        public int Hops { get; set; }
        public int Points { get; set; }
        public string? Reason { get; set; }
        public DateTime Date { get; set; }

        public Guid IdDriver { get; set; }
        public virtual Driver? Driver { get; set; }
    }
}
