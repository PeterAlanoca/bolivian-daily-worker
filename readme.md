# News Scraping Worker (.NET + DDD + Clean Architecture)

Este proyecto forma parte de un sistema Full Stack para la publicación
de noticias digitales.

El Worker Service es el componente encargado de ejecutar procesos
automatizados de web scraping, procesar la información obtenida y
enviarla al backend para su almacenamiento y posterior análisis.

La implementación sigue principios de Domain-Driven Design (DDD) y Clean
Architecture, garantizando:

-   Separación clara de responsabilidades
-   Mantenibilidad del código
-   Escalabilidad del sistema
-   Facilidad de pruebas

------------------------------------------------------------------------

# Características principales

-   Extracción automática de noticias desde múltiples fuentes digitales.
-   Normalización y limpieza de datos obtenidos.
-   Integración con API REST del backend para persistencia.
-   Ejecución periódica mediante Jobs programados.
-   Preparación de datos para análisis con servicios externos de IA.
-   Arquitectura basada en DDD + Clean Architecture.

------------------------------------------------------------------------

# Tecnologías utilizadas

  -----------------------------------------------------------------------
  Tecnología                         Descripción
  ---------------------------------- ------------------------------------
  .NET 8 Worker Service              Servicio en segundo plano para
                                     procesos automatizados

  HttpClient                         Consumo de APIs

  HtmlAgilityPack                    Web scraping y parsing de HTML

  Entity Framework Core              Acceso a base de datos

  Serilog                            Logging estructurado
  -----------------------------------------------------------------------

------------------------------------------------------------------------

# Estructura del proyecto

El proyecto sigue Clean Architecture combinada con DDD.

    /Worker
     ├── src/
     │    ├── Domain/           
     │    │    └── Entidades y reglas de negocio
     │    │        (NewsItem, ValueObjects, Domain Services)
     │    │
     │    ├── Application/      
     │    │    └── Casos de uso
     │    │        (ScrapingUseCase, SaveNewsUseCase)
     │    │
     │    ├── Infrastructure/   
     │    │    └── Implementaciones técnicas
     │    │        (ScraperService, ApiClient, EF Core)
     │    │
     │    ├── Presentation/     
     │    │    └── Worker Service
     │    │        (Program.cs, Worker.cs)
     │    │
     │    └── Utils/            
     │         └── Utilidades y helpers
     │            (HtmlParser, Helpers)
     │
     ├── tests/                 
     │    └── Pruebas unitarias y de integración
     │
     └── appsettings.json      
          └── Configuración del sistema

------------------------------------------------------------------------

# Configuración

## 1. Clonar el repositorio

``` bash
git clone https://github.com/usuario/news-worker-dotnet.git
```

## 2. Configurar las fuentes de scraping

Editar el archivo appsettings.json:

``` json
{
  "ScrapingSources": [
    "https://www.noticias1.com",
    "https://www.noticias2.com"
  ],
  "BackendApi": "https://localhost:5001/api/news"
}
```

## 3. Ejecutar el Worker

``` bash
dotnet run
```

------------------------------------------------------------------------

# Flujo de trabajo

Flujo general del sistema:

    Scraping Sources
          |
          v
    Web Scraper
          |
          v
    Domain Entities (NewsItem)
          |
          v
    Application Use Cases
          |
          v
    Infrastructure (API Client)
          |
          v
    Backend API
          |
          v
    Database + AI Analysis

1.  El Worker ejecuta tareas programadas de scraping.
2.  Extrae noticias desde las fuentes configuradas.
3.  Convierte los datos en entidades de dominio (NewsItem).
4.  Los casos de uso coordinan la lógica del sistema.
5.  La capa Infrastructure envía los datos al backend mediante API REST.
6.  El backend almacena las noticias y las envía al módulo de IA para
    clasificación.

------------------------------------------------------------------------

# Pruebas

El proyecto incluye pruebas unitarias y de integración.

Ejecutarlas con:

``` bash
dotnet test
```

Componentes probados:

-   ScraperService
-   ApiClient

------------------------------------------------------------------------

# Próximas mejoras

-   Implementar scraping distribuido con Azure Functions o Hangfire.
-   Mejorar la detección de noticias duplicadas.
-   Integrar métricas de rendimiento con Prometheus y Grafana.
-   Incorporar análisis semántico avanzado con IA.

------------------------------------------------------------------------

# Autor

Peter Ciro Alanoca Aruquipa

Proyecto de Especialidad\
Maestría en Full Stack Development

Universidad Católica Boliviana "San Pablo"

------------------------------------------------------------------------

# Licencia

Este proyecto se desarrolla con fines académicos y de investigación.
