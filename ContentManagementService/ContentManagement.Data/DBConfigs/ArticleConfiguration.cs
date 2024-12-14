using ContentManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentManagement.Data.DBConfigs;

public class ArticleConfiguration : IEntityTypeConfiguration<Article> {
    public void Configure(EntityTypeBuilder<Article> builder) {
        builder.ToTable("Articles");
        
        builder.HasKey(x => x.ArticleId);
        
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Content)
            .IsRequired();
        
        
    }
}