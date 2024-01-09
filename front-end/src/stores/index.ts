import { ref, computed } from "vue";
import { defineStore, storeToRefs } from "pinia";
import type { Types } from "ably";
import { Realtime } from "ably/promises";
import type { PizzaWorkflow } from "@/types/PizzaWorkflow";
import OrderImage from "../assets/Order.png";
import PizzaInOvenImage from "../assets/PizzaInOven.png";
import PizzaBoxImage from "../assets/PizzaBox.png";
import PizzaPepperoni from "../assets/Pizza1.png";
import PizzaHawaii from "../assets/Pizza2.png";
import PizzaVegetarian from "../assets/Pizza3.png";
import PizzaMargherita from "../assets/Pizza4.png";
import { type Pizza, PizzaType, type Order, type Notification } from "@/types/Types";

export const pizzaProcessStore = defineStore("pizza-process", {
  state: (): PizzaWorkflow => ({
    realtimeClient: undefined,
    channelInstance: undefined,
    isConnected: false,
    channelPrefix: "pizza-notifications",
    clientId: "",
    orderId: "",
    disableOrdering: false,
    isWorkflowComplete: false,
    isOrderPlaced: false,
    orderItems:[ 
      { pizzaType: { name: PizzaType.Pepperoni, image: PizzaPepperoni}, quantity: 5 },
      { pizzaType:{ name: PizzaType.Margherita, image: PizzaMargherita}, quantity: 0 },
      { pizzaType:{ name: PizzaType.Hawaiian, image: PizzaHawaii}, quantity: 0 },
      { pizzaType:{ name: PizzaType.Vegetarian, image: PizzaVegetarian}, quantity: 0 },
    ],
    receivedOrderState: {
      title: "Order Received",
      orderId: "",
      image: OrderImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
    checkedInventoryState: {
      title: "Pizzas are in stock",
      orderId: "",
      image: OrderImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
    insufficientInventoryState: {
      title: "Pizzas are not in stock",
      orderId: "",
      image: OrderImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
    sentToKitchenState: {
      title: "Preparing your order",
      orderId: "",
      image: PizzaInOvenImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
    completedPreparationState: {
      title: "Order is complete and can be collected.",
      orderId: "",
      image: PizzaBoxImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
    cancelledLimitedInventoryState: {
      title: "Order cancelled due to limited inventory",
      orderId: "",
      image: OrderImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
  }),
  actions: {
    incrementPizzaCount(pizza: PizzaType) {
      const pizzaIndex = this.orderItems.findIndex((item) => item.pizzaType.name === pizza);
      this.orderItems[pizzaIndex].quantity++;
    },
    async start(clientId: string, order: Order) {
      this.$reset();
      this.$state.clientId = clientId;
      this.$state.orderId = order.orderId;
      this.$state.disableOrdering = true;
      this.$state.receivedOrderState.isVisible = true;
      await this.createRealtimeConnection(clientId, order);
    },
    async createRealtimeConnection(clientId: string, order: Order) {
      if (!this.isConnected) {
        this.realtimeClient = new Realtime.Promise({
          authUrl: `/api/CreateTokenRequest/${clientId}`,
          echoMessages: false,
        });
        this.realtimeClient.connection.on(
          "connected",
          async (message: Types.ConnectionStateChange) => {
            this.isConnected = true;
            this.attachToChannel(`${this.channelPrefix}-${order.orderId}`);
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
        this.attachToChannel(`${this.channelPrefix}-${this.orderId}`);
      }
    },

    disconnect() {
      this.realtimeClient?.connection.close();
    },

    async placeOrder(order: Order) {
      const response = await window.fetch("/api/PlaceOrder", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(order),
      });
      if (response.ok) {
        const payload = await response.json();
        this.$state.orderId = payload.result;
        console.log(`Order ID: ${this.orderId}`);
      } else {
        this.$state.disableOrdering = false;
        console.log(response.statusText);
      }
    },

    attachToChannel(orderId: string) {
      const channelName = `pizza-workflow:${orderId}`;
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
        "CancelledLimitedInventory",
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
          orderId: message.data.order.orderId,
          isDisabled: false,
          isCurrentState: true,
        },
        checkedInventoryState: {
          isVisible: true,
        },
      });
    },

    handleSufficientInventory(message: Types.Message) {
      this.$patch({
        checkedInventoryState: {
          orderId: message.data.order.orderId,
          isDisabled: false,
          isCurrentState: true,
        },
        receivedOrderState: {
          isCurrentState: false,
        },
        insufficientInventoryState: {
          isVisible: true,
        },
      });
    },

    handleInsufficientInventory(message: Types.Message) {
      this.$patch({
        insufficientInventoryState: {
          orderId: message.data.order.orderId,
          isDisabled: false,
          isCurrentState: true,
        },
        checkedInventoryState: {
          isCurrentState: false,
        },
        sentToKitchenState: {
          isVisible: true,
        },
      });
    },

    handleSentToKitchen(message: Types.Message) {
      this.$patch({
        sentToKitchenState: {
          orderId: message.data.id,
          isDisabled: false,
          isCurrentState: true,
        },
        insufficientInventoryState: {
          isCurrentState: false,
        },
        completedPreparationState: {
          isVisible: true,
        },
      });
    },

    handleOrderCompleted(message: Types.Message) {
      this.$patch({
        completedPreparationState: {
          orderId: message.data.id,
          isDisabled: false,
          isCurrentState: true,
        },
        sentToKitchenState: {
          isCurrentState: false,
        }
      });
    },
  },
});
