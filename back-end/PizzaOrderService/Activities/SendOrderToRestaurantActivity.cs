using Dapr.Client;
using Dapr.Workflow;
using Shared.Models;

namespace OrderService.Activities
{
    public class SendOrderToRestaurantActivity : WorkflowActivity<Order, object?>
    {
        readonly ILogger _logger;
        readonly DaprClient _daprClient;

        public SendOrderToRestaurantActivity(ILoggerFactory loggerFactory, DaprClient daprClient)
        {
            _logger = loggerFactory.CreateLogger<SendOrderToRestaurantActivity>();
            _daprClient = daprClient;
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, Order order)
        {
            _logger.LogInformation($"Sending order {order.OrderId} to restaurant.");
            await _daprClient.PublishEventAsync("pizza-pubsub", "pizza-orders", order);

            return null;
        }
    }
}