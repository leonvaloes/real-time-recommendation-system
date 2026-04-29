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
- Persistencia em MongoDB.

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

Modulo futuro para produtos, categorias, compras ou historico de consumo.

Esse modulo pode seguir Onion Architecture, mantendo o dominio no centro e adicionando aneis ao redor dele.

Ideia principal:

```text
Domain
Application Services
Infrastructure
Presentation
```

Possiveis responsabilidades:

- Cadastro de produtos.
- Registro de compras.
- Historico de consumo.
- Base para alimentar recomendacoes.

Conceitos possiveis:

- `Product`
- `Category`
- `Purchase`
- `PurchaseItem`

### Catalog Service - MVC

Microservico separado para catalogo de produtos.

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
- Validar o JWT emitido pelo User/Auth Service.
- Autorizar acesso por permissions do token.

Endpoints planejados:

```text
GET /api/products
  Exige permission: catalog:read

POST /api/products
  Exige permission: catalog:write
```

## Escopo Atual Implementado

O escopo atualmente implementado esta concentrado no modulo `User/Auth`.

Funcionalidades implementadas:

- `POST /api/auth/register`
- `POST /api/auth/login`
- JWT com assinatura HMAC SHA-256.
- Senha salva como hash PBKDF2.
- Pessoa fisica e juridica no dominio.
- Roles e permissions no dominio.
- Protecao de endpoint por permission.
- Protecao de endpoint por role.
- Docker Compose com API, MongoDB e Mongo Express.

Endpoints protegidos:

```text
GET /api/user
  Exige permission: users:read

DELETE /api/user/{id}
  Exige role: Admin
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

## Proximas Etapas

1. Criar CRUD de roles e permissions.
2. Permitir associar roles a usuarios.
3. Criar modulo `Recommendation` usando Hexagonal Architecture.
4. Criar modulo `Catalog/Purchase` usando Onion Architecture.
5. Evoluir o `CatalogServiceMvc` MVC com persistencia propria.
6. Adicionar testes de aplicacao para autenticacao e autorizacao.
7. Adicionar testes de integracao com MongoDB via Docker.

## Observacao de Design

Este projeto tambem tem objetivo academico: demonstrar diferentes estilos arquiteturais em contextos separados.

Por isso, a separacao por modulo e mais importante do que tentar aplicar todas as arquiteturas ao mesmo tempo no mesmo lugar. Cada modulo deve ter uma fronteira clara e depender dos outros apenas por contratos, DTOs ou eventos.
