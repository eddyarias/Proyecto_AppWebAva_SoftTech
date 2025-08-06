# ------------------------------------------
# Etapa 1: Build
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiamos el archivo de solución y los csproj necesarios
COPY GaleriaArte_V2.sln ./
COPY src/backend/services/ObraService/GaleriaArte.ObraService.API/GaleriaArte.ObraService.API.csproj ./src/backend/services/ObraService/GaleriaArte.ObraService.API/
COPY src/backend/services/ObraService/GaleriaArte.ObraService.Application/GaleriaArte.ObraService.Application.csproj ./src/backend/services/ObraService/GaleriaArte.ObraService.Application/
COPY src/backend/services/ObraService/GaleriaArte.ObraService.Domain/GaleriaArte.ObraService.Domain.csproj ./src/backend/services/ObraService/GaleriaArte.ObraService.Domain/
COPY src/backend/services/ObraService/GaleriaArte.ObraService.Infrastructure/GaleriaArte.ObraService.Infrastructure.csproj ./src/backend/services/ObraService/GaleriaArte.ObraService.Infrastructure/

# Restauramos dependencias
RUN dotnet restore src/backend/services/ObraService/GaleriaArte.ObraService.API/GaleriaArte.ObraService.API.csproj

# Copiamos el resto del código
COPY src/backend/services/ObraService/ ./src/backend/services/ObraService/

# Publicamos el proyecto en modo Release
RUN dotnet publish src/backend/services/ObraService/GaleriaArte.ObraService.API/GaleriaArte.ObraService.API.csproj -c Release -o /app/publish

# ------------------------------------------
# Etapa 2: Runtime
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Copia los archivos publicados
COPY --from=build /app/publish .

# Expone el puerto 5003
EXPOSE 5003

# Configura las URLs
ENV ASPNETCORE_URLS=http://+:5003

# Punto de entrada
ENTRYPOINT ["dotnet", "GaleriaArte.ObraService.API.dll"]
