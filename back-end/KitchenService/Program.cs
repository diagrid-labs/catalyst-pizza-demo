using KitchenService;
using Dapr;
using Dapr.Client;
using Shared.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseCloudEvents();
app.MapSubscribeHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

var daprHttpEndpoint = Environment.GetEnvironmentVariable("DAPR_HTTP_ENDPOINT");
var daprGrpcEndpoint = Environment.GetEnvironmentVariable("DAPR_GRPC_ENDPOINT");
var daprApiToken = Environment.GetEnvironmentVariable("DAPR_API_TOKEN");
var daprClient = new DaprClientBuilder()
    .UseGrpcEndpoint(daprGrpcEndpoint)
    .UseHttpEndpoint(daprHttpEndpoint)
    .UseDaprApiToken(daprApiToken)
    .Build();
var stateManagement = new StateManagement(daprClient);

app.UseHttpsRedirection();

app.MapPost("/prepare", [Topic("pubsub", "pizza-orders")] async (Order order) => {
    Console.WriteLine("Kitchen received: " + order.OrderId);
    await stateManagement.UpdatePizzaInventoryAsync(order.OrderItems);
    Thread.Sleep(2000);
    var updatedOrder = order with { Status = OrderStatus.CompletedPreparation };
    await stateManagement.SaveOrderAsync(updatedOrder);
    await daprClient.PublishEventAsync("pubsub", "prepared-orders", updatedOrder);
});

app.Run();