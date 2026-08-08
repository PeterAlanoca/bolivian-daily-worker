---
description: Inspects scraper_db (tables, counts, migration history).
---

Inspect `scraper_db` to check the current state:

1. Tables: `docker exec scraper-postgres psql -U scraper_user -d scraper_db -c "\dt"`
2. Counts per table:
   - `docker exec scraper-postgres psql -U scraper_user -d scraper_db -c "SELECT 'categories' AS t, count(*) FROM categories UNION ALL SELECT 'news_sources', count(*) FROM news_sources UNION ALL SELECT 'source_categories', count(*) FROM source_categories UNION ALL SELECT 'articles', count(*) FROM articles;"`
3. Migration history: `docker exec scraper-postgres psql -U scraper_user -d scraper_db -c "SELECT migration_id FROM \"__EFMigrationsHistory\";"`
4. Summarize the state in a short table (table → rows) and flag any anomaly.
