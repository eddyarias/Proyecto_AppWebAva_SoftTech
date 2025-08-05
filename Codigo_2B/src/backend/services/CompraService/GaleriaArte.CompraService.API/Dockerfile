# ------------------------------------------
# Etapa 1: Build
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiamos el archivo de solución y los csproj necesarios
COPY GaleriaArte_V2.sln ./
COPY src/backend/services/CompraService/GaleriaArte.CompraService.API/GaleriaArte.CompraService.API.csproj ./src/backend/services/CompraService/GaleriaArte.CompraService.API/
COPY src/backend/services/CompraService/GaleriaArte.CompraService.Application/GaleriaArte.CompraService.Application.csproj ./src/backend/services/CompraService/GaleriaArte.CompraService.Application/
COPY src/backend/services/CompraService/GaleriaArte.CompraService.Domain/GaleriaArte.CompraService.Domain.csproj ./src/backend/services/CompraService/GaleriaArte.CompraService.Domain/
COPY src/backend/services/CompraService/GaleriaArte.CompraService.Infrastructure/GaleriaArte.CompraService.Infrastructure.csproj ./src/backend/services/CompraService/GaleriaArte.CompraService.Infrastructure/

# Restauramos dependencias
RUN dotnet restore src/backend/services/CompraService/GaleriaArte.CompraService.API/GaleriaArte.CompraService.API.csproj

# Copiamos el resto del código
COPY src/backend/services/CompraService/ ./src/backend/services/CompraService/

# Publicamos el proyecto en modo Release
RUN dotnet publish src/backend/services/CompraService/GaleriaArte.CompraService.API/GaleriaArte.CompraService.API.csproj -c Release -o /app/publish

# ------------------------------------------
# Etapa 2: Runtime
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Copia los archivos publicados
COPY --from=build /app/publish .

# Expone el puerto 5051
EXPOSE 5051

# Configura las URLs
ENV ASPNETCORE_URLS=http://+:5051

# Punto de entrada
ENTRYPOINT ["dotnet", "GaleriaArte.CompraService.API.dll"]
