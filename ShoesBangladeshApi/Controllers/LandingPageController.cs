using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesBangladesh.API.Data;
using ShoesBangladesh.API.Models;
using ShoesBangladesh.API.ViewModels;
using System;
using System.Linq;


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
            try
            {
                var settings = await _context.SystemSettings.FirstOrDefaultAsync();
                var products = await _context.Products.Include(p => p.Category).ToListAsync();
                var categories = await _context.Categories.ToListAsync();

                var response = new LandingPageResponse
                {
                    Settings = new SystemSettingsDTO
                    {
                        EidOfferTitle = settings?.EidOfferTitle ?? "Eid Special Offer",
                        EidOfferSubtitle = settings?.EidOfferSubtitle ?? "Premium Footwear Collection",
                        DiscountPercentage = settings?.DiscountPercentage ?? 0,
                        EidOfferEndTime = settings?.EidOfferEndTime ?? DateTime.Now.AddDays(7),
                        IsOfferActive = settings?.IsOfferActive ?? true,
                        CompanyName = settings?.CompanyName ?? "Shoes Bangladesh",
                        CompanyDescription = settings?.CompanyDescription ?? "Your trusted destination for premium footwear in Bangladesh.",
                        ProductSectionDescription = settings?.ProductSectionDescription ?? "Discover our latest and most exclusive footwear collection.",
                        FacebookPageLink = settings?.FacebookPageLink ?? "#"
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}. Inner Exception: {ex.InnerException?.Message}");
            }
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
            existing.CompanyName = settings.CompanyName;
            existing.CompanyDescription = settings.CompanyDescription;
            existing.ProductSectionDescription = settings.ProductSectionDescription;
            existing.FacebookPageLink = settings.FacebookPageLink;

            await _context.SaveChangesAsync();
            return Ok(new { IsSuccess = true, Message = "Settings updated successfully." });
        }
    }
}
