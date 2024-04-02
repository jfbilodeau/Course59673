using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public Employee[] Employees { get; set; } = [];

    public async Task OnGetAsync()
    {
        var client = _clientFactory.CreateClient("HrSystem");
        var response = await client.GetAsync("/api/employees");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            var employees = JsonSerializer.Deserialize<List<Employee>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }) ?? [];

            // ViewData["Employees"] = employees;
            Employees = [.. employees];
        }
    }
}
