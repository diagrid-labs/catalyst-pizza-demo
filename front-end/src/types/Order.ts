export type Order = {
  id: string;
  dateTime: Date;
  customerName: string;
  customerEmail: string;
  orderItems: OrderItem[];
};

export type OrderItem = {
  type: Pizza;
  amount: number;
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
