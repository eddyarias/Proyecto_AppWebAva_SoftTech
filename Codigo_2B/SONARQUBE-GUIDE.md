# Script de Análisis SonarQube - Proyecto Galería de Arte

Para ejecutar el análisis SonarQube en este proyecto:

## 🚀 Setup Rápido para Nuevos Desarrolladores

```bash
# 1. Iniciar Docker compose
docker-compose -f docker-compose.sonarqube.yml up -d

# 2. Generar token
# Ingresa a http://localhost:9000 con admin/admin, cambia la contraseña a AdminPassword1.
# Click en el usuario superior izquierda, My Account, Security, genera nuevo token con nombre analysis_token, de tipo Global.
# Copia el token y pégalo en "SONAR_TOKEN=" de "sonar-analysis-solution.sh".

# 3. Conseguir ip de sonarqube
docker inspect sonarqube_server --format='{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}'
# Pégalo en "SONAR_IP=" de "sonar-analysis-solution.sh".

# 4. Ejecución
docker run --rm -v ${PWD}:/workspace -v /var/run/docker.sock:/var/run/docker.sock --network codigo_2b_sonarqube_net mcr.microsoft.com/dotnet/sdk:8.0 bash -c "cd /workspace && bash sonar-analysis-solution.sh"

# 5. Visualización
# Ingresa a http://localhost:9000 > Project > Click en el generado y visualiza.
```