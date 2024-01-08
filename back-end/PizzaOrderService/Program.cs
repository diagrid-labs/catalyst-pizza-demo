using Dapr.Workflow;
using OrderService.Workflows;
using OrderService.Activities;
using OrderService.Controllers;
using IO.Ably;

var builder = WebApplication.CreateBuilder(args);

var httpClient = new HttpClient {
    BaseAddress = new Uri("http://localhost:3500/v1.0/")
};
var response = await httpClient.GetAsync("secrets/localsecretstore/AblyApiKey");
var secrets = await response.Content.ReadFromJsonAsync<Secrets>();
builder.Services.AddSingleton<IRestClient>(new AblyRest(secrets?.AblyApiKey));

builder.Services.AddControllers();
builder.Services.AddDaprClient();
builder.Services.AddDaprWorkflowClient();
builder.Services.AddSingleton<StateManagement>();

builder.Services.AddDaprWorkflow(options =>
{
    // Note that it's also possible to register a lambda function as the workflow
    // or activity implementation instead of a class.
    options.RegisterWorkflow<PizzaOrderWorkflow>();

    // These are the activities that get invoked by the workflow(s).
    options.RegisterActivity<NotifyActivity>();
    options.RegisterActivity<SaveOrderActivity>();
    options.RegisterActivity<CheckInventoryActivity>();
    options.RegisterActivity<SendOrderToRestaurantActivity>();
});

// Dapr uses a random port for gRPC by default. If we don't know what that port
// is (because this app was started separate from dapr), then assume 50001.
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DAPR_GRPC_PORT")))
{
    Environment.SetEnvironmentVariable("DAPR_GRPC_PORT", "50001");
}

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCloudEvents();
app.MapSubscribeHandler();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


class Secrets
{
    public string? AblyApiKey { get; set; }
}