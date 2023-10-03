import type { Types } from "ably";
import type { WorkflowState } from "./WorkflowState";

export type PizzaWorkflow = RealtimeState & {
  clientId: string;
  orderId: string;
  isWorkflowComplete: boolean;
  disableOrdering: boolean;
  orderReceivedState: WorkflowState;
  kitchenInstructionsState: WorkflowState;
  preparationState: WorkflowState;
  collectionState: WorkflowState;
  deliveryState: WorkflowState;
  deliveredState: WorkflowState;
  isOrderPlaced: boolean;
};

export type RealtimeState = {
  realtimeClient: Types.RealtimePromise | undefined;
  channelInstance: Types.RealtimeChannelPromise | undefined;
  isConnected: boolean;
  channelPrefix: string;
};
