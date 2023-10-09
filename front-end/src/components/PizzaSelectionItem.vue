<script setup lang="ts">
import PizzaItem from "./PizzaItem.vue";
import { storeToRefs } from "pinia";
import { pizzaProcessStore } from "../stores";

const store = pizzaProcessStore();
const {
  orderItems
} = storeToRefs(store);

</script>

<template>
    <div class="flex-row-pizza" >
        <PizzaItem :state=orderItems[0] />
        <PizzaItem :state=orderItems[1] />
        <PizzaItem :state=orderItems[2] />
        <PizzaItem :state=orderItems[3] />
    </div>
</template>

<style scoped>

.flex-row-pizza {
      display: flex;
      flex-direction: wrap;
      align-items: center;
      justify-content: center;
  }

@media (min-width: 1024px) {
  .flex-row-pizza {
      display: flex;
      flex-direction: row;
      align-items: center;
      justify-content: center;
  }
}

</style>