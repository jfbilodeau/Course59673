using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class DetailsModel : PageModel
{
    private IHttpClientFactory _httpClientFactory;

    public DetailsModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = "";

    public Employee? Employee { get; set; } = null;

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("HrSystem");
        var response = await client.GetAsync($"/api/employees/{Id}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            Employee = JsonSerializer.Deserialize<Employee>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
