---
name: naming-conventions
description: Mandatory naming conventions for BolivianDaily workers types
  (suffixes and placement of repositories, API integrations, providers,
  parsers, publishers, consumers, workers, use cases, options, mappers, DTOs,
  events). Use it when creating, renaming or reviewing any type, interface,
  file or folder across ScraperWorker, CheckerWorker, SyncWorker or Shared.
---

# Naming conventions — mandatory taxonomy

Every project type gets a suffix that says what it does and lives in the
folder that says where it belongs. There are no generic names
(`*ApiClient`, `*Service`, `*Manager`, `*Helper` are **forbidden** for
project types).

## Suffix → meaning → location

| Suffix | Meaning | Interface (Application/Interfaces) | Implementation | Folder |
|---|---|---|---|---|
| `*Repository` | Data access to the worker's own DB | `IArticleRepository` | `SqlArticleRepository`, `SqlNewsSourceRepository`, `SqlCheckedArticleRepository`, `SyncedArticleRepository` | interface in `Domain/Repositories/`; impl in `Infrastructure/Persistence/` |
| `*Syncer` | Outbound API integration (sends data to an external system) | `IArticleSyncer` | `ExtranetArticleSyncer` | `Infrastructure/<Integration>/` |
| `*Checker` | External integration that validates/processes content | `IArticleChecker` | `OpenRouterArticleChecker` | `Infrastructure/<Integration>/` |
| `*Parser` | HTML parsing for one news outlet | `INewsSourceParser` | `JornadaParser`, `ElDiarioParser` | `Infrastructure/Parsers/` |
| `*ParserRegistry` | Parser lookup by source alias | `INewsSourceParserRegistry` | `NewsSourceParserRegistry` | `Infrastructure/Parsers/` |
| `*Provider` | Supplies ONE concrete thing (options, token, …) — never a whole API | `INewsSourceOptionsProvider<T>` | `NewsSourceOptionsProvider<T>`, `ExtranetTokenProvider` | `Infrastructure/Configuration/` or `Infrastructure/<Integration>/` |
| `*Publisher` | Publishes events to RabbitMQ | `IArticleScrapedEventPublisher`, `IArticleCheckedEventPublisher` | `RabbitMqArticleScrapedEventPublisher`, `RabbitMqArticleCheckedEventPublisher` | `Infrastructure/Messaging/` |
| `*Consumer` | Consumes from RabbitMQ; lives in the host project | — | `CheckerConsumer`, `SyncConsumer` | host project root |
| `*Worker` | `BackgroundService` (scheduled scraping) | — | `JornadaScrapingWorker` | host `Workers/` |
| `*UseCase` | Orchestrates one domain action | — | `ScrapeSourceUseCase`, `CheckScrapedArticleUseCase`, `SyncCheckedArticleUseCase` | `Application/UseCases/<Action>/` |
| `*Command` / `*Result` | Use case input/output records | — | `ScrapeSourceCommand`, `ScrapeSourceResult`, `ArticleSyncResult` | `Application/UseCases/<Action>/` (or `Application/Interfaces/` for cross-use-case results) |
| `*Options` | Configuration section, has static `SectionName` | — | `JornadaOptions`, `ElDiarioOptions`, `ExtranetOptions`, `OpenRouterOptions`, `RabbitMqOptions` | `Infrastructure/Configuration/` |
| `*Mappers` | Static extension classes mapping between types | — | `ArticleMappers`, `CheckedArticleMappers`, `SyncedArticleMappers` (Application), `OpenRouterMapper`, `ExtranetArticleMappers` (Infrastructure) | `Application/Mappers/` or `Infrastructure/Mappers/` |
| `*Request` / `*Response` / `*Data` / `*Detail` | External API DTOs | — | `OpenRouterChatRequest`, `ArticleValidationResponse`, `ExtranetArticleData`, `ValidationDetail` | `Infrastructure/<Integration>/Dtos/` |
| `*DbContext` | One per worker | — | `ScraperDbContext`, `CheckerDbContext`, `SyncDbContext` | `Infrastructure/Persistence/` |

## Rules

1. **Naming says the role**: repos are `Sql*Repository`, publishers are
   `RabbitMq*Publisher`, integrations are named after the external system
   (`Extranet*`, `OpenRouter*`), parsers after the outlet (`Jornada*`,
   `ElDiario*`).
2. **`*Provider` only for concrete suppliers** (`NewsSourceOptionsProvider<T>`
   supplies options for a source; `ExtranetTokenProvider` supplies/refreshes a
   token). An API integration is a `*Syncer` (outbound) or `*Checker`
   (validation). When in doubt, the type does not get `Provider`.
3. **Interfaces** live in `Application/Interfaces/` (or `Domain/Repositories/`
   for data access contracts) and use the `I` prefix; domain entities live in
   `Domain/Entities/`, one file per type, filename = type name
   (`BaaseOptions.cs` containing `NewsSourceOptions` is a filename bug — fix it).
4. **Events** live in `Shared/Messaging/`, named past-tense by the producer's
   domain action: `ArticleScrapedEvent` → `ArticleCheckedEvent` →
   `ArticleSyncedEvent`. Never by destination and never `Process/Processed`.
5. **Layers never invert**: Infrastructure → Application → Domain →
   Shared. Implementation types (Sql/RabbitMq/Extranet/OpenRouter…) stay in
   Infrastructure.
6. **Variables** follow the type: camelCase of the declared type
   (`articleSyncer`, `syncDbContext`, `checkedArticle`) — see AGENTS.md.
   Framework types keep their natural names (`httpClient`, `logger`,
   `options`, `connection`, `channel`, `serviceProvider`, `scope`, `ct`).
7. Names in English; one file per type; no comments unless requested.
