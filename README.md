# Real-time Recommendation System

Projeto em .NET 8 organizado com uma estrutura inspirada em Clean Architecture.

A ideia principal dessa arquitetura e separar responsabilidades: a API recebe as requisicoes, a camada de aplicacao coordena os casos de uso, o dominio concentra as regras principais, e a infraestrutura cuida de detalhes externos como banco de dados e injecao de dependencia.

## Estrutura do projeto

```text
UserAuthCleanArch/
  Dockerfile
  UserAuth.CleanArch.API/
  UserAuth.CleanArch.Application/
  UserAuth.CleanArch.Domain/
  UserAuth.CleanArch.Infra.Data/
  UserAuth.CleanArch.Infra.IoC/
  UserAuth.CleanArch.Domain.Tests/
  UserAuth.CleanArch.Data/
CatalogServiceMvc/
  Dockerfile
  Controllers/
  Data/
  Models/
  Services/
  Repositories/
  Views/
Frontend/
  Dockerfile
  src/
docker-compose.yml
Docker/
```

## UserAuth.CleanArch.API

Camada de entrada da aplicacao.

Ela expoe os endpoints HTTP usando ASP.NET Core. O principal exemplo atual e o `UserController`, que recebe chamadas como listar, buscar, criar e remover usuarios.

Responsabilidades:

- Receber requisicoes HTTP.
- Validar o formato basico da entrada quando necessario.
- Chamar os servicos da camada de aplicacao.
- Retornar respostas HTTP, como `Ok`, `NotFound` e `CreatedAtAction`.

Exemplo de fluxo:

```text
POST /api/user
  -> UserController.Create
  -> IUserService.CreateAsync
```

A API nao deve conter regra de negocio. Ela deve apenas adaptar o mundo HTTP para a aplicacao.

Tambem existe o `AuthController`, responsavel pelos endpoints de autenticacao:

```text
POST /api/auth/register
POST /api/auth/login
```

Esses endpoints recebem os dados da requisicao, chamam `IAuthService` e retornam uma resposta com dados do usuario autenticado e um token.

## UserAuth.CleanArch.Application

Camada de aplicacao.

Essa camada coordena os casos de uso do sistema. No projeto atual, o exemplo principal e o `UserService`, que implementa `IUserService`.

Responsabilidades:

- Definir interfaces de servico, como `IUserService`.
- Implementar servicos de aplicacao, como `UserService`.
- Trabalhar com DTOs, como `CreateUserDTO`.
- Converter DTOs em entidades de dominio usando mappings.
- Chamar contratos do dominio, como `IUserRepository`.

Exemplo:

```text
CreateUserDTO
  -> CreateUserMapping
  -> User
  -> IUserRepository.CreateAsync
```

Essa camada sabe o que o caso de uso precisa fazer, mas nao deve saber detalhes de MySQL, HTTP ou Docker.

O modulo de autenticacao tambem fica nesta camada por meio do `AuthService`.

Responsabilidades do `AuthService`:

- Registrar novo usuario de autenticacao.
- Validar se o email ja esta cadastrado.
- Validar credenciais de login.
- Chamar `IPasswordHasher` para hash e verificacao de senha.
- Chamar `ITokenService` para gerar o token de acesso.

As interfaces `IPasswordHasher` e `ITokenService` ficam na camada de aplicacao para que o caso de uso dependa de contratos, nao de detalhes de implementacao.

## UserAuth.CleanArch.Domain

Camada central do sistema.

Aqui ficam as regras mais importantes da aplicacao. Essa camada deve ser a mais independente possivel.

Responsabilidades:

- Definir entidades, como `User` e `Purchase`.
- Definir pessoas do dominio, como `NaturalPerson` e `JuridicalPerson`.
- Definir papeis e permissoes, como `Role` e `PermissionClaim`.
- Definir objetos de valor, como `Cpf` e `Cnpj`.
- Proteger regras de negocio e validacoes de dominio.
- Definir contratos de repositorio, como `IUserRepository` e `IPurchaseRepository`.
- Definir excecoes e validacoes de dominio.

Exemplo atual:

```csharp
public sealed class User
{
    public User(string email, string passwordHash, IPerson person, string? phone = null)
    {
        ValidateDomain(email, passwordHash, phone);
        Email = email;
        PasswordHash = passwordHash;
        SetPerson(person);
    }
}
```

O `User` valida email, hash de senha, telefone e associacao com pessoa fisica ou juridica. Se os dados forem invalidos, a validacao lanca `DomainExceptionValidation`.

O `IPerson` fica em `Domain/Abstractions` porque representa um conceito do dominio, nao uma porta de infraestrutura. As interfaces de repositorio continuam em `Domain/Interfaces`.

O `User` guarda o hash da senha, nao a senha em texto puro. A pessoa associada ao usuario pode ser `NaturalPerson` ou `JuridicalPerson`.

Essa camada nao deve depender da API, da infraestrutura, do MySQL ou de frameworks externos de persistencia.

## UserAuth.CleanArch.Infra.Data

Camada de infraestrutura de dados.

