-- ================================================
-- 🔧 EXTENSIONES NECESARIAS
-- ================================================

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ================================================
-- 🧩 TABLAS PRINCIPALES
-- ================================================

CREATE TABLE usuarios (
    id SERIAL PRIMARY KEY,
    nickname VARCHAR(50) UNIQUE NOT NULL,
    correo VARCHAR(255) UNIQUE NOT NULL,
    contraseña_hash TEXT NOT NULL,
    rol VARCHAR(20) NOT NULL CHECK (rol IN ('comprador', 'artista', 'admin')),
    estado BOOLEAN DEFAULT TRUE,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE obras (
    id SERIAL PRIMARY KEY,
    titulo VARCHAR(255) NOT NULL,
    descripcion TEXT,
    archivo_url TEXT NOT NULL,
    firma_digital TEXT NOT NULL,
    artista_id INTEGER NOT NULL REFERENCES usuarios(id),
    precio DECIMAL(10, 2) NOT NULL,
    estado VARCHAR(20) NOT NULL CHECK (estado IN ('activa', 'vendida', 'oculta')),
    fecha_publicacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE compras (
    id SERIAL PRIMARY KEY,
    obra_id INTEGER NOT NULL REFERENCES obras(id),
    comprador_id INTEGER NOT NULL REFERENCES usuarios(id),
    hash_prueba_compra TEXT NOT NULL,
    fecha_compra TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE accesos_obras (
    id SERIAL PRIMARY KEY,
    compra_id INTEGER NOT NULL REFERENCES compras(id),
    token_acceso UUID DEFAULT uuid_generate_v4(),
    expiracion TIMESTAMP NOT NULL,
    visualizaciones INT DEFAULT 0
);

CREATE TABLE logs_seguridad (
    id SERIAL PRIMARY KEY,
    usuario_id INTEGER REFERENCES usuarios(id),
    accion TEXT NOT NULL,
    ip_origen INET,
    user_agent TEXT,
    fecha_evento TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    detalles TEXT
);

CREATE TABLE intentos_recuperacion (
    id SERIAL PRIMARY KEY,
    usuario_id INTEGER NOT NULL REFERENCES usuarios(id),
    token_recuperacion UUID NOT NULL,
    expiracion TIMESTAMP NOT NULL,
    usado BOOLEAN DEFAULT FALSE,
    fecha_solicitud TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- FUNCIONES TRIGGER LOGS
CREATE OR REPLACE FUNCTION registrar_log(
    p_usuario_id INTEGER,
    p_accion TEXT,
    p_ip_origen INET DEFAULT NULL,
    p_user_agent TEXT DEFAULT NULL,
    p_detalles TEXT DEFAULT NULL
) RETURNS VOID AS $$
BEGIN
    INSERT INTO logs_seguridad(usuario_id, accion, ip_origen, user_agent, detalles)
    VALUES (p_usuario_id, p_accion, p_ip_origen, p_user_agent, p_detalles);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION log_creacion_usuario() RETURNS TRIGGER AS $$
BEGIN
    PERFORM registrar_log(NEW.id, 'CREACION_USUARIO', NULL, NULL, 'Usuario creado con nickname: ' || NEW.nickname);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_log_creacion_usuario
AFTER INSERT ON usuarios
FOR EACH ROW EXECUTE FUNCTION log_creacion_usuario();

CREATE OR REPLACE FUNCTION log_creacion_obra() RETURNS TRIGGER AS $$
BEGIN
    PERFORM registrar_log(NEW.artista_id, 'CREACION_OBRA', NULL, NULL, 'Obra: ' || NEW.titulo);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE TRIGGER trg_log_creacion_obra
AFTER INSERT ON obras
FOR EACH ROW EXECUTE FUNCTION log_creacion_obra();

CREATE OR REPLACE FUNCTION log_compra_y_cambio_estado() RETURNS TRIGGER AS $$
BEGIN
    -- Cambiar estado de la obra a 'vendida'
    UPDATE obras SET estado = 'vendida' WHERE id = NEW.obra_id;

    -- Registrar log de la compra
    PERFORM registrar_log(NEW.comprador_id, 'COMPRA_OBRA', NULL, NULL, 'Obra comprada ID: ' || NEW.obra_id);

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_log_compra
AFTER INSERT ON compras
FOR EACH ROW EXECUTE FUNCTION log_compra_y_cambio_estado();

CREATE OR REPLACE FUNCTION log_acceso_obra() RETURNS TRIGGER AS $$
DECLARE
    v_comprador_id INTEGER;
BEGIN
    SELECT comprador_id INTO v_comprador_id FROM compras WHERE id = NEW.compra_id;

    PERFORM registrar_log(v_comprador_id, 'ACCESO_OBRA', NULL, NULL, 'Acceso a obra con compra ID: ' || NEW.compra_id);

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_log_acceso_obra
AFTER INSERT ON accesos_obras
FOR EACH ROW EXECUTE FUNCTION log_acceso_obra();

CREATE OR REPLACE FUNCTION log_login_exitoso(p_nickname TEXT, p_ip INET, p_user_agent TEXT)
RETURNS VOID AS $$
DECLARE
    v_usuario_id INTEGER;
BEGIN
    SELECT id INTO v_usuario_id FROM usuarios WHERE nickname = p_nickname;

    IF v_usuario_id IS NOT NULL THEN
        PERFORM registrar_log(v_usuario_id, 'LOGIN_EXITOSO', p_ip, p_user_agent, 'Inicio de sesión exitoso.');
    END IF;
END;
$$ LANGUAGE plpgsql;



-- ================================================
-- 🔐 CREACIÓN DE DATOS DE PRUEBA
-- ================================================

-- Insertar usuarios
INSERT INTO usuarios (nickname, correo, contraseña_hash, rol)
VALUES
('artista1', 'artista1@email.com', crypt('clave1', gen_salt('bf')), 'artista'),
('comprador1', 'comprador1@email.com', crypt('clave2', gen_salt('bf')), 'comprador'),
('admin', 'admin@email.com', crypt('claveadmin', gen_salt('bf')), 'admin');

-- Insertar obras
INSERT INTO obras (titulo, descripcion, archivo_url, firma_digital, artista_id, precio, estado)
VALUES
('Obra A', 'Arte digital abstracto.', 'https://storage.obras/a.jpg', 'FIRMA_DIGITAL_123', 1, 250.00, 'activa'),
('Obra B', 'Retrato conceptual.', 'https://storage.obras/b.jpg', 'FIRMA_DIGITAL_456', 1, 340.00, 'activa');

-- Insertar compra
INSERT INTO compras (obra_id, comprador_id, hash_prueba_compra)
VALUES (1, 2, encode(digest('obra1-comprador1', 'sha256'), 'hex'));

-- Insertar acceso a obra adquirida
INSERT INTO accesos_obras (compra_id, expiracion)
VALUES (1, CURRENT_TIMESTAMP + INTERVAL '7 days');




-- ================================================
-- ✅ PERMISOS
-- ================================================

GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO galeria_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO galeria_user;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO galeria_user;

INSERT INTO usuarios (nickname, correo, contraseña_hash, rol)
VALUES ('comprador2', 'comprador2@email.com', crypt('clave3', gen_salt('bf')), 'comprador');

INSERT INTO obras (titulo, descripcion, archivo_url, firma_digital, artista_id, precio, estado)
VALUES ('Obra C', 'Paisaje surrealista.', 'https://storage.obras/c.jpg', 'FIRMA_DIGITAL_789', 1, 400.00, 'activa');

INSERT INTO compras (obra_id, comprador_id, hash_prueba_compra)
VALUES (3, 4, encode(digest('obra3-comprador2', 'sha256'), 'hex'));

INSERT INTO accesos_obras (compra_id, expiracion)

SELECT * FROM logs_seguridad ORDER BY fecha_evento DESC;

VALUES (2, CURRENT_TIMESTAMP + INTERVAL '7 days');
