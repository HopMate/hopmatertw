using hopmate.Server.Data;
using hopmate.Server.Models.DTOs;
using hopmate.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace hopmate.Server.Services
{
    public class TripParticipationService
    {
        private readonly ApplicationDbContext _context;

        public TripParticipationService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all trips for a specific driver
        /// </summary>
        /// <param name="driverId">The ID of the driver</param>
        /// <returns>List of DriverTripDto</returns>
        public async Task<List<DriverTripDto>> GetDriverTripsAsync(Guid driverId)
        {
            Console.WriteLine($"Fetching trips for driver: {driverId}");

            var trips = await _context.Trips
                .Where(t => t.IdDriver == driverId)
                .Include(t => t.TripStatus)
                .Include(t => t.PassengerTrips)
                .Include(t => t.TripLocations)
                    .ThenInclude(tl => tl.Location)
                .AsSplitQuery() // Helps with complex joins
                .ToListAsync();

            var tripDtos = new List<DriverTripDto>();

            Console.WriteLine($"Found {trips.Count} trips");

            foreach (var trip in trips)
            {
                var startLocation = trip.TripLocations?
                    .FirstOrDefault(tl => tl.IsStart)?.Location?.Address ?? "Unknown";

                var endLocation = trip.TripLocations?
                    .FirstOrDefault(tl => !tl.IsStart)?.Location?.Address ?? "Unknown";

                tripDtos.Add(new DriverTripDto
                {
                    Id = trip.Id,
                    Origin = startLocation,
                    Destination = endLocation,
                    DepartureTime = trip.DtDeparture.DateTime,
                    AvailableSeats = trip.AvailableSeats,
                    Status = trip.TripStatus?.Status ?? "Unknown",
                    RequestCount = trip.PassengerTrips.Count(pt => pt.IdRequestStatus == 1)
                });
            }

            return tripDtos;
        }

        /// <summary>
        /// Checks if a trip has available seats
        /// </summary>
        /// <param name="tripId">The ID of the trip to check</param>
        /// <returns>True if the trip has available seats, false otherwise</returns>
        public async Task<bool> HasAvailableSeatsAsync(Guid tripId)
        {
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null)
                throw new ArgumentException("Trip not found", nameof(tripId));

            // Agora a verificação é simples: apenas verificamos se AvailableSeats é maior que 0
            return trip.AvailableSeats > 0;
        }

        public async Task<PassengerTrip> CreateParticipationRequestAsync(
            Guid tripId,
            Guid passengerId,
            string locationName,
            string postalCode)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(locationName))
                throw new ArgumentException("Location name is required", nameof(locationName));

            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("Postal code is required", nameof(postalCode));

            // Check if trip exists
            var tripExists = await _context.Trips.AnyAsync(t => t.Id == tripId);
            if (!tripExists)
                throw new ArgumentException("Trip not found", nameof(tripId));

            // Check if passenger exists
            var passengerExists = await _context.Passengers.AnyAsync(p => p.IdUser == passengerId);
            if (!passengerExists)
                throw new ArgumentException("Passenger not found", nameof(passengerId));

            // Check for existing request
            if (await _context.PassengerTrips.AnyAsync(pt =>
                pt.IdPassenger == passengerId && pt.IdTrip == tripId))
                throw new InvalidOperationException("Passenger already has a request for this trip");

            // Find or create location
            var normalizedLocationName = locationName.Trim().ToLower();
            var normalizedPostalCode = postalCode.Trim().ToLower();

            var location = await _context.Locations
                .FirstOrDefaultAsync(l =>
                    l.Address.ToLower() == normalizedLocationName &&
                    l.PostalCode.ToLower() == normalizedPostalCode);

            if (location == null)
            {
                location = new Location
                {
                    Id = Guid.NewGuid(),
                    Address = locationName.Trim(),
                    PostalCode = postalCode.Trim(),
                    // Set other required properties
                };
                _context.Locations.Add(location);
            }

            // Create passenger trip
            var passengerTrip = new PassengerTrip
            {
                Id = Guid.NewGuid(),
                IdPassenger = passengerId,
                IdTrip = tripId,
                LocationName = locationName,
                IdLocation = location.Id,
                IdRequestStatus = 1, // Pending
                DateRequest = DateTime.UtcNow
            };

            _context.PassengerTrips.Add(passengerTrip);
            await _context.SaveChangesAsync();

            return passengerTrip;
        }

        public async Task<PassengerTrip> AddToWaitingListAsync(
            Guid tripId, Guid passengerId, string locationName, string postalCode)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(locationName))
                throw new ArgumentException("Location name is required", nameof(locationName));

            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("Postal code is required", nameof(postalCode));

            // Find or create location (same as CreateParticipationRequestAsync)
            var normalizedLocationName = locationName.Trim().ToLower();
            var normalizedPostalCode = postalCode.Trim().ToLower();

            var location = await _context.Locations
                .FirstOrDefaultAsync(l =>
                    l.Address.ToLower() == normalizedLocationName &&
                    l.PostalCode.ToLower() == normalizedPostalCode);

            if (location == null)
            {
                location = new Location
                {
                    Id = Guid.NewGuid(),
                    Address = locationName.Trim(),
                    PostalCode = postalCode.Trim()
                };
                _context.Locations.Add(location);
            }

            var passengerTrip = new PassengerTrip
            {
                Id = Guid.NewGuid(),
                IdPassenger = passengerId,
                IdTrip = tripId,
                LocationName = locationName,
                IdLocation = location.Id, // Associate with location
                IdRequestStatus = 4, // WaitingList status
                DateRequest = DateTime.UtcNow
            };

            _context.PassengerTrips.Add(passengerTrip);
            await _context.SaveChangesAsync();

            return passengerTrip;
        }

        /// <summary>
        /// Gets all participation requests for a trip
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <returns>List of PassengerTrip entities</returns>
        public async Task<List<PassengerTrip>> GetRequestsByTripAsync(Guid tripId)
        {
            return await _context.PassengerTrips
                .Include(pt => pt.Passenger)
                    .ThenInclude(p => p.User)
                .Include(pt => pt.Location)
                .Include(pt => pt.RequestStatus)
                .Where(pt => pt.IdTrip == tripId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all pending participation requests for a driver
        /// </summary>
        /// <param name="driverId">The ID of the driver</param>
        /// <returns>List of PassengerTrip entities</returns>
        public async Task<List<PassengerTrip>> GetPendingRequestsForDriverAsync(Guid driverId)
        {
            return await _context.PassengerTrips
                .Include(pt => pt.Passenger)
                    .ThenInclude(p => p.User)
                .Include(pt => pt.Location)
                .Include(pt => pt.RequestStatus)
                .Include(pt => pt.Trip)
                .Where(pt => pt.Trip.IdDriver == driverId && pt.IdRequestStatus == 1) // Pending status
                .ToListAsync();
        }

        /// <summary>
        /// Accepts a participation request
        /// </summary>
        /// <param name="requestId">The ID of the request</param>
        /// <returns>The updated PassengerTrip entity</returns>
        public async Task<PassengerTrip> AcceptRequestAsync(Guid requestId)
        {
            var request = await _context.PassengerTrips
                .Include(pt => pt.Trip)
                .FirstOrDefaultAsync(pt => pt.Id == requestId);

            if (request == null)
                throw new ArgumentException("Request not found", nameof(requestId));

            // Only pending requests can be accepted
            if (request.IdRequestStatus != 1)
                throw new InvalidOperationException("Only pending requests can be accepted");

            // Check if the trip still has available seats
            var trip = await _context.Trips.FindAsync(request.IdTrip);
            if (trip.AvailableSeats <= 0)
                throw new InvalidOperationException("The trip has no available seats");

            request.IdRequestStatus = 2; // Accepted status
            trip.AvailableSeats -= 1;

            await _context.SaveChangesAsync();

            // If there are other pending requests and no available seats,
            // move them to waiting list
            if (trip.AvailableSeats <= 0)
            {
                await MoveOtherPendingRequestsToWaitingListAsync(request.IdTrip, requestId);
            }

            return request;
        }

        /// <summary>
        /// Moves other pending requests to waiting list when trip becomes full
        /// </summary>
        private async Task MoveOtherPendingRequestsToWaitingListAsync(Guid tripId, Guid acceptedRequestId)
        {
            var pendingRequests = await _context.PassengerTrips
                .Where(pt => pt.IdTrip == tripId &&
                              pt.IdRequestStatus == 1 && // Pending
                              pt.Id != acceptedRequestId)
                .ToListAsync();

            foreach (var request in pendingRequests)
            {
                request.IdRequestStatus = 4; // WaitingList status
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Cancels a passenger trip and potentially moves waiting list passengers to pending
        /// </summary>
        /// <param name="requestId">The ID of the passenger trip request</param>
        /// <returns>The updated PassengerTrip entity</returns>
        public async Task<PassengerTrip> CancelPassengerTripAsync(Guid requestId)
        {
            var request = await _context.PassengerTrips
                .Include(pt => pt.Trip)
                .FirstOrDefaultAsync(pt => pt.Id == requestId);

            if (request == null)
                throw new ArgumentException("Request not found", nameof(requestId));

            // Can only cancel if it's currently accepted
            if (request.IdRequestStatus != 2) // Accepted
                throw new InvalidOperationException("Only accepted trips can be cancelled");

            request.IdRequestStatus = 5; // Cancelled status
            request.Trip.AvailableSeats += 1; // Free up the seat

            await _context.SaveChangesAsync();

            // Move the next waiting list passenger to pending
            await MoveNextWaitingListToPendingAsync(request.IdTrip);

            return request;
        }

        /// <summary>
        /// Moves the next passenger from waiting list to pending status when a seat becomes available
        /// </summary>
        private async Task MoveNextWaitingListToPendingAsync(Guid tripId)
        {
            var waitingListRequest = await _context.PassengerTrips
                .Where(pt => pt.IdTrip == tripId && pt.IdRequestStatus == 4) // WaitingList
                .OrderBy(pt => pt.DateRequest) // Take the first one (FIFO)
                .FirstOrDefaultAsync();

            if (waitingListRequest != null)
            {
                waitingListRequest.IdRequestStatus = 1; // Pending - requires driver approval
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Rejects a participation request
        /// </summary>
        /// <param name="requestId">The ID of the request</param>
        /// <param name="reason">The reason for rejection</param>
        /// <returns>The updated PassengerTrip entity</returns>
        public async Task<PassengerTrip> RejectRequestAsync(Guid requestId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Rejection reason is required", nameof(reason));

            var request = await _context.PassengerTrips
                .Include(pt => pt.Trip)
                .FirstOrDefaultAsync(pt => pt.Id == requestId);

            if (request == null)
                throw new ArgumentException("Request not found", nameof(requestId));

            // Só pode rejeitar se estiver pendente
            if (request.IdRequestStatus != 1)
                throw new InvalidOperationException("Only pending requests can be rejected");

            request.IdRequestStatus = 3; // Rejected status
            request.Reason = reason;

            await _context.SaveChangesAsync();

            return request;
        }

        /// <summary>
        /// Checks the waiting list for a trip and moves passengers if seats become available
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <returns>List of PassengerTrip entities that were moved from waiting list to pending</returns>
        public async Task<List<PassengerTrip>> CheckWaitingListAsync(Guid tripId)
        {
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null)
                throw new ArgumentException("Trip not found", nameof(tripId));

            if (trip.AvailableSeats <= 0)
                return new List<PassengerTrip>();

            // Get waiting list requests ordered by request date
            var waitingList = await _context.PassengerTrips
                .Where(pt => pt.IdTrip == tripId && pt.IdRequestStatus == 4) // 4 = WaitingList
                .OrderBy(pt => pt.DateRequest)
                .Take(trip.AvailableSeats) // Only take as many as there are available seats
                .ToListAsync();

            // Move them to pending (not directly accepted)
            foreach (var request in waitingList)
            {
                request.IdRequestStatus = 1; // 1 = Pending - requires driver approval
            }

            await _context.SaveChangesAsync();

            return waitingList;
        }

        /// <summary>
        /// Gets all requests for a passenger
        /// </summary>
        /// <param name="passengerId">The ID of the passenger</param>
        /// <returns>List of PassengerTrip entities</returns>
        public async Task<List<PassengerTrip>> GetRequestsByPassengerAsync(Guid passengerId)
        {
            return await _context.PassengerTrips
                .Include(pt => pt.Trip)
                .Include(pt => pt.Location)
                .Include(pt => pt.RequestStatus)
                .Where(pt => pt.IdPassenger == passengerId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a specific passenger trip request
        /// </summary>
        /// <param name="requestId">The ID of the request</param>
        /// <returns>The PassengerTrip entity</returns>
        public async Task<PassengerTrip> GetRequestByIdAsync(Guid requestId)
        {
            var request = await _context.PassengerTrips
                .Include(pt => pt.Trip)
                .Include(pt => pt.Passenger)
                    .ThenInclude(p => p.User)
                .Include(pt => pt.Location)
                .Include(pt => pt.RequestStatus)
                .FirstOrDefaultAsync(pt => pt.Id == requestId);

            if (request == null)
                throw new ArgumentException("Request not found", nameof(requestId));

            return request;
        }

        // Add to TripParticipationService.cs

        public async Task<List<AvailableTripDto>> GetAvailableTripsAsync()
        {
            var now = DateTime.UtcNow;

            var trips = await _context.Trips
                .Include(t => t.Driver)
                    .ThenInclude(d => d.User)
                .Include(t => t.TripStatus)
                .Include(t => t.TripLocations)
                    .ThenInclude(tl => tl.Location)
                .Where(t => t.DtDeparture > now &&
                           t.IdStatusTrip == 1) // Active status
                .OrderBy(t => t.DtDeparture)
                .ToListAsync();

            return trips.Select(t => new AvailableTripDto
            {
                Id = t.Id,
                Origin = t.TripLocations.FirstOrDefault(tl => tl.IsStart)?.Location?.Address ?? "Unknown",
                Destination = t.TripLocations.FirstOrDefault(tl => !tl.IsStart)?.Location?.Address ?? "Unknown",
                DepartureTime = t.DtDeparture.DateTime,
                AvailableSeats = t.AvailableSeats,
                Status = t.TripStatus?.Status ?? "Unknown",
                DriverName = t.Driver?.User?.Name ?? "Unknown"
            }).ToList();
        }

        public async Task<List<AvailableTripDto>> SearchAvailableTripsAsync(
            string origin, string destination, DateTime? date)
        {
            var query = _context.Trips
                .Include(t => t.Driver)
                    .ThenInclude(d => d.User)
                .Include(t => t.TripStatus)
                .Include(t => t.TripLocations)
                    .ThenInclude(tl => tl.Location)
                .Where(t => t.AvailableSeats > 0 &&
                           t.IdStatusTrip == 1); // Active status

            if (!string.IsNullOrEmpty(origin))
            {
                query = query.Where(t => t.TripLocations
                    .Any(tl => tl.IsStart && tl.Location.Address.Contains(origin)));
            }

            if (!string.IsNullOrEmpty(destination))
            {
                query = query.Where(t => t.TripLocations
                    .Any(tl => !tl.IsStart && tl.Location.Address.Contains(destination)));
            }

            if (date.HasValue)
            {
                var startDate = date.Value.Date;
                var endDate = startDate.AddDays(1);
                query = query.Where(t => t.DtDeparture >= startDate && t.DtDeparture < endDate);
            }

            var trips = await query
                .OrderBy(t => t.DtDeparture)
                .ToListAsync();

            return trips.Select(t => new AvailableTripDto
            {
                Id = t.Id,
                Origin = t.TripLocations.FirstOrDefault(tl => tl.IsStart)?.Location?.Address ?? "Unknown",
                Destination = t.TripLocations.FirstOrDefault(tl => !tl.IsStart)?.Location?.Address ?? "Unknown",
                DepartureTime = t.DtDeparture.DateTime,
                AvailableSeats = t.AvailableSeats,
                Status = t.TripStatus?.Status ?? "Unknown",
                DriverName = t.Driver?.User?.Name ?? "Unknown"
            }).ToList();
        }

        public async Task<List<LocationDto>> GetTripLocationsAsync(Guid tripId)
        {
            var trip = await _context.Trips
                .Include(t => t.TripLocations)
                    .ThenInclude(tl => tl.Location)
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null)
                throw new ArgumentException("Trip not found", nameof(tripId));

            return trip.TripLocations
                .Select(tl => new LocationDto
                {
                    Id = tl.IdLocation,
                    Address = tl.Location?.Address ?? "Unknown address"
                })
                .ToList();
        }
    }
}