using ContentManagement.Data.DBConfigs;
using ContentManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContentManagement.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options) {
    public DbSet<Article> Articles { get; set; }
    public DbSet<ArticleGroup> ArticleGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfiguration(new ArticleConfiguration());
        modelBuilder.ApplyConfiguration(new ArticleGroupConfiguration());
    }
}