# ------------------------------------------
# Etapa 1: Build
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiamos el archivo de solución y los csproj necesarios
COPY GaleriaArte_V2.sln ./
COPY src/backend/services/UsuarioService/GaleriaArte.UsuarioService.API/GaleriaArte.UsuarioService.API.csproj ./src/backend/services/UsuarioService/GaleriaArte.UsuarioService.API/
COPY src/backend/services/UsuarioService/GaleriaArte.UsuarioService.Application/GaleriaArte.UsuarioService.Application.csproj ./src/backend/services/UsuarioService/GaleriaArte.UsuarioService.Application/
COPY src/backend/services/UsuarioService/GaleriaArte.UsuarioService.Domain/GaleriaArte.UsuarioService.Domain.csproj ./src/backend/services/UsuarioService/GaleriaArte.UsuarioService.Domain/
COPY src/backend/services/UsuarioService/GaleriaArte.UsuarioService.Infrastructure/GaleriaArte.UsuarioService.Infrastructure.csproj ./src/backend/services/UsuarioService/GaleriaArte.UsuarioService.Infrastructure/

# Restauramos dependencias
RUN dotnet restore src/backend/services/UsuarioService/GaleriaArte.UsuarioService.API/GaleriaArte.UsuarioService.API.csproj

# Copiamos el resto del código
COPY src/backend/services/UsuarioService/ ./src/backend/services/UsuarioService/

# Publicamos el proyecto en modo Release
RUN dotnet publish src/backend/services/UsuarioService/GaleriaArte.UsuarioService.API/GaleriaArte.UsuarioService.API.csproj -c Release -o /app/publish

# ------------------------------------------
# Etapa 2: Runtime
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Copia los archivos publicados
COPY --from=build /app/publish .

# Expone el puerto 5002
EXPOSE 5002

# Configura las URLs
ENV ASPNETCORE_URLS=http://+:5002

# Punto de entrada
ENTRYPOINT ["dotnet", "GaleriaArte.UsuarioService.API.dll"]
