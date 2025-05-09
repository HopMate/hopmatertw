using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using hopmate.Server.Services;
using hopmate.Server.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Azure.Core;

namespace hopmate.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PassengerTripController : ControllerBase
    {
        private readonly TripParticipationService _tripParticipationService;
        private readonly NotificationService _notificationService;

        public PassengerTripController(
            TripParticipationService tripParticipationService,
            NotificationService notificationService)
        {
            _tripParticipationService = tripParticipationService;
            _notificationService = notificationService;
        }


        // GET: api/PassengerTrip/trips/{tripId}/availability
        [HttpGet("trips/{tripId}/availability")]
        public async Task<ActionResult<TripAvailabilityDto>> CheckTripAvailability(Guid tripId)
        {
            try
            {
                bool hasAvailableSeats = await _tripParticipationService.HasAvailableSeatsAsync(tripId);

                // For a real implementation, you'd want to get the actual available seats count
                // This is a simplified version
                var availability = new TripAvailabilityDto
                {
                    TripId = tripId,
                    HasAvailableSeats = hasAvailableSeats,
                    // These would be populated from the database in a real implementation
                    AvailableSeats = hasAvailableSeats ? 1 : 0,
                    TotalSeats = 1
                };

                return Ok(availability);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/PassengerTrip/request
        [HttpPost("request")]
        public async Task<ActionResult<PassengerTripDto>> CreateParticipationRequest(
            [FromBody] CreateParticipationRequestDto dto)
        {
            try
            {
                // Get the current user's ID
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Create the participation request
                var request = await _tripParticipationService.CreateParticipationRequestAsync(
                                dto.TripId,
                                userId,
                                dto.PickupAddress,
                                dto.PostalCode);

                // Map to DTO (in a real application you might use AutoMapper)
                var requestDto = new PassengerTripDto
                {
                    Id = request.Id,
                    PassengerId = request.IdPassenger,
                    TripId = request.IdTrip,
                    LocationId = request.IdLocation,
                    PickupLocation = request.LocationName, // Use LocationName directly
                    RequestStatusId = request.IdRequestStatus,
                    RequestDate = request.DateRequest,
                    Reason = request.Reason
                    // Other properties would be populated in a real implementation
                };

                return CreatedAtAction(nameof(GetRequest), new { id = request.Id }, requestDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/PassengerTrip/waitinglist
        [HttpPost("waitinglist")]
        public async Task<ActionResult<PassengerTripDto>> JoinWaitingList(JoinWaitingListDto dto)
        {
            try
            {
                // Get the current user's ID
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Add to waiting list
                var request = await _tripParticipationService.AddToWaitingListAsync(
                    dto.TripId, userId, dto.PickupAddress, dto.PostalCode);

                // Map to DTO
                var requestDto = new PassengerTripDto
                {
                    Id = request.Id,
                    PassengerId = request.IdPassenger,
                    TripId = request.IdTrip,
                    LocationId = request.IdLocation,
                    PickupLocation = request.LocationName, // Use LocationName directly
                    RequestStatusId = request.IdRequestStatus,
                    RequestDate = request.DateRequest,
                    Reason = request.Reason
                    // Other properties would be populated in a real implementation
                };

                return CreatedAtAction(nameof(GetRequest), new { id = request.Id }, requestDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/PassengerTrip/myrequests
        [HttpGet("myrequests")]
        public async Task<ActionResult<IEnumerable<PassengerTripDto>>> GetMyRequests()
        {
            try
            {
                // Get the current user's ID
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var requests = await _tripParticipationService.GetRequestsByPassengerAsync(userId);

                // Map to DTOs
                var requestDtos = requests.Select(r => new PassengerTripDto
                {
                    Id = r.Id,
                    PassengerId = r.IdPassenger,
                    TripId = r.IdTrip,
                    LocationId = r.IdLocation,
                    RequestStatusId = r.IdRequestStatus,
                    RequestStatus = r.RequestStatus?.Status,
                    RequestDate = r.DateRequest,
                    Reason = r.Reason,
                    PickupLocation = r.Location?.Address
                }).ToList();

                return Ok(requestDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/PassengerTrip/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PassengerTripDto>> GetRequest(Guid id)
        {
            try
            {
                var request = await _tripParticipationService.GetRequestByIdAsync(id);

                // Check if the request belongs to the current user
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (request.IdPassenger != userId)
                {
                    return Forbid();
                }

                // Map to DTO
                var requestDto = new PassengerTripDto
                {
                    Id = request.Id,
                    PassengerId = request.IdPassenger,
                    PassengerName = request.Passenger?.User?.Name,
                    TripId = request.IdTrip,
                    LocationId = request.IdLocation,
                    PickupLocation = request.Location?.Address,
                    RequestStatusId = request.IdRequestStatus,
                    RequestStatus = request.RequestStatus?.Status,
                    RequestDate = request.DateRequest,
                    Reason = request.Reason
                };

                return Ok(requestDto);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<AvailableTripDto>>> GetAvailableTrips()
        {
            try
            {
                var trips = await _tripParticipationService.GetAvailableTripsAsync();
                return Ok(trips);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/PassengerTrip/available/search
        [HttpGet("available/search")]
        public async Task<ActionResult<IEnumerable<AvailableTripDto>>> SearchAvailableTrips(
            [FromQuery] string origin,
            [FromQuery] string destination,
            [FromQuery] DateTime? date)
        {
            try
            {
                var trips = await _tripParticipationService.SearchAvailableTripsAsync(
                    origin, destination, date);
                return Ok(trips);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("cancel/{requestId}")]
        public async Task<ActionResult> CancelRequest(Guid requestId)
        {
            try
            {
                await _tripParticipationService.CancelPassengerTripAsync(requestId);
                return Ok(new { message = "Request cancelled successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Trip/{tripId}/locations
        [HttpGet("{tripId}/locations")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetTripLocations(Guid tripId)
        {
            try
            {
                var locations = await _tripParticipationService.GetTripLocationsAsync(tripId);
                return Ok(locations);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}