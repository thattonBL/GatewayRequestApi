#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Microservices/GatewayRequestApi/GatewayRequestApi/GatewayRequestApi.csproj", "src/Microservices/GatewayRequestApi/GatewayRequestApi/"]
COPY ["src/Microservices/GatewayRequestApi/Message.Domain/Message.Domain.csproj", "src/Microservices/GatewayRequestApi/Message.Domain/"]
COPY ["src/Microservices/GatewayRequestApi/Message.Infrastructure/Message.Infrastructure.csproj", "src/Microservices/GatewayRequestApi/Message.Infrastructure/"]
RUN dotnet restore "./src/Microservices/GatewayRequestApi/GatewayRequestApi/GatewayRequestApi.csproj"
COPY . .
WORKDIR "/src/src/Microservices/GatewayRequestApi/GatewayRequestApi"
RUN dotnet build "./GatewayRequestApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GatewayRequestApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GatewayRequestApi.dll"]