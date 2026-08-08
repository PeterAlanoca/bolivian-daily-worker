# AGENTS.md — BolivianDaily Workers

## Project
.NET 10 solution `BolivianDaily.Worker.slnx` with 3 workers sharing infrastructure (PostgreSQL 16 + RabbitMQ in docker-compose):

- **ScraperWorker** — scrapes Bolivian news outlets and persists them into `scraper_db`.
- **CheckerWorker** — validates articles (duplicates, dead links) reading from `scraper_db`; uses `checker_db`.
- **SyncWorker** — syncs with the CMS; uses its own DB (container pending in compose).

Each worker has 4 projects: `BolivianDaily.<X>Worker` (host), `.Application`, `.Domain`, `.Infrastructure`. Plus `Shared/BolivianDaily.Shared` with shared entities/interfaces.

## Commands

- Build: `dotnet build BolivianDaily.Worker.slnx`
- Run scraper (host): `dotnet run --project ScraperWorker/BolivianDaily.ScraperWorker/BolivianDaily.ScraperWorker.csproj`
- Docker: `docker compose up -d` — services `scraper-postgres` (localhost:5432), `checker-postgres` (5433), `rabbitmq` (5672/15672). Passwords via `${DB_PASSWORD}` from `.env` (local value `root`).
- psql scraper: `docker exec scraper-postgres psql -U scraper_user -d scraper_db -c "..."`

## Database

- DBs: `scraper_db` (user `scraper_user`), `checker_db` (user `checker_user`). `sync_db` pending.
- **Each worker runs `Database.Migrate()` in its `Program.cs`** — the DB creates/migrates/seeds itself on startup. No manual SQL scripts.
- Migrations ARE versioned (`Migrations/` folder in each Infrastructure, tracked in git).
- Post-migration check: `docker exec scraper-postgres psql -U scraper_user -d scraper_db -c "\dt"`.

### Migration conventions (mandatory — see skill `.opencode/skills/ef-migrations/SKILL.md`)

- Seed inside the migration via `migrationBuilder.InsertData` **without the `id` column** (the identity generates it).
- **FORBIDDEN**: `HasData` with fixed ids and `setval()`.
- Timestamps: `created_at`/`updated_at` columns with `HasDefaultValueSql("CURRENT_TIMESTAMP")` + `UpdateTimestampInterceptor` (`IHasTimestamps` interface in `Shared/BolivianDaily.Shared/Entities/`). No triggers.
- `articles` FKs → categories/sources with `DeleteBehavior.SetNull`; unique index on `articles.url`.
- Generate migration: `dotnet ef migrations add <Name> --project ScraperWorker/BolivianDaily.ScraperWorker.Infrastructure/BolivianDaily.ScraperWorker.Infrastructure.csproj --startup-project ScraperWorker/BolivianDaily.ScraperWorker/BolivianDaily.ScraperWorker.csproj --output-dir Migrations`

## Code conventions

- C#: file-scoped namespaces, primary constructors, ImplicitUsings, Nullable enabled.
- **NO comments unless requested.**
- Names in English; tables and columns `snake_case`, entities plural.
- Repositories: `Sql*Repository` in Infrastructure; one `DbContext` per worker; snake_case mapping in `OnModelCreating`.
- Layers: Infrastructure → Application → Domain; Domain/Infrastructure reference Shared. Do not invert dependencies.
- Respond in Spanish, keep answers concise, no lengthy explanations.
