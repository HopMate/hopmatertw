//using hopmate.Server.Data;
//using hopmate.Server.Models.Dto;
//using hopmate.Server.Models.DTOs;
//using hopmate.Server.Models.Entities;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace hopmate.Server.Services
//{
//    public class TripExecutionService
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly NotificationService _notificationService;

//        public TripExecutionService(ApplicationDbContext context, NotificationService notificationService)
//        {
//            _context = context;
//            _notificationService = notificationService;
//        }

//        /// <summary>
//        /// Begins a trip execution
//        /// </summary>
//        /// <param name="tripId">The ID of the trip</param>
//        /// <returns>The updated Trip entity</returns>
//        public async Task<Trip> BeginTripAsync(Guid tripId)
//        {
//            var trip = await _context.Trips
//                .Include(t => t.TripStatus)
//                .FirstOrDefaultAsync(t => t.Id == tripId);

//            if (trip == null)
//                throw new ArgumentException("Trip not found", nameof(tripId));

//            // Check if trip is in correct state to begin (Active/1)
//            if (trip.IdStatusTrip != 1) // Active
//                throw new InvalidOperationException($"Trip cannot be started because its current status is '{trip.TripStatus?.Status}'");

//            // Update trip status to In Progress (2)
//            trip.IdStatusTrip = 2; // In Progress

//            await _context.SaveChangesAsync();

//            // Notify all accepted passengers that the trip has begun
//            var acceptedPassengers = await _context.PassengerTrips
//                .Where(pt => pt.IdTrip == tripId && pt.IdRequestStatus == 2) // Accepted status
//                .ToListAsync();

//            return trip;
//        }

//        /// <summary>
//        /// Ends a trip execution
//        /// </summary>
//        /// <param name="tripId">The ID of the trip</param>
//        /// <returns>The updated Trip entity</returns>
//        public async Task<Trip> EndTripAsync(Guid tripId)
//        {
//            var trip = await _context.Trips
//                .Include(t => t.TripStatus)
//                .FirstOrDefaultAsync(t => t.Id == tripId);

//            if (trip == null)
//                throw new ArgumentException("Trip not found", nameof(tripId));

//            // Check if trip is in correct state to end (In Progress/2)
//            if (trip.IdStatusTrip != 2) // In Progress
//                throw new InvalidOperationException($"Trip cannot be ended because its current status is '{trip.TripStatus?.Status}'");

//            // Update trip status to Completed (3)
//            trip.IdStatusTrip = 3; // Completed
//            trip.DtArrival = DateTime.UtcNow;

//            await _context.SaveChangesAsync();

//            // Send trip completion notifications
//            var acceptedPassengers = await _context.PassengerTrips
//                .Where(pt => pt.IdTrip == tripId &&
//                      (pt.IdRequestStatus == 2 || pt.IdRequestStatus == 6 || pt.IdRequestStatus == 7)) // Accepted, Present, or Absent
//                .ToListAsync();

//            return trip;
//        }

//        /// <summary>
//        /// Gets all accepted passengers for a trip
//        /// </summary>
//        /// <param name="tripId">The ID of the trip</param>
//        /// <returns>List of PassengerAttendanceDto</returns>
//        public async Task<List<PassengerAttendanceDto>> GetTripPassengersAsync(Guid tripId)
//        {
//            var trip = await _context.Trips
//                .FirstOrDefaultAsync(t => t.Id == tripId);

//            if (trip == null)
//                throw new ArgumentException("Trip not found", nameof(tripId));

//            // Get all accepted passengers for this trip
//            var passengers = await _context.PassengerTrips
//                .Include(pt => pt.Passenger)
//                    .ThenInclude(p => p.User)
//                .Include(pt => pt.Location)
//                .Where(pt => pt.IdTrip == tripId &&
//                      (pt.IdRequestStatus == 2 || pt.IdRequestStatus == 6 || pt.IdRequestStatus == 7)) // Accepted, Present, or Absent
//                .ToListAsync();

//            return passengers.Select(p => new PassengerAttendanceDto
//            {
//                RequestId = p.Id,
//                PassengerId = p.IdPassenger,
//                PassengerName = p.Passenger?.User?.Name ?? "Unknown passenger",
//                PickupLocation = p.Location?.Address ?? p.LocationName ?? "Unknown location",
//                AttendanceStatus = GetAttendanceStatusName(p.IdRequestStatus),
//                StatusId = p.IdRequestStatus
//            }).ToList();
//        }

