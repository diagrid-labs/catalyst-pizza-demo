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

var stateManagement = new StateManagement(new DaprClientBuilder().Build());
var httpClient = new HttpClient {
    BaseAddress = new Uri("http://localhost:3500/v1.0-beta1/")
};

app.UseHttpsRedirection();

app.MapPost("/prepare", [Topic("pizza-pubsub", "pizza-orders")] async (Order order) => {
    Console.WriteLine("Restaurant received : " + order.OrderId);
    var updatedOrder = order with { Status = OrderStatus.Completed };
    await stateManagement.SaveOrderAsync(updatedOrder);
    var response = await httpClient.PostAsync(
        $"workflows/dapr/{order.OrderId}/raiseEvent/order-prepared",
        new StringContent("true",
        Encoding.UTF8,
        "application/text"));
    
    if (response.IsSuccessStatusCode)
    {
        return Results.Ok(new { status = "SUCCESS" });
    }
    else
    {
        Console.WriteLine($"Workflow raise event status: {response.StatusCode} {response.ReasonPhrase}");
        return Results.BadRequest(new { status = "FAILED", reason = response.ReasonPhrase });
    }
});

app.Run();