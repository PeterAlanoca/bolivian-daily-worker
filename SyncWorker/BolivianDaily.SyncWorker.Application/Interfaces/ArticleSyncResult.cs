namespace BolivianDaily.SyncWorker.Application.Interfaces;

public sealed record ArticleSyncResult(string? Id, string? Url, string? Status, string? Message);
