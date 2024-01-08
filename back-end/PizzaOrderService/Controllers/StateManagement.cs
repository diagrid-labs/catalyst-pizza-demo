using System.Text.Json;
using Dapr.Client;
using Shared.Models;

namespace OrderService.Controllers
{
    public class StateManagement
    {
        private readonly DaprClient _client;
        private static readonly string storeName = "pizza-statestore";

        public StateManagement(DaprClient client)
        {
            _client = client;
        }

        public async Task<List<OrderItem>> GetPizzasAsync(PizzaType[] pizzaTypes)
        {
            var bulkStateItems = await _client.GetBulkStateAsync(
                storeName,
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

        public async Task SavePizzasAsync(IEnumerable<OrderItem> pizzas)
        {
            var saveStateItems = new List<SaveStateItem<OrderItem>>();

            foreach (var pizza in pizzas)
            {
                saveStateItems.Add(new SaveStateItem<OrderItem>(
                    FormatKey(nameof(PizzaType), pizza.PizzaType.ToString()),
                    pizza,
                    null));
            };

            await _client.SaveBulkStateAsync(
                storeName,
                saveStateItems.AsReadOnly());
        }

        public async Task SaveOrderAsync(Order order)
        {
            Console.WriteLine($"Saving order {order.OrderId} with status {order.Status}.");
            await _client.SaveStateAsync(
                storeName,
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