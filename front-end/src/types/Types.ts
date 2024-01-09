export type Notification = {
  message: string;
  order: Order;
}

export type Order = {
  orderId: string;
  orderDate: Date;
  customer: Customer;
  orderItems: OrderItem[];
  status?: OrderStatus;
};

export type Customer = {
    name: string;
    email: string;
}

export type OrderItem = {
  pizzaType: Pizza;
  quantity: number;
};

export type Pizza = {
  name: PizzaType;
  image: string;
};

export enum PizzaType {
  Pepperoni = "pepperoni",
  Margherita = "margherita",
  Hawaiian = "hawaiian",
  Vegetarian = "vegetarian",
}

export enum OrderStatus {
  Received = "Received",
  CheckedInventory = "CheckedInventory",
  SentToKitchen = "SentToKitchen",
  CompletedPreparation = "CompletedPreparation",
  CancelledLimitedInventory = "CancelledLimitedInventory",
  Unknown = "Unknown",
}