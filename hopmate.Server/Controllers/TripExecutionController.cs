//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using hopmate.Server.Services;
//using hopmate.Server.Models.DTOs;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Security.Claims;
//using Microsoft.EntityFrameworkCore;
//using hopmate.Server.Data;
//using hopmate.Server.Models.Dto;

//namespace hopmate.Server.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize]
//    public class TripExecutionController : ControllerBase
//    {
//        private readonly TripExecutionService _tripExecutionService;
//        private readonly ApplicationDbContext _context;
//        private readonly ILogger<TripExecutionController> _logger; // Add logger

//        public TripExecutionController(
//            TripExecutionService tripExecutionService,
//            ApplicationDbContext context,
//            ILogger<TripExecutionController> logger) // Inject logger
//        {
//            _tripExecutionService = tripExecutionService;
//            _context = context;
//            _logger = logger;
//        }

//        // Helper method to check if the current user is the driver of the trip
//        private async Task<bool> IsDriverOfTrip(Guid tripId)
//        {
//            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
//            var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == tripId);
//            return trip != null && trip.IdDriver == userId;
//        }

//        // GET: api/TripExecution/{tripId}/status
//        [HttpGet("{tripId}/status")]
//        public async Task<ActionResult<TripExecutionStatusDto>> GetTripStatus(Guid tripId)
//        {
//            try
//            {
//                _logger.LogInformation($"Getting status for trip {tripId}");

//                // Verify the current user is the driver of the trip
//                if (!await IsDriverOfTrip(tripId))
//                {
//                    _logger.LogWarning($"Unauthorized access attempt for trip {tripId} status");
//                    return Forbid();
//                }

//                var tripStatus = await _tripExecutionService.GetTripExecutionStatusAsync(tripId);
//                return Ok(tripStatus);
//            }
//            catch (ArgumentException ex)
//            {
//                _logger.LogWarning(ex, $"Trip not found: {tripId}");
//                return NotFound(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error getting trip status for trip {tripId}");
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }

//        // GET: api/TripExecution/{tripId}/passengers
//        [HttpGet("{tripId}/passengers")]
//        public async Task<ActionResult<IEnumerable<PassengerAttendanceDto>>> GetTripPassengers(Guid tripId)
//        {
//            try
//            {
//                _logger.LogInformation($"Getting passengers for trip {tripId}");

//                // Verify the current user is the driver of the trip
//                if (!await IsDriverOfTrip(tripId))
//                {
//                    _logger.LogWarning($"Unauthorized access attempt for trip {tripId} passengers");
//                    return Forbid();
//                }

//                var passengers = await _tripExecutionService.GetTripPassengersAsync(tripId);
//                return Ok(passengers);
//            }
//            catch (ArgumentException ex)
//            {
//                _logger.LogWarning(ex, $"Trip not found: {tripId}");
//                return NotFound(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error getting passengers for trip {tripId}: {ex.Message}");
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }

//        // POST: api/TripExecution/begin
//        [HttpPost("begin")]
//        public async Task<ActionResult> BeginTrip(BeginTripDto dto)
//        {
//            try
//            {
//                _logger.LogInformation($"Beginning trip {dto.TripId}");

//                // Verify the current user is the driver of the trip
//                if (!await IsDriverOfTrip(dto.TripId))
//                {
//                    _logger.LogWarning($"Unauthorized access attempt to begin trip {dto.TripId}");
//                    return Forbid();
//                }

//                var trip = await _tripExecutionService.BeginTripAsync(dto.TripId);
//                return Ok(new { message = "Trip started successfully", status = trip.TripStatus?.Status });
//            }
//            catch (ArgumentException ex)
//            {
//                _logger.LogWarning(ex, $"Trip not found: {dto.TripId}");
//                return NotFound(ex.Message);
//            }
//            catch (InvalidOperationException ex)
//            {
//                _logger.LogWarning(ex, $"Invalid operation when beginning trip {dto.TripId}");
//                return BadRequest(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error beginning trip {dto.TripId}");
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }

//        // POST: api/TripExecution/end
//        [HttpPost("end")]
//        public async Task<ActionResult> EndTrip(EndTripDto dto)
//        {
//            try
//            {
//                _logger.LogInformation($"Ending trip {dto.TripId}");

//                // Verify the current user is the driver of the trip
//                if (!await IsDriverOfTrip(dto.TripId))
//                {
//                    _logger.LogWarning($"Unauthorized access attempt to end trip {dto.TripId}");
//                    return Forbid();
//                }

//                var trip = await _tripExecutionService.EndTripAsync(dto.TripId);
//                return Ok(new { message = "Trip ended successfully", status = trip.TripStatus?.Status });
//            }
//            catch (ArgumentException ex)
//            {
//                _logger.LogWarning(ex, $"Trip not found: {dto.TripId}");
//                return NotFound(ex.Message);
//            }
//            catch (InvalidOperationException ex)
//            {
//                _logger.LogWarning(ex, $"Invalid operation when ending trip {dto.TripId}");
//                return BadRequest(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error ending trip {dto.TripId}");
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }

//        // POST: api/TripExecution/attendance
//        [HttpPost("attendance")]
//        public async Task<ActionResult> MarkAttendance(MarkAttendanceDto dto)
//        {
//            try
//            {
//                _logger.LogInformation($"Marking attendance for request {dto.RequestId}, present: {dto.IsPresent}");

//                // First get the request to check the trip
//                var request = await _context.PassengerTrips
//                    .Include(pt => pt.Trip)
//                    .FirstOrDefaultAsync(pt => pt.Id == dto.RequestId);

//                if (request == null)
//                {
//                    _logger.LogWarning($"Passenger request not found: {dto.RequestId}");
//                    return NotFound("Passenger request not found");
//                }

//                // Verify the current user is the driver of the trip
//                if (!await IsDriverOfTrip(request.Trip.Id))
//                {
//                    _logger.LogWarning($"Unauthorized access attempt to mark attendance for request {dto.RequestId}");
//                    return Forbid();
//                }

//                if (dto.IsPresent)
//                {
//                    await _tripExecutionService.MarkPassengerPresentAsync(dto.RequestId);
//                    return Ok(new { message = "Passenger marked as present" });
//                }
//                else
//                {
//                    await _tripExecutionService.MarkPassengerAbsentAsync(dto.RequestId);
//                    return Ok(new { message = "Passenger marked as absent" });
//                }
//            }
//            catch (ArgumentException ex)
//            {
//                _logger.LogWarning(ex, $"Not found when marking attendance: {ex.Message}");
//                return NotFound(ex.Message);
//            }
//            catch (InvalidOperationException ex)
//            {
//                _logger.LogWarning(ex, $"Invalid operation when marking attendance: {ex.Message}");
//                return BadRequest(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error marking attendance for request {dto.RequestId}");
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }
//    }
//}