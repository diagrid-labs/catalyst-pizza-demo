import type { Types } from "ably";
import type { WorkflowState } from "./WorkflowState";
import type { OrderItem } from "./Types";

export type PizzaWorkflow = RealtimeState & {
  orderItems: OrderItem[];
  clientId: string;
  orderId: string;
  isWorkflowComplete: boolean;
  disableOrdering: boolean;
  receivedOrderState: WorkflowState;
  checkedInventoryState: WorkflowState;
  insufficientInventoryState: WorkflowState;
  sentToKitchenState: WorkflowState;
  completedPreparationState: WorkflowState;
  cancelledLimitedInventoryState: WorkflowState;
  isOrderPlaced: boolean;
};

export type RealtimeState = {
  realtimeClient: Types.RealtimePromise | undefined;
  channelInstance: Types.RealtimeChannelPromise | undefined;
  isConnected: boolean;
  channelPrefix: string;
};
