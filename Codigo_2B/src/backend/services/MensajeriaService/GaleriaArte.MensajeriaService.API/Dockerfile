# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/backend/services/MensajeriaService/GaleriaArte.MensajeriaService.API/GaleriaArte.MensajeriaService.API.csproj", "src/backend/services/MensajeriaService/GaleriaArte.MensajeriaService.API/"]
COPY ["src/backend/services/MensajeriaService/GaleriaArte.MensajeriaService.Domain/GaleriaArte.MensajeriaService.Domain.csproj", "src/backend/services/MensajeriaService/GaleriaArte.MensajeriaService.Domain/"]
COPY ["src/backend/services/MensajeriaService/GaleriaArte.MensajeriaService.Infrastructure/GaleriaArte.MensajeriaService.Infrastructure.csproj", "src/backend/services/MensajeriaService/GaleriaArte.MensajeriaService.Infrastructure/"]
COPY ["src/backend/services/MensajeriaService/GaleriaArte.MensajeriaService.Application/GaleriaArte.MensajeriaService.Application.csproj", "src/backend/services/MensajeriaService/GaleriaArte.MensajeriaService.Application/"]

RUN dotnet restore "src/backend/services/MensajeriaService/GaleriaArte.MensajeriaService.API/GaleriaArte.MensajeriaService.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/backend/services/MensajeriaService/GaleriaArte.MensajeriaService.API"
RUN dotnet build "GaleriaArte.MensajeriaService.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "GaleriaArte.MensajeriaService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN addgroup --system --gid 1001 dotnet && \
    adduser --system --uid 1001 --ingroup dotnet dotnet

# Copy published app
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs && chown -R dotnet:dotnet /app

# Switch to non-root user
USER dotnet

EXPOSE 5105

# Configure URLs
ENV ASPNETCORE_URLS=http://+:5105

# Health check
HEALTHCHECK --interval=30s --timeout=30s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5105/health || exit 1

ENTRYPOINT ["dotnet", "GaleriaArte.MensajeriaService.API.dll"]
