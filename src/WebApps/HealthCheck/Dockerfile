FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/WebApps/HealthCheck/HealthCheck.csproj", "src/WebApps/HealthCheck/"]
COPY ["src/BuildingBlocks/Logging/Logging.csproj", "src/BuildingBlocks/Logging/"]

RUN dotnet restore "src/WebApps/HealthCheck/HealthCheck.csproj"
COPY . . 
WORKDIR "/src/src/WebApps/HealthCheck/"
RUN dotnet build "./HealthCheck.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HealthCheck.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HealthCheck.dll"]