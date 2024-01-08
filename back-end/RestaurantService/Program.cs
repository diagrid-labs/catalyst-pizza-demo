using Microsoft.AspNetCore.Mvc;

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

app.UseHttpsRedirection();

app.MapPost("/prepare", async () => {
    //Prepare order

});

app.Run();