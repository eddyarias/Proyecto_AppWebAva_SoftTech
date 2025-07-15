# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln ./
COPY src/backend/services/UsuarioService/*.csproj ./src/backend/services/UsuarioService/
RUN dotnet restore ./src/backend/services/UsuarioService/UsuarioService.csproj

COPY . .
WORKDIR /app/src/backend/services/UsuarioService
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "UsuarioService.dll"]