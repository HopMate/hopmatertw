using hopmate.Server.Data;
using hopmate.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hopmate.Server.Services
{
    // Since you mentioned not having a messaging platform, this service will serve as a placeholder
    // for handling notifications in your system. In a real implementation, this could integrate
    // with email, push notifications, or an in-app messaging system.
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Simulates sending a notification about a request acceptance
        /// </summary>
        /// <param name="requestId">The ID of the accepted request</param>
        /// <returns>Task</returns>
        public async Task SendAcceptanceNotificationAsync(Guid requestId)
        {
            var request = await _context.PassengerTrips
                .Include(pt => pt.Passenger)
                    .ThenInclude(p => p.User)
                .Include(pt => pt.Trip)
                    .ThenInclude(t => t.Driver)
                        .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(pt => pt.Id == requestId);

            if (request == null)
                throw new ArgumentException("Request not found", nameof(requestId));

            // In a real system, you would send an email or push notification here
            // For now, we'll just log the notification (you can implement actual notification logic later)

            Console.WriteLine($"[NOTIFICATION] Request Accepted: Driver {request.Trip.Driver.User.Name} " +
                $"has accepted the trip request from {request.Passenger.User.Name} for trip {request.IdTrip}");

            // This is where you would integrate with your messaging system
            // For example: await _emailService.SendEmailAsync(request.Passenger.User.Email, "Trip Request Accepted", emailBody);
        }

        /// <summary>
        /// Simulates sending a notification about a request rejection
        /// </summary>
        /// <param name="requestId">The ID of the rejected request</param>
        /// <returns>Task</returns>
        public async Task SendRejectionNotificationAsync(Guid requestId)
        {
            var request = await _context.PassengerTrips
                .Include(pt => pt.Passenger)
                    .ThenInclude(p => p.User)
                .Include(pt => pt.Trip)
                    .ThenInclude(t => t.Driver)
                        .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(pt => pt.Id == requestId);

            if (request == null)
                throw new ArgumentException("Request not found", nameof(requestId));

            // In a real system, you would send an email or push notification here
            // For now, we'll just log the notification

            Console.WriteLine($"[NOTIFICATION] Request Rejected: Driver {request.Trip.Driver.User.Name} " +
                $"has rejected the trip request from {request.Passenger.User.Name} for trip {request.IdTrip}. " +
                $"Reason: {request.Reason}");

            // This is where you would integrate with your messaging system
            // For example: await _emailService.SendEmailAsync(request.Passenger.User.Email, "Trip Request Rejected", emailBody);
        }

        /// <summary>
        /// Simulates sending a notification about a waiting list opportunity
        /// </summary>
        /// <param name="requestId">The ID of the request that has been moved from waiting list to pending</param>
        /// <returns>Task</returns>
        public async Task SendWaitingListNotificationAsync(Guid requestId)
        {
            var request = await _context.PassengerTrips
                .Include(pt => pt.Passenger)
                    .ThenInclude(p => p.User)
                .Include(pt => pt.Trip)
                .FirstOrDefaultAsync(pt => pt.Id == requestId);

            if (request == null)
                throw new ArgumentException("Request not found", nameof(requestId));

            // In a real system, you would send an email or push notification here
            // For now, we'll just log the notification

            Console.WriteLine($"[NOTIFICATION] Waiting List Update: A seat has become available for " +
                $"{request.Passenger.User.Name} on trip {request.IdTrip}. The request has been moved to pending status.");

            // This is where you would integrate with your messaging system
            // For example: await _emailService.SendEmailAsync(request.Passenger.User.Email, "Trip Seat Available", emailBody);
        }

        /// <summary>
        /// Simulates sending a trip booking confirmation
        /// </summary>
        /// <param name="requestId">The ID of the confirmed request</param>
        /// <returns>Task</returns>
        public async Task SendTripBookingConfirmationAsync(Guid requestId)
        {
            var request = await _context.PassengerTrips
                .Include(pt => pt.Passenger)
                    .ThenInclude(p => p.User)
                .Include(pt => pt.Trip)
                .Include(pt => pt.Location)
                .FirstOrDefaultAsync(pt => pt.Id == requestId);

            if (request == null)
                throw new ArgumentException("Request not found", nameof(requestId));

            // In a real system, you would send an email or push notification here with booking details
            // For now, we'll just log the notification

            Console.WriteLine($"[NOTIFICATION] Trip Booking Confirmation: {request.Passenger.User.Name} " +
                $"has been confirmed for trip {request.IdTrip}. Pickup location: {request.Location.Address}. " +
                $"Departure time: {request.Trip.DtDeparture}");

            // This is where you would integrate with your messaging system
            // For example: await _emailService.SendEmailAsync(request.Passenger.User.Email, "Trip Booking Confirmation", emailBody);
        }
    }
}