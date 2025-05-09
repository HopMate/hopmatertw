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
        public async Task<TripDto?> SearchSimilarTripsAsync(TripSimilarityRequestDto dto)
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

            // Excluir a viagem original (dto.IdTrip) da pesquisa
            var trip = await _context.Trips
                .Where(t =>
                    t.Id != dto.Id && // 👈 Excluir a própria viagem
                    _context.TripLocations.Any(tl => tl.IdTrip == t.Id && tl.IsStart && originIds.Contains(tl.IdLocation)) &&
                    _context.TripLocations.Any(tl => tl.IdTrip == t.Id && !tl.IsStart && destinationIds.Contains(tl.IdLocation))
                )
                .Include(t => t.Driver).ThenInclude(d => d.User)
                .Include(t => t.Vehicle)
                .Include(t => t.TripStatus)
                .FirstOrDefaultAsync();

            if (trip == null)
                return null;

            return new TripDto
            {
                Id = trip.Id,
                DtDeparture = trip.DtDeparture,
                DtArrival = trip.DtArrival,
                AvailableSeats = trip.AvailableSeats,
                IdDriver = trip.IdDriver,
                IdVehicle = trip.IdVehicle,
                IdStatusTrip = trip.IdStatusTrip
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
