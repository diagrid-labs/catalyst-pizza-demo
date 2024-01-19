using Dapr.Workflow;
using OrderService.Workflows;
using OrderService.Activities;
using OrderService.Controllers;
using IO.Ably;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

var ablyApiKey = Environment.GetEnvironmentVariable("ABLY_API_KEY");
builder.Services.AddSingleton<IRestClient>(new AblyRest(ablyApiKey));

var apiToken = Environment.GetEnvironmentVariable("DAPR_API_TOKEN");
Console.WriteLine($"DAPR_API_TOKEN: {apiToken}");
var grpcEndpoint = Environment.GetEnvironmentVariable("DAPR_GRPC_ENDPOINT");
Console.WriteLine($"DAPR_GRPC_ENDPOINT: {grpcEndpoint}");
var httpEndpoint = Environment.GetEnvironmentVariable("DAPR_HTTP_ENDPOINT");
Console.WriteLine($"DAPR_HTTP_ENDPOINT: {httpEndpoint}");

builder.Services.AddControllers();
builder.Services.AddDaprClient(options => {
    options.UseDaprApiToken(apiToken);
    options.UseGrpcEndpoint(grpcEndpoint);
    options.UseHttpEndpoint(httpEndpoint);
});

builder.Services.AddDaprWorkflowClient();
builder.Services.AddSingleton<StateManagement>();
builder.Services.AddDaprWorkflow(options =>
{
    options.RegisterWorkflow<PizzaOrderWorkflow>();
    options.RegisterActivity<NotifyActivity>();
    options.RegisterActivity<SaveOrderActivity>();
    options.RegisterActivity<CheckInventoryActivity>();
    options.RegisterActivity<SendOrderToKitchenActivity>();
});

// Dapr uses a random port for gRPC by default. If we don't know what that port
// is (because this app was started separate from dapr), then assume 50001.
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DAPR_GRPC_PORT")))
{
    Environment.SetEnvironmentVariable("DAPR_GRPC_PORT", "50001");
}

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