using hopmate.Server.Data;
using Microsoft.EntityFrameworkCore;
using hopmate.Server.Models.Dto;
using hopmate.Server.Models.Entities;

namespace hopmate.Server.Services
{
    public class TripService
    {
        private readonly ApplicationDbContext _context;

        public TripService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Trip> CreateTripAsync(TripDto tripDto)
        {
            var trip = new Trip
            {
                DtDeparture = tripDto.DtDeparture,
                DtArrival = tripDto.DtArrival,
                AvailableSeats = tripDto.AvailableSeats,
                IdDriver = tripDto.IdDriver,
                IdVehicle = tripDto.IdVehicle,
                IdStatusTrip = tripDto.IdStatusTrip
            };

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();
            return trip;
        }

        public async Task<List<Trip>> GetTripsAsync()
        {
            return await _context.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .Include(t => t.TripStatus)
                .ToListAsync();
        }

        public async Task<Trip?> GetTripAsync(Guid id)
        {
            var trip = await _context.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .Include(t => t.TripStatus)
                .FirstOrDefaultAsync(t => t.Id == id);
            if(trip != null)
                return trip;
            return null;
        }

        public async Task<List<Trip>> GetTripsByDriverIdAsync(Guid driverId)
        {
            return await _context.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .Include(t => t.TripStatus)
                .Where(t => t.IdDriver == driverId)
                .ToListAsync();
        }

        public async Task<List<Guid>> GetPassengerIdsAsync(Guid idTrip)
        {
            return await _context.PassengerTrips
                .Where(p => p.IdTrip == idTrip && (p.IdRequestStatus == 2 || p.IdRequestStatus == 1))
                .Select(p => p.IdPassenger)
                .ToListAsync();
        }

        public async Task<string> GetLocationOrigin(Guid idTrip)
        {
            var tripLocation = await _context.TripLocations
                .Where(a => a.IdTrip == idTrip && a.IsStart)
                .Select(a => a.IdLocation)
                .FirstOrDefaultAsync();

            if (tripLocation == Guid.Empty)
            {
                return string.Empty; 
            }

            var location = await _context.Locations
                .Where(l => l.Id == tripLocation)
                .Select(l => l.PostalCode)
                .FirstOrDefaultAsync();

            return location ?? string.Empty;
        }
        
        public async Task<string> GetLocationDestination(Guid idTrip)
        {
            var tripLocation = await _context.TripLocations
                .Where(a => a.IdTrip == idTrip && a.IsStart==false)
                .Select(a => a.IdLocation)
                .FirstOrDefaultAsync();

            if (tripLocation == Guid.Empty)
            {
                return string.Empty; 
            }

            var location = await _context.Locations
                .Where(l => l.Id == tripLocation)
                .Select(l => l.PostalCode)
                .FirstOrDefaultAsync();

            return location ?? string.Empty;
        }
        public async Task<List<TripSummaryDto>?> SearchSimilarTripsAsync(TripSimilarityRequestDto dto, Guid idPassenger)
        {
            // Obter os IDs de localização para origem e destino
            var originIds = await _context.Locations
                .Where(l => l.PostalCode == dto.PostalOrigin)
                .Select(l => l.Id)
                .ToListAsync();

            var destinationIds = await _context.Locations
                .Where(l => l.PostalCode == dto.PostalDestination)
                .Select(l => l.Id)
                .ToListAsync();

            if (!originIds.Any() || !destinationIds.Any())
                return null;

            // Obter os IDs das viagens onde o passageiro já está inscrito
            var tripsPassengerIsIn = await _context.PassengerTrips
                .Where(pt => pt.IdPassenger == idPassenger)
                .Select(pt => pt.IdTrip)
                .ToListAsync();

            // Data da viagem original + ou - 1 dia para busca
            var minDeparture = dto.DateDeparture.AddDays(-1);
            var maxDeparture = dto.DateDeparture.AddDays(1);

            // Procurar viagens semelhantes
            var similarTrips = await _context.Trips
                .Include(t => t.TripLocations).ThenInclude(tl => tl.Location)
                .Include(t => t.Driver).ThenInclude(d => d.User)
                .Include(t => t.Vehicle)
                .Include(t => t.TripStatus)
                .Where(t =>
                    t.Id != dto.Id &&
                    t.IdStatusTrip != 4 && // Excluir canceladas
                    !tripsPassengerIsIn.Contains(t.Id) && // Excluir onde o passageiro já está
                    t.DtDeparture >= minDeparture &&
                    t.DtDeparture <= maxDeparture &&
                    t.TripLocations.Any(tl => tl.IsStart && originIds.Contains(tl.IdLocation)) &&
                    t.TripLocations.Any(tl => !tl.IsStart && destinationIds.Contains(tl.IdLocation))
                )
                .ToListAsync();

            if (!similarTrips.Any())
                return null;

            // Converter para DTO com informações necessárias para o frontend
            var result = similarTrips.Select(trip => {
                var startLoc = trip.TripLocations.FirstOrDefault(tl => tl.IsStart)?.Location;
                var endLoc = trip.TripLocations.FirstOrDefault(tl => !tl.IsStart)?.Location;

                return new TripSummaryDto
                {
                    TripId = trip.Id,
                    DepartureTime = trip.DtDeparture,
                    ArrivalTime = trip.DtArrival,
                    AvailableSeats = trip.AvailableSeats,
                    DriverId = trip.IdDriver,
                    DriverName = trip.Driver?.User?.Name,
                    IdVehicle = trip.IdVehicle,
                    StatusTripId = trip.IdStatusTrip,
                    StartLocation = startLoc?.PostalCode,
                    EndLocation = endLoc?.PostalCode
                };
            }).ToList();

            return result;
        }

