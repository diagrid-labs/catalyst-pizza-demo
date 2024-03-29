using System.Text.Json;
using Dapr.Client;
using Shared.Models;

namespace KitchenService
{
    public class StateManagement
    {
        private readonly DaprClient _client;
        private static readonly string StoreName = "kvstore";

        public StateManagement(DaprClient client)
        {
            _client = client;
        }

        public async Task<List<OrderItem>> GetPizzaInventoryAsync(PizzaType[] pizzaTypes)
        {
            var bulkStateItems = await _client.GetBulkStateAsync(
                StoreName,
                pizzaTypes.Select(p => FormatKey(nameof(PizzaType), p.ToString())).ToList().AsReadOnly(),
                1);

            var pizzas = new List<OrderItem>();
            foreach (var item in bulkStateItems)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    var orderItem = JsonSerializer.Deserialize<OrderItem>(
                        item.Value,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (orderItem == null)
                    {
                        throw new Exception($"Pizza {item.Key} not found in inventory!");
                    }
                    pizzas.Add(orderItem);
                }
                else {
                    pizzas.Add(new OrderItem(Enum.Parse<PizzaType>(GetPizzaTypeFromKey(item.Key)), 0));
                }
            }

            return pizzas;
        }

        public async Task UpdatePizzaInventoryAsync(IEnumerable<OrderItem> pizzas)
        {
            Console.WriteLine($"Updating inventory for {string.Join(',', pizzas.Select(p => p.PizzaType.ToString()))}.");
            var pizzasInStore = await GetPizzaInventoryAsync(pizzas.Select(p => p.PizzaType).ToArray());
            var saveStateItems = new List<SaveStateItem<OrderItem>>();

            foreach (var pizza in pizzas)
            {
                var pizzaInStore = pizzasInStore.First(p => p.PizzaType == pizza.PizzaType);
                var updatedQuantity = pizzaInStore.Quantity - pizza.Quantity;
                var updatedItem = pizzaInStore with { Quantity = updatedQuantity };
                var stateKey = FormatKey(nameof(PizzaType), pizza.PizzaType.ToString());

                await _client.SaveStateAsync(
                    StoreName,
                    stateKey,
                    updatedItem);
            };
        }

        public async Task SaveOrderAsync(Order order)
        {
            Console.WriteLine($"Saving order {order.OrderId} with status {order.Status}.");
            await _client.SaveStateAsync(
                StoreName,
                FormatKey(nameof(Order), order.OrderId),
                order);
        }
        private static string FormatKey(string typeName, string key)
        {
            return $"{typeName}-{key}";
        }

        private static string GetPizzaTypeFromKey(string key)
        {
            return key.Split("-")[1];
        }

    }
}