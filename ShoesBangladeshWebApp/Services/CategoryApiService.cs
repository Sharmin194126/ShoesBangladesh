using ShoesBangladeshWebApp.Controllers;
using System.Text.Json;
using System.Text;

namespace ShoesBangladesh.Web.Services
{
    public interface ICategoryApiService
    {
        Task<List<CategoryViewModel>> GetAllCategoriesAsync();
        Task<CategoryViewModel?> GetCategoryByIdAsync(int id);
        Task<bool> CreateCategoryAsync(CategoryViewModel model, IFormFile? imageFile);
        Task<bool> UpdateCategoryAsync(int id, CategoryViewModel model, IFormFile? imageFile);
        Task<bool> DeleteCategoryAsync(int id);
    }

    public class CategoryApiService : ICategoryApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOpts;

        public CategoryApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<CategoryViewModel>> GetAllCategoriesAsync()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync("api/Category");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CategoryViewModel>>(json, _jsonOpts) ?? new();
            }
            return new List<CategoryViewModel>();
        }

        public async Task<CategoryViewModel?> GetCategoryByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.GetAsync($"api/Category/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<CategoryViewModel>(json, _jsonOpts);
            }
            return null;
        }

        public async Task<bool> CreateCategoryAsync(CategoryViewModel model, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                model.ImageUrl = await UploadImageAsync(imageFile);
            }

            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Category", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryViewModel model, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadedUrl = await UploadImageAsync(imageFile);
                if (!string.IsNullOrEmpty(uploadedUrl)) model.ImageUrl = uploadedUrl;
            }

            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/Category/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var response = await client.DeleteAsync($"api/Category/{id}");
            return response.IsSuccessStatusCode;
        }

        private async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            using var form = new MultipartFormDataContent();
            using var fileStream = imageFile.OpenReadStream();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
            form.Add(streamContent, "file", imageFile.FileName);

            var client = _httpClientFactory.CreateClient("ShoesAPI");
            var uploadResponse = await client.PostAsync("api/products/upload", form);
            if (uploadResponse.IsSuccessStatusCode)
            {
                var uploadResult = await uploadResponse.Content.ReadAsStringAsync();
                var uploadJson = JsonSerializer.Deserialize<JsonElement>(uploadResult);
                return uploadJson.GetProperty("imageUrl").GetString() ?? "";
            }
            return "";
        }
    }
}
