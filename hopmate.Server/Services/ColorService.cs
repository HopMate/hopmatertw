using hopmate.Server.Data;
using hopmate.Server.Models.Dto;
using Microsoft.EntityFrameworkCore;
using hopmate.Server.Models.Entities;

namespace hopmate.Server.Services
{
    public class ColorService
    {
        private readonly ApplicationDbContext _context;

        public ColorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ColorDto>> GetColorsAsync()
        {
            var colors = await _context.Colors.ToListAsync();
            return colors.Select(c => new ColorDto
            {
                Name = c.Name
            }).ToList();
        }
        public async Task<ColorDto?> GetColorByIdAsync(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
                return null;

            return new ColorDto
            {
                Name = color.Name
            };
        }

        public async Task<Color> CreateColorAsync(ColorDto colorDto)
        {
            var color = new Color
            {
                Name = colorDto.Name
            };
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            return color;
        }

        public async Task<Color?> UpdateColorAsync(int id, ColorDto colorDto)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return null;

            color.Name = colorDto.Name;

            _context.Entry(color).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return color;
        }

        public async Task<bool> DeleteColorAsync(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return false;

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}