//        /// <summary>
//        /// Marks a passenger as present for a trip
//        /// </summary>
//        /// <param name="requestId">The ID of the passenger trip request</param>
//        /// <returns>The updated PassengerTrip entity</returns>
//        public async Task<PassengerTrip> MarkPassengerPresentAsync(Guid requestId)
//        {
//            var passengerTrip = await _context.PassengerTrips
//                .Include(pt => pt.Trip)
//                .FirstOrDefaultAsync(pt => pt.Id == requestId);

//            if (passengerTrip == null)
//                throw new ArgumentException("Passenger request not found", nameof(requestId));

//            // Check if trip is in progress
//            if (passengerTrip.Trip.IdStatusTrip != 2) // In Progress
//                throw new InvalidOperationException("Attendance can only be marked for trips that are in progress");

//            // Only accepted passengers can be marked present
//            if (passengerTrip.IdRequestStatus != 2) // Accepted
//                throw new InvalidOperationException("Only accepted passengers can be marked as present");

//            passengerTrip.IdRequestStatus = 6; // Present

//            await _context.SaveChangesAsync();
//            return passengerTrip;
//        }

//        /// <summary>
//        /// Marks a passenger as absent for a trip
//        /// </summary>
//        /// <param name="requestId">The ID of the passenger trip request</param>
//        /// <returns>The updated PassengerTrip entity</returns>
//        public async Task<PassengerTrip> MarkPassengerAbsentAsync(Guid requestId)
//        {
//            var passengerTrip = await _context.PassengerTrips
//                .Include(pt => pt.Trip)
//                .FirstOrDefaultAsync(pt => pt.Id == requestId);

//            if (passengerTrip == null)
//                throw new ArgumentException("Passenger request not found", nameof(requestId));

//            // Check if trip is in progress
//            if (passengerTrip.Trip.IdStatusTrip != 2) // In Progress
//                throw new InvalidOperationException("Attendance can only be marked for trips that are in progress");

//            // Only accepted passengers can be marked absent
//            if (passengerTrip.IdRequestStatus != 2) // Accepted
//                throw new InvalidOperationException("Only accepted passengers can be marked as absent");

//            passengerTrip.IdRequestStatus = 7; // Absent (Fix: was 5 before, which was incorrect)

//            await _context.SaveChangesAsync();

//            return passengerTrip;
//        }

//        /// <summary>
//        /// Gets the trip execution status
//        /// </summary>
//        /// <param name="tripId">The ID of the trip</param>
//        /// <returns>TripExecutionStatusDto</returns>
//        public async Task<TripExecutionStatusDto> GetTripExecutionStatusAsync(Guid tripId)
//        {
//            var trip = await _context.Trips
//                .Include(t => t.TripStatus)
//                .Include(t => t.PassengerTrips)
//                .Include(t => t.TripLocations)
//                    .ThenInclude(tl => tl.Location)
//                .FirstOrDefaultAsync(t => t.Id == tripId);

//            if (trip == null)
//                throw new ArgumentException("Trip not found", nameof(tripId));

//            var startLocation = trip.TripLocations?
//                .FirstOrDefault(tl => tl.IsStart)?.Location?.Address ?? "Unknown";

//            var endLocation = trip.TripLocations?
//                .FirstOrDefault(tl => !tl.IsStart)?.Location?.Address ?? "Unknown";

//            // Fix: Check if PassengerTrips is null before using it
//            int acceptedPassengers = 0;
//            int presentPassengers = 0;
//            int absentPassengers = 0;

//            if (trip.PassengerTrips != null)
//            {
//                acceptedPassengers = trip.PassengerTrips.Count(pt => pt.IdRequestStatus == 2); // Accepted
//                presentPassengers = trip.PassengerTrips.Count(pt => pt.IdRequestStatus == 6);  // Present
//                absentPassengers = trip.PassengerTrips.Count(pt => pt.IdRequestStatus == 7);   // Absent
//            }

//            return new TripExecutionStatusDto
//            {
//                TripId = trip.Id,
//                Origin = startLocation,
//                Destination = endLocation,
//                StatusId = trip.IdStatusTrip,
//                Status = trip.TripStatus?.Status ?? "Unknown",
//                // Fix: Handle nullable DateTime properly
//                EndTime = trip.DtArrival.DateTime,
//                DepartureTime = trip.DtDeparture.DateTime,
//                TotalPassengers = acceptedPassengers + presentPassengers + absentPassengers,
//                PresentPassengers = presentPassengers,
//                AbsentPassengers = absentPassengers,
//                PendingAttendanceCount = acceptedPassengers
//            };
//        }

//        /// <summary>
//        /// Helper method to get attendance status name
//        /// </summary>
//        private string GetAttendanceStatusName(int statusId)
//        {
//            return statusId switch
//            {
//                2 => "Pending Attendance",
//                6 => "Present",
//                7 => "Absent",
//                _ => "Unknown"
//            };
//        }
//    }
//}