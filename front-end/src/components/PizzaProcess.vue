<script setup lang="ts">
import ProgressItem from "./ProgressItem.vue";
import { storeToRefs } from "pinia";
import { pizzaProcessStore } from "../stores";
import type { WorkflowState } from "@/types/WorkflowState";

const store = pizzaProcessStore();
const {
  orderPlacedState: orderReceivedState,
  inStockState: kitchenInstructionsState,
  notInStockState: preparationState,
  inPreparationState: collectionState,
  completedState: deliveryState,
} = storeToRefs(store);
</script>

<template>
  <ProgressItem
    class="animate"
    v-if="(orderReceivedState as WorkflowState).isVisible"
    :state="(orderReceivedState as WorkflowState)"
  />
  <ProgressItem
    class="animate"
    v-if="(kitchenInstructionsState as WorkflowState).isVisible"
    :state="(kitchenInstructionsState as WorkflowState)"
  />
  <ProgressItem
    class="animate"
    v-if="(preparationState as WorkflowState).isVisible"
    :state="(preparationState as WorkflowState)"
  />
  <ProgressItem
    class="animate"
    v-if="(collectionState as WorkflowState).isVisible"
    :state="(collectionState as WorkflowState)"
  />
  <ProgressItem
    class="animate"
    v-if="(deliveryState as WorkflowState).isVisible"
    :state="(deliveryState as WorkflowState)"
  />
</template>

<style scoped>
.animate {
  animation-duration: 0.5s;
  animation-name: animate-fade;
  animation-delay: 0.5s;
  animation-fill-mode: backwards;
}

@keyframes animate-fade {
  0% {
    opacity: 0;
  }
  100% {
    opacity: 1;
  }
}
</style>
