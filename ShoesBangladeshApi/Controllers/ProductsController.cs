using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesBangladesh.API.Data;
using ShoesBangladesh.API.Models;
using ShoesBangladesh.API.ViewModels;

namespace ShoesBangladesh.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        [HttpGet("{id}/Details")]
        public async Task<ActionResult<ProductDetailsViewModel>> GetProductDetails(int id, int? currentUserId = null)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var reviews = await _context.Reviews
                .Where(r => r.ProductId == id && r.Status == "Approved")
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var reviewViewModels = new List<ReviewViewModel>();
            foreach (var r in reviews)
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == r.UserId);
                var replies = await _context.ReviewReplies.Where(rp => rp.ReviewId == r.Id).OrderBy(rp => rp.CreatedAt).ToListAsync();
                
                var isLiked = currentUserId.HasValue && await _context.ReviewReactions.AnyAsync(rr => rr.ReviewId == r.Id && rr.UserId == currentUserId.Value && rr.ReactionType == "Like");
                var isDisliked = currentUserId.HasValue && await _context.ReviewReactions.AnyAsync(rr => rr.ReviewId == r.Id && rr.UserId == currentUserId.Value && rr.ReactionType == "Dislike");

                reviewViewModels.Add(new ReviewViewModel
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    CustomerName = customer?.Name ?? "Anonymous",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ImageUrls = r.ImageUrls,
                    CreatedAt = r.CreatedAt,
                    LikeCount = r.LikeCount,
                    DislikeCount = r.DislikeCount,
                    IsLikedByCurrentUser = isLiked,
                    IsDislikedByCurrentUser = isDisliked,
                    Replies = replies.Select(rp => new ReviewReplyViewModel
                    {
                        Id = rp.Id,
                        ReviewId = rp.ReviewId,
                        ReplierName = rp.ReplierName,
                        ReplyText = rp.ReplyText,
                        IsSeller = rp.IsSeller,
                        AttachmentUrls = rp.AttachmentUrls,
                        CreatedAt = rp.CreatedAt
                    }).ToList()
                });
            }

            var avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            var ratingSummary = reviews.GroupBy(r => r.Rating)
                .ToDictionary(g => g.Key, g => g.Count());

            var relatedProducts = await _context.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                .Take(6)
                .ToListAsync();

            var viewModel = new ProductDetailsViewModel
            {
                Product = new ProductDetailsViewModel.ProductDetailItem
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    ImageUrl = product.ImageUrl,
                    Price = product.DiscountPrice ?? product.Price,
                    RegularPrice = product.Price,
                    OfferPercentage = product.OfferPercentage,
                    VatPercentage = product.VatPercentage,
                    DeliveryCharge = product.DeliveryCharge,
                    StockQty = product.StockQuantity,
                    MaxOrderQty = product.MaxOrderLimit,
                    AverageRating = avgRating,
                    AvailableSizes = string.Join(", ", product.AvailableSizes),
                    ProductTypeId = product.ProductTypeId,
                    ProductType = product.ProductType != null ? new ProductDetailsViewModel.ProductTypeInfo { Id = product.ProductType.Id, Name = product.ProductType.Name } : null
                },
                RelatedImages = product.AdditionalImages,
                TotalReviews = reviews.Count,
                RatingSummary = ratingSummary,
                Reviews = reviewViewModels,
                SizeStocks = product.SizeQuantities.Select(sq => new ProductDetailsViewModel.SizeStockViewModel { Size = sq.Key, StockQty = sq.Value }).ToList(),
                RelatedProducts = relatedProducts.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    ImageUrl = p.ImageUrl
                }).ToList()
            };

            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id) return BadRequest();

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative URL to the image
            var imageUrl = $"/uploads/{fileName}";
            return Ok(new { imageUrl });
        }

        // GET: api/Products/Types
        [HttpGet("Types")]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetProductTypes()
        {
            return await _context.ProductTypes.ToListAsync();
        }
    }
}