        public async Task<TripSimilarityRequestDto?> GetTripSimilarityDataAsync(Guid tripId)
        {
            var trip = await _context.Trips
                .Where(t => t.Id == tripId)
                .Include(t => t.TripLocations)
                .FirstOrDefaultAsync();

            if (trip == null)
                return null;

            var startLocation = trip.TripLocations.FirstOrDefault(tl => tl.IsStart);
            var endLocation = trip.TripLocations.FirstOrDefault(tl => !tl.IsStart);

            if (startLocation == null || endLocation == null)
                return null;

            var locationOrigin = await _context.Locations
                .Where(l => l.Id == startLocation.IdLocation)
                .FirstOrDefaultAsync();

            var locationDestination = await _context.Locations
                .Where(l => l.Id == endLocation.IdLocation)
                .FirstOrDefaultAsync();

            if (locationOrigin == null || locationDestination == null)
                return null;

            return new TripSimilarityRequestDto
            {
                Id = trip.Id,
                PostalOrigin = locationOrigin.PostalCode,
                PostalDestination = locationDestination.PostalCode,
                DateDeparture = trip.DtDeparture,
                DateArrival = trip.DtArrival
            };
        }

        public async Task<Trip?> UpdateTripAsync(Guid id, TripDto tripDto)
        {
            var existingTrip = await _context.Trips.FindAsync(id);
            if (existingTrip == null)
            {
                return null;
            }

            existingTrip.DtDeparture = tripDto.DtDeparture;
            existingTrip.DtArrival = tripDto.DtArrival;
            existingTrip.AvailableSeats = tripDto.AvailableSeats;
            existingTrip.IdDriver = tripDto.IdDriver;
            existingTrip.IdVehicle = tripDto.IdVehicle;
            existingTrip.IdStatusTrip = tripDto.IdStatusTrip;

            await _context.SaveChangesAsync();
            return existingTrip;
        }

        public async Task<bool> DeleteTripAsync(Guid id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return false;
            }

            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CancelTripAsync(Guid id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return 0;
            }
            trip.IdStatusTrip = 4;
            await _context.SaveChangesAsync();
            return 4;
        }

        public async Task<Driver?> GetDriverAsync(Guid driverId)
        {
            return await _context.Drivers.FindAsync(driverId);
        }

        public async Task<Vehicle?> GetVehicleAsync(Guid vehicleId)
        {
            return await _context.Vehicles.FindAsync(vehicleId);
        }

        public async Task<TripStatus?> GetTripStatusAsync(Guid statusId)
        {
            return await _context.TripStatuses.FindAsync(statusId);
        }
    }
}
