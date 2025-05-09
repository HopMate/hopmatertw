using Microsoft.AspNetCore.Mvc;
using hopmate.Server.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using hopmate.Server.Models.Dto;
using hopmate.Server.Models.Entities;

namespace hopmate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DriverController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Driver>> RegisterDriver([FromBody] RegisterDriverDto registerDriverDto)
        {
            var user = await _context.Users.FindAsync(registerDriverDto.IdUser);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var driver = new Driver
            {
                IdUser = user.Id,
                User = user,
                DrivingLicense = registerDriverDto.DrivingLicense
            };

            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDriver), new { id = driver.IdUser }, driver);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriver(Guid id)
        {
            var driver = await _context.Drivers.Include(d => d.User).FirstOrDefaultAsync(d => d.IdUser == id);

            if (driver == null)
            {
                return NotFound();
            }

            return driver;
        }
    }
}
