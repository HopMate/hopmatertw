using hopmate.Server.Models.Dto;
using hopmate.Server.Services;
using Microsoft.AspNetCore.Mvc;
using hopmate.Server.Models.Entities;

namespace hopmate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestStatusController : ControllerBase
    {
        private readonly RequestStatusService _service;

        public RequestStatusController(RequestStatusService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<RequestStatusDto>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequestStatusDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<RequestStatus>> Create([FromBody] RequestStatusDto statusDto)
        {
            var created = await _service.CreateAsync(statusDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RequestStatus>> Update(int id, [FromBody] RequestStatusDto statusDto)
        {
            var updated = await _service.UpdateAsync(id, statusDto);
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