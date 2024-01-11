<script setup lang="ts">
import PizzaSelectionItem from "./PizzaSelectionItem.vue";
import { pizzaProcessStore } from "../stores";
import { v4 as uuid } from "uuid";
import type { Order } from "@/types/Types";
import { storeToRefs } from "pinia";
import type { Types } from "ably/promises";

const store = pizzaProcessStore();
const { disableOrdering } = storeToRefs(store);

async function placeOrder() {
  const clientId = store.clientId === "" ? uuid() : store.clientId;
  const order = createOrder();
  console.log("Placing order", order);
  store.start(clientId, order);
}

function createOrder() {
  const today = new Date();
  const orderId = uuid();
  const order: Order = {
    OrderId: orderId,
    OrderDate: today,
    Customer: {
      Name: "Leonardo",
      Email: "leo@tmnt.shell",
    },
    OrderItems: store.orderItems
  };

  return order;
}

function createSimulatedMessage(order: Order) {
  const message = {
    clientId: "",
    connectionId: "",
    data: order,
    encoding: "",
    extras: "",
    id: "",
    name: "order-placed",
    timestamp: 1,
  } as Types.Message;

  return message;
}

async function simulateOrderPlaced() {
  const order = createOrder();
  const message = createSimulatedMessage(order);
  store.handleOrderReceived(message);
}

</script>

<template>
  <div class="greetings">
    <h1>
      Diagrid Pizza Store
    </h1>
    <div class="flex-row">
      <h3>
        Select some pizzas, place an order, and see the progress of the pizza workflow.
      </h3>
    </div>
    <div class="flex-row">
      <details>
        <summary>More info about the workflow...</summary>
        <p class="animate">
          The serverless workflow is implemented using
          <a
            href="https://docs.dapr.io/reference/api/workflow_api/"
          >
            Dapr Workflow API
          </a>.
          The <code>PizzaWorkflow</code> function calls 4 activity
          functions in sequence. Each of these functions publishes a message via
          websockets, which is received by this website, so you can see how far the
          workflow has progressed in real-time.
        </p>
      </details>
    </div>
    
    <PizzaSelectionItem />
    
    <div class="flex-center">
      <button @click="placeOrder" :disabled="disableOrdering">
        Place order
      </button>
    </div>
    <div class="flex-center">
      <button @click="simulateOrderPlaced">
        Simulate order placed
      </button>
    </div>
    
  </div>
</template>

<style scoped>
@import url("https://fonts.googleapis.com/css2?family=Comic+Neue&display=swap");

h1 {
  font-weight: bold;
  font-size: 2.6rem;
  top: -10px;
  color: var(--color-heading);
  border-bottom: 0.2rem solid var(--color-accent);
}

h3 {
  font-weight: normal;
  font-size: 1.2rem;
  color: var(--color-text);
}

button {
  background-color: var(--vt-c-green-light);
  border-color: var(--vt-c-green-dark);
  border-radius: 0.5rem;
  padding: 0.7rem;
  font-size: 1.2rem;
  margin-top: 1rem;
  font-family: 'Space Grotesk', sans-serif;
  transition: all 0.4s ease-out;
}

button:hover:enabled {
  box-shadow: 0px 0px 10px var(--vt-c-green-dark);
  transition: all 0.1s ease-out;
}

button:disabled {
  border-color: var(--vt-c-divider-dark-2);
  background-color: var(--vt-c-text-dark-2);
}

.greetings h1,
.greetings h3 {
  text-align: center;
}

.flex-row {
  margin-top: 10px;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  justify-content: flex-start;
}

.flex-center {
  margin-top: 10px;
  display: flex;
  justify-content: center;
  text-align: left;
}

@media (min-width: 1024px) {
  .greetings h1,
  .greetings h3 {
    text-align: left;
  }

  .flex-row {
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: flex-start;
  }

  .flex-center {
    display: flex;
    justify-content: center;
    text-align: left;
  }
}
</style>
