using Dapr.Client;
using Dapr.Workflow;
using Shared.Models;

namespace OrderService.Activities
{
    public class SendOrderToKitchenActivity : WorkflowActivity<Order, object?>
    {
        private readonly ILogger _logger;
        private readonly DaprClient _daprClient;
        private static readonly string PubSubName = "pubsub";
        private static readonly string TopicName = "pizza-orders";

        public SendOrderToKitchenActivity(ILoggerFactory loggerFactory, DaprClient daprClient)
        {
            _logger = loggerFactory.CreateLogger<SendOrderToKitchenActivity>();
            _daprClient = daprClient;
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, Order order)
        {
            _logger.LogInformation($"Sending order {order.OrderId} to the kitchen.");
            
            // Simulate a delay in communicating with the kitchen.
            Thread.Sleep(1000);
            
            await _daprClient.PublishEventAsync(PubSubName, TopicName, order);

            return null;
        }
    }
}