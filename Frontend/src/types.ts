export type ProductType = "ticket" | "clothing" | "combo" | "parking" | string;

export type Product = {
  id: string;
  eventId: string;
  name: string;
  type: ProductType;
  price: number;
  stockQuantity: number;
  metadata: Record<string, unknown>;
  isActive: boolean;
};
