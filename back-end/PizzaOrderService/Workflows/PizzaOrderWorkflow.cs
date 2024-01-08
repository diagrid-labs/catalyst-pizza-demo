using Dapr.Workflow;
using OrderService.Activities;
using Shared.Models;

namespace OrderService.Workflows
{
    public class PizzaOrderWorkflow : Workflow<Order, OrderResult>
    {
        public override async Task<OrderResult> RunAsync(WorkflowContext context, Order order)
        {
            string orderId = context.InstanceId;

            await context.CallActivityAsync(
                nameof(SaveOrderActivity),
                order);

            await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    new Notification($"Created order for {order.Customer.Name}"));

            // Determine if there is enough of the item available for purchase by checking the inventory.
            var inventoryRequest = new InventoryRequest(order.PizzasRequested);
            var inventoryResult = await context.CallActivityAsync<InventoryResult>(
                nameof(CheckInventoryActivity),
                inventoryRequest);

            // If there is insufficient inventory, inform the user and stop the workflow.
            if (!inventoryResult.IsSufficientInventory)
            {
                // End the workflow here since we don't have sufficient inventory.
                await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    new Notification($"Insufficient inventory for {order.Customer.Name}"));

                return new OrderResult(OrderStatus.Cancelled, order, "Insufficient inventory");
            }

            // await context.CallActivityAsync(
            //     nameof(UpdateInventoryActivity),
            //     inventoryRequest);

            await context.CallActivityAsync(
                nameof(NotifyActivity),
                new Notification($"Order {orderId} has completed for {order.Customer.Name}!"));

            return new OrderResult(OrderStatus.Completed, order);
        }
    }
}