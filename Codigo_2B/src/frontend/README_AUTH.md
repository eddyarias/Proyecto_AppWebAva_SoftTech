# Sistema de Autenticación - Galería de Arte Frontend

Este documento describe el sistema de autenticación implementado para el frontend de la Galería de Arte.

## Características Implementadas

### 1. **Iniciar Sesión (`/login`)**
- Permite a los usuarios autenticarse usando nickname o correo electrónico
- Validación del lado cliente y servidor
- Manejo de cookies JWT automático
- Redirección automática después del login exitoso

### 2. **Registro de Usuario (`/registro`)**
- Formulario completo de registro con validaciones
- Selección de rol (cliente/artista)
- Validación de correo electrónico y contraseñas
- Confirmación de contraseña
- Redirección automática al login después del registro

### 3. **Recuperación de Contraseña (`/recuperar-password`)**
- Solicitud de recuperación por correo electrónico
- Interfaz intuitiva con instrucciones claras
- Manejo de errores y confirmaciones

### 4. **Restablecimiento de Contraseña (`/restablecer-password`)**
- Acepta token de recuperación por URL o input manual
- Validación de nueva contraseña
- Confirmación de contraseña
- Feedback visual del proceso

### 5. **Protección de Rutas**
- Componente `AuthGuard` para proteger páginas
- Verificación automática de autenticación
- Redirección a login si no está autenticado
- Soporte para roles específicos

## Estructura del Sistema

### Servicios

#### `AuthService`
- **Ubicación**: `Services/AuthService.cs`
- **Funciones principales**:
  - `LoginAsync()` - Autenticación de usuario
  - `RegistrarAsync()` - Registro de nuevo usuario
  - `SolicitarRecuperacionAsync()` - Solicitud de recuperación
  - `RestablecerPasswordAsync()` - Restablecimiento de contraseña
  - `LogoutAsync()` - Cierre de sesión
  - `CheckAuthStateAsync()` - Verificación de estado de autenticación

### Modelos

#### `Usuario.cs`
- **LoginRequest** - Datos para login
- **RegistroRequest** - Datos para registro con validaciones
- **SolicitarRecuperacionRequest** - Solicitud de recuperación
- **RestablecerPasswordRequest** - Restablecimiento de contraseña
- **ApiResponse** - Respuesta estándar de la API

### Componentes

#### `AuthGuard.razor`
Componente para proteger rutas que requieren autenticación:

```razor
<AuthGuard RequiereAutenticacion="true" RolRequerido="artista">
    <!-- Contenido protegido -->
</AuthGuard>
```

**Parámetros**:
- `RequiereAutenticacion` - Si requiere estar autenticado (default: true)
- `RolRequerido` - Rol específico requerido (opcional)
- `RedirigirSiNoAutenticado` - URL de redirección si no está autenticado (default: "/login")
- `RedirigirSiNoAutorizado` - URL de redirección si no tiene permisos (default: "/acceso-denegado")

### Páginas de Autenticación

1. **Login.razor** (`/login`)
2. **Registro.razor** (`/registro`) 
3. **RecuperarPassword.razor** (`/recuperar-password`)
4. **RestablecerPassword.razor** (`/restablecer-password`)
5. **AccesoDenegado.razor** (`/acceso-denegado`)

## Integración con API Gateway

El sistema está configurado para trabajar con el API Gateway en `http://localhost:5000/`:

- **Login**: `POST /auth/login`
- **Registro**: `POST /usuario/registrar`
- **Recuperación**: `POST /recuperacion/solicitar`
- **Restablecimiento**: `POST /recuperacion/restablecer`

### Manejo de Cookies

El sistema maneja automáticamente las cookies JWT:
- `acces_token` - Token de acceso (15 minutos)
- `refreshToken` - Token de refresco (7 días)

## Uso en Páginas

### Página Básica Protegida
```razor
@page "/mi-pagina"
@using GaleriaArteFrontend.Components

<AuthGuard>
    <h1>Contenido solo para usuarios autenticados</h1>
</AuthGuard>
```

### Página para Artistas Únicamente
```razor
@page "/artistas-solo"
@using GaleriaArteFrontend.Components

<AuthGuard RolRequerido="artista">
    <h1>Panel de Artista</h1>
</AuthGuard>
```

### Verificación Manual de Autenticación
```razor
@inject AuthService AuthService

@if (AuthService.EstaAutenticado)
{
    <p>Bienvenido, @AuthService.UsuarioActual?.Nickname</p>
}
else
{
    <a href="/login">Iniciar Sesión</a>
}
```

## Navegación Adaptativa

El `MainLayout.razor` incluye navegación que se adapta según el estado de autenticación:

- **No autenticado**: Enlaces a Login y Registro
- **Autenticado**: Menú de usuario con perfil y logout
- **Artista**: Enlaces adicionales para gestión de obras

## Estilos y UX

Cada página de autenticación incluye:
- Diseño responsive
- Animaciones y transiciones suaves
- Feedback visual para errores y éxitos
- Iconografía consistente con Font Awesome
- Gradientes y efectos modernos

## Validaciones

### Del lado Cliente:
- Campos requeridos
- Formato de email
- Longitud mínima de contraseñas
- Coincidencia de contraseñas

### Del lado Servidor:
- Validación adicional en el backend
- Manejo de errores específicos
- Mensajes descriptivos

## Consideraciones de Seguridad

1. **Tokens JWT**: Almacenados en cookies HttpOnly
2. **HTTPS**: Configurado para producción
3. **Validación**: Doble validación cliente/servidor
4. **Timeout**: Tokens con expiración automática
5. **CORS**: Configurado en el API Gateway

## Testing

Página de prueba disponible en `/test-auth` para verificar:
- Estado de autenticación
- Información del usuario actual
- Funcionalidad de logout

## Próximos Pasos

1. Implementar refresh token automático
2. Agregar autenticación por redes sociales
3. Implementar remember me
4. Mejorar manejo de errores de red
5. Agregar tests unitarios

## Troubleshooting

### Problemas Comunes:

1. **Error de CORS**: Verificar configuración del API Gateway
2. **Cookies no se guardan**: Verificar configuración HTTPS
3. **Redirect loop**: Verificar rutas de autenticación en AuthGuard
4. **Validation errors**: Verificar anotaciones en modelos

### Logs Útiles:
- Consola del navegador para errores JavaScript
- Red del navegador para requests HTTP
- Logs del API Gateway para errores de backend
