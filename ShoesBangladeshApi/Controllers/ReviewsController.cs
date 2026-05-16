using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesBangladesh.API.Data;
using ShoesBangladesh.API.Models;
using ShoesBangladesh.API.ViewModels;

namespace ShoesBangladesh.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ReviewsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReview([FromForm] ReviewViewModel model, List<IFormFile>? images)
        {
            var review = new Review
            {
                ProductId = model.ProductId,
                UserId = model.UserId,
                Rating = model.Rating,
                Comment = model.Comment,
                Status = "Approved", // Auto-approve for now or set to Pending
                CreatedAt = DateTime.UtcNow
            };

            if (images != null && images.Any())
            {
                var uploadedUrls = new List<string>();
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "reviews");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                foreach (var file in images)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    uploadedUrls.Add($"/uploads/reviews/{fileName}");
                }
                review.ImageUrls = string.Join(",", uploadedUrls);
            }

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [HttpPost("React")]
        public async Task<IActionResult> React(int reviewId, int userId, string reactionType)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return NotFound();

            var existing = await _context.ReviewReactions
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId && r.UserId == userId);

            if (existing != null)
            {
                if (existing.ReactionType == reactionType)
                {
                    // Remove reaction
                    _context.ReviewReactions.Remove(existing);
                    if (reactionType == "Like") review.LikeCount = Math.Max(0, review.LikeCount - 1);
                    else review.DislikeCount = Math.Max(0, review.DislikeCount - 1);
                }
                else
                {
                    // Switch reaction
                    if (reactionType == "Like")
                    {
                        review.LikeCount++;
                        review.DislikeCount = Math.Max(0, review.DislikeCount - 1);
                    }
                    else
                    {
                        review.DislikeCount++;
                        review.LikeCount = Math.Max(0, review.LikeCount - 1);
                    }
                    existing.ReactionType = reactionType;
                }
            }
            else
            {
                // New reaction
                var reaction = new ReviewReaction
                {
                    ReviewId = reviewId,
                    UserId = userId,
                    ReactionType = reactionType
                };
                _context.ReviewReactions.Add(reaction);
                if (reactionType == "Like") review.LikeCount++;
                else review.DislikeCount++;
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpPost("Reply")]
        public async Task<IActionResult> Reply([FromForm] int reviewId, [FromForm] string replyText, [FromForm] int replierId, [FromForm] string replierName, [FromForm] bool isSeller, List<IFormFile>? attachments)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return NotFound();

            var reply = new ReviewReply
            {
                ReviewId = reviewId,
                ReplierId = replierId,
                ReplierName = replierName,
                ReplyText = replyText,
                IsSeller = isSeller,
                CreatedAt = DateTime.UtcNow
            };

            if (attachments != null && attachments.Any())
            {
                var uploadedUrls = new List<string>();
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "replies");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                foreach (var file in attachments)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    uploadedUrls.Add($"/uploads/replies/{fileName}");
                }
                reply.AttachmentUrls = string.Join(",", uploadedUrls);
            }

            _context.ReviewReplies.Add(reply);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }
}
