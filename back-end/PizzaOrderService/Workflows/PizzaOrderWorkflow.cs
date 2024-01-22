using Dapr.Workflow;
using OrderService.Activities;
using Shared.Models;

namespace OrderService.Workflows
{
    public class PizzaOrderWorkflow : Workflow<Order, OrderResult>
    {
        public override async Task<OrderResult> RunAsync(WorkflowContext context, Order order)
        {
            var updatedOrder = order with { Status = OrderStatus.Received };
            
            await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    new Notification($"Received order {order.ShortId} from {order.Customer.Name}.", updatedOrder));

            await context.CallActivityAsync(
                nameof(SaveOrderActivity),
                updatedOrder);

            updatedOrder = order with { Status = OrderStatus.CheckingInventory };
            var checkInventoryNotification = new Notification($"Checking inventory for order {updatedOrder.ShortId}.", updatedOrder);
                await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    checkInventoryNotification);

            // Determine if there is enough of the item available for purchase by checking the inventory.
            var inventoryRequest = new InventoryRequest(order.OrderItems);
            var inventoryResult = await context.CallActivityAsync<InventoryResult>(
                nameof(CheckInventoryActivity),
                inventoryRequest);

            if (!inventoryResult.IsSufficientInventory)
            {
                updatedOrder = updatedOrder with { Status = OrderStatus.InsufficientInventory };
                var insufficientInventoryNotification = new Notification($"Inventory is insufficient for order {updatedOrder.ShortId}.", updatedOrder);
                await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    insufficientInventoryNotification);

                await context.CallActivityAsync(
                    nameof(RestockInventory),
                    null);
                
                updatedOrder = updatedOrder with { Status = OrderStatus.RestockedInventory };
                var restockedInventoryNotification = new Notification($"Inventory is restocked.", updatedOrder);
                await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    restockedInventoryNotification);
            } 

            updatedOrder = updatedOrder with { Status = OrderStatus.SufficientInventory };
            var inventoryNotification = new Notification($"Inventory is sufficient for order {updatedOrder.ShortId}.", updatedOrder);
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
                updatedOrder = updatedOrder with { Status = OrderStatus.Error };
            }

            return new OrderResult(updatedOrder.Status, updatedOrder);
        }
    }
}