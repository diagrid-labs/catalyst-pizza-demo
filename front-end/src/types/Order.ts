export type Order = {
  id: string;
  dateTime: Date;
  customerName: string;
  customerEmail: string;
  orderItems: OrderItem[];
};

export type OrderItem = {
  type: PizzaType;
  amount: number;
};

export type Pizza = {
  name: PizzaType;
};

export enum PizzaType {
  Hawaii = "hawaii",
  Margarita = "margarita",
  Pepperoni = "pepperoni",
  Spinach = "spinach",
}
