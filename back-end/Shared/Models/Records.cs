namespace Shared.Models
{

    public record OrderItem(PizzaType PizzaType, int Quantity)
    {
        public OrderItem() : this(default, 1)
        {
        }
    }

    public record Order(string OrderId, OrderItem[] OrderItems, DateTime OrderDate, Customer Customer, OrderStatus Status = OrderStatus.Received)
    {
        public string ShortId => OrderId.Substring(0, 8);
    }

    public record Customer(string Name, string Email);
    public record OrderResult(OrderStatus Status, Order Order, string? Message = null);
    public record InventoryRequest(OrderItem[] PizzasRequested);
    public record InventoryResult(bool IsSufficientInventory, OrderItem[] PizzasInStock);
    public record Notification(string Message, Order Order);
}