using Microsoft.AspNetCore.Mvc;
using RestaurantService;
using Dapr;
using Dapr.Client;
using Dapr.AspNetCore;
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

var stateManagement = new StateManagement(new DaprClientBuilder().Build());

app.UseHttpsRedirection();

app.MapPost("/prepare", [Topic("pizza-pubsub", "pizza-orders")] async (Order order) => {
    Console.WriteLine("RestaurantService received : " + order.OrderId);
    var updatedOrder = order with { Status = OrderStatus.Completed };
    await stateManagement.SaveOrderAsync(updatedOrder);
    return Results.Ok(new { status = "SUCCESS" });
});

app.Run();