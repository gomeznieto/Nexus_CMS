# Base: solo runtime (más liviano)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Etapa de build con SDK completo
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Backend_portafolio/Backend_portafolio.csproj", "Backend_portafolio/"]
RUN dotnet restore "./Backend_portafolio/Backend_portafolio.csproj"
COPY . .
WORKDIR "/src/Backend_portafolio"
RUN dotnet build "./Backend_portafolio.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de publicación
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Backend_portafolio.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final: producción
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Backend_portafolio.dll"]
