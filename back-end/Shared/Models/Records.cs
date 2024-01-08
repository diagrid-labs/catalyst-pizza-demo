namespace Shared.Models
{

    public record PizzaQuantity(PizzaType PizzaType, int Quantity)
    {
        public PizzaQuantity() : this(default, 1)
        {
        }
    }

    public record Order(string OrderId, PizzaQuantity[] PizzasRequested, DateTime OrderDate, Customer Customer, OrderStatus Status = OrderStatus.Created);
    public record Customer(string Name, string Email);
    public record OrderResult(OrderStatus Status, Order Order, string? Message = null);
    public record InventoryRequest(PizzaQuantity[] PizzasRequested);
    public record InventoryResult(bool IsSufficientInventory, PizzaQuantity[] PizzasInStock);
    public record Notification(string Message);
}