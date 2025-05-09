using hopmate.Server.Data;
using hopmate.Server.DTOs;
using hopmate.Server.Models.Dto;
using hopmate.Server.Models.Entities;
using hopmate.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hopmate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly TripService _tripService;
        private readonly PenaltyService _penaltyService;
        private readonly DriverService _driverService;
        private readonly RequestStatusService _requestStatus;

        public TripController(TripService tripService, PenaltyService penaltyService, DriverService driverService, RequestStatusService requestStatus)
        {
            _tripService = tripService;
            _penaltyService = penaltyService;
            _driverService = driverService;
            _requestStatus = requestStatus;
        }

        [HttpPost]
        public async Task<ActionResult<Trip>> CreateTrip([FromBody] TripDto tripDto)
        {
            if (tripDto == null)
            {
                return BadRequest("Trip data is invalid.");
            }

            var createdTrip = await _tripService.CreateTripAsync(tripDto);
            return CreatedAtAction(nameof(GetTrip), new { id = createdTrip.Id }, createdTrip);
        }

        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _tripService.GetTripsAsync();

            if (trips == null || !trips.Any())
                return NotFound("No trips found.");

            // Mapeia para DTOs se necessário — boa prática para evitar vazamento de dados sensíveis
            var tripDtos = trips.Select(t => new TripDto
            {
                Id = t.Id,
                DtDeparture = t.DtDeparture,
                DtArrival = t.DtArrival,
                AvailableSeats = t.AvailableSeats,
                IdDriver = t.IdDriver,
                IdVehicle = t.IdVehicle,
                IdStatusTrip = t.IdStatusTrip
            }).ToList();

            return Ok(tripDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Trip>> GetTrip(Guid id)
        {
            var trip = await _tripService.GetTripsAsync();
            var existingTrip = trip.Find(t => t.Id == id);

            if (existingTrip == null)
            {
                return NotFound("Trip not found.");
            }

            return Ok(existingTrip);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Trip>> UpdateTrip(Guid id, [FromBody] TripDto tripDto)
        {
            if (tripDto == null)
            {
                return BadRequest("Trip data is invalid.");
            }

            var updatedTrip = await _tripService.UpdateTripAsync(id, tripDto);

            if (updatedTrip == null)
            {
                return NotFound("Trip not found.");
            }

            return Ok(updatedTrip);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(Guid id)
        {
            var result = await _tripService.DeleteTripAsync(id);

            if (!result)
            {
                return NotFound("Trip not found.");
            }

            return NoContent();
        }

        // Controller corrigido
        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelTripDriver(Guid id, [FromBody] CancelTripDto dto)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Invalid trip ID.");
                }

                if (dto == null || dto.IdDriver == Guid.Empty)
                {
                    return BadRequest("Invalid or missing driver data.");
                }

                var trip = await _tripService.GetTripAsync(id);
                if (trip == null)
                {
                    return NotFound("Trip not found.");
                }

                bool isDriver = await _driverService.IsDriver(dto.IdDriver);
                if (!isDriver)
                {
                    return Unauthorized("User is not a driver.");
                }

                if (trip.IdDriver != dto.IdDriver)
                {
                    return Unauthorized("Only the responsible driver can cancel this trip.");
                }

                if (trip.IdStatusTrip == 4)
                {
                    return BadRequest("Trip is already cancelled.");
                }

                int status = await _tripService.CancelTripAsync(id);
                if (status != 4)
                {
                    return BadRequest("An error occurred while canceling the trip. Please try again.");
                }

                List<Guid> passengers = await _tripService.GetPassengerIdsAsync(id);
                if (!(passengers.Count > 0))
                    return Ok("Trip successfully cancelled!!");

                await _penaltyService.AddPenaltyAsync(new PenaltyDto
                {
                    IdUser = trip.IdDriver,
                    Hops = 10,
                    Points = 100,
                    Description = "Trip cancelled id:" + trip.Id
                });

                foreach (var passenger in passengers)
                {
                    var result = await _requestStatus.ChangeStatus(trip.Id, passenger, 5);
                    if (result != 5)
                    {
                        return StatusCode(500, $"Failed to update RequestStatus for passenger {passenger}. Expected status: 5, got: {result}");
                    }
                }

                return Ok("Trip successfully cancelled.");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error canceling trip: {ex.Message}");
                return StatusCode(500, $"Internal error processing request: {ex.Message}");
            }
        }

        [HttpPost("searchsimilar/{idTrip}/{idUser}")]
        public async Task<IActionResult> SearchSimilarTrips(Guid idTrip, Guid idUser)
        {
            var tripDto = await _tripService.GetTripSimilarityDataAsync(idTrip);
            if (tripDto == null)
                return NotFound("Não foi possível obter dados da viagem original.");

            var similarTrips = await _tripService.SearchSimilarTripsAsync(tripDto, idUser);
            if (similarTrips == null || !similarTrips.Any())
                return NotFound("Nenhuma viagem semelhante encontrada.");

            return Ok(similarTrips);
        }

        [HttpGet("driver/{driverId}")]
        public async Task<ActionResult<Driver>> GetDriver(Guid driverId)
        {
            var driver = await _tripService.GetDriverAsync(driverId);
            if (driver == null)
            {
                return NotFound("Driver not found.");
            }

            return Ok(driver);
        }

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<ActionResult<Vehicle>> GetVehicle(Guid vehicleId)
        {
            var vehicle = await _tripService.GetVehicleAsync(vehicleId);
            if (vehicle == null)
            {
                return NotFound("Vehicle not found.");
            }

            return Ok(vehicle);
        }

    }
}
