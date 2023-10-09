import { ref, computed } from "vue";
import { defineStore, storeToRefs } from "pinia";
import type { Types } from "ably";
import { Realtime } from "ably/promises";
import type { PizzaWorkflow } from "@/types/PizzaWorkflow";
import OrderImage from "../assets/Order.png";
import PizzaAndDrinkImage from "../assets/PizzaAndDrink.png";
import PizzaInOvenImage from "../assets/PizzaInOven.png";
import BoxAndDrinkImage from "../assets/BoxAndDrink.png";
import DeliveryImage from "../assets/Delivery.png";
import DeliveredImage from "../assets/Map.gif";
import { PizzaType, type Order } from "@/types/Order";

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
      { type: PizzaType.Pepperoni, amount: 10 },
      { type: PizzaType.Margherita, amount: 0 },
      { type: PizzaType.Hawaiian, amount: 0 },
      { type: PizzaType.Vegetarian, amount: 0 }
    ],
    orderReceivedState: {
      messageSentTimeStampUTC: 0,
      messageReceivedTimestamp: 0,
      messageDeliveredTimestamp: 0,
      title: "Order Received",
      orderId: "",
      image: OrderImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
    kitchenInstructionsState: {
      messageSentTimeStampUTC: 0,
      messageReceivedTimestamp: 0,
      messageDeliveredTimestamp: 0,
      title: "Sending instructions to the kitchen",
      orderId: "",
      image: PizzaAndDrinkImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
    preparationState: {
      messageSentTimeStampUTC: 0,
      messageReceivedTimestamp: 0,
      messageDeliveredTimestamp: 0,
      title: "Preparing your pizza",
      orderId: "",
      image: PizzaInOvenImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
    collectionState: {
      messageSentTimeStampUTC: 0,
      messageReceivedTimestamp: 0,
      messageDeliveredTimestamp: 0,
      title: "Collecting your order",
      orderId: "",
      image: BoxAndDrinkImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
    deliveryState: {
      messageSentTimeStampUTC: 0,
      messageReceivedTimestamp: 0,
      messageDeliveredTimestamp: 0,
      title: "Delivering your order",
      orderId: "",
      image: DeliveryImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
    deliveredState: {
      messageSentTimeStampUTC: 0,
      messageReceivedTimestamp: 0,
      messageDeliveredTimestamp: 0,
      title: "Order is delivered",
      orderId: "",
      image: DeliveredImage,
      isVisible: false,
      isDisabled: true,
      isCurrentState: false,
    },
  }),
  actions: {
    async start(clientId: string, order: Order) {
      this.$reset();
      this.$state.clientId = clientId;
      this.$state.orderId = order.id;
      this.$state.disableOrdering = true;
      this.$state.orderReceivedState.isVisible = true;
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
        "receive-order",
        (message: Types.Message) => {
          this.handleOrderReceived(message);
        }
      );
      this.channelInstance?.subscribe(
        "send-instructions-to-kitchen",
        (message: Types.Message) => {
          this.handleSendInstructions(message);
        }
      );
      this.channelInstance?.subscribe(
        "prepare-pizza",
        (message: Types.Message) => {
          this.handlePreparePizza(message);
        }
      );
      this.channelInstance?.subscribe(
        "collect-order",
        (message: Types.Message) => {
          this.handleCollectOrder(message);
        }
      );
      this.channelInstance?.subscribe(
        "deliver-order",
        (message: Types.Message) => {
          this.handleDeliverOrder(message);
        }
      );
      this.channelInstance?.subscribe(
        "delivered-order",
        (message: Types.Message) => {
          this.handleDeliveredOrder(message);
        }
      );
    },

    handleOrderReceived(message: Types.Message) {
      this.$patch({
        orderReceivedState: {
          orderId: message.data.orderId,
          messageSentTimeStampUTC: message.data.messageSentTimeStampUTC,
          messageReceivedTimestamp: message.timestamp,
          messageDeliveredTimestamp: Date.now(),
          isDisabled: false,
          isCurrentState: true,
        },
        kitchenInstructionsState: {
          isVisible: true,
        },
      });
    },

    handleSendInstructions(message: Types.Message) {
      this.$patch({
        kitchenInstructionsState: {
          orderId: message.data.orderId,
          messageSentTimeStampUTC: message.data.messageSentTimeStampUTC,
          messageReceivedTimestamp: message.timestamp,
          messageDeliveredTimestamp: Date.now(),
          isDisabled: false,
          isCurrentState: true,
        },
        orderReceivedState: {
          isCurrentState: false,
        },
        preparationState: {
          isVisible: true,
        },
      });
    },

    handlePreparePizza(message: Types.Message) {
      this.$patch({
        preparationState: {
          orderId: message.data.orderId,
          messageSentTimeStampUTC: message.data.messageSentTimeStampUTC,
          messageReceivedTimestamp: message.timestamp,
          messageDeliveredTimestamp: Date.now(),
          isDisabled: false,
          isCurrentState: true,
        },
        kitchenInstructionsState: {
          isCurrentState: false,
        },
        collectionState: {
          isVisible: true,
        },
      });
    },

    handleCollectOrder(message: Types.Message) {
      this.$patch({
        collectionState: {
          orderId: message.data.orderId,
          messageSentTimeStampUTC: message.data.messageSentTimeStampUTC,
          messageReceivedTimestamp: message.timestamp,
          messageDeliveredTimestamp: Date.now(),
          isDisabled: false,
          isCurrentState: true,
        },
        preparationState: {
          isCurrentState: false,
        },
        deliveryState: {
          isVisible: true,
        },
      });
    },

    handleDeliverOrder(message: Types.Message) {
      this.$patch({
        deliveryState: {
          orderId: message.data.orderId,
          messageSentTimeStampUTC: message.data.messageSentTimeStampUTC,
          messageReceivedTimestamp: message.timestamp,
          messageDeliveredTimestamp: Date.now(),
          isDisabled: false,
          isCurrentState: true,
        },
        collectionState: {
          isCurrentState: false,
        },
        deliveredState: {
          isVisible: true,
        },
      });
    },

    handleDeliveredOrder(message: Types.Message) {
      this.$patch({
        deliveredState: {
          orderId: message.data.orderId,
          messageSentTimeStampUTC: message.data.messageSentTimeStampUTC,
          messageReceivedTimestamp: message.timestamp,
          messageDeliveredTimestamp: Date.now(),
          isDisabled: false,
          isCurrentState: true,
        },
        collectionState: {
          isCurrentState: false,
        },
        isWorkflowComplete: true,
      });
      setTimeout(() => {
        this.disableOrdering = false;
      }, 2000);
    },
  },
});
