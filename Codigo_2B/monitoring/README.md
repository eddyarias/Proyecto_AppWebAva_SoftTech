# Monitoreo con Prometheus + Grafana

Este directorio contiene la configuración completa para monitorear el **UsuarioService** utilizando Prometheus y Grafana.

## 🚀 Inicio Rápido

### Prerrequisitos
- Docker y Docker Compose instalados
- UsuarioService ejecutándose localmente (puerto 5002 por defecto)

### 1. Iniciar el stack de monitoreo

```bash
cd monitoring
docker-compose -f docker-compose.monitoring.yml up -d
```

### 2. Verificar que todo esté funcionando

- **Prometheus**: http://localhost:9090
- **Grafana**: http://localhost:3000 (usuario: `admin`, contraseña: `admin123`)

## 📊 Dashboards

### Dashboard del Usuario Service
Una vez que Grafana esté ejecutándose, encontrarás el dashboard "Usuario Service Dashboard" que incluye:

- **Request Rate**: Tasa de solicitudes por segundo
- **Response Time**: Percentil 95 del tiempo de respuesta
- **Error Rate**: Porcentaje de errores (códigos 5xx y 4xx)
- **Service Status**: Estado del servicio (Up/Down)

## 🔧 Configuración

### Métricas Personalizadas
El UsuarioService incluye métricas personalizadas:

- `usuario_service_requests_total`: Total de requests por endpoint
- `usuario_service_request_duration_seconds`: Duración de requests
- `usuario_service_active_users`: Usuarios activos
- `usuario_service_database_operations_total`: Operaciones de base de datos
- `usuario_service_authentication_errors_total`: Errores de autenticación
- `usuario_service_users_registered_total`: Usuarios registrados
- `usuario_service_successful_logins_total`: Logins exitosos

### Health Checks
El servicio expone un endpoint de health check en `/health` que verifica:
- Estado general del servicio
- Conectividad con la base de datos

## 🚨 Alertas

Se incluyen alertas predefinidas:

1. **UsuarioServiceDown**: Servicio no disponible por más de 30 segundos
2. **HighRequestLatency**: Latencia del percentil 95 mayor a 500ms por más de 2 minutos
3. **HighErrorRate**: Tasa de error mayor al 10% por más de 5 minutos
4. **DatabaseConnectionFailure**: Fallo en la conexión a la base de datos por más de 1 minuto

## 📝 Configuración del UsuarioService

### Paquetes NuGet añadidos:
```xml
<PackageReference Include="prometheus-net.AspNetCore" Version="8.2.1" />
<PackageReference Include="prometheus-net.AspNetCore.HealthChecks" Version="8.2.1" />
```

### Endpoints agregados:
- `/metrics`: Métricas de Prometheus
- `/health`: Health check

## 🔄 Comandos Útiles

### Ver logs de los contenedores:
```bash
docker-compose -f docker-compose.monitoring.yml logs -f
```

### Detener el stack:
```bash
docker-compose -f docker-compose.monitoring.yml down
```

### Detener y limpiar volúmenes:
```bash
docker-compose -f docker-compose.monitoring.yml down -v
```

### Reiniciar un servicio específico:
```bash
docker-compose -f docker-compose.monitoring.yml restart prometheus
docker-compose -f docker-compose.monitoring.yml restart grafana
```

## 📁 Estructura de archivos

```
monitoring/
├── docker-compose.monitoring.yml    # Configuración principal
├── start-monitoring.ps1            # Script de inicio
├── prometheus/
│   ├── prometheus.yml              # Configuración de Prometheus
│   └── alert.rules.yml             # Reglas de alertas
├── grafana/
│   ├── provisioning/
│   │   ├── datasources/            # Configuración de datasources
│   │   └── dashboards/             # Configuración de dashboards
│   └── dashboards/
│       └── usuario-service-dashboard.json  # Dashboard personalizado
└── alertmanager/
    └── alertmanager.yml            # Configuración de alertas
```

## 🎯 Próximos Pasos

1. **Personalizar alertas**: Modifica `prometheus/alert.rules.yml` según tus necesidades
2. **Configurar notificaciones**: Edita `alertmanager/alertmanager.yml` para enviar alertas por email, Slack, etc.
3. **Agregar más dashboards**: Crea dashboards adicionales en Grafana
4. **Monitorear otros servicios**: Replica esta configuración para otros microservicios
5. **Configurar retención**: Ajusta la retención de datos en Prometheus

## 🔍 Troubleshooting

### El servicio no aparece en Prometheus
1. Verifica que el UsuarioService esté ejecutándose en el puerto 5002
2. Comprueba que el endpoint `/metrics` responda: http://localhost:5002/metrics
3. Revisa la configuración en `prometheus/prometheus.yml`

### Grafana no muestra datos
1. Verifica que Prometheus esté recolectando métricas
2. Comprueba la configuración del datasource en Grafana
3. Revisa que las queries en el dashboard sean correctas

### Los contenedores no inician
1. Verifica que Docker esté ejecutándose
2. Comprueba que los puertos 3000, 9090 y 9093 estén disponibles
3. Revisa los logs: `docker-compose logs`
