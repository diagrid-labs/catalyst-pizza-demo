using System.Text.Json.Serialization;

namespace Shared.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        Received = 0,
        CheckedInventory = 1,
        SentToKitchen = 2,
        CompletedPreparation = 3,
        CancelledLimitedInventory = 4,
        Unknown = 5
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