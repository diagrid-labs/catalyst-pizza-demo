using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Controllers;

namespace CheckoutService.Controllers;

[ApiController]
[Route("[controller]")]
public class InventoryController : ControllerBase
{
    private readonly ILogger<InventoryController> _logger;
    
    private readonly StateManagement _stateManagement;

    public InventoryController(ILogger<InventoryController> logger, StateManagement stateManagement)
    {
        _logger = logger;
        _stateManagement = stateManagement;
    }

    [HttpGet]
    public async Task<IActionResult> GetInventory()
    {
        var pizzas = await _stateManagement.GetPizzasAsync(
            new PizzaType[] {
                PizzaType.Pepperoni,
                PizzaType.Margherita,
                PizzaType.Hawaiian,
                PizzaType.Vegetarian });

        if (pizzas == null)
        {
            return new NotFoundResult();
        }
        return new OkObjectResult(pizzas);
    }

    [HttpPost("restock")]
    public async void RestockInventory()
    {
        const int Quantity = 20;
        var pizzas = GetPizzasWithQuantity(Quantity);
        await _stateManagement.SavePizzasAsync(pizzas);
        _logger.LogInformation($"Inventory Restocked to {Quantity} for all pizzas!");
    }

    [HttpDelete]
    public async void ClearInventory()
    {
        var pizzas = GetPizzasWithQuantity(0);
        await _stateManagement.SavePizzasAsync(pizzas);
        _logger.LogInformation("Cleared inventory!");
    }

    private List<PizzaQuantity> GetPizzasWithQuantity(int quantity)
    {
        var pizzas = new List<PizzaQuantity>() {
            new PizzaQuantity(PizzaType.Pepperoni, quantity),
            new PizzaQuantity(PizzaType.Margherita, quantity),
            new PizzaQuantity(PizzaType.Hawaiian, quantity),
            new PizzaQuantity(PizzaType.Vegetarian, quantity)
        };

        return pizzas;
    }
}
