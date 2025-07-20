#!/bin/bash

echo "=== CONFIGURANDO ENTORNO PARA SONARQUBE ==="

# Actualizar e instalar dependencias
apt-get update -qq
apt-get install -y curl openjdk-17-jdk >/dev/null 2>&1

echo "=== INSTALANDO SONARSCANNER .NET VERSIÓN ANTERIOR ==="
# Instalar versión específica que no tiene el problema del host
dotnet tool install --global dotnet-sonarscanner --version 5.15.0

# Agregar herramientas de .NET al PATH
export PATH="$PATH:/root/.dotnet/tools"

echo "=== CONFIGURACIONES ESPECÍFICAS ==="
PROJECT_KEY="GaleriaArte"
SONAR_TOKEN="sqa_894207520127384b663fc42f3c96440d83cfd98d"

# Detectar automáticamente la IP de SonarQube en la red Docker
echo "=== DETECTANDO IP DE SONARQUBE ==="
SONAR_IP="172.18.0.3"
if [ -z "$SONAR_IP" ]; then
    echo "Error: No se pudo detectar la IP de SonarQube"
    echo "Asegúrate de que SonarQube esté corriendo: docker-compose -f docker-compose.sonarqube.yml up -d"
    exit 1
fi
SONAR_HOST="http://$SONAR_IP:9000"

echo "Project Key: $PROJECT_KEY"
echo "Host: $SONAR_HOST (IP detectada automáticamente)"

echo "=== VERIFICANDO CONECTIVIDAD ==="
curl -s $SONAR_HOST/api/system/status | head -1

echo "=== INICIANDO ANÁLISIS SONARQUBE ==="

# BEGIN: Usar token de autenticación y trabajar desde la raíz con solution
cd /src
dotnet sonarscanner begin \
  /k:"$PROJECT_KEY" \
  /d:sonar.host.url="$SONAR_HOST" \
  /d:sonar.token="$SONAR_TOKEN"

if [ $? -ne 0 ]; then
    echo "ERROR: Falló el comando begin de SonarScanner"
    exit 1
fi

echo "=== COMPILANDO SOLUTION COMPLETA ==="
# Compilar usando el archivo .sln desde la raíz
dotnet build GaleriaArte_V2.sln

if [ $? -ne 0 ]; then
    echo "ERROR: Falló la compilación de la solution"
    exit 1
fi

echo "=== FINALIZANDO ANÁLISIS ==="
dotnet sonarscanner end \
  /d:sonar.token="$SONAR_TOKEN"

if [ $? -eq 0 ]; then
    echo "=== ANÁLISIS COMPLETADO CON ÉXITO ==="
    echo "Revisar resultados en: http://localhost:9000/dashboard?id=$PROJECT_KEY"
else
    echo "ERROR: Falló el comando end de SonarScanner"
    exit 1
fi
