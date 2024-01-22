export type Notification = {
  message: string;
  order: Order;
}

export type Order = {
  OrderId: string;
  OrderDate: Date;
  Customer: Customer;
  OrderItems: OrderItem[];
  Status?: OrderStatus;
};

export type Customer = {
    Name: string;
    Email: string;
}

export type OrderItem = {
  PizzaType: PizzaType;
  Quantity: number;
  Image: string;
};

export enum PizzaType {
  Pepperoni = "Pepperoni",
  Margherita = "Margherita",
  Hawaiian = "Hawaiian",
  Vegetarian = "Vegetarian",
}

export enum OrderStatus {
  Received = "Received",
  CheckedInventory = "CheckedInventory",
  RestockedInventory = "RestockedInventory",
  SentToKitchen = "SentToKitchen",
  CompletedPreparation = "CompletedPreparation",
  InsufficientInventory = "InsufficientInventory",
  Error = "Error",
}