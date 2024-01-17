<script setup lang="ts">
import { storeToRefs } from "pinia";
import { pizzaProcessStore } from "../stores";
import { PizzaType, type OrderItem } from '@/types/Types';
import type { ButtonHTMLAttributes } from "vue";

export interface OrderItemInterface {
  state: OrderItem;
}

const props = defineProps<OrderItemInterface>();

const store = pizzaProcessStore();
const { disableAddPizza } = storeToRefs(store);

function addPizza() {
  store.disableOrdering = false;
  store.incrementPizzaCount(props.state.PizzaType);
}

function logMessage(event: MouseEvent) {
  var el = event.target as HTMLInputElement;
  if (el.disabled) {
    const messages = ['Pizza with pineapple?! Seriously?!', 'Noooooooooooo', '🍍🚫']
    console.log(messages[Math.floor(Math.random() * messages.length)]);
  }
}

</script>

<template>
    <div class="pizza-item">
      <button @click="addPizza" :disabled="disableAddPizza || props.state.PizzaType===PizzaType.Hawaiian" @mouseover="logMessage">
        <img
          v-bind:alt="props.state.PizzaType"
          v-bind:title="props.state.PizzaType"
          :src="props.state.Image">
        {{props.state.PizzaType}}
        <div class="amount">{{props.state.Quantity}}</div>
      </button>
    </div>
</template>

<style scoped>
.pizza-item {
    margin: 0.5em;
    display: flex;
    flex-direction: column;
}

.Hawaiian > button {
   cursor: not-allowed;
}

img {
  margin: 0.5em 0.2em;
  width: 150px;
  height: auto;
}

button {
  display: flex;
  flex-direction: column;
  align-items: center;
  background-color: var(--vt-c-yellow-dark);
  border-color: var(--vt-c-yellow-dark);
  border-radius: 0.5rem;
  padding: 0.7rem;
  font-weight: bold;
  font-size: 1rem;
  margin-top: 1rem;
  font-family: 'Space Grotesk', sans-serif;
  transition: all 0.4s ease-out;
}

.amount {
  font-weight: normal;
}

button:hover:enabled {
  box-shadow: 0px 0px 10px var(--vt-c-yellow-dark);
  transition: all 0.1s ease-out;
}

button:disabled {
  border-color: var(--vt-c-divider-dark-2);
  background-color: var(--vt-c-text-dark-2);
}

.Hawaiian > button:disabled {
  background-color: var(--vt-c-yellow-dark);
  border-color: var(--vt-c-yellow-dark);
  color: var(--color-text);
}

</style>