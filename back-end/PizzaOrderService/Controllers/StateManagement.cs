using System.Text.Json;
using Dapr.Client;
using OrderService.Models;

namespace OrderService.Controllers
{
    public class StateManagement
    {
        private readonly DaprClient _client;
        private static readonly string storeName = "statestore";

        public StateManagement(DaprClient client)
        {
            _client = client;
        }

        public async Task<List<PizzaQuantity>> GetPizzasAsync(PizzaType[] pizzaTypes)
        {
            var bulkStateItems = await _client.GetBulkStateAsync(
                storeName,
                pizzaTypes.Select(p => p.ToString()).ToList().AsReadOnly(),
                1);

            var pizzas = new List<PizzaQuantity>();
            foreach (var item in bulkStateItems)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    Console.WriteLine($"Got item from state store: {item.Value}");
                    var pizzaQuantity = JsonSerializer.Deserialize<PizzaQuantity>(item.Value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (pizzaQuantity == null)
                    {
                        throw new Exception($"Pizza {item.Key} not found in inventory!");
                    }
                    pizzas.Add(pizzaQuantity);
                }
                else {
                    pizzas.Add(new PizzaQuantity(Enum.Parse<PizzaType>(item.Key), 0));
                }
            }

            return pizzas;
        }

        public async Task SavePizzasAsync(IEnumerable<PizzaQuantity> pizzas)
        {
            var saveStateItems = new List<SaveStateItem<PizzaQuantity>>();

            foreach (var pizza in pizzas)
            {
                saveStateItems.Add(new SaveStateItem<PizzaQuantity>(
                    pizza.PizzaType.ToString(),
                    pizza,
                    null));
            };

            await _client.SaveBulkStateAsync(
                storeName,
                saveStateItems.AsReadOnly());
        }

        public async Task SaveOrderAsync(Order order)
        {
            await _client.SaveStateAsync(
                storeName,
                order.OrderId,
                order);
        }
    }
}