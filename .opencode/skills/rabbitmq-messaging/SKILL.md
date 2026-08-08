---
name: rabbitmq-messaging
description: Mandatory workflow for RabbitMQ messaging of the workers
  (Scraper/Checker/Sync). Use it when creating or editing consumers, publishers,
  events, queues, exchanges, routing keys, DLX/DLQ, retry headers, `RabbitMqOptions`,
  `RabbitMqTopology`, `RabbitMqConsumerHostedService`, `RabbitMqEventPublisher`,
  `AddHostedService`, or anything under `Shared/BolivianDaily.Shared/Messaging/`.
---

# RabbitMQ messaging — mandatory conventions

## Naming (non-negotiable)

1. **Events are past-tense facts** named after the producer's action:
   - `ArticleScrapedEvent` — the Scraper scraped the article.
   - `ArticleCheckedEvent` — the Checker checked the article.
   - Future chain: `ArticleSyncedEvent` (Sync).
2. **NEVER name events by destination** (`ArticleToCheckEvent` style). Destination
   lives in the queue name (`checker.*`, `sync.*`), never in the event.
3. **FORBIDDEN: "Process/Processed"** anywhere in the article chain (event, entity,
   table, use case, method names). Use the domain action: Scraper scrapes → Checker
   checks → Sync syncs. Generic infra methods use "Handle" (`HandleAsync`),
   not "Process".
4. Queues name the destination: `checker.article-scraped`, `sync.article-checked`.

## Topology

- Exchange `bolivian-daily.articles` (direct, durable) — one shared exchange for the
  whole chain.
- Main queues are durable, non-exclusive, non-auto-delete, and declare
  `x-dead-letter-exchange` (DLX `{exchange}.dlx`, direct, durable) and
  `x-dead-letter-routing-key` (DLQ `{queue}.dead`) arguments.
- **`RabbitMqTopology.Declare(channel, options, queueName, routingKey)` is the single
  source of truth** for exchange/queue/DLX/DLQ/bindings. Consumers and publishers must
  call it, never inline declarations.
- **Pitfall**: redeclaring an existing queue with different arguments fails with 406
  `PRECONDITION_FAILED`. When queue args change, delete the queue once:
  `docker exec rabbitmq rabbitmqctl delete_queue <name>`.

## Adding a new event flow (e.g. Scraper → new consumer)

1. Create the event record in `Shared/BolivianDaily.Shared/Messaging/` (past-tense
   name, e.g. `ArticleSyncedEvent`).
2. Add queue + routing key properties to `RabbitMqOptions` (defaults matching the
   naming rules) and add the same keys to **all 3** `appsettings.json` files.
3. Add the routing key helper in `RabbitMqTopology` if needed.
4. Create a thin concrete publisher inheriting `RabbitMqEventPublisher<TMessage>`:
   it only passes the message type, queue and routing key (see
   `RabbitMqArticleCheckedEventPublisher`).
5. Register the publisher in the producer's Infrastructure DI
   (`services.AddScoped<I...Publisher, RabbitMq...Publisher>()`).
6. Create a thin concrete consumer inheriting `RabbitMqConsumerHostedService<TMessage>`
   (in the consumer's host project, next to `Program.cs`): override `QueueName`,
   `RoutingKey` and `HandleAsync` (resolve the use case from a DI scope).
7. Register it with `builder.Services.AddHostedService<...>()` in the consumer's
   `Program.cs`.

## Consumer rules (`RabbitMqConsumerHostedService<TMessage>`)

- Connection retry with exponential backoff (1s → 30s, unbounded until shutdown) —
  workers must survive RabbitMQ being down at startup.
- `BasicQos(prefetchCount: 1)`, `AsyncEventingBasicConsumer`, `DispatchConsumersAsync
  = true` on the `ConnectionFactory`.
- On failure: increment header `x-retry-count` and republish + ack while
  `retryCount < MaxRetries`; then `BasicNack(requeue: false)` → DLX → DLQ.
- Log a warning on requeue and an error when the message goes to the DLQ (handled by
  the base class).
- `HandleAsync` logs before ("Checking/Syncing article ...") and after
  ("Article ... checked/synced and acked").

## Publisher rules

- Reuse the singleton `IConnection` from DI; open a channel per message.
  **Never create a connection per publish.**
- `RabbitMqEventPublisher<TMessage>` sets persistent properties, `application/json`
  content type and the message type name; logs after publishing.

## Connections / options

- Each worker's Infrastructure registers `services.Configure<RabbitMqOptions>(...)`,
  a singleton `ConnectionFactory` (`DispatchConsumersAsync = true`) and a singleton
  `IConnection`. Never register these in individual classes or hosts.
- Keep the `RabbitMq` section keys in the 3 `appsettings.json` files in sync with
  `RabbitMqOptions`.

## Verification

- List queues/exchanges/consumers:
  - `docker exec rabbitmq rabbitmqctl list_queues name messages`
  - `docker exec rabbitmq rabbitmqctl list_exchanges name`
  - `docker exec rabbitmq rabbitmqctl list_consumers`
- Management UI: `http://localhost:15672` (guest/guest).
- Publish a test message (HTTP API avoids shell quoting issues):
  ```powershell
  $payload = '{"properties":{},"routing_key":"<routingKey>","payload":"{\"...\":\"...\"}","payload_encoding":"string"}'
  Invoke-RestMethod -Uri "http://localhost:15672/api/exchanges/%2f/bolivian-daily.articles/publish" -Method Post -Credential (New-Object System.Management.Automation.PSCredential("guest", (ConvertTo-SecureString "guest" -AsPlainText -Force))) -ContentType "application/json" -Body $payload
  ```
- Poison message test: publish a payload that fails processing → expect 3 retries
  (log "Requeued message ...") then the message in `{queue}.dead`
  ("... exceeded max retries (3), sent to dead letter queue").
- Cleanup test messages: `docker exec rabbitmq rabbitmqctl purge_queue <queue>.dead`.
