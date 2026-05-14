using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesBangladesh.Web.Services;

namespace ShoesBangladeshWebApp.Controllers
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int ProductCount { get; set; }
    }

    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryApiService _categoryService;

        public CategoryController(ICategoryApiService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        public IActionResult Create() => View(new CategoryViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 300 * 1024)
            {
                ModelState.AddModelError("ImageUrl", "Image storage size will be must 200-300 kb.");
            }

            if (!ModelState.IsValid) return View(model);

            var success = await _categoryService.CreateCategoryAsync(model, imageFile);
            if (success)
            {
                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to create category. Please try again.");
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel model, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 300 * 1024)
            {
                ModelState.AddModelError("ImageUrl", "Image storage size will be must 200-300 kb.");
            }

            if (!ModelState.IsValid) return View(model);

            var success = await _categoryService.UpdateCategoryAsync(id, model, imageFile);
            if (success)
            {
                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction(nameof(Edit), new { id = id });
            }

            ModelState.AddModelError("", "Failed to update category. Please try again.");
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (success)
                TempData["SuccessMessage"] = "Category deleted successfully!";
            else
                TempData["ErrorMessage"] = "Failed to delete category.";

            return RedirectToAction(nameof(Index));
        }
    }
}
