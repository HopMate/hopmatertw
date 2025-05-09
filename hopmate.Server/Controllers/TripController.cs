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
        public TripController(TripService tripService, PenaltyService penaltyService)
        {
            _tripService = tripService;
            _penaltyService = penaltyService;
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

        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelTripDriver(Guid id)
        {
            var trip = await _tripService.GetTripAsync(id);
            if (trip == null)
                return NotFound("Trip not found.");

            if (trip.IdStatusTrip == 4)
                return Content("Trip already cancelled");

            int error = await _tripService.CancelTripAsync(id);
            if (error != 4)
                return BadRequest("Something went wrong, please try again.");

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

            var origin = await _tripService.GetLocationOrigin(trip.Id);
            var destination = await _tripService.GetLocationDestination(trip.Id);

            if (origin == null || destination == null)
                return BadRequest("Trip locations are invalid.");

            TripSimilarityRequestDto tripDto = new TripSimilarityRequestDto
            {
                Id = trip.Id,
                DateDeparture = trip.DtDeparture,
                DateArrival = trip.DtArrival,
                PostalOrigin = origin,
                PostalDestination = destination
            };

            return Ok(tripDto);
        }

        [HttpPost("searchsimilar")]
        public async Task<IActionResult> SearchSimilarTrips([FromBody] TripSimilarityRequestDto dto)
        {
            var tripDto = await _tripService.SearchSimilarTripsAsync(dto);
            if (tripDto == null)
                return NotFound();

            return Ok(tripDto);
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
