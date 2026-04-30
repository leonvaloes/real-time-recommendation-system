import type { Product } from "../types";

type ProductResult = {
  products: Product[];
  source: "api" | "demo";
};

const fallbackEventId = "2d33e70b-fd66-4a7f-b833-b63d190a9645";

export const fallbackProducts: Product[] = [
  {
    id: "ticket-vip",
    eventId: fallbackEventId,
    name: "Ingresso VIP - Evento Demo",
    type: "ticket",
    price: 150,
    stockQuantity: 87,
    metadata: {
      sector: "VIP",
      batch: "1 lote",
      entryTime: "20:00",
      benefit: "Acesso preferencial"
    },
    isActive: true
  },
  {
    id: "ticket-solidary",
    eventId: fallbackEventId,
    name: "Ingresso solidario",
    type: "ticket",
    price: 95,
    stockQuantity: 140,
    metadata: {
      sector: "Pista",
      batch: "promocional",
      note: "Mediante doacao de 1kg de alimento"
    },
    isActive: true
  },
  {
    id: "combo-hype",
    eventId: fallbackEventId,
    name: "Combo evento + produto",
    type: "combo",
    price: 210,
    stockQuantity: 42,
    metadata: {
      includes: "2 ingressos + copo oficial",
      batch: "48h iniciais"
    },
    isActive: true
  },
  {
    id: "shirt-official",
    eventId: fallbackEventId,
    name: "Camiseta oficial do evento",
    type: "clothing",
    price: 80,
    stockQuantity: 58,
    metadata: {
      size: "P ao GG",
      color: "preta",
      material: "algodao"
    },
    isActive: true
  }
];

export async function getProducts(): Promise<ProductResult> {
  const baseUrl = import.meta.env.VITE_CATALOG_API_URL ?? "http://localhost:5001";
  const token = import.meta.env.VITE_CATALOG_TOKEN ?? localStorage.getItem("catalog_token");

  if (!token) {
    return { products: fallbackProducts, source: "demo" };
  }

  try {
    const response = await fetch(`${baseUrl}/api/products`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });

    if (!response.ok) {
      return { products: fallbackProducts, source: "demo" };
    }

    const products = (await response.json()) as Product[];
    return {
      products: products.length > 0 ? products : fallbackProducts,
      source: products.length > 0 ? "api" : "demo"
    };
  } catch {
    return { products: fallbackProducts, source: "demo" };
  }
}