Essa camada implementa os contratos definidos no dominio usando uma tecnologia concreta. Neste projeto, o modulo de autenticacao usa MySQL.

Responsabilidades:

- Configurar conexao com MySQL.
- Criar a tabela `users` quando necessario pelo `MySqlContext`.
- Implementar repositorios concretos, como `UserRepository`.
- Traduzir chamadas de repositorio para operacoes reais no banco.

Exemplo:

```text
IUserRepository
  -> UserRepository
  -> MySqlContext
  -> MySQL
```

O `MySqlContext` le a variavel de ambiente:

- `MYSQL_CONNECTION_STRING`

O `UserRepository` usa a tabela `users` para criar, buscar, atualizar e remover usuarios.

## UserAuth.CleanArch.Infra.IoC

Camada de injecao de dependencia.

IoC significa Inversion of Control. Essa camada centraliza os registros que dizem ao ASP.NET Core qual implementacao deve ser usada para cada interface.

Exemplo atual:

```csharp
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
services.AddScoped<ITokenService, JwtTokenService>();
services.AddScoped<CreateUserMapping>();
services.AddSingleton<MySqlContext>();
```

Isso significa:

```text
Quando alguem pedir IUserService, entregue UserService.
Quando alguem pedir IUserRepository, entregue UserRepository.
Quando alguem pedir IAuthService, entregue AuthService.
```

A API chama essa configuracao no `Program.cs`:

```csharp
builder.Services.AddInfrastructure(builder.Configuration);
```

## UserAuth.CleanArch.Domain.Tests

Projeto de testes.

Atualmente contem testes unitarios para a entidade `User`, usando xUnit e FluentAssertions.
Tambem contem testes para `NaturalPerson`, `JuridicalPerson`, CPF, CNPJ e roles.

Responsabilidades:

- Testar regras do dominio.
- Garantir que validacoes funcionam.
- Evitar regressao em regras de negocio.

Exemplo de teste existente:

```text
Criar pessoa fisica com CPF invalido deve lancar DomainExceptionValidation.
```

## UserAuth.CleanArch.Data

Esse projeto existe na estrutura, mas atualmente parece nao ter responsabilidade real implementada. Ele contem apenas um arquivo inicial `Class1.cs`.

Pelo desenho atual, a infraestrutura de dados esta sendo feita em `UserAuth.CleanArch.Infra.Data`.

## Docker

Cada microservico possui seu proprio `Dockerfile` na raiz do microservico:

```text
UserAuthCleanArch/Dockerfile
CatalogServiceMvc/Dockerfile
```

O `docker-compose.yml` da raiz e o orquestrador externo. Ele sobe os microservicos e as dependencias locais.

Servicos configurados no compose da raiz:

- UserAuthCleanArch na porta `5000`.
- CatalogServiceMvc na porta `5001`.
- Frontend na porta `3000`.
- MySQL na porta `3306`.
- PostgreSQL na porta `5432`.
- Adminer na porta `8081`.

Comando:

```bash
docker compose up -d --build
```

Existe tambem um compose em `Docker/docker-compose.yml` mantido como alternativa, mas o fluxo recomendado e usar o `docker-compose.yml` da raiz.

## Fluxo completo de uma requisicao

Exemplo: criar usuario.

```text
Cliente HTTP
  -> UserAuth.CleanArch.API/UserController
  -> UserAuth.CleanArch.Application/UserService
  -> UserAuth.CleanArch.Application/CreateUserMapping
  -> UserAuth.CleanArch.Domain/User
  -> UserAuth.CleanArch.Domain/IUserRepository
  -> UserAuth.CleanArch.Infra.Data/UserRepository
  -> UserAuth.CleanArch.Infra.Data/MySqlContext
  -> MySQL
```

Exemplo: login.

```text
Cliente HTTP
  -> UserAuth.CleanArch.API/AuthController
  -> UserAuth.CleanArch.Application/AuthService
  -> UserAuth.CleanArch.Domain/IUserRepository
  -> UserAuth.CleanArch.Infra.Data/UserRepository
  -> UserAuth.CleanArch.Infra.Data/MySqlContext
  -> MySQL
  -> UserAuth.CleanArch.Application/IPasswordHasher
  -> UserAuth.CleanArch.Application/ITokenService
```

## Dependencias entre camadas

O fluxo desejado e:

```text
API
  -> Application
  -> Domain

Infra.Data
  -> Domain

Infra.IoC
  -> Application
  -> Infra.Data
  -> Domain
```

O dominio fica no centro e nao deve depender das outras camadas.

## Comandos uteis

Build do projeto de testes e referencias:

```bash
dotnet build UserAuthCleanArch/UserAuth.CleanArch.Domain.Tests/UserAuth.CleanArch.Domain.Tests.csproj
```

Executar testes:

```bash
dotnet test UserAuthCleanArch/UserAuth.CleanArch.Domain.Tests/UserAuth.CleanArch.Domain.Tests.csproj
```

Executar testes com cobertura:

```bash
dotnet test UserAuthCleanArch/UserAuth.CleanArch.Domain.Tests/UserAuth.CleanArch.Domain.Tests.csproj --collect:"XPlat Code Coverage"
```

