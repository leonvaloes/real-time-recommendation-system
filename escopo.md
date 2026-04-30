# Escopo do Projeto

## Objetivo

O objetivo do projeto e construir uma aplicacao modular em .NET para explorar diferentes estilos arquiteturais em um mesmo sistema, mantendo separacao clara de responsabilidades, regras de dominio protegidas e infraestrutura desacoplada.

O sistema parte de um modulo de usuarios, autenticacao e autorizacao, e podera evoluir para recomendacoes em tempo real, administracao e outros contextos de negocio.

## Diretriz Arquitetural

Cada modulo principal do sistema deve demonstrar um estilo arquitetural diferente.

Importante: a ideia nao e misturar varias arquiteturas dentro da mesma camada. O mais correto e separar por modulo ou contexto de negocio. Assim, cada modulo pode ter sua propria organizacao interna, desde que respeite os limites de dependencia e se comunique com os demais por contratos claros.

## Modulos Planejados

### User/Auth - Clean Architecture

Modulo responsavel por usuarios, pessoas fisicas/juridicas, autenticacao e autorizacao.

Esse modulo segue Clean Architecture, com separacao entre:

```text
API
Application
Domain
Infra.Data
Infra.IoC
```

Responsabilidades atuais:

- Cadastro de usuario.
- Login.
- Hash de senha com PBKDF2.
- Geracao de token JWT.
- Autorizacao por roles e permissions.
- Modelagem de usuario como pessoa fisica ou juridica.
- Persistencia em MySQL.

Conceitos principais:

- `User`
- `IPerson`
- `NaturalPerson`
- `JuridicalPerson`
- `Cpf`
- `Cnpj`
- `Role`
- `PermissionClaim`

### Recommendation - Hexagonal Architecture

Modulo futuro responsavel por recomendacoes em tempo real.

Esse modulo deve seguir Hexagonal Architecture, tambem conhecida como Ports and Adapters.

Ideia principal:

```text
Core de recomendacao
  -> Ports
  -> Adapters
```

Possiveis responsabilidades:

- Receber eventos de usuario.
- Processar historico de comportamento.
- Gerar recomendacoes.
- Expor portas para entrada de eventos.
- Expor portas para saida de recomendacoes.
- Permitir troca de adaptadores, como MongoDB, fila, cache ou API externa.

Exemplos de ports:

- `IRecommendationEngine`
- `IUserEventStore`
- `IRecommendationPublisher`

Exemplos de adapters:

- MongoDB adapter.
- HTTP API adapter.
- Queue adapter.
- Cache adapter.

### Catalog/Purchase - Onion Architecture

Modulo futuro para compras, pedidos, historico de consumo e regras comerciais mais ricas.

Esse modulo pode seguir Onion Architecture, mantendo o dominio no centro e adicionando aneis ao redor dele.

Ideia principal:

```text
Domain
Application Services
Infrastructure
Presentation
```

Possiveis responsabilidades:

- Registro de compras.
- Registro de itens comprados.
- Historico de consumo.
- Base para alimentar recomendacoes.

Conceitos possiveis:

- `Purchase`
- `PurchaseItem`
- `Order`
- `Payment`

### Catalog Service - MVC

Microservico separado para catalogo de produtos de eventos.

Esse modulo usa arquitetura MVC interna, mas sem depender dos projetos do `User/Auth Service`.

Ideia principal:

```text
Model      -> Product
View       -> Request/Response DTOs
Controller -> ProductsController
```

Responsabilidades:

- Listar produtos.
- Buscar produto por id.
- Criar produto.
- Relacionar produto com evento usando `eventId`.
- Persistir produtos em PostgreSQL.
- Usar `metadata` em JSONB para detalhes especificos por tipo de produto.
- Validar o JWT emitido pelo User/Auth Service.
- Autorizar acesso por permissions do token.

Tipos de produtos esperados:

- Ingressos.
- Roupas.
- Combos.
- Estacionamento.
- Produtos promocionais.

Modelo base:

```text
Product
  id
  eventId
  name
  type
  price
  stockQuantity
  metadata
  isActive
```

O campo `metadata` deve guardar informacoes que variam conforme o tipo do produto.

Exemplo para ingresso:

```json
{
  "sector": "VIP",
  "batch": "1 lote",
  "entryTime": "20:00"
}
```

Exemplo para roupa:

```json
{
  "size": "M",
  "color": "preta",
  "material": "algodao"
}
```

Endpoints planejados:

```text
GET /api/products
  Exige permission: catalog:read

POST /api/products
  Exige permission: catalog:write
```

### Frontend - System Console

Aplicacao front-end separada para visualizar os microservicos, produtos de eventos, permissoes e sinais que futuramente alimentarao o modulo de recomendacao.

Objetivo principal:

