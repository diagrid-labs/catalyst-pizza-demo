using System.Net.Http;
using Dapr.Workflow;

namespace OrderService.Activities
{
    public class RestockInventory : WorkflowActivity<object?, object?>
    {
        readonly ILogger _logger;
        readonly HttpClient _httpClient;

        public RestockInventory(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory)
        {
            _logger = loggerFactory.CreateLogger<CheckInventoryActivity>();
            _httpClient = httpClientFactory.CreateClient("daprEndpoint");
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, object? input)
        {
            _logger.LogInformation($"Restocking inventory.");
            await _httpClient.PostAsync($"/inventory/restock", null);

            return null;
        }
    }
}