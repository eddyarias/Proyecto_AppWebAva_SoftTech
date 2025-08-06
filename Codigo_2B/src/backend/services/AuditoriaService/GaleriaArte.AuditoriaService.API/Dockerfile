# ------------------------------------------
# Etapa 1: Build
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiamos el archivo de solución y los csproj necesarios
COPY GaleriaArte_V2.sln ./
COPY src/backend/services/AuditoriaService/GaleriaArte.AuditoriaService.API/GaleriaArte.AuditoriaService.API.csproj ./src/backend/services/AuditoriaService/GaleriaArte.AuditoriaService.API/
COPY src/backend/services/AuditoriaService/GaleriaArte.AuditoriaService.Application/GaleriaArte.AuditoriaService.Application.csproj ./src/backend/services/AuditoriaService/GaleriaArte.AuditoriaService.Application/
COPY src/backend/services/AuditoriaService/GaleriaArte.AuditoriaService.Domain/GaleriaArte.AuditoriaService.Domain.csproj ./src/backend/services/AuditoriaService/GaleriaArte.AuditoriaService.Domain/
COPY src/backend/services/AuditoriaService/GaleriaArte.AuditoriaService.Infrastructure/GaleriaArte.AuditoriaService.Infrastructure.csproj ./src/backend/services/AuditoriaService/GaleriaArte.AuditoriaService.Infrastructure/

# Restauramos dependencias
RUN dotnet restore src/backend/services/AuditoriaService/GaleriaArte.AuditoriaService.API/GaleriaArte.AuditoriaService.API.csproj

# Copiamos el resto del código
COPY src/backend/services/AuditoriaService/ ./src/backend/services/AuditoriaService/

# Publicamos el proyecto en modo Release
RUN dotnet publish src/backend/services/AuditoriaService/GaleriaArte.AuditoriaService.API/GaleriaArte.AuditoriaService.API.csproj -c Release -o /app/publish

# ------------------------------------------
# Etapa 2: Runtime
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Copia los archivos publicados
COPY --from=build /app/publish .

# Expone el puerto 5004
EXPOSE 5004

# Configura las URLs
ENV ASPNETCORE_URLS=http://+:5004

# Punto de entrada
ENTRYPOINT ["dotnet", "GaleriaArte.AuditoriaService.API.dll"]