- Dar uma interface visual coerente para o projeto.
- Apresentar a separacao entre Auth, Catalog e Recommendation.
- Apresentar produtos e ingressos vindos do `CatalogServiceMvc`.
- Exibir indicadores de catalogo, estoque e eventos.
- Documentar visualmente permissoes exigidas por endpoint.
- Mostrar como os dados do catalogo podem gerar sinais de recomendacao.

Stack sugerida:

```text
React
Vite
TypeScript
Tailwind CSS
Docker
```

Direcao visual:

```text
Console operacional com identidade forte
Cores vivas
Bordas pretas fortes
Cards densos e escaneaveis
Tipografia forte
Textos com alto contraste
Controles claros para busca e filtros
```

Paleta inicial inspirada nas referencias visuais:

```text
Turquesa vivo: fundo principal e areas amplas
Roxo intenso: cards, secoes e palcos
Pink/rosa: chamadas fortes e blocos de impacto
Amarelo: urgencia, destaque de preco e beneficios
Preto: bordas, sombras e contraste
Branco: textos principais com contorno/sombra
Vermelho: erros, risco e mensagens criticas
```

Estrutura planejada da tela:

1. Sidebar de navegacao.
2. Header com estado da conexao com a API.
3. Visao geral dos modulos arquiteturais.
4. Indicadores do catalogo.
5. Tabela de produtos por evento.
6. Busca e filtro por tipo de produto.
7. Matriz de endpoints e permissoes.
8. Painel de sinais para recomendacao.

Secao de produtos:

```text
GET /api/products
```

Cada produto deve ser renderizado conforme o `type`:

```text
ticket   -> card de ingresso
clothing -> card de produto/roupa
combo    -> card promocional
parking  -> card de estacionamento
```

Campos visiveis na tabela:

- Nome.
- Tipo.
- Preco.
- Estoque.
- `eventId`.
- Dados relevantes do `metadata`, como lote ou setor.

UX esperada:

- CTA sempre facil de encontrar.
- Mobile-first.
- Informacao densa, mas organizada.
- Tabelas e cards escaneaveis.
- Estado claro quando os dados vierem da API ou de fallback.
- Inspiracao visual nas referencias, sem copiar o conteudo do evento.

## Escopo Atual Implementado

O escopo atualmente implementado esta concentrado nos modulos `User/Auth`, `CatalogServiceMvc` e `Frontend`.

Funcionalidades implementadas:

- `POST /api/auth/register`
- `POST /api/auth/login`
- JWT com assinatura HMAC SHA-256.
- Senha salva como hash PBKDF2.
- Pessoa fisica e juridica no dominio.
- Roles e permissions no dominio.
- Protecao de endpoint por permission.
- Protecao de endpoint por role.
- Docker Compose com Auth, Catalog, MySQL, PostgreSQL e Adminer.
- Catalogo de produtos com PostgreSQL.
- Produtos associados a eventos por `eventId`.
- Campo `metadata` em JSONB para detalhes variaveis.
- Console front-end em React/Vite.
- Dockerfile proprio para o front-end.
- Orquestracao do front-end pelo Docker Compose.

Endpoints protegidos:

```text
GET /api/user
  Exige permission: users:read

DELETE /api/user/{id}
  Exige role: Admin

GET /api/products
  Exige permission: catalog:read

POST /api/products
  Exige permission: catalog:write
```

## Fora do Escopo Inicial

Os itens abaixo nao fazem parte da primeira entrega:

- Refresh token.
- Recuperacao de senha.
- Confirmacao de email.
- Cadastro completo de roles via API.
- Cadastro completo de permissions via API.
- Painel administrativo.
- Motor real de recomendacao.
- Mensageria/event streaming.
- Cache distribuido.
- Observabilidade completa.
- Checkout real de pagamento.
- Gateway de pagamento.
- Integracao real com bilheteria externa.

## Proximas Etapas

1. Criar CRUD de roles e permissions.
2. Permitir associar roles a usuarios.
3. Integrar o `Frontend` com produtos reais autenticados do `CatalogServiceMvc`.
4. Criar fluxo de administracao para cadastrar produtos de evento.
5. Criar modulo `Recommendation` usando Hexagonal Architecture.
6. Criar modulo `Catalog/Purchase` usando Onion Architecture.
7. Adicionar testes de aplicacao para autenticacao e autorizacao.
8. Adicionar testes de integracao com MySQL e PostgreSQL via Docker.

## Observacao de Design

Este projeto tambem tem objetivo academico: demonstrar diferentes estilos arquiteturais em contextos separados.

Por isso, a separacao por modulo e mais importante do que tentar aplicar todas as arquiteturas ao mesmo tempo no mesmo lugar. Cada modulo deve ter uma fronteira clara e depender dos outros apenas por contratos, DTOs ou eventos.
