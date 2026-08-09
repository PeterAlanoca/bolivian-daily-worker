# AGENTS.md — BolivianDaily Workers

## Project
.NET 10 solution `BolivianDaily.Worker.slnx` with 3 workers sharing infrastructure (PostgreSQL 16 + RabbitMQ in docker-compose):

- **ScraperWorker** — scrapes Bolivian news outlets and persists them into `scraper_db`; publishes `ArticleScrapedEvent` to RabbitMQ.
- **CheckerWorker** — consumes `ArticleScrapedEvent` from RabbitMQ, validates each article with OpenRouter (HTML format, category accuracy, no advertising, ready-to-publish) and persists the result into `checker_db`; publishes `ArticleCheckedEvent`.
- **SyncWorker** — consumes `ArticleCheckedEvent` from RabbitMQ and syncs it with the CMS via the external API; persists the snapshot into its own `sync_db` (container `sync-postgres` in compose).

Each worker has 4 projects: `BolivianDaily.<X>Worker` (host), `.Application`, `.Domain`, `.Infrastructure`. Plus `Shared/BolivianDaily.Shared` with shared entities/interfaces.

## Commands

- Build: `dotnet build BolivianDaily.Worker.slnx`
- Run scraper (host): `dotnet run --project ScraperWorker/BolivianDaily.ScraperWorker/BolivianDaily.ScraperWorker.csproj`
- Run checker (host): `dotnet run --project CheckerWorker/BolivianDaily.CheckerWorker/BolivianDaily.CheckerWorker.csproj`
- Run sync (host): `dotnet run --project SyncWorker/BolivianDaily.SyncWorker/BolivianDaily.SyncWorker.csproj`
- Docker: `docker compose up -d` — services `scraper-postgres` (localhost:5432), `checker-postgres` (5433), `sync-postgres` (5434), `rabbitmq` (5672/15672). Passwords via `${DB_PASSWORD}` from `.env` (local value `root`).
- RabbitMQ management UI: `http://localhost:15672` (guest/guest).
- psql scraper: `docker exec scraper-postgres psql -U scraper_user -d scraper_db -c "..."`
- psql checker: `docker exec checker-postgres psql -U checker_user -d checker_db -c "..."`
- psql sync: `docker exec sync-postgres psql -U sync_user -d sync_db -c "..."`
- RabbitMQ introspection: `docker exec rabbitmq rabbitmqctl list_queues name messages` (also `list_exchanges`, `list_consumers`).

## Database

- DBs: `scraper_db` (user `scraper_user`), `checker_db` (user `checker_user`), `sync_db` (user `sync_user`).
- **All 3 workers run `Database.Migrate()` in their `Program.cs`** — each DB creates/migrates/seeds itself on startup.
- Migrations ARE versioned (`Migrations/` folder in each Infrastructure, tracked in git).
- Post-migration check: `docker exec scraper-postgres psql -U scraper_user -d scraper_db -c "\dt"`, `docker exec checker-postgres psql -U checker_user -d checker_db -c "\dt"` and `docker exec sync-postgres psql -U sync_user -d sync_db -c "\dt"`.

### Migration conventions (mandatory — see skill `.opencode/skills/ef-migrations/SKILL.md`)

- Seed inside the migration via `migrationBuilder.InsertData` **without the `id` column** (the identity generates it).
- **FORBIDDEN**: `HasData` with fixed ids and `setval()`.
- Timestamps: `created_at`/`updated_at` columns with `HasDefaultValueSql("CURRENT_TIMESTAMP")` + `UpdateTimestampInterceptor` (`IHasTimestamps` interface in `Shared/BolivianDaily.Shared/Entities/`). No triggers.
- `articles` FKs → categories/sources with `DeleteBehavior.SetNull`; unique index on `articles.url`.
- `state` (default `"A"`, active) exists **only** on catalog tables (`categories`, `news_sources`, `source_categories`) — content tables (`articles`, `article_media`, `checked_articles`, `synced_articles`) have no `state`; catalog queries filter `State == "A"`.
- Generate migration (host must reference `Microsoft.EntityFrameworkCore.Design`):
  - Scraper: `dotnet ef migrations add <Name> --project ScraperWorker/BolivianDaily.ScraperWorker.Infrastructure/BolivianDaily.ScraperWorker.Infrastructure.csproj --startup-project ScraperWorker/BolivianDaily.ScraperWorker/BolivianDaily.ScraperWorker.csproj --output-dir Migrations`
  - Checker: `dotnet ef migrations add <Name> --project CheckerWorker/BolivianDaily.CheckerWorker.Infrastructure/BolivianDaily.CheckerWorker.Infrastructure.csproj --startup-project CheckerWorker/BolivianDaily.CheckerWorker/BolivianDaily.CheckerWorker.csproj --output-dir Migrations`
  - Sync: `dotnet ef migrations add <Name> --project SyncWorker/BolivianDaily.SyncWorker.Infrastructure/BolivianDaily.SyncWorker.Infrastructure.csproj --startup-project SyncWorker/BolivianDaily.SyncWorker/BolivianDaily.SyncWorker.csproj --output-dir Migrations`

