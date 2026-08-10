namespace BolivianDaily.SyncWorker.Application.Interfaces;

public sealed record ArticleSyncResult(long? Id, string? Url, string? Status, string? Message);
