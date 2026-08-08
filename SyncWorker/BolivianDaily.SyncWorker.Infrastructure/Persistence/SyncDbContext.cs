using BolivianDaily.SyncWorker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BolivianDaily.SyncWorker.Infrastructure.Persistence;

public sealed class SyncDbContext(DbContextOptions<SyncDbContext> options) : DbContext(options)
{
    public DbSet<ArticleSyncLog> ArticleSyncLogs => Set<ArticleSyncLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArticleSyncLog>(entity =>
        {
            entity.ToTable("article_sync_logs");
            entity.HasKey(log => log.Id);
            entity.Property(log => log.Id).HasColumnName("id");
            entity.HasIndex(log => log.CheckedArticleId);
            entity.Property(log => log.CheckedArticleId).HasColumnName("checked_article_id");
            entity.Property(log => log.ScrapedArticleId).HasColumnName("scraped_article_id");
            entity.Property(log => log.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
            entity.Property(log => log.Attempts).HasColumnName("attempts");
            entity.Property(log => log.ExternalId).HasColumnName("external_id").HasMaxLength(250);
            entity.Property(log => log.ErrorMessage).HasColumnName("error_message");
            entity.Property(log => log.CreatedAt).HasColumnName("created_at");
            entity.Property(log => log.SyncedAt).HasColumnName("synced_at");
        });
    }
}
