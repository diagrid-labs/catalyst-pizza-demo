<script setup lang="ts">
import { storeToRefs } from "pinia";
import { pizzaProcessStore } from "../stores";
import type { OrderItem } from '@/types/Types';

export interface OrderItemInterface {
  state: OrderItem;
}

const props = defineProps<OrderItemInterface>();

const store = pizzaProcessStore();
const {
  // add store properties here
} = storeToRefs(store);

function addPizza() {
  store.incrementPizzaCount(props.state.pizzaType.name);
}

</script>

<template>
    <div class="pizza-item">
      
      <button @click="addPizza">
        <img
          v-bind:alt="props.state.pizzaType.name"
          v-bind:title="props.state.pizzaType.name"
          :src="props.state.pizzaType.image">
        {{props.state.pizzaType.name}}
        <div class="amount">{{props.state.quantity}}</div>
      </button>
    </div>
</template>

<style scoped>
.pizza-item {
    margin: 0.5em;
    display: flex;
    flex-direction: column;
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
</style>