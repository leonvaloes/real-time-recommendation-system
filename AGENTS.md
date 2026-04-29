# Repository Guidelines

## Project Structure & Module Organization
This repository follows a layered Clean Architecture layout under `CleanArch/`. `CleanArch.Domain` holds entities, repository contracts, and validation rules. `CleanArch.Application` contains service interfaces, mappings, and application logic. `CleanArch.Infra.Data` provides MongoDB context and repository implementations, while `CleanArch.Infra.IoC` centralizes dependency injection. Tests currently live in `CleanArch.Domain.Tests`. Docker assets are isolated in `Docker/`.

## Build, Test, and Development Commands
Use the test project as the entry point because it references the domain layer.

- `dotnet build CleanArch/CleanArch.Domain.Tests/CleanArch.Domain.Tests.csproj` builds the test project and referenced projects.
- `dotnet test CleanArch/CleanArch.Domain.Tests/CleanArch.Domain.Tests.csproj` runs the xUnit suite.
- `dotnet test CleanArch/CleanArch.Domain.Tests/CleanArch.Domain.Tests.csproj --collect:"XPlat Code Coverage"` collects coverage with Coverlet.
- `docker compose -f Docker/docker-compose.yml up -d` starts MongoDB and Mongo Express for local infrastructure.

## Coding Style & Naming Conventions
Follow existing C# conventions: 4-space indentation, one public type per file, `PascalCase` for types and methods, `_camelCase` for private readonly fields, and clear interface names prefixed with `I` such as `IUserRepository`. Keep domain validation close to the entity or value object it protects. Prefer nullable annotations consistent with `Nullable` being enabled on all projects.

## Testing Guidelines
Tests use `xUnit` with `FluentAssertions`. Name test files after the subject under test, for example `UserTests.cs`, and use method names in the `Action_ShouldResult_WhenCondition` style. Add domain tests for validation branches and application tests when service behavior is implemented. Run coverage locally before opening a PR when changing core business rules.

## Commit & Pull Request Guidelines
The visible history is minimal (`first commit`), so adopt short, imperative commit subjects such as `Add user repository validation`. Keep commits focused on a single layer or behavior. PRs should include a concise description, note any MongoDB or environment-variable changes, link the related issue if one exists, and include test results for `dotnet test`.

## Configuration & Security
`CleanArch.Infra.Data` expects `MONGODB_URI` and `MONGODB_DB` environment variables. Do not hardcode credentials in source files; prefer local environment configuration and the Docker Compose stack for development.
