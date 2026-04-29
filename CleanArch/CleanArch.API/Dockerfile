FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY NuGet.config ./
COPY real-time-recommendation-system.sln ./
COPY CleanArch/CleanArch.API/CleanArch.API.csproj CleanArch/CleanArch.API/
COPY CleanArch/CleanArch.Application/CleanArch.Application.csproj CleanArch/CleanArch.Application/
COPY CleanArch/CleanArch.Domain/CleanArch.Domain.csproj CleanArch/CleanArch.Domain/
COPY CleanArch/CleanArch.Infra.Data/CleanArch.Infra.Data.csproj CleanArch/CleanArch.Infra.Data/
COPY CleanArch/CleanArch.Infra.IoC/CleanArch.Infra.IoC.csproj CleanArch/CleanArch.Infra.IoC/

RUN dotnet restore CleanArch/CleanArch.API/CleanArch.API.csproj

COPY . .
RUN dotnet publish CleanArch/CleanArch.API/CleanArch.API.csproj -c Release -o /app/publish --no-restore

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CleanArch.API.dll"]
