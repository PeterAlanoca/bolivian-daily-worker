---
description: Generates an EF Core migration for a worker with the db-migrator
  agent (seed with InsertData, timestamps, FKs).
agent: db-migrator
---

Generate the EF Core migration `$ARGUMENTS` strictly following the `ef-migrations` skill and the conventions in `AGENTS.md`.

- Use the `--project` of the Infrastructure and `--startup-project` of the host corresponding to the worker being modified (default: ScraperWorker), with `--output-dir Migrations`.
- If there are no prior model changes, state which worker and which entities apply.
- Add the required seeds with `migrationBuilder.InsertData` **without the `id` column**.
- Validate with build and, if possible, worker startup + psql.