Subir todos os servicos:

```bash
docker compose up -d --build
```

UserAuthCleanArch fica disponivel em:

```text
http://localhost:5000
```

CatalogServiceMvc fica disponivel em:

```text
http://localhost:5001
```

Frontend fica disponivel em:

```text
http://localhost:3000
```

O Adminer fica disponivel em:

```text
http://localhost:8081
```

## Configuracao

Antes de executar os servicos, configure as variaveis de ambiente:

```text
MYSQL_CONNECTION_STRING
POSTGRES_CONNECTION_STRING
JWT_SECRET
JWT_ISSUER
JWT_AUDIENCE
```

Exemplo local usando o Docker Compose:

```text
MYSQL_CONNECTION_STRING=Server=localhost;Port=3306;Database=user_auth;User=user;Password=1234;Allow User Variables=True;
POSTGRES_CONNECTION_STRING=Host=localhost;Port=5432;Database=catalog;Username=catalog;Password=1234
JWT_SECRET=change-this-secret-in-development
JWT_ISSUER=UserAuth.CleanArch.API
JWT_AUDIENCE=UserAuth.CleanArch.Client
```

## Modulo de catalogo de produtos

O `CatalogServiceMvc` e um microservico separado em estilo MVC. Ele expoe endpoints de catalogo protegidos por JWT e permissoes:

```text
GET  /api/products       requer catalog:read
GET  /api/products/{id}  requer catalog:read
POST /api/products       requer catalog:write
```

O catalogo usa PostgreSQL. A tabela `products` e criada automaticamente pelo `PostgresContext`.

Campos principais:

- `id`: identificador do produto.
- `event_id`: identificador do evento ao qual o produto pertence.
- `name`: nome do produto.
- `type`: tipo do produto, como `ticket`, `clothing`, `parking` ou `combo`.
- `price`: preco.
- `stock_quantity`: quantidade em estoque.
- `metadata`: campo `jsonb` para detalhes variaveis por tipo de produto.
- `is_active`: indica se o produto esta ativo.

Exemplo de cadastro de ingresso:

```json
{
  "eventId": "2d33e70b-fd66-4a7f-b833-b63d190a9645",
  "name": "Ingresso VIP",
  "type": "ticket",
  "price": 150.00,
  "stockQuantity": 100,
  "metadata": {
    "sector": "VIP",
    "batch": "1 lote",
    "entryTime": "20:00"
  }
}
```

Exemplo de cadastro de roupa:

```json
{
  "eventId": "2d33e70b-fd66-4a7f-b833-b63d190a9645",
  "name": "Camiseta Oficial",
  "type": "clothing",
  "price": 80.00,
  "stockQuantity": 50,
  "metadata": {
    "size": "M",
    "color": "preta",
    "material": "algodao"
  }
}
```

## Frontend

O `Frontend` e uma aplicacao React com Vite para visualizar o sistema de microservicos.

Responsabilidades:

- Apresentar uma visao geral dos modulos do projeto.
- Mostrar indicadores do catalogo de produtos por evento.
- Renderizar produtos vindos do `CatalogServiceMvc`.
- Exibir endpoints protegidos e permissoes esperadas.
- Mostrar sinais que podem alimentar o futuro modulo de recomendacao.

O front tenta consumir produtos do `CatalogServiceMvc` usando:

```text
VITE_CATALOG_API_URL
VITE_CATALOG_TOKEN
```

Se nao houver token configurado, ele usa produtos de fallback para manter o console funcionando em modo demonstracao.

Comandos:

```bash
cd Frontend
npm install
npm run dev
npm run build
```

## Modulo de autenticacao

O modulo de autenticacao foi separado nas mesmas camadas da aplicacao:

```text
API
  AuthController

Application
  AuthService
  RegisterRequestDTO
  LoginRequestDTO
  AuthResponseDTO
  IAuthService
  IPasswordHasher
  ITokenService

Domain
  User
  IPerson
  NaturalPerson
  JuridicalPerson
  Role
  PermissionClaim
  Cpf
  Cnpj
  IUserRepository

Infra.Data
  UserRepository
  MySqlContext
  users table

Infra.IoC
  Pbkdf2PasswordHasher
  JwtTokenService
  Registros de injecao de dependencia
```

A senha nao e armazenada em texto puro. Ela e transformada em hash usando PBKDF2 com SHA-256, salt aleatorio e comparacao em tempo constante.

O token gerado segue o formato JWT com assinatura HMAC SHA-256. A chave deve ser configurada por `JWT_SECRET`.

Exemplo de cadastro de pessoa fisica:

```json
{
  "email": "carlos@email.com",
  "password": "12345678",
  "phone": "(18)99745-0885",
  "personType": "natural",
  "firstName": "Carlos",
  "lastName": "Silva",
  "cpf": "12345678901"
}
```

Exemplo de cadastro de pessoa juridica:

```json
{
  "email": "contato@empresa.com",
  "password": "12345678",
  "personType": "juridical",
  "cnpj": "12345678000190",
  "registeredName": "Empresa LTDA",
  "businessName": "Empresa"
}
```
