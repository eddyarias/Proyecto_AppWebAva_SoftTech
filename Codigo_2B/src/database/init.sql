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

CREATE TABLE usuarios.usuarios (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nickname VARCHAR(50) UNIQUE NOT NULL,
    correo VARCHAR(255) UNIQUE NOT NULL,
    contraseña_hash TEXT NOT NULL,
    estado BOOLEAN DEFAULT TRUE,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    refresh_token TEXT,
    refresh_token_exp TIMESTAMP
);

CREATE TABLE usuarios.roles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre VARCHAR(30) UNIQUE NOT NULL,
    descripcion TEXT
);

CREATE TABLE usuarios.usuarios_roles (
    usuario_id UUID REFERENCES usuarios.usuarios(id),
    rol_id UUID REFERENCES usuarios.roles(id),
    PRIMARY KEY (usuario_id, rol_id)
);

CREATE TABLE usuarios.intentos_recuperacion (
    id SERIAL PRIMARY KEY,
    usuario_id UUID REFERENCES usuarios.usuarios(id),
    token_recuperacion UUID NOT NULL,
    expiracion TIMESTAMP NOT NULL,
    usado BOOLEAN DEFAULT FALSE,
    fecha_solicitud TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insertar roles por defecto
INSERT INTO usuarios.roles (nombre, descripcion) VALUES
('artista', 'Usuario que publica y firma obras de arte'),
('comprador', 'Usuario que puede adquirir obras'),
('administrador', 'Usuario con privilegios de gestión de cuentas y contenido');

-- =============================
-- 🎨 OBRA SERVICE (GaleriaArteObras)
-- =============================

CREATE SCHEMA IF NOT EXISTS obras;

CREATE TABLE obras.obras (
    id SERIAL PRIMARY KEY,
    titulo VARCHAR(255) NOT NULL,
    descripcion TEXT,
    archivo_url TEXT NOT NULL,
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
-- 👥 USUARIOS DE BASE DE DATOS
-- =============================

CREATE USER galeria_user WITH PASSWORD 'galeria_pass';
GRANT ALL PRIVILEGES ON SCHEMA usuarios TO galeria_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA usuarios TO galeria_user;

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

CREATE USER notificacion_user WITH PASSWORD 'notificacion_pass';
GRANT ALL PRIVILEGES ON SCHEMA notificaciones TO notificacion_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA notificaciones TO notificacion_user;