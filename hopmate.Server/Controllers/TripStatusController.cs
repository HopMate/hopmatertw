using hopmate.Server.DTOs;
using hopmate.Server.Models.Entities;
using hopmate.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace hopmate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripStatusController : ControllerBase
    {
        private readonly TripStatusService _service;

        public TripStatusController(TripStatusService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TripStatusDto>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TripStatusDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<TripStatus>> Create([FromBody] TripStatusDto tripStatusDto)
        {
            var created = await _service.CreateAsync(tripStatusDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TripStatus>> Update(int id, [FromBody] TripStatusDto tripStatusDto)
        {
            var updated = await _service.UpdateAsync(id, tripStatusDto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
