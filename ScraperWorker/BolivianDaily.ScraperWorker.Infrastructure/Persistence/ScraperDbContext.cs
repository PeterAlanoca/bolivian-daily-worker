using BolivianDaily.ScraperWorker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BolivianDaily.ScraperWorker.Infrastructure.Persistence;

public class ScraperDbContext(DbContextOptions<ScraperDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<NewsSource> NewsSources => Set<NewsSource>();
    public DbSet<SourceCategory> SourceCategories => Set<SourceCategory>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<ArticleMedia> ArticleMedia => Set<ArticleMedia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Slug).HasColumnName("slug").HasMaxLength(100).IsRequired();
            entity.Property(e => e.State).HasColumnName("state").HasMaxLength(1).HasDefaultValue("A");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        modelBuilder.Entity<NewsSource>(entity =>
        {
            entity.ToTable("news_sources");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Alias).HasColumnName("alias").HasMaxLength(100).IsRequired();
            entity.Property(e => e.BaseUrl).HasColumnName("base_url").HasMaxLength(255).IsRequired();
            entity.Property(e => e.State).HasColumnName("state").HasMaxLength(1).HasDefaultValue("A");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(e => e.Alias).IsUnique();
        });

        modelBuilder.Entity<SourceCategory>(entity =>
        {
            entity.ToTable("source_categories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NewsSourceId).HasColumnName("news_source_id").IsRequired();
            entity.Property(e => e.CategoryId).HasColumnName("category_id").IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Url).HasColumnName("url").HasMaxLength(255).IsRequired();
            entity.Property(e => e.State).HasColumnName("state").HasMaxLength(1).HasDefaultValue("A");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.NewsSource)
                  .WithMany(s => s.Categories)
                  .HasForeignKey(e => e.NewsSourceId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                  .WithMany()
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.ToTable("articles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NewsSourceId).HasColumnName("news_source_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.SourceCategoryId).HasColumnName("source_category_id");
            entity.Property(e => e.Url).HasColumnName("url").IsRequired();
            entity.Property(e => e.Pretitle).HasColumnName("pretitle");
            entity.Property(e => e.Title).HasColumnName("title").IsRequired();
            entity.Property(e => e.Subtitle).HasColumnName("subtitle");
            entity.Property(e => e.Lead).HasColumnName("lead");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.Author).HasColumnName("author").HasMaxLength(190);
            entity.Property(e => e.PublishedAt).HasColumnName("published_at");
            entity.Property(e => e.ScrapedAt).HasColumnName("scraped_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.State).HasColumnName("state").HasMaxLength(1).HasDefaultValue("A");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.Url).IsUnique();

            entity.HasOne(e => e.NewsSource)
                  .WithMany()
                  .HasForeignKey(e => e.NewsSourceId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Category)
                  .WithMany()
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.SourceCategory)
                  .WithMany()
                  .HasForeignKey(e => e.SourceCategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Media)
                  .WithOne()
                  .HasForeignKey(m => m.ArticleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ArticleMedia>(entity =>
        {
            entity.ToTable("article_media");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArticleId).HasColumnName("article_id").IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Url).HasColumnName("url").IsRequired();
            entity.Property(e => e.Path).HasColumnName("path").HasMaxLength(255);
            entity.Property(e => e.Type).HasColumnName("type").HasMaxLength(50).IsRequired();
            entity.Property(e => e.State).HasColumnName("state").HasMaxLength(1).HasDefaultValue("A");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
