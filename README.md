# BolivianDaily Workers

.NET 10 solution with 3 workers that scrape, validate and sync Bolivian news. They share infrastructure (PostgreSQL 16 + RabbitMQ) running in Docker.

## Prerequisites

- .NET 10 SDK
- Docker (with Docker Compose)
- Global `dotnet-ef`: `dotnet tool install --global dotnet-ef`

## Getting started (first run)

1. Clone the repo and create the `.env` file with the DB password:

   ```powershell
   Copy-Item .env.example .env
   ```

2. Start the infrastructure (PostgreSQL + RabbitMQ):

   ```powershell
   docker compose up -d
   ```

   Available services:
   | Service | Port | DB | User |
   |---|---|---|---|
   | `scraper-postgres` | 5432 | `scraper_db` | `scraper_user` |
   | `checker-postgres` | 5433 | `checker_db` | `checker_user` |
   | `rabbitmq` | 5672 / 15672 | — | — |

3. Run the scraper worker. On startup it runs `Database.Migrate()`, which creates, migrates and seeds its DB automatically:

   ```powershell
   dotnet run --project ScraperWorker/BolivianDaily.ScraperWorker/BolivianDaily.ScraperWorker.csproj
   ```

4. Verify the migration and the seed:

   ```powershell
   docker exec scraper-postgres psql -U scraper_user -d scraper_db -c "\dt"
   docker exec scraper-postgres psql -U scraper_user -d scraper_db -c "SELECT id, name FROM categories ORDER BY id;"
   ```

5. Stop the worker (Ctrl+C). The infrastructure keeps running; bring it up again with `docker compose up -d`.

## EF Core migrations

- Each worker applies its migrations on startup (`Database.Migrate()` in `Program.cs`). No manual SQL scripts.
- Migrations are versioned in `Migrations/` inside each Infrastructure project.

Generate a new migration (scraper example):

```powershell
dotnet ef migrations add <Name> --project ScraperWorker/BolivianDaily.ScraperWorker.Infrastructure/BolivianDaily.ScraperWorker.Infrastructure.csproj --startup-project ScraperWorker/BolivianDaily.ScraperWorker/BolivianDaily.ScraperWorker.csproj --output-dir Migrations
```

> Mandatory conventions when creating migrations: seed inside the migration with `InsertData` without the `id` column (`HasData` with fixed ids and `setval` are forbidden), `created_at`/`updated_at` timestamps with `CURRENT_TIMESTAMP`, `articles` FKs with `DeleteBehavior.SetNull`. Full details in `AGENTS.md`.

## Reset the scraper DB (delete and recreate from scratch)

```powershell
docker compose rm -sf scraper-postgres
docker volume rm bolivian-daily-worker_scraper-postgres-data
docker compose up -d scraper-postgres
```

The DB is recreated and seeded by itself the next time you start the worker.

## Useful commands

- Full build: `dotnet build BolivianDaily.Worker.slnx`
- Inspect the scraper DB: `docker exec scraper-postgres psql -U scraper_user -d scraper_db -c "<query>"`
