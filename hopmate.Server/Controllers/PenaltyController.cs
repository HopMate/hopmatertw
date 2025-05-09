using hopmate.Server.Data;
using hopmate.Server.Models.Dto;
using hopmate.Server.Models.Entities;
using hopmate.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hopmate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PenaltyController : ControllerBase
    {
        private readonly PenaltyService _penaltyService;

        public PenaltyController(PenaltyService penaltyService)
        {
            _penaltyService = penaltyService;
        }

        [HttpPost]
        public async Task<ActionResult<Penalty>> CreatePenalty([FromBody] PenaltyDto penaltyDto)
        {
            var penalty = await _penaltyService.AddPenaltyAsync(penaltyDto);
            if (penalty == null)
            {
                return NotFound("User not found");
            }

            return CreatedAtAction(nameof(GetPenaltiesUser), new { id = penalty.IdUser }, penalty);
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<List<PenaltyDto>>> GetPenaltiesUser(Guid id)
        {
            var penalties = await _penaltyService.GetPenaltiesByUserIdAsync(id);

            if (penalties == null)
            {
                return NotFound("User not found");
            }

            return Ok(penalties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PenaltyDto>> GetPenalty(Guid id)
        {
            var penalty = await _penaltyService.GetPenaltyByIdAsync(id);
            if (penalty == null)
            {
                return NotFound("Penalty not found");
            }
            return Ok(penalty);
        }
    }
}
