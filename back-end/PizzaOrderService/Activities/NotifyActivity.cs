using Dapr.Workflow;
using IO.Ably;
using Shared.Models;

namespace OrderService.Activities
{
    public class NotifyActivity : WorkflowActivity<Notification, object?>
    {
        private readonly ILogger _logger;
        private readonly IRestClient _realtimeClient;

        public NotifyActivity(ILoggerFactory loggerFactory, IRestClient realtimeClient)
        {
            _logger = loggerFactory.CreateLogger<NotifyActivity>();
            _realtimeClient = realtimeClient;
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, Notification notification)
        {
            _logger.LogInformation(notification.Message);
            string channelName = $"pizza-notifications-{notification.Order.OrderId}";
            var _channel = _realtimeClient.Channels.Get(channelName);
            await _channel.PublishAsync(notification.Order.Status.ToString(), notification);

            return null;
        }
    }
}