using Dapr.Workflow;
using OrderService.Controllers;
using Shared.Models;

namespace OrderService.Activities
{
    class UpdateInventoryActivity : WorkflowActivity<InventoryRequest, object?>
    {
        readonly ILogger _logger;
        readonly StateManagement _stateManagement;

        public UpdateInventoryActivity(ILoggerFactory loggerFactory, StateManagement stateManagement)
        {
            _logger = loggerFactory.CreateLogger<UpdateInventoryActivity>();
            _stateManagement = stateManagement;
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, InventoryRequest req)
        {
           var pizzasInStock = await _stateManagement.GetPizzasAsync(
                req.PizzasRequested.Select(p => p.PizzaType)
                .ToArray());

            var updatedPizzaList = new List<PizzaQuantity>();
            foreach (var pizza in pizzasInStock)
            {
                var updatedPizza = new PizzaQuantity(pizza.PizzaType, pizza.Quantity - req.PizzasRequested.First(p => p.PizzaType == pizza.PizzaType).Quantity);
                updatedPizzaList.Add(pizza);
            }

            await _stateManagement.SavePizzasAsync(updatedPizzaList);

            return null;
        }
    }
}