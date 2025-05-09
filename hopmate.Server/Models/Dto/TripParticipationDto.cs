using System;

namespace hopmate.Server.Models.DTOs
{
    // Request models
    public class CreateParticipationRequestDto
    {
        public Guid TripId { get; set; }
        public string PickupAddress { get; set; }
        public string PostalCode { get; set; }
    }

    public class AcceptRequestDto
    {
        public Guid RequestId { get; set; }
    }

    public class RejectRequestDto
    {
        public Guid RequestId { get; set; }
        public string Reason { get; set; }
    }

    public class JoinWaitingListDto
    {
        public Guid TripId { get; set; }
        public string PickupAddress { get; set; }
        public string PostalCode { get; set; }
    }

    public class DriverTripDto
    {
        public Guid Id { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public string Status { get; set; }
        public int RequestCount { get; set; } // Number of pending requests
    }

    // Response models
    public class PassengerTripDto
    {
        public Guid Id { get; set; }
        public Guid PassengerId { get; set; }
        public string PassengerName { get; set; }
        public Guid TripId { get; set; }
        public string PickupLocation { get; set; }
        public Guid LocationId { get; set; }
        public string RequestStatus { get; set; }
        public int RequestStatusId { get; set; }
        public DateTime RequestDate { get; set; }
        public string Reason { get; set; }
    }

    public class TripAvailabilityDto
    {
        public Guid TripId { get; set; }
        public bool HasAvailableSeats { get; set; }
        public int AvailableSeats { get; set; }
        public int TotalSeats { get; set; }
    }

    // Backend DTO
    public class AvailableTripDto
    {
        public Guid Id { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public string Status { get; set; }
        public string DriverName { get; set; }
    }

    public class LocationDto
    {
        public Guid Id { get; set; }
        public string Address { get; set; } = string.Empty;
    }

    // Response model for detailed trip info
    public class TripDetailsDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset DtDeparture { get; set; }
        public DateTimeOffset DtArrival { get; set; }
        public int AvailableSeats { get; set; }
        public Guid DriverId { get; set; }
        public string DriverName { get; set; }
        public Guid VehicleId { get; set; }
        public string VehicleInfo { get; set; }
        public string TripStatus { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
    }

    // Model for notifications
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } // e.g. "RequestAccepted", "RequestRejected", "WaitingListUpdate"
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsRead { get; set; }
        public Guid UserId { get; set; }
        public Guid? RelatedEntityId { get; set; } // e.g. TripId or RequestId
    }
}