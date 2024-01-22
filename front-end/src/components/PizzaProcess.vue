<script setup lang="ts">
import ProgressItem from "./ProgressItem.vue";
import { storeToRefs } from "pinia";
import { pizzaProcessStore } from "../stores";
import type { WorkflowState } from "@/types/WorkflowState";

const store = pizzaProcessStore();
const {
  receivedOrderState: receivedOrderState,
  checkingInventoryState: checkingInventoryState,
  insufficientInventoryState: insufficientInventoryState,
  restockedInventoryState: restockedInventoryState,
  sufficientInventoryState: sufficientInventoryState,
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
    v-if="(checkingInventoryState as WorkflowState).IsVisible"
    :state="(checkingInventoryState as WorkflowState)"
  />
  <ProgressItem
  class="animate"
  v-if="(insufficientInventoryState as WorkflowState).IsVisible"
  :state="(insufficientInventoryState as WorkflowState)"
  />
  <ProgressItem
  class="animate"
  v-if="(restockedInventoryState as WorkflowState).IsVisible"
  :state="(restockedInventoryState as WorkflowState)"
  />
  <ProgressItem
  class="animate"
  v-if="(sufficientInventoryState as WorkflowState).IsVisible"
  :state="(sufficientInventoryState as WorkflowState)"
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
  animation-duration: 0.3s;
  animation-name: animate-fade;
  animation-delay: 0.3s;
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
