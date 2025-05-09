namespace hopmate.Server.Models.Dto
{
    public class TripSimilarityRequestDto
    {
        public Guid Id { get; set; }
        public required string PostalOrigin { get; set; }
        public required string PostalDestination { get; set; }
        public DateTimeOffset DateDeparture { get; set; }
        public DateTimeOffset DateArrival { get; set; }
    }
}
