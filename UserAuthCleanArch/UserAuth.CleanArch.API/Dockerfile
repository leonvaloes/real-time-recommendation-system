FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY NuGet.config ./
COPY real-time-recommendation-system.sln ./
COPY UserAuthCleanArch/UserAuth.CleanArch.API/UserAuth.CleanArch.API.csproj UserAuthCleanArch/UserAuth.CleanArch.API/
COPY UserAuthCleanArch/UserAuth.CleanArch.Application/UserAuth.CleanArch.Application.csproj UserAuthCleanArch/UserAuth.CleanArch.Application/
COPY UserAuthCleanArch/UserAuth.CleanArch.Domain/UserAuth.CleanArch.Domain.csproj UserAuthCleanArch/UserAuth.CleanArch.Domain/
COPY UserAuthCleanArch/UserAuth.CleanArch.Infra.Data/UserAuth.CleanArch.Infra.Data.csproj UserAuthCleanArch/UserAuth.CleanArch.Infra.Data/
COPY UserAuthCleanArch/UserAuth.CleanArch.Infra.IoC/UserAuth.CleanArch.Infra.IoC.csproj UserAuthCleanArch/UserAuth.CleanArch.Infra.IoC/

RUN dotnet restore UserAuthCleanArch/UserAuth.CleanArch.API/UserAuth.CleanArch.API.csproj

COPY . .
RUN dotnet publish UserAuthCleanArch/UserAuth.CleanArch.API/UserAuth.CleanArch.API.csproj -c Release -o /app/publish --no-restore

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UserAuth.CleanArch.API.dll"]
