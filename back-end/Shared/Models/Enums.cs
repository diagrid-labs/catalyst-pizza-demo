using System.Text.Json.Serialization;

namespace Shared.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        Received = 0,
        CheckingInventory = 1,
        SufficientInventory = 2,
        InsufficientInventory = 3,
        RestockedInventory = 4,
        SentToKitchen = 5,
        CompletedPreparation = 6,
        Error = 7,
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