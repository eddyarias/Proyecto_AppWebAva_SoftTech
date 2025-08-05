-- ==========================================
-- 📦 GALERIA DE ARTE - BASE DE DATOS POR SERVICIO
-- ==========================================

-- 🔧 EXTENSIONES NECESARIAS
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- =============================
-- 👤 USUARIO SERVICE (GaleriaArteUsuarios)
-- =============================

CREATE SCHEMA IF NOT EXISTS usuarios;

-- Tabla de roles
CREATE TABLE usuarios.roles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre VARCHAR(30) UNIQUE NOT NULL,
    descripcion TEXT
);

-- Tabla de usuarios (rol_id como FK directa)
CREATE TABLE usuarios.usuarios (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nickname VARCHAR(50) UNIQUE NOT NULL,
    correo VARCHAR(255) UNIQUE NOT NULL,
    contraseña_hash TEXT NOT NULL,
    estado BOOLEAN DEFAULT TRUE,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    refresh_token TEXT,
    refresh_token_exp TIMESTAMP,
    rol_id UUID NOT NULL REFERENCES usuarios.roles(id)
);

-- Tabla de intentos de recuperación
CREATE TABLE usuarios.intentos_recuperacion (
    id SERIAL PRIMARY KEY,
    usuario_id UUID REFERENCES usuarios.usuarios(id),
    token_recuperacion UUID NOT NULL,
    expiracion TIMESTAMP NOT NULL,
    usado BOOLEAN DEFAULT FALSE,
    fecha_solicitud TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insertar roles por defecto
INSERT INTO usuarios.roles (id,nombre, descripcion) VALUES
('9a66346f-164e-4ce2-8e5b-568b8f643799','artista', 'Usuario que publica y firma obras de arte'),
('62d2b61f-d92b-44d8-ada8-9d5dace7e6bc','comprador', 'Usuario que puede adquirir obras'),
('f50fdbe5-2e16-4e91-9e7b-a39219d57031','administrador', 'Usuario con privilegios de gestión de cuentas y contenido');

-- =============================
-- 🎨 OBRA SERVICE (GaleriaArteObras)
-- =============================

CREATE SCHEMA IF NOT EXISTS obras;

CREATE TABLE obras.obras (
    id SERIAL PRIMARY KEY,
    titulo VARCHAR(255) NOT NULL,
    descripcion TEXT,
    archivo_base64 TEXT NOT NULL, -- Imagen JPG en formato base64
    firma_digital TEXT NOT NULL,
    artista_nickname VARCHAR(50) NOT NULL,
    precio DECIMAL(10,2) NOT NULL,
    estado VARCHAR(20) CHECK (estado IN ('activa', 'vendida', 'oculta')),
    fecha_publicacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =============================
-- 🛒 COMPRA SERVICE (GaleriaArteCompras)
-- =============================

CREATE SCHEMA IF NOT EXISTS compras;

CREATE TABLE compras.compras (
    id SERIAL PRIMARY KEY,
    obra_id INT NOT NULL,
    comprador_nickname VARCHAR(50) NOT NULL,
    hash_prueba_compra TEXT NOT NULL,
    fecha_compra TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE compras.accesos_obras (
    id SERIAL PRIMARY KEY,
    compra_id INT REFERENCES compras.compras(id),
    token_acceso UUID DEFAULT uuid_generate_v4(),
    expiracion TIMESTAMP NOT NULL,
    visualizaciones INT DEFAULT 0
);

-- =============================
-- 🧠 AUDITORIA SERVICE (GaleriaArteAuditoria)
-- =============================

CREATE SCHEMA IF NOT EXISTS auditoria;

CREATE TABLE auditoria.logs_eventos (
    id SERIAL PRIMARY KEY,
    microservicio TEXT NOT NULL,
    evento TEXT NOT NULL,
    usuario_id TEXT,
    rol_id TEXT,
    ip TEXT,
    datos JSONB,
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =============================
-- 🔔 NOTIFICACION SERVICE (GaleriaArteNotificaciones)
-- =============================

CREATE SCHEMA IF NOT EXISTS notificaciones;

CREATE TABLE notificaciones.notificaciones (
    id SERIAL PRIMARY KEY,
    tipo TEXT NOT NULL,
    destinatario TEXT NOT NULL,
    mensaje TEXT NOT NULL,
    enviado BOOLEAN DEFAULT FALSE,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =============================
-- 💬 MENSAJERIA SERVICE (GaleriaArteMensajeria)
-- =============================

CREATE SCHEMA IF NOT EXISTS mensajeria;

-- Tabla de conversaciones
CREATE TABLE mensajeria.conversaciones (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    titulo VARCHAR(255),
    tipo VARCHAR(20) CHECK (tipo IN ('privada', 'grupo', 'soporte')) DEFAULT 'privada',
    obra_id INT, -- Referencia a obra si la conversación es sobre una obra específica
    creador_nickname VARCHAR(50) NOT NULL,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    activa BOOLEAN DEFAULT TRUE
);

-- Tabla de participantes en conversaciones
CREATE TABLE mensajeria.participantes_conversacion (
    id SERIAL PRIMARY KEY,
    conversacion_id UUID REFERENCES mensajeria.conversaciones(id) ON DELETE CASCADE,
    usuario_nickname VARCHAR(50) NOT NULL,
    fecha_union TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    activo BOOLEAN DEFAULT TRUE,
    UNIQUE(conversacion_id, usuario_nickname)
);

-- Tabla de mensajes
CREATE TABLE mensajeria.mensajes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    conversacion_id UUID REFERENCES mensajeria.conversaciones(id) ON DELETE CASCADE,
    remitente_nickname VARCHAR(50) NOT NULL,
    contenido TEXT NOT NULL,
    tipo_mensaje VARCHAR(20) CHECK (tipo_mensaje IN ('texto', 'imagen', 'archivo', 'sistema')) DEFAULT 'texto',
    archivo_adjunto TEXT, -- Base64 o referencia al archivo
    fecha_envio TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    editado BOOLEAN DEFAULT FALSE,
    fecha_edicion TIMESTAMP
);

-- Tabla de estados de lectura de mensajes
CREATE TABLE mensajeria.estados_lectura (
    id SERIAL PRIMARY KEY,
    mensaje_id UUID REFERENCES mensajeria.mensajes(id) ON DELETE CASCADE,
    usuario_nickname VARCHAR(50) NOT NULL,
    fecha_lectura TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(mensaje_id, usuario_nickname)
);

-- Tabla de mensajes eliminados (soft delete)
CREATE TABLE mensajeria.mensajes_eliminados (
    id SERIAL PRIMARY KEY,
    mensaje_id UUID REFERENCES mensajeria.mensajes(id) ON DELETE CASCADE,
    usuario_nickname VARCHAR(50) NOT NULL,
    fecha_eliminacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(mensaje_id, usuario_nickname)
);

-- Índices para mejorar el rendimiento
CREATE INDEX idx_conversaciones_creador ON mensajeria.conversaciones(creador_nickname);
CREATE INDEX idx_conversaciones_obra ON mensajeria.conversaciones(obra_id);
CREATE INDEX idx_participantes_conversacion ON mensajeria.participantes_conversacion(conversacion_id);
CREATE INDEX idx_participantes_usuario ON mensajeria.participantes_conversacion(usuario_nickname);
CREATE INDEX idx_mensajes_conversacion ON mensajeria.mensajes(conversacion_id);
CREATE INDEX idx_mensajes_remitente ON mensajeria.mensajes(remitente_nickname);
CREATE INDEX idx_mensajes_fecha ON mensajeria.mensajes(fecha_envio);
CREATE INDEX idx_estados_lectura_mensaje ON mensajeria.estados_lectura(mensaje_id);
CREATE INDEX idx_estados_lectura_usuario ON mensajeria.estados_lectura(usuario_nickname);

-- =============================
-- 👥 USUARIOS DE BASE DE DATOS
-- =============================

CREATE USER galeria_user WITH PASSWORD 'galeria_pass';
GRANT ALL PRIVILEGES ON SCHEMA usuarios TO galeria_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA usuarios TO galeria_user;
GRANT ALL ON TABLE usuarios.intentos_recuperacion TO galeria_user;
GRANT USAGE, SELECT, UPDATE ON SEQUENCE usuarios.intentos_recuperacion_id_seq TO galeria_user;

CREATE USER obra_user WITH PASSWORD 'obra_pass';
GRANT ALL PRIVILEGES ON SCHEMA obras TO obra_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA obras TO obra_user;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA obras TO obra_user;

CREATE USER compra_user WITH PASSWORD 'compra_pass';
GRANT ALL PRIVILEGES ON SCHEMA compras TO compra_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA compras TO compra_user;

CREATE USER auditoria_user WITH PASSWORD 'auditoria_pass';
GRANT ALL PRIVILEGES ON SCHEMA auditoria TO auditoria_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA auditoria TO auditoria_user;
GRANT ALL ON TABLE auditoria.logs_eventos TO auditoria_user;
GRANT USAGE, SELECT, UPDATE ON SEQUENCE auditoria.logs_eventos_id_seq TO auditoria_user;

CREATE USER notificacion_user WITH PASSWORD 'notificacion_pass';
GRANT ALL PRIVILEGES ON SCHEMA notificaciones TO notificacion_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA notificaciones TO notificacion_user;

CREATE USER mensajeria_user WITH PASSWORD 'mensajeria_pass';
GRANT ALL PRIVILEGES ON SCHEMA mensajeria TO mensajeria_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA mensajeria TO mensajeria_user;
GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA mensajeria TO mensajeria_user;