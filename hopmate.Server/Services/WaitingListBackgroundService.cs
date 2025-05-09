using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hopmate.Server.Data;
using hopmate.Server.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace hopmate.Server.Services
{
    public class WaitingListBackgroundService : BackgroundService
    {
        private readonly ILogger<WaitingListBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Check every hour

        public WaitingListBackgroundService(
            ILogger<WaitingListBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Waiting List Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking waiting lists for upcoming trips...");

                try
                {
                    await CheckWaitingListsForUpcomingTrips();
                    // Also check for cancelled trips that might have freed up seats
                    await CheckForCancelledTrips();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking waiting lists.");
                }

                _logger.LogInformation("Waiting list check completed. Sleeping for {Interval}.", _checkInterval);

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Waiting List Background Service is stopping.");
        }

        private async Task CheckWaitingListsForUpcomingTrips()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var tripParticipationService = scope.ServiceProvider.GetRequiredService<TripParticipationService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

                // Get trips that are happening within the next 24 hours and have waitlisted passengers
                var now = DateTimeOffset.UtcNow;
                var cutoffTime = now.AddHours(24);

                var upcomingTrips = await context.Trips
                    .Where(t => t.DtDeparture <= cutoffTime && t.DtDeparture > now)
                    .Include(t => t.PassengerTrips.Where(pt => pt.IdRequestStatus == 4)) // 4 = WaitingList
                    .Where(t => t.PassengerTrips.Any())
                    .ToListAsync();

                _logger.LogInformation("Found {Count} upcoming trips with waitlisted passengers.", upcomingTrips.Count);

                foreach (var trip in upcomingTrips)
                {
                    _logger.LogInformation("Checking waiting list for trip {TripId}", trip.Id);

                    // Check if any seats have become available
                    var movedRequests = await tripParticipationService.CheckWaitingListAsync(trip.Id);

                    if (movedRequests.Any())
                    {
                        _logger.LogInformation(
                            "Moved {Count} passengers from waiting list to pending for trip {TripId}",
                            movedRequests.Count, trip.Id);
                    }
                }
            }
        }

        private async Task CheckForCancelledTrips()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var tripParticipationService = scope.ServiceProvider.GetRequiredService<TripParticipationService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

                // Get trips with available seats that have waiting list passengers
                var tripsWithAvailableSeats = await context.Trips
                    .Where(t => t.AvailableSeats > 0)
                    .Include(t => t.PassengerTrips.Where(pt => pt.IdRequestStatus == 4)) // 4 = WaitingList
                    .Where(t => t.PassengerTrips.Any())
                    .ToListAsync();

                _logger.LogInformation("Found {Count} trips with available seats and waiting list passengers",
                    tripsWithAvailableSeats.Count);

                foreach (var trip in tripsWithAvailableSeats)
                {
                    _logger.LogInformation("Processing waiting list for trip {TripId} with {Seats} available seats",
                        trip.Id, trip.AvailableSeats);

                    // Move waiting list passengers to pending
                    var movedRequests = await tripParticipationService.CheckWaitingListAsync(trip.Id);

                    if (movedRequests.Any())
                    {
                        _logger.LogInformation(
                            "Moved {Count} passengers from waiting list to pending for trip {TripId}",
                            movedRequests.Count, trip.Id);
                    }
                }
            }
        }
    }
}