## Messaging (RabbitMQ)

See skill `.opencode/skills/rabbitmq-messaging/SKILL.md` for the full workflow (adding new events/queues/consumers/publishers).

### Topology

- Exchange `bolivian-daily.articles` (direct, durable).
- Queues: `checker.article-scraped` (Scraper → Checker), `sync.article-checked` (Checker → Sync).
- DLX `{exchange}.dlx` (direct, durable); DLQ `{queue}.dead`; main queues declare `x-dead-letter-exchange` / `x-dead-letter-routing-key` args.
- Topology is declared idempotently by consumers and publishers via `RabbitMqTopology.Declare(...)` — the single source of truth. **Redeclaring an existing queue with different arguments fails with 406 `PRECONDITION_FAILED`**; when queue args change, delete the queue once (`docker exec rabbitmq rabbitmqctl delete_queue <name>`).

### Naming (mandatory)

- **Events are past-tense facts**, named after the producer's action: `ArticleScrapedEvent` (Scraper), `ArticleCheckedEvent` (Checker). Future chain: `ArticleSyncedEvent` (Sync).
- **NEVER name events by destination** (`ArticleToCheckEvent` style) — destination lives in the queue name (`checker.*`, `sync.*`), not in the event.
- **FORBIDDEN: "Process/Processed"** anywhere in the article chain (event, entity, table, use case, methods). Use the domain action: Scraper scrapes → Checker checks → Sync syncs.
- Queues name the destination: `checker.article-scraped`, `sync.article-checked`.

### Connections

- Each worker's Infrastructure registers singleton `ConnectionFactory` + `IConnection` (`DispatchConsumersAsync = true`) and binds `RabbitMqOptions` via `services.Configure<RabbitMqOptions>(...)`.
- Publishers reuse the singleton `IConnection` and open a channel per message — **never** a new connection per publish.
- All 3 workers' `appsettings.json` contain the same `RabbitMq` section; keep queue/routing key keys in sync with `RabbitMqOptions`.

### Shared classes (`Shared/BolivianDaily.Shared/Messaging/`)

- `RabbitMqConsumerHostedService<TMessage>` — abstract base for consumers: connection retry with exponential backoff (1s → 30s, unbounded until shutdown), `RabbitMqTopology.Declare`, `BasicQos(prefetch: 1)`, `AsyncEventingBasicConsumer` with ack/nack, retry counter header `x-retry-count` (republish + ack until `MaxRetries`, then nack `requeue: false` → DLX → DLQ), graceful shutdown (`Task.Delay(Infinite)` + channel dispose in `finally`). Subclasses implement `QueueName`, `RoutingKey`, `HandleAsync(message, ct)`.
- `RabbitMqEventPublisher<TMessage>` — generic publisher (declares topology, persistent JSON properties, log). Concrete publishers are thin classes that only set the message type, queue and routing key.
- Concrete consumers live in the host projects (`CheckerConsumer`, `SyncConsumer`) and are registered with `AddHostedService<...>()`.

## Code conventions

- C#: file-scoped namespaces, primary constructors, ImplicitUsings, Nullable enabled.
- **NO comments unless requested.**
- Names in English; tables and columns `snake_case`, entities plural.
- Repositories: `Sql*Repository` in Infrastructure; one `DbContext` per worker; snake_case mapping in `OnModelCreating`.
- **Mappers**: static extension classes in `Application/Mappers/` (`ArticleMappers`, `CheckedArticleMappers`, `SyncedArticleMappers`) — use cases never build entities inline; they call the mapper.
- Host projects reference `Microsoft.EntityFrameworkCore.Design` (needed by `dotnet ef`).
- Connection string key is `DefaultConnection` in all 3 workers' `appsettings.json`.
- Layers: Infrastructure → Application → Domain; Domain/Infrastructure reference Shared. Do not invert dependencies.
- Respond in Spanish, keep answers concise, no lengthy explanations.
