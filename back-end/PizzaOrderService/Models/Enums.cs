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
}