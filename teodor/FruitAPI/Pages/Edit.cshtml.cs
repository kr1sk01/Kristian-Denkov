using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace FruitAPI.Pages
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public Fruit FruitModels { get; set; }
        public async Task OnGet(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("FruitAPI");
            using HttpResponseMessage response = await httpClient.GetAsync(id.ToString());

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                FruitModels = await JsonSerializer.DeserializeAsync<Fruit>(contentStream);
            }
        }

        public async Task<IActionResult> OnPut()
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(FruitModels), Encoding.UTF8, "application/json");

            var httpClient = _httpClientFactory.CreateClient("FruitAPI");

            using HttpResponseMessage response = await httpClient.PutAsync(FruitModels.Id.ToString(), jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }
            else
            {
                return RedirectToPage("Index");
            }
        }
    }
}
