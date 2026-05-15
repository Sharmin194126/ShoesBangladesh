using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesBangladesh.API.Data;
using ShoesBangladesh.API.Models;
using System.Threading.Tasks;

namespace ShoesBangladesh.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageSettingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public HomePageSettingsController(ApplicationDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<HomePageSettings>> Get()
        {
            var settings = await _context.HomePageSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new HomePageSettings();
                _context.HomePageSettings.Add(settings);
                await _context.SaveChangesAsync();
            }
            return settings;
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] HomePageSettings model)
        {
            var existing = await _context.HomePageSettings.FirstOrDefaultAsync();
            if (existing == null)
            {
                _context.HomePageSettings.Add(model);
            }
            else
            {
                existing.MarqueeText = model.MarqueeText;
                existing.HeroHeadline = model.HeroHeadline;
                existing.HeroSubtitle = model.HeroSubtitle;
                existing.HeroBtnText = model.HeroBtnText;
                existing.HeroBtnLink = model.HeroBtnLink;
                existing.IsActive = model.IsActive;
            }
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}
