using BolivianDaily.SyncWorker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BolivianDaily.SyncWorker.Infrastructure.Persistence;

public sealed class SyncDbContext(DbContextOptions<SyncDbContext> options) : DbContext(options)
{
    public DbSet<SyncedArticle> SyncedArticles => Set<SyncedArticle>();
    public DbSet<SyncedArticleMedia> SyncedArticleMedia => Set<SyncedArticleMedia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SyncedArticle>(entity =>
        {
            entity.ToTable("synced_articles");
            entity.HasKey(article => article.Id);
            entity.Property(article => article.Id).HasColumnName("id");
            entity.HasIndex(article => article.CheckedArticleId).IsUnique();
            entity.Property(article => article.CheckedArticleId).HasColumnName("checked_article_id").IsRequired();
            entity.Property(article => article.ScrapedArticleId).HasColumnName("scraped_article_id").IsRequired();
            entity.Property(article => article.SourceId).HasColumnName("source_id");
            entity.Property(article => article.SourceName).HasColumnName("source_name").HasMaxLength(100).IsRequired();
            entity.Property(article => article.SourceUrl).HasColumnName("source_url").HasMaxLength(255).IsRequired();
            entity.Property(article => article.Url).HasColumnName("url").IsRequired();
            entity.Property(article => article.CategoryId).HasColumnName("category_id");
            entity.Property(article => article.CategoryName).HasColumnName("category_name").HasMaxLength(100);
            entity.Property(article => article.Title).HasColumnName("title").IsRequired();
            entity.Property(article => article.Pretitle).HasColumnName("pretitle");
            entity.Property(article => article.Subtitle).HasColumnName("subtitle");
            entity.Property(article => article.Lead).HasColumnName("lead");
            entity.Property(article => article.Body).HasColumnName("body").IsRequired();
            entity.Property(article => article.Author).HasColumnName("author").HasMaxLength(250);
            entity.Property(article => article.PublishedAt).HasColumnName("published_at");
            entity.Property(article => article.ScrapedAt).HasColumnName("scraped_at");
            entity.Property(article => article.CheckedAt).HasColumnName("checked_at");
            entity.Property(article => article.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
            entity.Property(article => article.Attempts).HasColumnName("attempts");
            entity.Property(article => article.ExtranetId).HasColumnName("extranet_id").HasMaxLength(250);
            entity.Property(article => article.ExtranetUrl).HasColumnName("extranet_url").HasMaxLength(500);
            entity.Property(article => article.Details).HasColumnName("details");
            entity.Property(article => article.SyncedAt).HasColumnName("synced_at");
            entity.Property(article => article.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(article => article.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasMany(article => article.Media)
                .WithOne()
                .HasForeignKey(media => media.SyncedArticleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SyncedArticleMedia>(entity =>
        {
            entity.ToTable("synced_article_media");
            entity.HasKey(media => media.Id);
            entity.Property(media => media.Id).HasColumnName("id");
            entity.Property(media => media.SyncedArticleId).HasColumnName("synced_article_id");
            entity.HasIndex(media => media.SyncedArticleId);
            entity.Property(media => media.Url).HasColumnName("url").IsRequired();
            entity.Property(media => media.Type).HasColumnName("type").HasMaxLength(50).IsRequired();
            entity.Property(media => media.Description).HasColumnName("description");
            entity.Property(media => media.Path).HasColumnName("path").HasMaxLength(255);
            entity.Property(media => media.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(media => media.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
