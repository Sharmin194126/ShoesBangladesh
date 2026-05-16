using Microsoft.AspNetCore.Mvc;
using ShoesBangladesh.API.ViewModels;
using System.Text.Json;

namespace ShoesBangladeshWebApp.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FooterViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ShoesAPI");
            try
            {
                var response = await client.GetAsync("api/Dashboard/Footer");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var footer = JsonSerializer.Deserialize<FooterInfoViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(footer);
                }
            }
            catch
            {
                // Fallback or log error
            }

            return View(new FooterInfoViewModel());
        }
    }
}
