using hopmate.Server.Data;
using Microsoft.EntityFrameworkCore;
using hopmate.Server.Models.Entities;

namespace hopmate.Server.Services
{
    public class VehicleService
    {
        private readonly ApplicationDbContext _context;

        public VehicleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateVehicleAsync(Vehicle vehicle)
        {
            var user = await _context.Users.FindAsync(vehicle.IdDriver);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d => d.IdUser == user.Id);

            if (driver == null)
            {
                throw new Exception("Driver não encontrado para o usuário.");
            }

            vehicle.IdDriver = driver.IdUser;

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Vehicle>> GetVehiclesAsync()
        {
            return await _context.Vehicles
                .Include(v => v.Driver)
                .Include(v => v.Color)
                .ToListAsync();
        }

        public async Task<Vehicle?> UpdateVehicleAsync(Guid id, Vehicle vehicle)
        {
            var existingVehicle = await _context.Vehicles.FindAsync(id);
            if (existingVehicle == null)
            {
                return null;
            }

            existingVehicle.Brand = vehicle.Brand;
            existingVehicle.Model = vehicle.Model;
            existingVehicle.Plate = vehicle.Plate;
            existingVehicle.Seats = vehicle.Seats;
            existingVehicle.ImageFilePath = vehicle.ImageFilePath;
            existingVehicle.IdDriver = vehicle.IdDriver;

            await _context.SaveChangesAsync();
            return existingVehicle;
        }

        public async Task<bool> DeleteVehicleAsync(Guid id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return false;
            }

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}