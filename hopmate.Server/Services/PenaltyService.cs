using hopmate.Server.Data;
using hopmate.Server.Models.Dto;
using hopmate.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace hopmate.Server.Services
{
    public class PenaltyService
    {
        private readonly ApplicationDbContext _context;

        public PenaltyService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Read
        public async Task<List<Penalty>> GetAllPenaltiesAsync()
        {
            var penalties = await _context.Penalties.ToListAsync();
            return penalties.Select(c => new Penalty
            {
                Id = c.Id,
                Hops = c.Hops,
                Points = c.Points,
                Description = c.Description
            }).ToList();
        }

        public async Task<PenaltyDto?> GetPenaltyByIdAsync(Guid id)
        {
            var penalty = await _context.Penalties.FindAsync(id);
            if (penalty == null)
                return null;

            return new PenaltyDto
            {
                Id = penalty.Id,
                Description = penalty.Description,
                Hops = penalty.Hops,
                Points = penalty.Points,
                IdUser = penalty.IdUser
            };
        }

        public async Task<List<PenaltyDto>?> GetPenaltiesByUserIdAsync(Guid userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!userExists)
            {
                return null;
            }

            return await _context.Penalties
            .Select(c => new PenaltyDto
            {
                Id = c.Id,
                Hops = c.Hops,
                Points = c.Points,
                Description = c.Description,
                IdUser = c.IdUser
            }).ToListAsync();
            }

        //Create
        public async Task<Penalty?> AddPenaltyAsync(PenaltyDto penaltyDto)
        {
            var user = await _context.Users
                .Include(u => u.Penalties) 
                .FirstOrDefaultAsync(u => u.Id == penaltyDto.IdUser);

            if (user == null)
                return null;

            var penalty = new Penalty
            {
                Id = Guid.NewGuid(),
                Hops = penaltyDto.Hops,
                Points = penaltyDto.Points,
                Description = penaltyDto.Description,
                IdUser = user.Id,
                User = user
            };

            _context.Penalties.Add(penalty);

            user.Penalties.Add(penalty);
            user.Hops -= penalty.Hops;
            user.Points -= penalty.Points;

            _context.Users.Update(user); 

            await _context.SaveChangesAsync();

            return penalty;
        }



        // Update
        public async Task<Penalty?> UpdatePenaltyAsync(Guid id, Penalty updatedPenalty)
        {
            var penalty = await _context.Penalties.FindAsync(id);
            if (penalty == null) return null;

            penalty.Description = updatedPenalty.Description;
            penalty.Hops = updatedPenalty.Hops;
            penalty.Points = updatedPenalty.Points;
            penalty.IdUser = updatedPenalty.IdUser;

            _context.Entry(penalty).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return penalty;
        }

        // Delete
        public async Task<bool> RemovePenaltyAsync(Guid id)
        {
            var penalty = await _context.Penalties.FindAsync(id);
            if (penalty == null) return false;

            _context.Penalties.Remove(penalty);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
