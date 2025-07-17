# ------------------------------------------
# Etapa 1: Build
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Creamos carpeta de trabajo principal
WORKDIR /src

# Copiamos el archivo de solución y los csproj necesarios
COPY GaleriaArte_V2.sln ./
COPY src/frontend/GaleriaArteFrontend/GaleriaArteFrontend.csproj ./GaleriaArteFrontend/

# Restauramos dependencias
RUN dotnet restore ./GaleriaArteFrontend/GaleriaArteFrontend.csproj

# Copiamos el resto del código del frontend
COPY src/frontend/GaleriaArteFrontend/ ./GaleriaArteFrontend/

# Publicamos el proyecto en modo Release
RUN dotnet publish ./GaleriaArteFrontend/GaleriaArteFrontend.csproj -c Release -o /app/publish

# ------------------------------------------
# Etapa 2: Runtime (Nginx sirve los archivos estáticos)
# ------------------------------------------
FROM nginx:alpine AS final

# Borra archivos por defecto de nginx
RUN rm -rf /usr/share/nginx/html/*

# Copia los archivos publicados al directorio de nginx
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Expone el puerto 80
EXPOSE 80

# Inicia nginx
CMD ["nginx", "-g", "daemon off;"]