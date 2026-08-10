# Plan: convención de nomenclatura de variables (camelCase del tipo)

## Convención a documentar en AGENTS.md

> **Variables = camelCase del tipo declarado** (tipos del proyecto: interfaces, classes, entities, DTOs, options, DbContexts, repos, use cases, syncer, provider, publishers, mappers).
> Tipos de framework conservan su idioma natural: `httpClient`, `logger`, `options` (`IOptions<T>`), `connection`, `channel`, `serviceProvider`, `ct`/`cancellationToken`/`stoppingToken`, `scope`, `eventData`/`result` (EF).
> Aplica a: parámetros de ctor, parámetros de método, extension `this`, fields y `var` locals de tipos de proyecto. Colecciones de tipos de proyecto → camelCase del elemento en plural.

## Cambios por archivo

### SyncWorker
1. `SyncWorker/BolivianDaily.SyncWorker.Application/UseCases/SyncCheckedArticle/SyncCheckedArticleUseCase.cs`
   - `IArticleSyncer apiClient` → `articleSyncer` (+ usos internos).
2. `SyncWorker/BolivianDaily.SyncWorker.Infrastructure/Persistence/SyncedArticleRepository.cs`
   - `SyncDbContext context` → `syncDbContext` (+ usos internos).
3. `SyncWorker/BolivianDaily.SyncWorker/SyncConsumer.cs:21` — `var useCase` → `syncCheckedArticleUseCase`.
4. `SyncWorker/BolivianDaily.SyncWorker/Program.cs:19` — `var db` → `syncDbContext`.
5. `SyncWorker/BolivianDaily.SyncWorker.Infrastructure/DependencyInjection/InfrastructureServiceExtensions.cs`
   - `var opts` (ExtranetOptions ×2) → `extranetOptions`; `var opts` (RabbitMqOptions ×1) → `rabbitMqOptions`; `var factory` → `connectionFactory`.

### CheckerWorker
6. `CheckerWorker/BolivianDaily.CheckerWorker.Infrastructure/Persistence/SqlCheckedArticleRepository.cs`
   - `CheckerDbContext context` → `checkerDbContext`; `CheckedArticle article` → `checkedArticle` (+ usos internos).
7. `CheckerWorker/BolivianDaily.CheckerWorker.Application/UseCases/CheckScrapedArticle/CheckScrapedArticleUseCase.cs:12`
   - `IArticleCheckedEventPublisher eventPublisher` → `articleCheckedEventPublisher` (+ usos internos).
8. `CheckerWorker/BolivianDaily.CheckerWorker/CheckerConsumer.cs:21` — `var useCase` → `checkScrapedArticleUseCase`.
9. `CheckerWorker/BolivianDaily.CheckerWorker/Program.cs:18` — `var db` → `checkerDbContext`.
10. `CheckerWorker/BolivianDaily.CheckerWorker.Infrastructure/DependencyInjection/InfrastructureServiceExtensions.cs`
    - `var options` (OpenRouterOptions) → `openRouterOptions`; `var factory` → `connectionFactory`.

### ScraperWorker
11. `ScraperWorker/BolivianDaily.ScraperWorker.Infrastructure/Persistence/SqlArticleRepository.cs:7` — `ScraperDbContext context` → `scraperDbContext`.
12. `ScraperWorker/BolivianDaily.ScraperWorker.Infrastructure/Persistence/SqlNewsSourceRepository.cs:7` — `ScraperDbContext context` → `scraperDbContext`.
13. `ScraperWorker/BolivianDaily.ScraperWorker.Application/UseCases/ScrapeSource/ScrapeSourceUseCase.cs`
    - `INewsSourceRepository sourceRepository` → `newsSourceRepository`; `INewsSourceParserRegistry parserRegistry` → `newsSourceParserRegistry`; `IArticleScrapedEventPublisher eventPublisher` → `articleScrapedEventPublisher` (+ usos internos).
14. `ScraperWorker/BolivianDaily.ScraperWorker/Program.cs:19` — `var db` → `scraperDbContext`.
15. `ScraperWorker/BolivianDaily.ScraperWorker.Infrastructure/DependencyInjection/InfrastructureServiceExtensions.cs` — `var factory` → `connectionFactory` (revisar contenido completo al ejecutar).

### Shared
16. `Shared/BolivianDaily.Shared/Messaging/RabbitMqTopology.cs:7,9,11,15` — `RabbitMqOptions options` → `rabbitMqOptions` (4 métodos).
17. `ScraperWorker/BolivianDaily.ScraperWorker.Infrastructure/Parsers/NewsSourceParserRegistry.cs:5` — `IEnumerable<INewsSourceParser> parsers` → `newsSourceParsers`; field `_parsers` → `_newsSourceParsers` (usos internos).
18. `SyncWorker/BolivianDaily.SyncWorker.Application/Mappers/SyncedArticleMappers.cs:8` — `this ArticleCheckedEvent message` → `articleCheckedEvent` (+ usos internos).
19. `CheckerWorker/BolivianDaily.CheckerWorker.Infrastructure/OpenRouter/OpenRouterArticleChecker.cs:19` — typo `openRouterChatReques` → `openRouterChatRequest`.

### Documentación
20. `AGENTS.md` — agregar la convención en sección "Code conventions".

## Verificación
- `dotnet build BolivianDaily.Worker.slnx` (0 warnings/errors).
- Grep de nombres viejos (`apiClient`, `context`, `sourceRepository`, `parserRegistry`, `eventPublisher`, `openRouterChatReques`) = 0 en código fuente.
