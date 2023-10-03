<script setup lang="ts">
import FlagIcon from "./icons/FlagIcon.vue";
import { pizzaProcessStore } from "../stores";
import { v4 as uuid } from "uuid";
import { PizzaType, type Order } from "@/types/Order";
import { storeToRefs } from "pinia";

const store = pizzaProcessStore();
const { disableOrdering } = storeToRefs(store);

async function placeOrder() {
  const clientId = store.clientId === "" ? uuid() : store.clientId;
  const today = new Date();
  const orderId = uuid();
  const order: Order = {
    id: orderId,
    dateTime: today,
    customerName: "Ada",
    customerEmail: "ada@wantspizza.now",
    orderItems: [
      {
        type: PizzaType.Margarita,
        amount: 1,
      },
      {
        type: PizzaType.Hawaii,
        amount: 1,
      },
    ],
  };
  store.start(clientId, order);
}

</script>

<template>
  <div class="greetings">
    <h1>
      <FlagIcon />
      Diagrid Pizza Store
      <FlagIcon />
    </h1>
    <div class="flex-row">
      <h3>
        Place an order and see the progress of the pizza workflow.
      </h3>
      <button @click="placeOrder" :disabled="disableOrdering">
        Place order
      </button>
    </div>
    <div class="flex-center">
      <details>
        <summary>More info about the workflow...</summary>
        <p class="animate">
          The serverless workflow is implemented using
          <a
            href="https://docs.dapr.io/reference/api/workflow_api/"
          >
            Dapr Workflow API
          </a>.
          The <code>PizzaWorkflow</code> function calls 6 activity
          functions in sequence. Each of these functions publishes a message via
          websockets, which is received by this website, so you can see how far the
          workflow has progressed in real-time.
        </p>
      </details>
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
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

details {
  cursor: pointer;
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
    justify-content: center;
  }

  .flex-center {
    display: flex;
    justify-content: left;
    text-align: left;
  }
}
</style>
