using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using hopmate.Server.Services;
using hopmate.Server.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using hopmate.Server.Data;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace hopmate.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DriverTripController : ControllerBase
    {
        private readonly TripParticipationService _tripParticipationService;
        private readonly NotificationService _notificationService;
        private readonly ApplicationDbContext _context;

        public DriverTripController(
            TripParticipationService tripParticipationService,
            NotificationService notificationService,
            ApplicationDbContext context)
        {
            _tripParticipationService = tripParticipationService;
            _notificationService = notificationService;
            _context = context;
        }

        // Helper method to check if the current user is the driver of the trip
        private async Task<bool> IsDriverOfTrip(Guid tripId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == tripId);
            return trip != null && trip.IdDriver == userId;
        }

        // Add this method to your DriverTripController.cs

        // GET: api/DriverTrip/trips
        [HttpGet("trips")]
        public async Task<ActionResult<IEnumerable<DriverTripDto>>> GetDriverTrips()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var trips = await _tripParticipationService.GetDriverTripsAsync(userId);

                // Disable reference handling to prevent $id/$values format
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                return new JsonResult(trips, options);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/DriverTrip/pendingrequests
        [HttpGet("pendingrequests")]
        public async Task<ActionResult<IEnumerable<PassengerTripDto>>> GetPendingRequests()
        {
            try
            {
                // Get the current user's ID
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var pendingRequests = await _tripParticipationService.GetPendingRequestsForDriverAsync(userId);

                // Map to DTOs
                var requestDtos = pendingRequests.Select(r => new PassengerTripDto
                {
                    Id = r.Id,
                    PassengerId = r.IdPassenger,
                    PassengerName = r.Passenger?.User?.Name,
                    TripId = r.IdTrip,
                    LocationId = r.IdLocation,
                    PickupLocation = r.Location?.Address,
                    RequestStatusId = r.IdRequestStatus,
                    RequestStatus = r.RequestStatus?.Status,
                    RequestDate = r.DateRequest,
                    Reason = r.Reason
                }).ToList();

                return Ok(requestDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/DriverTrip/trips/{tripId}/requests
        [HttpGet("trips/{tripId}/requests")]
        public async Task<ActionResult<IEnumerable<PassengerTripDto>>> GetTripRequests(Guid tripId)
        {
            try
            {
                // Verify the current user is the driver of the trip
                if (!await IsDriverOfTrip(tripId))
                {
                    return Forbid();
                }

                var requests = await _tripParticipationService.GetRequestsByTripAsync(tripId);

                // Map to DTOs
                var requestDtos = requests.Select(r => new PassengerTripDto
                {
                    Id = r.Id,
                    PassengerId = r.IdPassenger,
                    PassengerName = r.Passenger?.User?.Name,
                    TripId = r.IdTrip,
                    LocationId = r.IdLocation,
                    PickupLocation = r.Location?.Address,
                    RequestStatusId = r.IdRequestStatus,
                    RequestStatus = r.RequestStatus?.Status,
                    RequestDate = r.DateRequest,
                    Reason = r.Reason
                }).ToList();

                return Ok(requestDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/DriverTrip/accept
        [HttpPost("accept")]
        public async Task<ActionResult> AcceptRequest(AcceptRequestDto dto)
        {
            try
            {
                // Get the request to check if the current user is the driver
                var request = await _tripParticipationService.GetRequestByIdAsync(dto.RequestId);

                // Check if the current user is the driver of the trip
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (request.Trip?.IdDriver != userId)
                {
                    return Forbid();
                }

                // Accept the request
                var updatedRequest = await _tripParticipationService.AcceptRequestAsync(dto.RequestId);

                // Send notification to passenger
                await _notificationService.SendAcceptanceNotificationAsync(dto.RequestId);

                // Send trip booking confirmation
                await _notificationService.SendTripBookingConfirmationAsync(dto.RequestId);

                return Ok(new { message = "Request accepted successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/DriverTrip/reject
        [HttpPost("reject")]
        public async Task<ActionResult> RejectRequest(RejectRequestDto dto)
        {
            try
            {
                // Get the request to check if the current user is the driver
                var request = await _tripParticipationService.GetRequestByIdAsync(dto.RequestId);

                // Check if the current user is the driver of the trip
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (request.Trip?.IdDriver != userId)
                {
                    return Forbid();
                }

                // Validate the reason is provided
                if (string.IsNullOrWhiteSpace(dto.Reason))
                {
                    return BadRequest("Rejection reason is required");
                }

                // Reject the request
                var updatedRequest = await _tripParticipationService.RejectRequestAsync(dto.RequestId, dto.Reason);

                // Send notification to passenger
                await _notificationService.SendRejectionNotificationAsync(dto.RequestId);

                return Ok(new { message = "Request rejected successfully" });
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

        // POST: api/DriverTrip/trips/{tripId}/checkwaitinglist
        [HttpPost("trips/{tripId}/checkwaitinglist")]
        public async Task<ActionResult> CheckWaitingList(Guid tripId)
        {
            try
            {
                // Verify the current user is the driver of the trip
                if (!await IsDriverOfTrip(tripId))
                {
                    return Forbid();
                }

                // Check waiting list and move passengers if seats are available
                var movedRequests = await _tripParticipationService.CheckWaitingListAsync(tripId);

                // Send notifications to moved passengers
                foreach (var request in movedRequests)
                {
                    await _notificationService.SendWaitingListNotificationAsync(request.Id);
                }

                return Ok(new
                {
                    message = $"{movedRequests.Count} passengers moved from waiting list to pending status",
                    movedRequests = movedRequests.Select(r => r.Id).ToList()
                });
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