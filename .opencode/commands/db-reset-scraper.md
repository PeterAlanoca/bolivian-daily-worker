---
description: Resets the scraper DB from scratch (container + volume) and validates
  that the worker creates, migrates and seeds it by itself on startup.
---

Reset `scraper_db` and validate the ScraperWorker self-initialization:

1. Stop and remove the container and its volume:
   - `docker compose rm -sf scraper-postgres`
   - `docker volume rm bolivian-daily-worker_scraper-postgres-data` (check the real name with `docker volume ls | findstr scraper`)
2. Bring it up again: `docker compose up -d scraper-postgres`
3. Run the worker (host) for a few seconds so it runs `Database.Migrate()`:
   - `dotnet run --project ScraperWorker/BolivianDaily.ScraperWorker/BolivianDaily.ScraperWorker.csproj`
   - If it scrapes, stop it after the verification.
4. Verify:
   - `docker exec scraper-postgres psql -U scraper_user -d scraper_db -c "\dt"` (must list the tables + `__EFMigrationsHistory`)
   - `docker exec scraper-postgres psql -U scraper_user -d scraper_db -c "SELECT id, name FROM categories ORDER BY id;"`
   - `docker exec scraper-postgres psql -U scraper_user -d scraper_db -c "SELECT migration_id FROM \"__EFMigrationsHistory\";"`
5. Confirm idempotency: start the worker a 2nd time without errors.
