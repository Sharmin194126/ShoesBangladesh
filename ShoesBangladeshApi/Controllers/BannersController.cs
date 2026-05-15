using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesBangladesh.API.Data;
using ShoesBangladesh.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoesBangladesh.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BannersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HomeBanner>>> GetBanners()
        {
            return await _context.HomeBanners
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<HomeBanner>> CreateBanner(HomeBanner banner)
        {
            _context.HomeBanners.Add(banner);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBanners), new { id = banner.Id }, banner);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBanner(int id, HomeBanner banner)
        {
            if (id != banner.Id) return BadRequest();
            _context.Entry(banner).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            var banner = await _context.HomeBanners.FindAsync(id);
            if (banner == null) return NotFound();
            _context.HomeBanners.Remove(banner);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
