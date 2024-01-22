import { ref, computed } from "vue";
import { defineStore, storeToRefs } from "pinia";
import type { Types } from "ably";
import { Realtime } from "ably/promises";
import type { PizzaWorkflow } from "@/types/PizzaWorkflow";
import OrderImage from "../assets/Order.png";
import InStockImage from "../assets/Inventory1.png";
import NotInStockImage from "../assets/Inventory2.png";
import OvenImage from "../assets/Oven.png";
import PizzaBoxImage from "../assets/PizzaBox.png";
import PizzaPepperoni from "../assets/Pizza1.png";
import PizzaHawaii from "../assets/Pizza2.png";
import PizzaVegetarian from "../assets/Pizza3.png";
import PizzaMargherita from "../assets/Pizza4.png";
import { PizzaType, type Order, type OrderItem, type Notification } from "@/types/Types";

export const pizzaProcessStore = defineStore("pizza-process", {
  state: (): PizzaWorkflow => ({
    realtimeClient: undefined,
    channelInstance: undefined,
    isConnected: false,
    channelPrefix: "pizza-notifications:",
    clientId: "",
    orderId: "",
    disableOrdering: true,
    disableAddPizza: false,
    isWorkflowComplete: false,
    isOrderPlaced: false,
    orderItems: [ 
      { PizzaType: PizzaType.Pepperoni, Image: PizzaPepperoni, Quantity: 0 },
      { PizzaType: PizzaType.Margherita, Image: PizzaMargherita, Quantity: 0 },
      { PizzaType: PizzaType.Hawaiian, Image: PizzaHawaii, Quantity: 0 },
      { PizzaType: PizzaType.Vegetarian, Image: PizzaVegetarian, Quantity: 0 },
    ],
    receivedOrderState: {
      Title: "",
      OrderId: "",
      Image: OrderImage,
      IsVisible: false,
      IsDisabled: true,
      IsCurrentState: false,
    },
    checkedInventoryState: {
      Title: "",
      OrderId: "",
      Image: InStockImage,
      IsVisible: false,
      IsDisabled: true,
      IsCurrentState: false,
    },
    insufficientInventoryState: {
      Title: "",
      OrderId: "",
      Image: NotInStockImage,
      IsVisible: false,
      IsDisabled: true,
      IsCurrentState: false,
    },
    restockedInventoryState: {
      Title: "",
      OrderId: "",
      Image: NotInStockImage,
      IsVisible: false,
      IsDisabled: true,
      IsCurrentState: false,
    },
    sentToKitchenState: {
      Title: "",
      OrderId: "",
      Image: OvenImage,
      IsVisible: false,
      IsDisabled: true,
      IsCurrentState: false,
    },
    completedPreparationState: {
      Title: "",
      OrderId: "",
      Image: PizzaBoxImage,
      IsVisible: false,
      IsDisabled: true,
      IsCurrentState: false,
    },
    unknownState: {
      Title: "Oops we don't know what happened!",
      OrderId: "",
      Image: NotInStockImage,
      IsVisible: false,
      IsDisabled: true,
      IsCurrentState: false,
    },
  }),
  actions: {
    incrementPizzaCount(pizza: PizzaType) {
      const pizzaIndex = this.orderItems.findIndex((item) => item.PizzaType === pizza);
      this.orderItems[pizzaIndex].Quantity++;
    },
    async start(clientId: string, order: Order) {
      this.$reset();
      this.$state.clientId = clientId;
      this.$state.orderId = order.OrderId;
      this.$state.disableAddPizza = true;
      this.$state.disableOrdering = true;
      this.$state.receivedOrderState.IsVisible = true;
      await this.createRealtimeConnection(clientId, order);
    },
    async createRealtimeConnection(clientId: string, order: Order) {
      const channelName = `${this.channelPrefix}${order.OrderId}`;
      if (!this.isConnected) {
        this.realtimeClient = new Realtime.Promise({
          authUrl: `api/getAblyToken?clientId=${clientId}`,
          echoMessages: false,
        });
        this.realtimeClient.connection.on(
          "connected",
          async (message: Types.ConnectionStateChange) => {
            this.isConnected = true;
            this.attachToChannel(channelName);
            if (!this.isOrderPlaced) {
              await this.placeOrder(order);
              this.$state.isOrderPlaced = true;
            }
          }
        );

        this.realtimeClient.connection.on("disconnected", () => {
          this.$state.isConnected = false;
        });
        this.realtimeClient.connection.on("closed", () => {
          this.$state.isConnected = false;
        });
      } else {
        this.attachToChannel(channelName);
      }
    },

    disconnect() {
      this.realtimeClient?.connection.close();
    },

    async placeOrder(order: Order) {
      const response = await window.fetch("/api/placeOrder", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(order),
      });
      if (response.ok) {
        const result = await response.json();
        this.$state.orderId = result.orderId;
      } else {
        this.$state.disableAddPizza = false;
        console.log(response.statusText);
      }
    },

    attachToChannel(channelName: string) {
      this.$state.channelInstance = this.realtimeClient?.channels.get(
        channelName,
        { params: { rewind: "2m" } }
      );
      this.subscribeToMessages();
    },

    subscribeToMessages() {
      this.channelInstance?.subscribe(
        "Received",
        (message: Types.Message) => {
          this.handleOrderReceived(message);
        }
      );
      this.channelInstance?.subscribe(
        "CheckedInventory",
        (message: Types.Message) => {
          this.handleSufficientInventory(message);
        }
      );
      this.channelInstance?.subscribe(
        "RestockedInventory",
        (message: Types.Message) => {
          this.handleRestockedInventory(message);
        }
      );
      this.channelInstance?.subscribe(
        "InsufficientInventory",
        (message: Types.Message) => {
          this.handleInsufficientInventory(message);
        }
      );
      this.channelInstance?.subscribe(
        "SentToKitchen",
        (message: Types.Message) => {
          this.handleSentToKitchen(message);
        }
      );
      this.channelInstance?.subscribe(
        "CompletedPreparation",
        (message: Types.Message) => {
          this.handleOrderCompleted(message);
        }
      );
    },

    handleOrderReceived(message: Types.Message) {
      this.$patch({
        receivedOrderState: {
          Title: message.data.Message,
          OrderId: message.data.Order.OrderId,
          IsDisabled: false,
          IsCurrentState: true,
        },
        checkedInventoryState: {
          IsVisible: true,
        },
      });
    },

    handleSufficientInventory(message: Types.Message) {
      this.$patch({
        checkedInventoryState: {
          Title: message.data.Message,
          OrderId: message.data.Order.OrderId,
          IsDisabled: false,
          IsCurrentState: true,
        },
        receivedOrderState: {
          IsCurrentState: false,
        },
        sentToKitchenState: {
          IsVisible: true,
        },
      });
    },

    handleInsufficientInventory(message: Types.Message) {
      this.$patch({
        insufficientInventoryState: {
          Title: message.data.Message,
          OrderId: message.data.Order.OrderId,
          IsDisabled: false,
          IsCurrentState: true,
        },
        restockedInventoryState: {
          IsVisible: true,
        },
      });
    },

    handleRestockedInventory(message: Types.Message) {
      this.$patch({
        restockedInventoryState: {
          Title: message.data.Message,
          OrderId: message.data.Order.OrderId,
          IsDisabled: false,
          IsCurrentState: true,
        },
        insufficientInventoryState: {
          IsCurrentState: false,
        },
        sentToKitchenState: {
          IsVisible: true,
        },
      });
    },

    handleSentToKitchen(message: Types.Message) {
      this.$patch({
        sentToKitchenState: {
          Title: message.data.Message,
          OrderId: message.data.Order.OrderId,
          IsDisabled: false,
          IsCurrentState: true,
        },
        checkedInventoryState: {
          IsCurrentState: false,
        },
        completedPreparationState: {
          IsVisible: true,
        },
      });
    },

    handleOrderCompleted(message: Types.Message) {
      this.$patch({
        completedPreparationState: {
          Title: message.data.Message,
          OrderId: message.data.Order.OrderId,
          IsDisabled: false,
          IsCurrentState: true,
        },
        sentToKitchenState: {
          IsCurrentState: false,
        },
        disableAddPizza: false,
      });
    },
  },
});
