using RestaurantService;
using Dapr;
using Dapr.Client;
using Shared.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseCloudEvents();
app.MapSubscribeHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

var daprClient = new DaprClientBuilder().Build();
var stateManagement = new StateManagement(daprClient);

app.UseHttpsRedirection();

app.MapPost("/prepare", [Topic("pizza-pubsub", "pizza-orders")] async (Order order) => {
    Console.WriteLine("Restaurant received: " + order.OrderId);
    Thread.Sleep(2000);
    var updatedOrder = order with { Status = OrderStatus.Completed };
    await stateManagement.SaveOrderAsync(updatedOrder);
    await daprClient.PublishEventAsync("pizza-pubsub", "prepared-orders", updatedOrder);
});

app.Run();