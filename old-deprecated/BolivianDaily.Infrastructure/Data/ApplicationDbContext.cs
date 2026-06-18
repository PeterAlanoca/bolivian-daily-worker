namespace BolivianDaily.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using BolivianDaily.Domain.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<News> News { get; set; } = null!;
    public DbSet<Multimedia> Multimedia { get; set; } = null!;
    public DbSet<Source> Sources { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<SourceCategory> SourceCategories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── CATEGORY ────────────────────────────────────────────────
        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("category");
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasColumnName("id");
            e.Property(c => c.Name).HasColumnName("name");
            e.Property(c => c.Url).HasColumnName("url");
            e.Property(c => c.State).HasColumnName("state");
            e.Property(c => c.CreatedAt).HasColumnName("created_at");
            e.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        });

        // ── SOURCE ──────────────────────────────────────────────────
        modelBuilder.Entity<Source>(e =>
        {
            e.ToTable("source");
            e.HasKey(s => s.Id);
            e.Property(s => s.Id).HasColumnName("id");
            e.Property(s => s.Name).HasColumnName("name");
            e.Property(s => s.Alias).HasColumnName("alias");
            e.Property(s => s.Url).HasColumnName("url");
            e.Property(s => s.State).HasColumnName("state");
            e.Property(s => s.CreatedAt).HasColumnName("created_at");
            e.Property(s => s.UpdatedAt).HasColumnName("updated_at");
        });

        // ── SOURCE_CATEGORY ─────────────────────────────────────────
        modelBuilder.Entity<SourceCategory>(e =>
        {
            e.ToTable("source_category");
            e.HasKey(sc => sc.Id);
            e.Property(sc => sc.Id).HasColumnName("id");
            e.Property(sc => sc.SourceId).HasColumnName("source_id");
            e.Property(sc => sc.CategoryId).HasColumnName("category_id");
            e.Property(sc => sc.Name).HasColumnName("name");
            e.Property(sc => sc.Url).HasColumnName("url");
            e.Property(sc => sc.State).HasColumnName("state");
            e.Property(sc => sc.CreatedAt).HasColumnName("created_at");
            e.Property(sc => sc.UpdatedAt).HasColumnName("updated_at");

            e.HasOne(sc => sc.Source)
             .WithMany(s => s.Categories)
             .HasForeignKey(sc => sc.SourceId);

            e.HasOne(sc => sc.Category)
             .WithMany(c => c.Sources)
             .HasForeignKey(sc => sc.CategoryId);
        });

        // ── NEWS ────────────────────────────────────────────────────
        modelBuilder.Entity<News>(e =>
        {
            e.ToTable("news");
            e.HasKey(n => n.Id);
            e.Property(n => n.Id).HasColumnName("id");
            e.Property(n => n.CategoryId).HasColumnName("category_id");
            e.Property(n => n.SourceId).HasColumnName("source_id");
            e.Property(n => n.Url).HasColumnName("url");
            e.Property(n => n.Pretitle).HasColumnName("pretitle");
            e.Property(n => n.Title).HasColumnName("title");
            e.Property(n => n.Subtitle).HasColumnName("subtitle");
            e.Property(n => n.Enter).HasColumnName("enter");
            e.Property(n => n.Body).HasColumnName("body");
            e.Property(n => n.Author).HasColumnName("author");
            e.Property(n => n.PublicationDate).HasColumnName("publication_date");
            e.Property(n => n.State).HasColumnName("state");
            e.Property(n => n.CreatedAt).HasColumnName("created_at");
            e.Property(n => n.UpdatedAt).HasColumnName("updated_at");

            e.HasOne(n => n.Source)
             .WithMany(s => s.News)
             .HasForeignKey(n => n.SourceId);

            e.HasOne(n => n.Category)
             .WithMany(c => c.News)
             .HasForeignKey(n => n.CategoryId);
        });

        // ── MULTIMEDIA ──────────────────────────────────────────────
        modelBuilder.Entity<Multimedia>(e =>
        {
            e.ToTable("multimedia");
            e.HasKey(m => m.Id);
            e.Property(m => m.Id).HasColumnName("id");
            e.Property(m => m.NewsId).HasColumnName("news_id");
            e.Property(m => m.Description).HasColumnName("description");
            e.Property(m => m.Url).HasColumnName("url");
            e.Property(m => m.Path).HasColumnName("path");
            e.Property(m => m.Type).HasColumnName("type");
            e.Property(m => m.State).HasColumnName("state");
            e.Property(m => m.CreatedAt).HasColumnName("created_at");
            e.Property(m => m.UpdatedAt).HasColumnName("updated_at");

            e.HasOne(m => m.News)
             .WithMany(n => n.Multimedia)
             .HasForeignKey(m => m.NewsId);
        });
    }
}
