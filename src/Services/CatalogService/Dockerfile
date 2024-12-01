FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Services/CatalogService/CatalogService.csproj", "src/Services/CatalogService/"]
COPY ["src/BuildingBlocks/Logging/Logging.csproj", "src/BuildingBlocks/Logging/"]

RUN dotnet restore "src/Services/CatalogService/CatalogService.csproj"
COPY . .
WORKDIR "/src/src/Services/CatalogService/"
RUN dotnet build "./CatalogService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CatalogService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.dll"]