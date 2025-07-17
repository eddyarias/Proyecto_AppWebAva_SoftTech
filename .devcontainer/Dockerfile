FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

# Instalar herramientas necesarias
RUN apt-get update && apt-get install -y \
    curl \
    git \
    unzip \
    postgresql-client \
    && rm -rf /var/lib/apt/lists/*

# Opcional: Instalar Node.js si lo necesitas
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y nodejs

# RabbitMQ Tools (opcional CLI para debugging)
# RUN apt-get install -y rabbitmq-server

# Configurar el PATH de dotnet para global tools
ENV PATH="${PATH}:/root/.dotnet/tools"
