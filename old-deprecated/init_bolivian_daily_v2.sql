-- ============================================================
--  bolivian_daily  –  Script de inicialización V2 (PostgreSQL)
-- ============================================================

-- 1. Crear las tablas

-- ── categories ──────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS categories (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    slug VARCHAR(100) NOT NULL UNIQUE,
    state VARCHAR(1) DEFAULT 'A',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ── news_sources ────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS news_sources (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    alias VARCHAR(100) NOT NULL UNIQUE,
    base_url VARCHAR(255) NOT NULL,
    state VARCHAR(1) DEFAULT 'A',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ── source_categories ───────────────────────────────────────
CREATE TABLE IF NOT EXISTS source_categories (
    id BIGSERIAL PRIMARY KEY,
    news_source_id BIGINT NOT NULL,
    category_id BIGINT NOT NULL,
    name VARCHAR(100) NOT NULL,
    url VARCHAR(255) NOT NULL,
    state VARCHAR(1) DEFAULT 'A',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_source_categories_news_source FOREIGN KEY (news_source_id) REFERENCES news_sources (id) ON DELETE CASCADE,
    CONSTRAINT fk_source_categories_category FOREIGN KEY (category_id) REFERENCES categories (id) ON DELETE CASCADE
);

-- ── articles ────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS articles (
    id BIGSERIAL PRIMARY KEY,
    news_source_id BIGINT,
    category_id BIGINT,
    source_category_id BIGINT,
    url TEXT NOT NULL UNIQUE,
    pretitle TEXT,
    title TEXT NOT NULL,
    subtitle TEXT,
    lead TEXT,
    body TEXT,
    author VARCHAR(190),
    published_at TIMESTAMP,
    state VARCHAR(1) DEFAULT 'A',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_articles_category FOREIGN KEY (category_id) REFERENCES categories (id) ON DELETE SET NULL,
    CONSTRAINT fk_articles_news_source FOREIGN KEY (news_source_id) REFERENCES news_sources (id) ON DELETE SET NULL,
    CONSTRAINT fk_articles_source_category FOREIGN KEY (source_category_id) REFERENCES source_categories (id) ON DELETE SET NULL
);

-- ── article_media ───────────────────────────────────────────
CREATE TABLE IF NOT EXISTS article_media (
    id BIGSERIAL PRIMARY KEY,
    article_id BIGINT NOT NULL,
    description TEXT,
    url TEXT NOT NULL,
    path VARCHAR(255),
    type VARCHAR(50) NOT NULL,
    state VARCHAR(1) DEFAULT 'A',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_article_media_article FOREIGN KEY (article_id) REFERENCES articles (id) ON DELETE CASCADE
);

-- 2. Trigger para updated_at automático
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER update_categories_modtime BEFORE UPDATE ON categories FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();
CREATE TRIGGER update_news_sources_modtime BEFORE UPDATE ON news_sources FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();
CREATE TRIGGER update_source_categories_modtime BEFORE UPDATE ON source_categories FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();
CREATE TRIGGER update_articles_modtime BEFORE UPDATE ON articles FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();
CREATE TRIGGER update_article_media_modtime BEFORE UPDATE ON article_media FOR EACH ROW EXECUTE PROCEDURE update_updated_at_column();

-- 3. Carga de datos iniciales

INSERT INTO categories (id, name, slug, state) VALUES 
(1, 'NACIONAL', 'nacional', 'A'),
(2, 'ECONOMÍA', 'economia', 'A'),
(3, 'INTERNACIONAL', 'internacional', 'A'),
(4, 'SEGURIDAD', 'seguridad', 'A'),
(5, 'SOCIEDAD', 'sociedad', 'A'),
(6, 'CULTURA', 'cultura', 'A'),
(7, 'TECNOLOGÍA', 'tecnologia', 'A'),
(8, 'DEPORTES', 'deportes', 'A'),
(9, 'SALUD', 'salud', 'A'),
(10, 'INTERESANTE', 'interesante', 'A')
ON CONFLICT (id) DO UPDATE SET name = EXCLUDED.name, slug = EXCLUDED.slug, state = EXCLUDED.state;

-- Ajustar la secuencia del serial para categories
SELECT setval('categories_id_seq', COALESCE((SELECT MAX(id)+1 FROM categories), 1), false);

INSERT INTO news_sources (id, name, alias, base_url, state)
VALUES (1, 'Jornada', 'jornada', 'https://jornada.com.bo/', 'A')
ON CONFLICT (id) DO UPDATE SET name = EXCLUDED.name, alias = EXCLUDED.alias, base_url = EXCLUDED.base_url, state = EXCLUDED.state;

-- Ajustar la secuencia del serial para news_sources
SELECT setval('news_sources_id_seq', COALESCE((SELECT MAX(id)+1 FROM news_sources), 1), false);

INSERT INTO source_categories (id, news_source_id, category_id, name, url, state) VALUES
(1, 1, 1, 'BOLIVIA', 'https://jornada.com.bo/seccion/bolivia/', 'A'),
(2, 1, 2, 'ECONOMÍA', 'https://jornada.com.bo/seccion/economia/', 'A'),
(3, 1, 3, 'MUNDO', 'https://jornada.com.bo/seccion/mundo/', 'A'),
(4, 1, 8, 'DEPORTES', 'https://jornada.com.bo/seccion/deportes/', 'A'),
(5, 1, 10, 'GENTE', 'https://jornada.com.bo/seccion/gente/', 'A'),
(6, 1, 9, 'SALUD', 'https://jornada.com.bo/seccion/salud/', 'A'),
(7, 1, 7, 'TECNOLOGÍA', 'https://jornada.com.bo/seccion/tecnologia/', 'A')
ON CONFLICT (id) DO UPDATE SET news_source_id = EXCLUDED.news_source_id, category_id = EXCLUDED.category_id, name = EXCLUDED.name, url = EXCLUDED.url, state = EXCLUDED.state;

-- Ajustar la secuencia del serial para source_categories
SELECT setval('source_categories_id_seq', COALESCE((SELECT MAX(id)+1 FROM source_categories), 1), false);
