# ------------------------------------------
# Etapa 1: Build
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiamos el archivo de solución y los csproj necesarios
COPY GaleriaArte_V2.sln ./
COPY src/backend/ApiGateway/ApiGateway.csproj ./src/backend/ApiGateway/

# Restauramos dependencias
RUN dotnet restore src/backend/ApiGateway/ApiGateway.csproj

# Copiamos el resto del código
COPY src/backend/ApiGateway/ ./src/backend/ApiGateway/

# Publicamos el proyecto en modo Release
RUN dotnet publish src/backend/ApiGateway/ApiGateway.csproj -c Release -o /app/publish

# ------------------------------------------
# Etapa 2: Runtime
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Copia los archivos publicados
COPY --from=build /app/publish .

# Expone el puerto 5000
EXPOSE 5000

# Configura las URLs
ENV ASPNETCORE_URLS=http://+:5000

# Punto de entrada
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
