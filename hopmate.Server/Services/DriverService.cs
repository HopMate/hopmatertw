using hopmate.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace hopmate.Server.Services
{
    public class DriverService
    {
        private readonly ApplicationDbContext _context;

        public DriverService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsDriver(Guid id)
        {
            var exists = await _context.Drivers.AnyAsync(d => d.IdUser == id);

            return exists;
        }
    }
}
