<script setup lang="ts">
import { storeToRefs } from "pinia";
import { pizzaProcessStore } from "../stores";
import type { OrderItem } from '@/types/Order';

export interface OrderItemInterface {
  state: OrderItem;
}

const props = defineProps<OrderItemInterface>();

const store = pizzaProcessStore();
const {
  // add store properties here
} = storeToRefs(store);

function addPizza() {
  // add pizza to order
}

</script>

<template>
    <div class="pizza-item">
        <button @click="addPizza">{{props.state.type}}</button>
        <div>{{props.state.amount}}</div>
    </div>
</template>

<style scoped>
.pizza-item {
    margin: 0.7em;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
}

button {
  background-color: var(--vt-c-yellow-dark);
  border-color: var(--vt-c-yellow-dark);
  border-radius: 0.5rem;
  padding: 0.7rem;
  font-size: 1rem;
  margin-top: 1rem;
  font-family: 'Space Grotesk', sans-serif;
  transition: all 0.4s ease-out;
}

button:hover:enabled {
  box-shadow: 0px 0px 10px var(--vt-c-yellow-dark);
  transition: all 0.1s ease-out;
}
</style>