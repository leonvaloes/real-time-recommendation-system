FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY NuGet.config ./
COPY CatalogServiceMvc/CatalogServiceMvc.csproj CatalogServiceMvc/
RUN dotnet restore CatalogServiceMvc/CatalogServiceMvc.csproj

COPY . .
RUN dotnet publish CatalogServiceMvc/CatalogServiceMvc.csproj -c Release -o /app/publish --no-restore

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CatalogServiceMvc.dll"]
