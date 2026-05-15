using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesBangladesh.API.Data;
using ShoesBangladesh.API.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ShoesBangladesh.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsletterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NewsletterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Newsletter/Subscribe
        [HttpPost("Subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required.");

            var existing = await _context.Newsletters.FirstOrDefaultAsync(n => n.Email == email);
            if (existing != null)
                return Conflict("This email is already subscribed.");

            var newsletter = new Newsletter { Email = email };
            _context.Newsletters.Add(newsletter);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully subscribed!" });
        }

        // GET: api/Newsletter/Subscribers
        [HttpGet("Subscribers")]
        public async Task<ActionResult<IEnumerable<Newsletter>>> GetSubscribers()
        {
            return await _context.Newsletters.OrderByDescending(n => n.SubscribedAt).ToListAsync();
        }

        // DELETE: api/Newsletter/Unsubscribe/5
        [HttpDelete("Unsubscribe/{id}")]
        public async Task<IActionResult> Unsubscribe(int id)
        {
            var newsletter = await _context.Newsletters.FindAsync(id);
            if (newsletter == null)
                return NotFound();

            _context.Newsletters.Remove(newsletter);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully unsubscribed." });
        }
    }
}
