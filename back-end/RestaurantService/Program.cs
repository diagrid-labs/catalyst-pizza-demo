using Microsoft.AspNetCore.Mvc;
using RestaurantService;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

// var httpClient = new HttpClient {
//     BaseAddress = new Uri("http://localhost:3500/v1.0/")
// };

// builder.Services.AddSingleton<IRealtimeNotification, RealtimeNotification>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

var stateManagement = new StateManagement(new DaprClientBuilder().Build());

app.UseHttpsRedirection();

app.MapPost("/prepare", async () => {
    //Prepare order

});

app.Run();