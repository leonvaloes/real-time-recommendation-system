import { useEffect, useMemo, useState } from "react";
import {
  Activity,
  BadgeCheck,
  Boxes,
  Database,
  Filter,
  Gauge,
  KeyRound,
  LayoutDashboard,
  PackagePlus,
  Search,
  ShieldCheck,
  Sparkles,
  Ticket,
  Users
} from "lucide-react";
import { fallbackProducts, getProducts } from "./api/products";
import type { Product } from "./types";

type ProductSummary = {
  totalProducts: number;
  activeProducts: number;
  totalStock: number;
  averagePrice: number;
  eventCount: number;
};

function formatCurrency(value: number) {
  return value.toLocaleString("pt-BR", {
    style: "currency",
    currency: "BRL"
  });
}

function getMetadataValue(product: Product, key: string) {
  const value = product.metadata?.[key];
  return typeof value === "string" || typeof value === "number" ? String(value) : undefined;
}

function productLabel(type: string) {
  const labels: Record<string, string> = {
    ticket: "Ingresso",
    clothing: "Produto",
    combo: "Combo",
    parking: "Estacionamento"
  };

  return labels[type] ?? type;
}

function summarizeProducts(products: Product[]): ProductSummary {
  const activeProducts = products.filter((product) => product.isActive);
  const totalStock = activeProducts.reduce((sum, product) => sum + product.stockQuantity, 0);
  const totalPrice = activeProducts.reduce((sum, product) => sum + product.price, 0);
  const eventCount = new Set(activeProducts.map((product) => product.eventId)).size;

  return {
    totalProducts: products.length,
    activeProducts: activeProducts.length,
    totalStock,
    averagePrice: activeProducts.length ? totalPrice / activeProducts.length : 0,
    eventCount
  };
}

export function App() {
  const [products, setProducts] = useState<Product[]>(fallbackProducts);
  const [search, setSearch] = useState("");
  const [selectedType, setSelectedType] = useState("all");
  const [source, setSource] = useState<"api" | "demo">("demo");

  useEffect(() => {
    getProducts().then((result) => {
      setProducts(result.products);
      setSource(result.source);
    });
  }, []);

  const productTypes = useMemo(
    () => ["all", ...Array.from(new Set(products.map((product) => product.type)))],
    [products]
  );

  const filteredProducts = useMemo(() => {
    const normalizedSearch = search.trim().toLowerCase();

    return products.filter((product) => {
      const matchesType = selectedType === "all" || product.type === selectedType;
      const matchesSearch =
        !normalizedSearch ||
        product.name.toLowerCase().includes(normalizedSearch) ||
        product.eventId.toLowerCase().includes(normalizedSearch) ||
        product.type.toLowerCase().includes(normalizedSearch);

      return matchesType && matchesSearch;
    });
  }, [products, search, selectedType]);

  const summary = useMemo(() => summarizeProducts(products), [products]);

  return (
    <main className="app-shell">
      <aside className="sidebar" aria-label="Navegacao principal">
        <div className="brand-mark">
          <span>RT</span>
          <strong>Recommendation System</strong>
        </div>

        <nav>
          <a className="active" href="#overview">
            <LayoutDashboard aria-hidden="true" />
            Visao geral
          </a>
          <a href="#catalog">
            <Boxes aria-hidden="true" />
            Catalogo
          </a>
          <a href="#security">
            <ShieldCheck aria-hidden="true" />
            Autorizacao
          </a>
          <a href="#signals">
            <Sparkles aria-hidden="true" />
            Sinais
          </a>
        </nav>
      </aside>

      <div className="workspace">
        <header className="topbar">
          <div>
            <p className="eyebrow">Microservices console</p>
            <h1>Eventos, catalogo e recomendacoes em tempo real</h1>
          </div>
          <div className={`source-pill source-${source}`}>
            <Database aria-hidden="true" />
            {source === "api" ? "CatalogServiceMvc conectado" : "Modo demonstracao"}
          </div>
        </header>

        <section className="hero-panel" id="overview">
          <div className="hero-copy">
            <span>Arquiteturas separadas no mesmo produto</span>
            <h2>Auth em Clean Architecture, catalogo em MVC e front independente.</h2>
            <p>
              Esta tela centraliza o que o sistema ja entrega: usuarios com JWT,
              permissions, produtos por evento e dados que podem alimentar um
              motor de recomendacao.
            </p>
          </div>

          <div className="architecture-grid">
            <ArchitectureCard
              icon={<KeyRound />}
              title="UserAuthCleanArch"
              text="Cadastro, login, JWT, roles e permissions."
              accent="pink"
            />
            <ArchitectureCard
              icon={<Boxes />}
              title="CatalogServiceMvc"
              text="Produtos por eventId com PostgreSQL e metadata JSONB."
              accent="purple"
            />
            <ArchitectureCard
              icon={<Activity />}
              title="Recommendation"
              text="Proximo modulo para eventos de usuario e sugestoes."
              accent="yellow"
            />
          </div>
        </section>

        <section className="metrics-grid" aria-label="Indicadores do catalogo">
          <MetricCard label="Produtos" value={summary.totalProducts.toString()} detail="Itens carregados" />
          <MetricCard label="Ativos" value={summary.activeProducts.toString()} detail="Disponiveis no catalogo" />
          <MetricCard label="Estoque" value={summary.totalStock.toString()} detail="Unidades totais" />
          <MetricCard label="Preco medio" value={formatCurrency(summary.averagePrice)} detail={`${summary.eventCount} evento(s)`} />
        </section>

        <section className="content-panel" id="catalog">
          <div className="panel-heading">
            <div>
              <p className="eyebrow">CatalogServiceMvc</p>
              <h2>Produtos vinculados a eventos</h2>
            </div>
            <button type="button">
              <PackagePlus aria-hidden="true" />
              Novo produto
            </button>
          </div>

          <div className="toolbar">
            <label className="search-field">
              <Search aria-hidden="true" />
              <input
                value={search}
                onChange={(event) => setSearch(event.target.value)}
                placeholder="Buscar por nome, tipo ou eventId"
              />
            </label>

            <label className="select-field">
              <Filter aria-hidden="true" />
              <select value={selectedType} onChange={(event) => setSelectedType(event.target.value)}>
                {productTypes.map((type) => (
                  <option key={type} value={type}>
                    {type === "all" ? "Todos os tipos" : productLabel(type)}
                  </option>
                ))}
              </select>
            </label>
          </div>

          <div className="product-table" role="table" aria-label="Produtos do catalogo">
            <div className="table-row table-head" role="row">
              <span>Produto</span>
              <span>Tipo</span>
              <span>Evento</span>
              <span>Estoque</span>
              <span>Preco</span>
            </div>
            {filteredProducts.map((product) => (
              <ProductRow key={product.id} product={product} />
            ))}
          </div>
        </section>

        <section className="split-grid">
          <div className="content-panel" id="security">
            <div className="panel-heading compact">
              <div>
                <p className="eyebrow">Autorizacao</p>
                <h2>Permissoes por endpoint</h2>
              </div>
            </div>

            <div className="permission-list">
              <PermissionRow method="GET" endpoint="/api/products" permission="catalog:read" />
              <PermissionRow method="POST" endpoint="/api/products" permission="catalog:write" />
              <PermissionRow method="GET" endpoint="/api/user" permission="users:read" />
              <PermissionRow method="DELETE" endpoint="/api/user/{id}" permission="role:Admin" />
            </div>
          </div>

          <div className="content-panel" id="signals">
            <div className="panel-heading compact">
              <div>
                <p className="eyebrow">Recommendation ready</p>
                <h2>Sinais para evoluir</h2>
              </div>
            </div>

            <div className="signal-stack">
              <SignalCard title="Produto visualizado" text="Base para recomendacao por interesse." />
              <SignalCard title="Tipo preferido" text="ticket, clothing, combo ou parking." />
              <SignalCard title="Evento relacionado" text="eventId conecta catalogo, compra e recomendacao." />
            </div>
          </div>
        </section>
      </div>
    </main>
  );
}

