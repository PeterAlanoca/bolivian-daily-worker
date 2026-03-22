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

## Estructura del Proyecto

El sistema está diseñado bajo una arquitectura de **limpia (Clean Architecture)** con una clara separación entre las reglas de negocio y los detalles externos:

### 1. BolivianDaily.Domain (Core/Entidades)
Representa el corazón del sistema. No tiene dependencias externas (proyectos o paquetes de terceros).
- **Entities/**: Define los objetos de dominio persistentes como `News`, `Source`, `Category`, `Multimedia`, `SourceCategory` y la base abstracta `AuditableEntity`.
- **Repositories/**: Define los contratos de acceso a datos (`INewsRepository`, `ISourceRepository`, `ICategoryRepository`) que la capa de infraestructura deberá implementar.

### 2. BolivianDaily.Application (Capas de Casos de Uso)
Coordina la ejecución de la lógica de negocio y define las interfaces para servicios externos.
- **UseCases/**: Implementa la orquestación mediante **MediatR**. El comando `ScrapeSourcesCommand` y su controlador `ScrapeSourcesCommandHandler` coordinan el flujo de raspado, validación con IA y persistencia.
- **Interfaces/**: Define los contratos para la infraestructura: `IWebScraperService` (para leer portales), `INewsContentAnalyst` (analítica de IA), `INewsPublisher` (publicación externa) e `IScraperFactory`.
- **Dtos/**: Objetos de transferencia de datos como `NewsAnalysisResult`, utilizados para recibir el análisis de la IA.

### 3. BolivianDaily.Infrastructure (Adaptadores/Implementaciones)
Contiene todos los detalles técnicos y de infraestructura que conectan el sistema con el mundo real.
- **Data (EF Core)**: El `ApplicationDbContext` y las configuraciones de mapeo de base de datos a PostgreSQL.
- **Repositories/**: Implementaciones concretas de los repositorios del dominio utilizando Entity Framework.
- **ExternalServices/**:
  - **BolivianDaily API**: Integración con el backend principal para publicar noticias.
  - **DeepSeek IA**: Analista de contenido basado en modelos de lenguaje (LLM).
- **Services/Scrapers/**: Lógica específica para extraer noticias de portales bolivianos (`JornadaScraperService`, `ElDeberScraperService`), heredando de un `BaseScraperService` común.
- **ScraperFactory**: Clase encargada de resolver dinámicamente qué scraper utilizar según la fuente configurada.

### 4. BolivianDaily.Worker (Entorno de Ejecución)
Capa de presentación de tipo servicio de consola para ejecución en segundo plano.
- **ScraperWorker.cs**: El `BackgroundService` de .NET que ejecuta el proceso periódicamente (cada 15 minutos por defecto).
- **appsettings.json**: Configuración centralizada de cadenas de conexión, claves API y parámetros de IA.
- **Program.cs**: Bootstrapper que configura el Host y la inyección de dependencias de todas las capas.

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
