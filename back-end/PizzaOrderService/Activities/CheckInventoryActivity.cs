using System.Text.Json;
using Dapr.Client;
using Dapr.Workflow;
using OrderService.Controllers;
using OrderService.Models;

namespace OrderService.Activities
{
    public class CheckInventoryActivity : WorkflowActivity<InventoryRequest, InventoryResult>
    {
        readonly ILogger _logger;
        readonly StateManagement _stateManagement;

        public CheckInventoryActivity(ILoggerFactory loggerFactory, StateManagement stateManagement)
        {
            _logger = loggerFactory.CreateLogger<CheckInventoryActivity>();
            _stateManagement = stateManagement;
        }

        public override async Task<InventoryResult> RunAsync(WorkflowActivityContext context, InventoryRequest req)
        {
            var pizzasToCheck = req.PizzasRequested.Select(p => p.PizzaType).ToArray();
            _logger.LogInformation($"Checking inventory for {pizzasToCheck[0]} pizzas.");
            
            var pizzasInStock = await _stateManagement.GetPizzasAsync(
                req.PizzasRequested.Select(p => p.PizzaType)
                .ToArray());

            bool isSufficientInventory = true;
            foreach (var pizza in pizzasInStock)
            {
                var firstPizza = req.PizzasRequested.First();
                _logger.LogInformation($"Checking inventory for {firstPizza.PizzaType} pizzas. Quantity in stock: {firstPizza.Quantity}. Quantity requested: {pizza.Quantity}.");

                if (isSufficientInventory && (pizza.Quantity < req.PizzasRequested.First(p => p.PizzaType == pizza.PizzaType).Quantity))
                {
                    isSufficientInventory = false;
                }
            }

            return new InventoryResult(isSufficientInventory, pizzasInStock.ToArray());
        }
    }
}