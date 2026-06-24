using BolivianDaily.CheckerWorker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BolivianDaily.CheckerWorker.Infrastructure.Persistence;

public sealed class CheckerDbContext(DbContextOptions<CheckerDbContext> options) : DbContext(options)
{
    public DbSet<ProcessedArticle> ProcessedArticles => Set<ProcessedArticle>();
    public DbSet<ProcessedArticleMedia> ProcessedArticleMedia => Set<ProcessedArticleMedia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessedArticle>(entity =>
        {
            entity.ToTable("processed_articles");
            entity.HasKey(article => article.Id);
            entity.Property(article => article.Id).HasColumnName("id");
            entity.HasIndex(article => article.ScrapedArticleId).IsUnique();
            entity.Property(article => article.ScrapedArticleId).HasColumnName("scraped_article_id");
            entity.Property(article => article.CategoryId).HasColumnName("category_id");
            entity.Property(article => article.SourceId).HasColumnName("source_id");
            entity.Property(article => article.Title).HasColumnName("title").HasMaxLength(500).IsRequired();
            entity.Property(article => article.Pretitle).HasColumnName("pretitle").HasMaxLength(500);
            entity.Property(article => article.Subtitle).HasColumnName("subtitle").HasMaxLength(500);
            entity.Property(article => article.Enter).HasColumnName("enter");
            entity.Property(article => article.Body).HasColumnName("body").IsRequired();
            entity.Property(article => article.Author).HasColumnName("author").HasMaxLength(250);
            entity.Property(article => article.PublicationDate).HasColumnName("publication_date");
            entity.Property(article => article.State).HasColumnName("state").HasMaxLength(1).HasDefaultValue("A");
            entity.Property(article => article.ProcessedAt).HasColumnName("processed_at");
            entity.Property(article => article.Warnings).HasColumnName("warnings");
            entity.Property(article => article.IsValid).HasColumnName("is_valid");
            entity.Property(article => article.HtmlFormatPassed).HasColumnName("html_format_passed");
            entity.Property(article => article.HtmlFormatReason).HasColumnName("html_format_reason");
            entity.Property(article => article.CategoryAccuracyPassed).HasColumnName("category_accuracy_passed");
            entity.Property(article => article.CategoryAccuracyReason).HasColumnName("category_accuracy_reason");
            entity.Property(article => article.SuggestedCategory).HasColumnName("suggested_category").HasMaxLength(50);
            entity.Property(article => article.NoAdvertisingPassed).HasColumnName("no_advertising_passed");
            entity.Property(article => article.NoAdvertisingReason).HasColumnName("no_advertising_reason");
            entity.Property(article => article.DetectedNetworksJson).HasColumnName("detected_networks");
            entity.Property(article => article.ReadyToPublishPassed).HasColumnName("ready_to_publish_passed");
            entity.Property(article => article.ReadyToPublishReason).HasColumnName("ready_to_publish_reason");
            entity.HasMany(article => article.Multimedia)
                .WithOne()
                .HasForeignKey(media => media.ProcessedArticleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProcessedArticleMedia>(entity =>
        {
            entity.ToTable("processed_article_media");
            entity.HasKey(media => media.Id);
            entity.Property(media => media.Id).HasColumnName("id");
            entity.Property(media => media.ProcessedArticleId).HasColumnName("processed_article_id");
            entity.Property(media => media.Url).HasColumnName("url").HasMaxLength(1000).IsRequired();
            entity.Property(media => media.Type).HasColumnName("type").HasMaxLength(100).IsRequired();
            entity.Property(media => media.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(media => media.Path).HasColumnName("path").HasMaxLength(1000);
        });
    }
}
