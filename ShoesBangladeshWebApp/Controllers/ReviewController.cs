using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ShoesBangladesh.API.ViewModels;

namespace ShoesBangladesh.Web.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ReviewController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReview(int productId, int rating, string comment, List<IFormFile> images)
        {
            if (!User.Identity?.IsAuthenticated ?? true) return RedirectToAction("Login", "Account");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var client = _httpClientFactory.CreateClient("ShoesAPI");

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(productId.ToString()), "ProductId");
            content.Add(new StringContent(userId.ToString()), "UserId");
            content.Add(new StringContent(rating.ToString()), "Rating");
            content.Add(new StringContent(comment), "Comment");

            if (images != null)
            {
                foreach (var file in images)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "images", file.FileName);
                }
            }

            var response = await client.PostAsync("api/Reviews", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Thank you for your review!";
            }

            return RedirectToAction("Details", "Home", new { id = productId });
        }

        [HttpPost]
        public async Task<IActionResult> React(int reviewId, string reactionType)
        {
            if (!User.Identity?.IsAuthenticated ?? true) return Json(new { success = false, message = "Unauthorized" });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var client = _httpClientFactory.CreateClient("ShoesAPI");

            var response = await client.PostAsync($"api/Reviews/React?reviewId={reviewId}&userId={userId}&reactionType={reactionType}", null);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Reply(int reviewId, string replyText, List<IFormFile> attachments)
        {
            if (!User.Identity?.IsAuthenticated ?? true) return Json(new { success = false, message = "Unauthorized" });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userName = User.Identity.Name ?? "Staff";
            var isSeller = User.IsInRole("Admin") || User.IsInRole("Employee");

            var client = _httpClientFactory.CreateClient("ShoesAPI");

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(reviewId.ToString()), "reviewId");
            content.Add(new StringContent(replyText), "replyText");
            content.Add(new StringContent(userId.ToString()), "replierId");
            content.Add(new StringContent(userName), "replierName");
            content.Add(new StringContent(isSeller.ToString().ToLower()), "isSeller");

            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "attachments", file.FileName);
                }
            }

            var response = await client.PostAsync("api/Reviews/Reply", content);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
    }
}
