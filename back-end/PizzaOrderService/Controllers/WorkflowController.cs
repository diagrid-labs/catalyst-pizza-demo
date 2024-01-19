using Dapr;
using Dapr.Workflow;
using Microsoft.AspNetCore.Mvc;
using OrderService.Workflows;
using Shared.Models;

namespace CheckoutService.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkflowController : ControllerBase
{
    private readonly ILogger<InventoryController> _logger;
    
    private readonly DaprWorkflowClient _daprWorkflowClient;

    public WorkflowController(ILogger<InventoryController> logger, DaprWorkflowClient daprWorkflowClient)
    {
        _logger = logger;
        _daprWorkflowClient = daprWorkflowClient;
    }

    [HttpPost("orderReceived")]
    [Topic("pubsub", "received-orders")]
    public async Task<IResult> StartPizzaWorkflow([FromBody] Order order)
    {
        _logger.LogInformation($"Received message to start pizza workflow with ID {order.OrderId}.");
        var instanceId = await _daprWorkflowClient.ScheduleNewWorkflowAsync(
            nameof(PizzaOrderWorkflow),
            order.OrderId,
            order);

        return Results.Ok(instanceId);
    }

    [HttpPost("orderPrepared")]
    [Topic("pubsub", "prepared-orders")]
    public async Task<IResult> RaiseOrderPreparedEvent([FromBody] Order order)
    {
        var instanceId = order.OrderId;
        _logger.LogInformation($"Received message to raise order-prepared event for {instanceId}.");
        await _daprWorkflowClient.RaiseEventAsync(
            instanceId,
            "order-prepared",
            true);

        return Results.Ok();
    }
}
