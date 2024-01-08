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
                    new Notification($"Created order for {order.Customer.Name}", order));

            // Determine if there is enough of the item available for purchase by checking the inventory.
            var inventoryRequest = new InventoryRequest(order.PizzasRequested);
            var inventoryResult = await context.CallActivityAsync<InventoryResult>(
                nameof(CheckInventoryActivity),
                inventoryRequest);

            Order updatedOrder;
            Notification inventoryNotification;
            if (!inventoryResult.IsSufficientInventory)
            {
                // End the workflow here since we don't have sufficient inventory.
                updatedOrder = order with { Status = OrderStatus.Cancelled };
                inventoryNotification = new Notification($"Inventory is insufficient for {order.Customer.Name}", updatedOrder);

                return new OrderResult(OrderStatus.Cancelled, order, "Insufficient inventory");
            } else {
                inventoryNotification = new Notification($"Inventory is sufficient for {order.Customer.Name}", order);
            }

            await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    inventoryNotification);

            await context.CallActivityAsync(
                nameof(SendOrderToRestaurantActivity),
                order);

            var orderPreparedResult = await context.WaitForExternalEventAsync<bool>("order-prepared");

            if (orderPreparedResult) {
                updatedOrder = order with { Status = OrderStatus.Completed };
                await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    new Notification($"Order {orderId} has completed for {updatedOrder.Customer.Name}!", order));
            }
            else {
                updatedOrder = order with { Status = OrderStatus.Cancelled };
            }

            return new OrderResult(updatedOrder.Status, updatedOrder);
        }
    }
}