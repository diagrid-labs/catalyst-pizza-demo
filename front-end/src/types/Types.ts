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
  CheckingInventory = "CheckingInventory",
  SufficientInventory = "SufficientInventory",
  InsufficientInventory = "InsufficientInventory",
  RestockedInventory = "RestockedInventory",
  SentToKitchen = "SentToKitchen",
  CompletedPreparation = "CompletedPreparation",
  Error = "Error",
}