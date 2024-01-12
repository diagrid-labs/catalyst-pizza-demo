<script setup lang="ts">
import ProgressItem from "./ProgressItem.vue";
import { storeToRefs } from "pinia";
import { pizzaProcessStore } from "../stores";
import type { WorkflowState } from "@/types/WorkflowState";

const store = pizzaProcessStore();
const {
  receivedOrderState: receivedOrderState,
  checkedInventoryState: checkedInventoryState,
  insufficientInventoryState: insufficientInventoryState,
  sentToKitchenState: sentToKitchenState,
  completedPreparationState: completedPreparationState,
  unknownState: unknownState,
} = storeToRefs(store);
</script>

<template>
  <ProgressItem
    class="animate"
    v-if="(receivedOrderState as WorkflowState).IsVisible"
    :state="(receivedOrderState as WorkflowState)"
  />
  <ProgressItem
    class="animate"
    v-if="(checkedInventoryState as WorkflowState).IsVisible"
    :state="(checkedInventoryState as WorkflowState)"
  />
  <ProgressItem
    class="animate"
    v-if="(insufficientInventoryState as WorkflowState).IsVisible"
    :state="(insufficientInventoryState as WorkflowState)"
  />
  <ProgressItem
    class="animate"
    v-if="(sentToKitchenState as WorkflowState).IsVisible"
    :state="(sentToKitchenState as WorkflowState)"
  />
  <ProgressItem
    class="animate"
    v-if="(completedPreparationState as WorkflowState).IsVisible"
    :state="(completedPreparationState as WorkflowState)"
  />
  <ProgressItem
    class="animate"
    v-if="(unknownState as WorkflowState).IsVisible"
    :state="(unknownState as WorkflowState)"
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
