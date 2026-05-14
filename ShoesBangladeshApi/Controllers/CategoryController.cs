using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesBangladesh.API.Data;
using ShoesBangladesh.API.Models;

namespace ShoesBangladesh.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.ImageUrl,
                    c.Status,
                    c.CreatedDate,
                    ProductCount = c.Products.Count
                })
                .ToListAsync();
            return Ok(categories);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.ImageUrl,
                    c.Status,
                    c.CreatedDate,
                    ProductCount = c.Products.Count
                })
                .FirstOrDefaultAsync();

            if (category == null) return NotFound();
            return Ok(category);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Category created successfully", id = category.Id });
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = category.Name;
            existing.Description = category.Description;
            existing.ImageUrl = category.ImageUrl;
            existing.Status = category.Status;
            // Not updating CreatedDate
            await _context.SaveChangesAsync();
            return Ok(new { message = "Category updated successfully" });
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Category deleted successfully" });
        }
    }
}
