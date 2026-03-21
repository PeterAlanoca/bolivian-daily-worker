-- ============================================================
--  bolivian_daily  –  Script de inicialización (PostgreSQL)
--  Generado: 2026-03-21
-- ============================================================

-- 1. Crear la base de datos (Ejecutar manualmente si es necesario)
-- CREATE DATABASE bolivian_daily;

-- Conectarse a la base de datos
-- \c bolivian_daily;

-- 2. Crear las tablas

-- ── category ────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS category (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(100),
    url VARCHAR(100),
    state VARCHAR(1),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ── source ──────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS source (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(100),
    alias VARCHAR(100),
    url VARCHAR(100),
    state VARCHAR(1),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ── source_category ─────────────────────────────────────────
CREATE TABLE IF NOT EXISTS source_category (
    id BIGSERIAL PRIMARY KEY,
    source_id BIGINT NOT NULL,
    category_id BIGINT NOT NULL,
    name VARCHAR(100),
    url VARCHAR(100),
    state VARCHAR(1),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_source_category_source FOREIGN KEY (source_id) REFERENCES source (id) ON DELETE CASCADE,
    CONSTRAINT fk_source_category_category FOREIGN KEY (category_id) REFERENCES category (id) ON DELETE CASCADE
);

-- ── news ────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS news (
    id BIGSERIAL PRIMARY KEY,
    category_id BIGINT,
    source_id BIGINT,
    url TEXT,
    pretitle TEXT,
    title TEXT,
    subtitle TEXT,
    enter TEXT,
    body TEXT,
    author VARCHAR(190),
    publication_date TIMESTAMP,
    state VARCHAR(1),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_news_category FOREIGN KEY (category_id) REFERENCES category (id) ON DELETE SET NULL,
    CONSTRAINT fk_news_source FOREIGN KEY (source_id) REFERENCES source (id) ON DELETE SET NULL
);

-- ── multimedia ──────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS multimedia (
    id BIGSERIAL PRIMARY KEY,
    news_id BIGINT,
    description TEXT,
    url TEXT,
    path VARCHAR(200),
    type VARCHAR(30),
    state VARCHAR(1),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_multimedia_news FOREIGN KEY (news_id) REFERENCES news (id) ON DELETE CASCADE
);

-- 3. Trigger para updated_at automático (Opcional pero recomendado en Postgres)
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER update_category_modtime BEFORE UPDATE ON category FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();
CREATE TRIGGER update_source_modtime BEFORE UPDATE ON source FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();
CREATE TRIGGER update_source_category_modtime BEFORE UPDATE ON source_category FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();
CREATE TRIGGER update_news_modtime BEFORE UPDATE ON news FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();
CREATE TRIGGER update_multimedia_modtime BEFORE UPDATE ON multimedia FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();


---------------------------------


INSERT INTO category (name, url, state) VALUES 
('NACIONAL', 'nacional', 'A'),
('ECONOMÍA', 'economia', 'A'),
('INTERNACIONAL', 'internacional', 'A'),
('SEGURIDAD', 'seguridad', 'A'),
('SOCIEDAD', 'sociedad', 'A'),
('CULTURA', 'cultura', 'A'),
('TECNOLOGÍA', 'tecnologia', 'A'),
('DEPORTES', 'deportes', 'A'),
('SALUD', 'salud', 'A'),
('INTERESANTE', 'interesante', 'A');

INSERT INTO source (name, alias, url, state)
VALUES ('Jornada', 'jornada', 'https://jornada.com.bo/', 'A')
ON CONFLICT DO NOTHING;


INSERT INTO public.source_category(source_id, category_id, name, url, state)
VALUES (1, 1, 'BOLIVIA', 'https://jornada.com.bo/seccion/bolivia/', 'A');

INSERT INTO public.source_category(source_id, category_id, name, url, state) 
VALUES (1, 2, 'ECONOMÍA', 'https://jornada.com.bo/seccion/economia/', 'A');

INSERT INTO public.source_category(source_id, category_id, name, url, state) 
VALUES (1, 3, 'MUNDO', 'https://jornada.com.bo/seccion/mundo/', 'A');

INSERT INTO public.source_category(source_id, category_id, name, url, state) 
VALUES (1, 8, 'DEPORTES', 'https://jornada.com.bo/seccion/deportes/', 'A');

INSERT INTO public.source_category(source_id, category_id, name, url, state) 
VALUES (1, 10, 'GENTE', 'https://jornada.com.bo/seccion/gente/', 'A');

INSERT INTO public.source_category(source_id, category_id, name, url, state) 
VALUES (1, 9, 'SALUD', 'https://jornada.com.bo/seccion/salud/', 'A');

INSERT INTO public.source_category(source_id, category_id, name, url, state) 
VALUES (1, 7, 'TECNOLOGÍA', 'https://jornada.com.bo/seccion/tecnologia/', 'A');

INSERT INTO public.source_category(source_id, category_id, name, url, state) 
VALUES (1, 7, 'TECNOLOGÍA', 'https://jornada.com.bo/seccion/tecnologia/', 'A');










