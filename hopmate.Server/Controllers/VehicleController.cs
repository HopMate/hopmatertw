using hopmate.Server.Services;
using Microsoft.AspNetCore.Mvc;
using hopmate.Server.Models.Dto;
using hopmate.Server.Models.Entities;

namespace hopmate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly VehicleService _vehicleService;

        public VehicleController(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody] VehicleDto vehicleDTO)
        {
            try
            {
                var vehicle = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    Brand = vehicleDTO.Brand,
                    Model = vehicleDTO.Model,
                    Plate = vehicleDTO.Plate,
                    Seats = vehicleDTO.Seats,
                    ImageFilePath = vehicleDTO.ImageFilePath,
                    IdDriver = vehicleDTO.IdDriver,
                    IdColor = vehicleDTO.IdColor
                };

                await _vehicleService.CreateVehicleAsync(vehicle);

                return CreatedAtAction(nameof(GetVehicles), new { id = vehicle.Id }, vehicle);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(Guid id, [FromBody] VehicleDto vehicleDTO)
        {
            var updatedVehicle = await _vehicleService.UpdateVehicleAsync(id, new Vehicle
            {
                Id = id,
                Brand = vehicleDTO.Brand,
                Model = vehicleDTO.Model,
                Plate = vehicleDTO.Plate,
                Seats = vehicleDTO.Seats,
                ImageFilePath = vehicleDTO.ImageFilePath,
                IdDriver = vehicleDTO.IdDriver,
                IdColor = vehicleDTO.IdColor
            });

            if (updatedVehicle == null)
            {
                return NotFound();
            }

            return Ok(updatedVehicle);
        }

        [HttpGet]
        public async Task<ActionResult<List<Vehicle>>> GetVehicles()
        {
            return await _vehicleService.GetVehiclesAsync();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(Guid id)
        {
            var result = await _vehicleService.DeleteVehicleAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}