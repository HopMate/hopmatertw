using hopmate.Server.Models.Dto;
using hopmate.Server.Models.Entities;
using hopmate.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace hopmate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly ColorService _colorService;

        public ColorsController(ColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ColorDto>>> GetColors()
        {
            var colors = await _colorService.GetColorsAsync();
            return Ok(colors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ColorDto>> GetColor(int id)
        {
            var color = await _colorService.GetColorByIdAsync(id);
            if (color == null) return NotFound();
            return Ok(color);
        }

        [HttpPost]
        public async Task<ActionResult<Color>> PostColor(ColorDto colorDto)
        {
            var createdColor = await _colorService.CreateColorAsync(colorDto);
            return CreatedAtAction(nameof(GetColor), new { id = createdColor.Id }, createdColor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutColor(int id, ColorDto colorDto)
        {
            var updatedColor = await _colorService.UpdateColorAsync(id, colorDto);
            if (updatedColor == null) return NotFound();
            return NoContent();
        }

        // Método DELETE para remover uma cor
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColor(int id)
        {
            var result = await _colorService.DeleteColorAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}