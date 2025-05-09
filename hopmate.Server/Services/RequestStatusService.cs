using hopmate.Server.Data;
using hopmate.Server.Models.Dto;
using hopmate.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace hopmate.Server.Services
{
    public class RequestStatusService
    {
        private readonly ApplicationDbContext _context;
        private readonly TripService _tripService;

        public RequestStatusService(ApplicationDbContext context, TripService tripService)
        {
            _context = context;
            _tripService = tripService;
        }

        // Retorna todos os Status no formato DTO
        public async Task<List<RequestStatusDto>> GetAllAsync()
        {
            var statuses = await _context.RequestStatuses.ToListAsync();
            return statuses.Select(s => new RequestStatusDto
            {
                Status = s.Status
            }).ToList();
        }

        // Retorna um Status específico no formato DTO
        public async Task<RequestStatusDto?> GetByIdAsync(int id)
        {
            var status = await _context.RequestStatuses.FindAsync(id);
            if (status == null)
                return null;

            return new RequestStatusDto
            {
                Status = status.Status
            };
        }

        // Cria um novo Status e retorna a entidade RequestStatus
        public async Task<RequestStatus> CreateAsync(RequestStatusDto statusDto)
        {
            var status = new RequestStatus
            {
                Status = statusDto.Status
            };
            _context.RequestStatuses.Add(status);
            await _context.SaveChangesAsync();
            return status;
        }

        // Atualiza um Status existente e retorna a entidade RequestStatus
        public async Task<RequestStatus?> UpdateAsync(int id, RequestStatusDto updatedStatusDto)
        {
            var existing = await _context.RequestStatuses.FindAsync(id);
            if (existing == null) return null;

            existing.Status = updatedStatusDto.Status;

            _context.Entry(existing).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<int> ChangeStatus(Guid idTrip, Guid idUser, int status)
        {
            var trip = await _tripService.GetTripAsync(idTrip);

            if (trip == null)
                return 0;

            var trip_user = await _context.PassengerTrips
                .FirstOrDefaultAsync(t => t.IdTrip == idTrip && t.IdPassenger == idUser);

            if (trip_user == null)
                return 0;

            trip_user.IdRequestStatus = status;

            _context.Entry(trip_user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return status;
        }


        // Deleta um Status
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.RequestStatuses.FindAsync(id);
            if (existing == null) return false;

            _context.RequestStatuses.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}