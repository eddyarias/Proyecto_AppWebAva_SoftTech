FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002
ENV ASPNETCORE_URLS=http://+:5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/backend/services/UsuarioService/UsuarioService.csproj", "backend/services/UsuarioService/"]
RUN dotnet restore "backend/services/UsuarioService/UsuarioService.csproj"

COPY src/backend/services/UsuarioService/. backend/services/UsuarioService/
WORKDIR "/src/backend/services/UsuarioService"
RUN dotnet build "UsuarioService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish UsuarioService.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsuarioService.dll"]