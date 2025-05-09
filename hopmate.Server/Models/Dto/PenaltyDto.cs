using hopmate.Server.Models.Entities;

namespace hopmate.Server.Models.Dto
{
    public class PenaltyDto
    {
        public Guid Id { get; set; }
        public int Hops { get; set; }
        public int Points { get; set; }
        public string? Description { get; set; }
        public Guid IdUser { get; set; }
    }
}
