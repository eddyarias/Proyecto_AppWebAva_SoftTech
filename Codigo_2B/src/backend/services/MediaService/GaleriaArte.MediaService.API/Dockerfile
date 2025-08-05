# ------------------------------------------
# Etapa 1: Build
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiamos el archivo de solución y los csproj necesarios
COPY GaleriaArte_V2.sln ./
COPY src/backend/services/MediaService/GaleriaArte.MediaService.API/GaleriaArte.MediaService.API.csproj ./src/backend/services/MediaService/GaleriaArte.MediaService.API/
COPY src/backend/services/MediaService/GaleriaArte.MediaService.Application/GaleriaArte.MediaService.Application.csproj ./src/backend/services/MediaService/GaleriaArte.MediaService.Application/
COPY src/backend/services/MediaService/GaleriaArte.MediaService.Domain/GaleriaArte.MediaService.Domain.csproj ./src/backend/services/MediaService/GaleriaArte.MediaService.Domain/
COPY src/backend/services/MediaService/GaleriaArte.MediaService.Infrastructure/GaleriaArte.MediaService.Infrastructure.csproj ./src/backend/services/MediaService/GaleriaArte.MediaService.Infrastructure/

# Restauramos dependencias
RUN dotnet restore src/backend/services/MediaService/GaleriaArte.MediaService.API/GaleriaArte.MediaService.API.csproj

# Copiamos el resto del código
COPY src/backend/services/MediaService/ ./src/backend/services/MediaService/

# Publicamos el proyecto en modo Release
RUN dotnet publish src/backend/services/MediaService/GaleriaArte.MediaService.API/GaleriaArte.MediaService.API.csproj -c Release -o /app/publish

# ------------------------------------------
# Etapa 2: Runtime
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Copia los archivos publicados
COPY --from=build /app/publish .

# Expone el puerto 5108
EXPOSE 5108

# Configura las URLs
ENV ASPNETCORE_URLS=http://+:5108

# Punto de entrada
ENTRYPOINT ["dotnet", "GaleriaArte.MediaService.API.dll"]
