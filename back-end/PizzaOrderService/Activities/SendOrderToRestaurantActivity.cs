using Dapr.Client;
using Dapr.Workflow;
using Shared.Models;

namespace OrderService.Activities
{
    public class SendOrderToRestaurantActivity : WorkflowActivity<Order, object?>
    {
        readonly DaprClient _daprClient;

        public SendOrderToRestaurantActivity(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, Order order)
        {
            await _daprClient.PublishEventAsync("pizza-pubsub", "pizza-orders", order);

            return null;
        }
    }
}