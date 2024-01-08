using Dapr.Workflow;
using IO.Ably;
using IO.Ably.Rest;
using Shared.Models;

namespace OrderService.Activities
{
    public class NotifyActivity : WorkflowActivity<Notification, object?>
    {
        private readonly ILogger _logger;
        private readonly IRestChannel _channel;

        public NotifyActivity(ILoggerFactory loggerFactory, IRestClient realtimeClient)
        {
            _logger = loggerFactory.CreateLogger<NotifyActivity>();
            _channel = realtimeClient.Channels.Get("pizza-order-notifications");
        }

        public override Task<object?> RunAsync(WorkflowActivityContext context, Notification notification)
        {
            _logger.LogInformation(notification.Message);
            _channel.Publish("notification", notification);

            return Task.FromResult<object?>(null);
        }
    }
}