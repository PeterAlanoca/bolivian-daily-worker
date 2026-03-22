# Bolivian Daily - News Scraping Worker

Este proyecto es el componente de procesamiento automatizado (Worker) de un sistema Full Stack diseñado para la recopilación, análisis y publicación automatizada de noticias digitales. Utiliza tecnologías de Web Scraping e Inteligencia Artificial (DeepSeek) para garantizar contenido de alta calidad y relevancia.

## Resumen Ejecutivo
El sistema aborda la problemática de los procesos manuales e ineficientes de selección de noticias. Propone una arquitectura desacoplada donde este "Worker" se encarga de la extracción automatizada desde múltiples fuentes públicas, procesando la información y validándola mediante un motor de IA antes de su publicación final en una plataforma web.

---

## Arquitectura del Proyecto (Clean Architecture)

El servicio sigue estrictamente los principios de Clean Architecture y DDD (Domain-Driven Design), organizado en las siguientes capas independientes:

- **`BolivianDaily.Domain` (Core)**: Contiene las entidades de negocio (`News`, `Source`), interfaces de repositorios y reglas fundamentales que no dependen de ninguna tecnología externa.
- **`BolivianDaily.Application` (Casos de Uso)**: Implementa la lógica de orquestación mediante **MediatR**. Define los contratos de servicios e interfaces para el análisis de contenido y publicación.
- **`BolivianDaily.Infrastructure` (Adaptadores)**: Contiene las implementaciones técnicas: scrapers de sitios específicos (Jornada, El Deber), clientes de API (BolivianDaily API), el analista de IA (DeepSeek API) y el acceso a datos mediante **Entity Framework Core**.
- **`BolivianDaily.Worker` (Presentación/Consola)**: Es el punto de entrada que configura el servicio en segundo plano, la inyección de dependencias y el logging visual con **Spectre.Console**.

---

## Estructura de Directorios

Principales carpetas y archivos del sistema (.NET Clean Architecture):

```text
bolivian-daily-worker/
├── BolivianDaily.Domain/          # Núcleo del negocio (Entidades y Contratos)
│   ├── Entities/                  # Clases base (News, Source, Category, Multimedia)
│   └── Repositories/              # Interfaces de acceso a datos (DDD Repositories)
├── BolivianDaily.Application/     # Capa de Orquestación y Casos de Uso
│   ├── Dtos/                      # Datos para transferencia (AI Analysis Results)
│   ├── Interfaces/                # Contratos para servicios externos (Scraping, IA)
│   └── UseCases/                  # Lógica de scraping y procesamiento (MediatR)
├── BolivianDaily.Infrastructure/  # Implementaciones Técnicas (Adaptadores)
│   ├── Data/                      # Contexto de BD y Mapeos EF Core (PostgreSQL)
│   ├── ExternalServices/          # Clientes API para DeepSeek IA y Backend Web
│   ├── Repositories/              # Persistencia concreta en base de datos
│   └── Services/                  # Lógica de scrapers específicos (Jornada, El Deber)
├── BolivianDaily.Worker/          # Punto de Entrada y Ejecución del Servicio
│   ├── appsettings.json           # Configuración central (BD, IA, API, Logs)
│   ├── Program.cs                 # Configuración de Inyección de Dependencias
│   └── ScraperWorker.cs           # Orquestador del ciclo de vida del Background Service
├── Dockerfile                     # Configuración para despliegue en contenedores
├── init_bolivian_daily.sql        # Script SQL de inicialización de tablas y semillas
└── readme.md                      # Documentación principal del proyecto
```

---

## Stack Tecnológico
- **Lenguaje**: C# (.NET 10)
- **Base de Datos**: PostgreSQL
- **Orquestación**: MediatR (Casos de Uso)
- **Scraping**: HttpClient & HtmlAgilityPack
- **IA**: DeepSeek API (Modelos de Chat para validación)
- **UI de Consola**: Spectre.Console (Logs enriquecidos en español)
- **ORM**: Entity Framework Core

---

## Instalación y Configuración

### Prerrequisitos
- .NET 10 SDK
- PostgreSQL (Base de datos configurada con el archivo `init_bolivian_daily.sql`)
- Clave de API de DeepSeek (Configurada en el `appsettings.json`)

### Opción 1: Instalación Local
1.  **Clonar el repositorio**.
2.  **Configurar la base de datos**: Ejecutar el script `init_bolivian_daily.sql` en tu instancia de PostgreSQL.
3.  **Configurar `appsettings.json`**: Asegúrate de que las cadenas de conexión y las claves API sean correctas.
4.  **Ejecutar**:
    ```bash
    dotnet build
    dotnet run --project BolivianDaily.Worker
    ```

### Opción 2: Instalación con Docker
1.  **Construir la imagen**:
    ```bash
    docker build -t bolivian-daily-worker .
    ```
2.  **Ejecutar el contenedor**:
    ```bash
    docker run -d --name news-worker bolivian-daily-worker
    ```

---

## Base de Datos
El proyecto incluye un script de inicialización (`init_bolivian_daily.sql`) que crea las tablas necesarias:
- `sources`: Fuentes de noticias configurables.
- `categories`: Categorías de noticias (Local, Nacional, Deporte).
- `news`: Repositorio de noticias procesadas y validadas.

---

## Autor y Licencia
- **Autor**: Peter Ciro Alanoca Aruquipa
- **Contexto**: Proyecto de Especialidad - Maestría en Full Stack Development
- **Institución**: Universidad Católica Boliviana "San Pablo" (UCB)
- **Licencia**: Este software se desarrolla con fines exclusivamente académicos y de investigación.

---
*Nota: Este proyecto es un prototipo funcional que demuestra la viabilidad técnica de integrar IA y Scraping en una arquitectura Full Stack empresarial.*
