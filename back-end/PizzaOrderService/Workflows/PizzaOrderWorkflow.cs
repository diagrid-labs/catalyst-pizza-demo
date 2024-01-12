using Dapr.Workflow;
using OrderService.Activities;
using Shared.Models;

namespace OrderService.Workflows
{
    public class PizzaOrderWorkflow : Workflow<Order, OrderResult>
    {
        public override async Task<OrderResult> RunAsync(WorkflowContext context, Order order)
        {
            Order updatedOrder;
            updatedOrder = order with { Status = OrderStatus.Received };
            
            await context.CallActivityAsync(
                nameof(SaveOrderActivity),
                updatedOrder);

            await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    new Notification($"Received order {order.ShortId} from {order.Customer.Name}.", updatedOrder));

            // Determine if there is enough of the item available for purchase by checking the inventory.
            var inventoryRequest = new InventoryRequest(order.OrderItems);
            var inventoryResult = await context.CallActivityAsync<InventoryResult>(
                nameof(CheckInventoryActivity),
                inventoryRequest);

            
            Notification inventoryNotification;
            if (!inventoryResult.IsSufficientInventory)
            {
                // End the workflow here since we don't have sufficient inventory.
                updatedOrder = updatedOrder with { Status = OrderStatus.CancelledLimitedInventory };
                inventoryNotification = new Notification($"Inventory is insufficient for {updatedOrder.ShortId}.", updatedOrder);

                return new OrderResult(OrderStatus.CancelledLimitedInventory, order, "Insufficient inventory");
            } else {
                updatedOrder = updatedOrder with { Status = OrderStatus.CheckedInventory };
                inventoryNotification = new Notification($"Inventory is sufficient for {updatedOrder.ShortId}.", updatedOrder);
            }

            await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    inventoryNotification);

            await context.CallActivityAsync(
                nameof(SendOrderToKitchenActivity),
                updatedOrder);

            updatedOrder = updatedOrder with { Status = OrderStatus.SentToKitchen };
            await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    new Notification($"Order {updatedOrder.ShortId} has been sent to the kitchen.", updatedOrder));

            var orderPreparedResult = await context.WaitForExternalEventAsync<bool>("order-prepared");

            if (orderPreparedResult) {
                updatedOrder = order with { Status = OrderStatus.CompletedPreparation };
                await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    new Notification($"Order {updatedOrder.ShortId} is completed for {updatedOrder.Customer.Name}!", updatedOrder));
            }
            else {
                updatedOrder = updatedOrder with { Status = OrderStatus.Unknown };
            }

            return new OrderResult(updatedOrder.Status, updatedOrder);
        }
    }
}