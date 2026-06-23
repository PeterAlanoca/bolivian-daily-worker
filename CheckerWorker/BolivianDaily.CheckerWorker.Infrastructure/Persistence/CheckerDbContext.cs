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
            entity.HasIndex(article => article.SourceArticleId).IsUnique();
            entity.Property(article => article.SourceArticleId).HasColumnName("source_article_id");
            entity.Property(article => article.CategoryId).HasColumnName("category_id");
            entity.Property(article => article.SourceId).HasColumnName("source_id");
            entity.Property(article => article.UserId).HasColumnName("user_id");
            entity.Property(article => article.Title).HasColumnName("title").HasMaxLength(500).IsRequired();
            entity.Property(article => article.Pretitle).HasColumnName("pretitle").HasMaxLength(500);
            entity.Property(article => article.Subtitle).HasColumnName("subtitle").HasMaxLength(500);
            entity.Property(article => article.Enter).HasColumnName("enter");
            entity.Property(article => article.Body).HasColumnName("body").IsRequired();
            entity.Property(article => article.Author).HasColumnName("author").HasMaxLength(250);
            entity.Property(article => article.PublicationDate).HasColumnName("publication_date");
            entity.Property(article => article.State).HasColumnName("state").HasMaxLength(1).HasDefaultValue("A");
            entity.Property(article => article.ProcessedAt).HasColumnName("processed_at");
            entity.Property(article => article.WarningsJson).HasColumnName("warnings_json");
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
