import { ref, computed } from "vue";
import { defineStore, storeToRefs } from "pinia";
import type { Types } from "ably";
import { Realtime } from "ably/promises";
import type { PizzaWorkflow } from "@/types/PizzaWorkflow";
import OrderImage from "../assets/Order.png";
import PizzaAndDrinkImage from "../assets/PizzaAndDrink.png";
import PizzaInOvenImage from "../assets/PizzaInOven.png";
import BoxAndDrinkImage from "../assets/BoxAndDrink.png";
import PizzaPepperoni from "../assets/PizzaPepperoni.png";
import DeliveryImage from "../assets/Delivery.png";
import DeliveredImage from "../assets/Map.gif";
import { type Pizza, PizzaType, type Order } from "@/types/Order";

export const pizzaProcessStore = defineStore("pizza-process", {
  state: (): PizzaWorkflow => ({
    realtimeClient: undefined,
    channelInstance: undefined,
    isConnected: false,
    channelPrefix: "pizza-process",
    clientId: "",
    orderId: "",
    disableOrdering: false,
    isWorkflowComplete: false,
    isOrderPlaced: false,
    orderItems:[ 
      { type: { name: PizzaType.Pepperoni, image: PizzaPepperoni}, amount: 5 },
      { type:{ name: PizzaType.Margherita, image: PizzaPepperoni}, amount: 0 },
      { type:{ name: PizzaType.Hawaiian, image: PizzaPepperoni}, amount: 0 },
      { type:{ name: PizzaType.Vegetarian, image: PizzaPepperoni}, amount: 0 },
    ],
    orderPlacedState: {
      title: "Order Received",
      orderId: "",
      image: OrderImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
    inStockState: {
      title: "Sending instructions to the kitchen",
      orderId: "",
      image: PizzaAndDrinkImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
    notInStockState: {
      title: "Preparing your pizza",
      orderId: "",
      image: PizzaInOvenImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
    inPreparationState: {
      title: "Collecting your order",
      orderId: "",
      image: BoxAndDrinkImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
    completedState: {
      title: "Your order is complete and can be collected.",
      orderId: "",
      image: DeliveryImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
  }),
  actions: {
    incrementPizzaCount(pizza: PizzaType) {
      const pizzaIndex = this.orderItems.findIndex((item) => item.type.name === pizza);
      this.orderItems[pizzaIndex].amount++;
    },
    async start(clientId: string, order: Order) {
      this.$reset();
      this.$state.clientId = clientId;
      this.$state.orderId = order.id;
      this.$state.disableOrdering = true;
      this.$state.orderPlacedState.isVisible = true;
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
            this.attachToChannel(order.id);
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
        this.attachToChannel(this.orderId);
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
        "order-placed",
        (message: Types.Message) => {
          this.handleOrderPlaced(message);
        }
      );
      this.channelInstance?.subscribe(
        "items-in-stock",
        (message: Types.Message) => {
          this.handleItemsInStock(message);
        }
      );
      this.channelInstance?.subscribe(
        "items-not-in-stock",
        (message: Types.Message) => {
          this.handleItemsNotInStock(message);
        }
      );
      this.channelInstance?.subscribe(
        "order-in-preparation",
        (message: Types.Message) => {
          this.handleOrderInPreperation(message);
        }
      );
      this.channelInstance?.subscribe(
        "order-completed",
        (message: Types.Message) => {
          this.handleOrderCompleted(message);
        }
      );
    },

    handleOrderPlaced(message: Types.Message) {
      this.$patch({
        orderPlacedState: {
          orderId: message.data.id,
          isDisabled: false,
          isCurrentState: true,
        },
        inStockState: {
          isVisible: true,
        },
      });
    },

    handleItemsInStock(message: Types.Message) {
      this.$patch({
        inStockState: {
          orderId: message.data.id,
          isDisabled: false,
          isCurrentState: true,
        },
        orderPlacedState: {
          isCurrentState: false,
        },
        notInStockState: {
          isVisible: true,
        },
      });
    },

    handleItemsNotInStock(message: Types.Message) {
      this.$patch({
        notInStockState: {
          orderId: message.data.id,
          isDisabled: false,
          isCurrentState: true,
        },
        inStockState: {
          isCurrentState: false,
        },
        inPreparationState: {
          isVisible: true,
        },
      });
    },

    handleOrderInPreperation(message: Types.Message) {
      this.$patch({
        inPreparationState: {
          orderId: message.data.id,
          isDisabled: false,
          isCurrentState: true,
        },
        notInStockState: {
          isCurrentState: false,
        },
        completedState: {
          isVisible: true,
        },
      });
    },

    handleOrderCompleted(message: Types.Message) {
      this.$patch({
        completedState: {
          orderId: message.data.id,
          isDisabled: false,
          isCurrentState: true,
        },
        inPreparationState: {
          isCurrentState: false,
        }
      });
    },
  },
});
