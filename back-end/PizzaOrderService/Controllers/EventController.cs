using Dapr;
using Dapr.Client;
using Dapr.Workflow;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace CheckoutService.Controllers;

[ApiController]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly ILogger<InventoryController> _logger;
    
    private readonly DaprWorkflowClient _daprWorkflowClient;

    public EventController(ILogger<InventoryController> logger, DaprWorkflowClient daprWorkflowClient)
    {
        _logger = logger;
        _daprWorkflowClient = daprWorkflowClient;
    }

    [HttpPost("orderPrepared")]
    [Topic("pizza-pubsub", "prepared-orders")]
    public async Task<IResult> RaiseOrderPreparedEvent([FromBody] Order order)
    {
        var instanceId = order.OrderId;
        _logger.LogInformation($"Received message to raise order-prepared event for {instanceId}.");
        await _daprWorkflowClient.RaiseEventAsync(instanceId, "order-prepared", true);

        return Results.Ok();
    }
}