function ArchitectureCard({
  icon,
  title,
  text,
  accent
}: {
  icon: React.ReactNode;
  title: string;
  text: string;
  accent: "pink" | "purple" | "yellow";
}) {
  return (
    <article className={`architecture-card accent-${accent}`}>
      <div>{icon}</div>
      <h3>{title}</h3>
      <p>{text}</p>
    </article>
  );
}

function MetricCard({ label, value, detail }: { label: string; value: string; detail: string }) {
  return (
    <article className="metric-card">
      <span>{label}</span>
      <strong>{value}</strong>
      <p>{detail}</p>
    </article>
  );
}

function ProductRow({ product }: { product: Product }) {
  const batch = getMetadataValue(product, "batch");
  const sector = getMetadataValue(product, "sector");
  const metadataLabel = [batch, sector].filter(Boolean).join(" / ");

  return (
    <div className="table-row" role="row">
      <span className="product-name">
        <Ticket aria-hidden="true" />
        <span>
          <strong>{product.name}</strong>
          <small>{metadataLabel || "metadata flexivel"}</small>
        </span>
      </span>
      <span>
        <BadgeCheck aria-hidden="true" />
        {productLabel(product.type)}
      </span>
      <span className="event-id">{product.eventId}</span>
      <span>{product.stockQuantity}</span>
      <span>{formatCurrency(product.price)}</span>
    </div>
  );
}

function PermissionRow({
  method,
  endpoint,
  permission
}: {
  method: string;
  endpoint: string;
  permission: string;
}) {
  return (
    <div className="permission-row">
      <span>{method}</span>
      <code>{endpoint}</code>
      <strong>{permission}</strong>
    </div>
  );
}

function SignalCard({ title, text }: { title: string; text: string }) {
  return (
    <article className="signal-card">
      <Gauge aria-hidden="true" />
      <h3>{title}</h3>
      <p>{text}</p>
    </article>
  );
}
