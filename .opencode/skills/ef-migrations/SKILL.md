---
name: ef-migrations
description: Mandatory workflow for EF Core migrations of the workers
  (Scraper/Checker/Sync). Use it when generating, editing, verifying or
  regenerating migrations, creating seed data, adding tables/columns/timestamps,
  touching `__EFMigrationsHistory`, `HasData`, `setval`, `InsertData`,
  `Database.Migrate` or `dotnet ef migrations`.
---

# EF Core migrations — mandatory conventions

## Non-negotiable rules

1. **Seed inside the migration** with `migrationBuilder.InsertData(...)` in C#,
   **without including the `id` column** — the identity generates it.
   - Valid example (scraper):
     ```csharp
     migrationBuilder.InsertData(
         table: "categories",
         columns: ["name", "slug", "created_at", "updated_at"],
         values: new object[,]
         {
             { "Nacional", "nacional", "CURRENT_TIMESTAMP", "CURRENT_TIMESTAMP" },
         });
     ```
   - `values` may use `"CURRENT_TIMESTAMP"` (inserts run as SQL without typed parameters).
2. **FORBIDDEN**: `HasData` with fixed ids and **FORBIDDEN** `setval()` in the migration.
3. **Forbidden** manual seed by SQL outside migrations (the DB seeds itself on worker startup).
4. Migrations **are versioned**: `Migrations/` folder in each Infrastructure, tracked in git. Never delete `InitialCreate` if already applied to a DB.

## Timestamps (mandatory on all tables)

- `created_at` / `updated_at` columns → `HasDefaultValueSql("CURRENT_TIMESTAMP")` in `OnModelCreating`.
- Entity implements `IHasTimestamps` (`Shared/BolivianDaily.Shared/Entities/IHasTimestamps.cs`).
- `UpdateTimestampInterceptor` (Persistence) updates `UpdatedAt` on `Added`/`Modified`. No DB triggers.

## FKs and constraints

- `articles` FKs → `categories`, `news_sources`, `source_categories` with `DeleteBehavior.SetNull`.
- Unique index on `articles.url` (avoids duplicates on re-scrapes).
- Tables `snake_case`, entities plural.

## Commands

- Generate migration (scraper example):
  ```
  dotnet ef migrations add <Name> --project ScraperWorker/BolivianDaily.ScraperWorker.Infrastructure/BolivianDaily.ScraperWorker.Infrastructure.csproj --startup-project ScraperWorker/BolivianDaily.ScraperWorker/BolivianDaily.ScraperWorker.csproj --output-dir Migrations
  ```
- Verify schema: `docker exec scraper-postgres psql -U scraper_user -d scraper_db -c "\dt"`
- Verify seed/history:
  - `SELECT * FROM categories ORDER BY id;`
  - `SELECT migration_id FROM "__EFMigrationsHistory";`

## Full workflow

1. `docker compose up -d` (containers up).
2. Modify the model/`DbContext` — **no seed in the model** (no `HasData`/`SeedData` in `OnModelCreating`).
3. `dotnet build BolivianDaily.Worker.slnx`.
4. Generate the migration (command above).
5. Open the migration and add seeds with `InsertData` **without the `id` column**.
6. Start the worker host (runs `Database.Migrate()` on startup) and verify with psql: tables, seed rows, and idempotency (2nd startup without errors).
