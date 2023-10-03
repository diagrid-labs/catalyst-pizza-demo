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

</script>

<template>
    <div>
        <button>{{props.state.type}}</button>
        <div>{{props.state.amount}}</div>
    </div>
</template>

<style scoped>

</style>