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
  cancelledLimitedInventoryState: cancelledLimitedInventoryState,
} = storeToRefs(store);
</script>

<template>
  <ProgressItem
    class="animate"
    v-if="(receivedOrderState as WorkflowState).isVisible"
    :state="(receivedOrderState as WorkflowState)"
  />
  <ProgressItem
    class="animate"
    v-if="(checkedInventoryState as WorkflowState).isVisible"
    :state="(checkedInventoryState as WorkflowState)"
  />
  <ProgressItem
    class="animate"
    v-if="(insufficientInventoryState as WorkflowState).isVisible"
    :state="(insufficientInventoryState as WorkflowState)"
  />
  <ProgressItem
    class="animate"
    v-if="(sentToKitchenState as WorkflowState).isVisible"
    :state="(sentToKitchenState as WorkflowState)"
  />
  <ProgressItem
    class="animate"
    v-if="(completedPreparationState as WorkflowState).isVisible"
    :state="(completedPreparationState as WorkflowState)"
  />
  <ProgressItem
    class="animate"
    v-if="(cancelledLimitedInventoryState as WorkflowState).isVisible"
    :state="(cancelledLimitedInventoryState as WorkflowState)"
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
