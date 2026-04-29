FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY NuGet.config ./
COPY Services/CatalogService/CatalogService.csproj Services/CatalogService/
RUN dotnet restore Services/CatalogService/CatalogService.csproj

COPY . .
RUN dotnet publish Services/CatalogService/CatalogService.csproj -c Release -o /app/publish --no-restore

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.dll"]
