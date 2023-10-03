export type WorkflowState = {
  title: string;
  orderId: string;
  image: string;
  isVisible: boolean;
  isDisabled: boolean;
  isCurrentState: boolean;
  messageSentTimeStampUTC: number;
  messageReceivedTimestamp: number;
  messageDeliveredTimestamp: number;
};
