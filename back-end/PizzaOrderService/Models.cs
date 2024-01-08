using System.Text.Json.Serialization;

namespace OrderService.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        Created = 0,
        Processing = 1,
        Completed = 2,
        Cancelled = 3
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PizzaType
    {
        Pepperoni = 1,
        Margherita = 2,
        Hawaiian = 3,
        Vegetarian = 4,
    }

    public record Order(string OrderId, PizzaQuantity[] Pizzas, DateTime OrderDate, Customer Customer, OrderStatus Status = OrderStatus.Created);
    public record Customer(string Name, string Email);
    
    public class PizzaQuantity
    {
        public PizzaQuantity()
        {
        }

        public PizzaQuantity(PizzaType pizzaType, int quantity)
        {
            PizzaType = pizzaType;
            Quantity = quantity;
        }

        public PizzaType PizzaType {get; set;}
        public int Quantity {get; set;}
    }
    public record OrderResult(OrderStatus Status, Order Order, string? Message = null);
    public record InventoryRequest(PizzaQuantity[] PizzasRequested);
    public record InventoryResult(bool IsSufficientInventory, PizzaQuantity[] PizzasInStock);
    public record Notification(string Message);
}