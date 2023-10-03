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
    <div>
        <PizzaItem :state=orderItems[0] />
        <PizzaItem :state=orderItems[1] />
        <PizzaItem :state=orderItems[2] />
        <PizzaItem :state=orderItems[3] />
    </div>
</template>

<style scoped>

</style>