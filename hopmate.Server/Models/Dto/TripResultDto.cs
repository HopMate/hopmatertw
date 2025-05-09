namespace hopmate.Server.Models.Dto
{
    public class TripResultDto
    {
        public Guid Id { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DateDeparture { get; set; }
        public DateTime DateArrival { get; set; }
    }
}
