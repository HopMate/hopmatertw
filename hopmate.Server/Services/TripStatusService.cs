using hopmate.Server.Data;
using hopmate.Server.DTOs;
using hopmate.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace hopmate.Server.Services
{
    public class TripStatusService
    {
        private readonly ApplicationDbContext _context;

        public TripStatusService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TripStatusDto>> GetAllAsync()
        {
            var statuses = await _context.TripStatuses.ToListAsync();
            return statuses.Select(ts => new TripStatusDto
            {
                Status = ts.Status
            }).ToList();
        }

        public async Task<TripStatusDto?> GetByIdAsync(int id)
        {
            var status = await _context.TripStatuses.FindAsync(id);
            if (status == null)
                return null;

            return new TripStatusDto
            {
                Status = status.Status
            };
        }

        public async Task<TripStatus> CreateAsync(TripStatusDto tripStatusDto)
        {
            var tripStatus = new TripStatus
            {
                Status = tripStatusDto.Status
            };
            _context.TripStatuses.Add(tripStatus);
            await _context.SaveChangesAsync();
            return tripStatus;
        }

        public async Task<TripStatus?> UpdateAsync(int id, TripStatusDto updatedTripStatusDto)
        {
            var existingTripStatus = await _context.TripStatuses.FindAsync(id);
            if (existingTripStatus == null)
            {
                return null;
            }

            existingTripStatus.Status = updatedTripStatusDto.Status;

            await _context.SaveChangesAsync();
            return existingTripStatus;
        }

        // Deleta um Status de viagem
        public async Task<bool> DeleteAsync(int id)
        {
            var tripStatus = await _context.TripStatuses.FindAsync(id);
            if (tripStatus == null)
            {
                return false;
            }

            _context.TripStatuses.Remove(tripStatus);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
