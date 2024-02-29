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
var grpcEndpoint = Environment.GetEnvironmentVariable("DAPR_GRPC_ENDPOINT");
var httpEndpoint = Environment.GetEnvironmentVariable("DAPR_HTTP_ENDPOINT");
var appId = Environment.GetEnvironmentVariable("DAPR_APP_ID");

builder.Services.AddControllers();
builder.Services.AddHttpClient(
    "daprEndpoint", 
    client => {
        client.BaseAddress = new Uri(httpEndpoint);
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("dapr-app-id", appId);
        client.DefaultRequestHeaders.Add("dapr-api-token", apiToken);
    });
builder.Services.AddDaprClient(options => {
    options.UseDaprApiToken(apiToken);
    options.UseGrpcEndpoint(grpcEndpoint);
    options.UseHttpEndpoint(httpEndpoint);
});

builder.Services.AddSingleton<StateManagement>();
builder.Services.AddDaprWorkflowClient();
builder.Services.AddDaprWorkflow(options =>
{
    options.RegisterWorkflow<PizzaOrderWorkflow>();
    options.RegisterActivity<NotifyActivity>();
    options.RegisterActivity<SaveOrderActivity>();
    options.RegisterActivity<CheckInventoryActivity>();
    options.RegisterActivity<SendOrderToKitchenActivity>();
    options.RegisterActivity<RestockInventory>();
});

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