using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesBangladesh.API.Data;
using ShoesBangladesh.API.Models;

using ShoesBangladesh.API.ViewModels;

namespace ShoesBangladesh.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LandingPageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LandingPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLandingPageData()
        {
            var settings = await _context.SystemSettings.FirstOrDefaultAsync();
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            var response = new LandingPageResponse
            {
                Settings = new SystemSettingsDTO
                {
                    EidOfferTitle = settings?.EidOfferTitle ?? "",
                    EidOfferSubtitle = settings?.EidOfferSubtitle ?? "",
                    DiscountPercentage = settings?.DiscountPercentage ?? 0,
                    EidOfferEndTime = settings?.EidOfferEndTime ?? DateTime.Now,
                    IsOfferActive = settings?.IsOfferActive ?? false,
                    CompanyName = settings?.CompanyName ?? "Shoes Bangladesh",
                    FacebookPageLink = settings?.FacebookPageLink ?? ""
                },
                Categories = categories.Select(c => new CategoryDTO { Id = c.Id, Name = c.Name }).ToList(),
                Products = products.Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    ImageUrl = p.ImageUrl,
                    AdditionalImages = p.AdditionalImages,
                    IsEidOffer = p.IsEidOffer,
                    IsFeatured = p.IsFeatured,
                    CategoryName = p.Category?.Name ?? "Uncategorized"
                }).ToList()
            };

            return Ok(response);
        }


        [HttpPost("UpdateSettings")]
        public async Task<IActionResult> UpdateSettings(SystemSettingsDTO settings)
        {
            var existing = await _context.SystemSettings.FirstOrDefaultAsync();
            if (existing == null)
            {
                existing = new SystemSettings();
                _context.SystemSettings.Add(existing);
            }

            existing.EidOfferTitle = settings.EidOfferTitle;
            existing.EidOfferSubtitle = settings.EidOfferSubtitle;
            existing.EidOfferEndTime = settings.EidOfferEndTime;
            existing.DiscountPercentage = settings.DiscountPercentage;
            existing.IsOfferActive = settings.IsOfferActive;
            existing.FacebookPageLink = settings.FacebookPageLink;

            await _context.SaveChangesAsync();
            return Ok(new { IsSuccess = true, Message = "Settings updated successfully." });
        }
    }
}

    }
}
