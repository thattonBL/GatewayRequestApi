#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GatewayRequestApi/GatewayRequestApi.csproj", "GatewayRequestApi/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/IntegrationEventLogEF/IntegrationEventLogEF.csproj", "BuildingBlocks/IntegrationEventLogEF/"]
COPY ["Common/Events.Common/Events.Common.csproj", "Common/Events.Common/"]
COPY ["Common/Services.Common/Services.Common.csproj", "Common/Services.Common/"]
COPY ["BuildingBlocks/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "BuildingBlocks/EventBusRabbitMQ/"]
COPY ["BuildingBlocks/EventBusServiceBus/EventBusServiceBus.csproj", "BuildingBlocks/EventBusServiceBus/"]
COPY ["Message.Domain/Message.Domain.csproj", "Message.Domain/"]
COPY ["Message.Infrastructure/Message.Infrastructure.csproj", "Message.Infrastructure/"]
RUN dotnet restore "./GatewayRequestApi/./GatewayRequestApi.csproj"
COPY . .
WORKDIR "/src/GatewayRequestApi"
RUN dotnet build "./GatewayRequestApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GatewayRequestApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GatewayRequestApi.dll"]