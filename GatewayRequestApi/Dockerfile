#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG AZURE_DEVOPS_PAT  # Add ARG for the PAT
WORKDIR /src
COPY ["GatewayRequestApi/nuget.config", "/src/nuget.config"]

# Replace the PAT placeholder in nuget.config
RUN sed -i "s|<add key=\"Gateway_Nuget_Feed\".*|<add key=\"Gateway_Nuget_Feed\" value=\"https://pkgs.dev.azure.com/BritishLibrary-AppDev/Gateway/_packaging/Gateway_Nuget_Feed/nuget/v3/index.json\" username=\"AzureDevOps\" password=\"$AZURE_DEVOPS_PAT\" />|" /src/nuget.config


COPY ["GatewayRequestApi/GatewayRequestApi.csproj", "GatewayRequestApi/"]
COPY ["Message.Domain/Message.Domain.csproj", "Message.Domain/"]
COPY ["Message.Infrastructure/Message.Infrastructure.csproj", "Message.Infrastructure/"]

# Authenticate and restore
RUN dotnet nuget add source "https://pkgs.dev.azure.com/BritishLibrary-AppDev/Gateway/_packaging/Gateway_Nuget_Feed/nuget/v3/index.json" --username AzureDevOps --password $AZURE_DEVOPS_PAT --store-password-in-clear-text
RUN dotnet restore "./GatewayRequestApi/GatewayRequestApi.csproj" --configfile /src/nuget.config

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