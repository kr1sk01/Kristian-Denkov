using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace FruitAPI.Pages
{
    public class AddModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AddModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public Fruit FruitModels { get; set; }

        public async Task<IActionResult> OnPost()
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(FruitModels), Encoding.UTF8, "application/json");

            var httpClient = _httpClientFactory.CreateClient("FruitAPI");

            using HttpResponseMessage response = await httpClient.PostAsync("", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }
            return Page();
        }

        public void OnGet()
        {
        }
    }
}
