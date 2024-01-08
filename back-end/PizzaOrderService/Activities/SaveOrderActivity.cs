using Dapr.Workflow;
using OrderService.Controllers;
using Shared.Models;

namespace OrderService.Activities
{
    public class SaveOrderActivity : WorkflowActivity<Order, OrderResult>
    {
        readonly StateManagement _stateManagement;

        public SaveOrderActivity(StateManagement stateManagement)
        {
            _stateManagement = stateManagement;
        }

        public override async Task<OrderResult> RunAsync(WorkflowActivityContext context, Order order)
        {
            await _stateManagement.SaveOrderAsync(order);

            return new OrderResult(OrderStatus.Created, order);
        }
    }
}