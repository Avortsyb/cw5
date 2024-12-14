using ContentManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ContentManagement.Data.Entities;


namespace ContentManagement.Data.DBConfigs;

public class ArticleGroupConfiguration: IEntityTypeConfiguration<ArticleGroup> {
    public void Configure(EntityTypeBuilder<ArticleGroup> builder) {
        builder.ToTable("ArticleGroup");
        builder.HasKey(ag => new { ag.ArticleId, ag.GroupId });
        builder.Property(ag => ag.ArticleId).IsRequired();
        builder.Property(ag => ag.GroupId).IsRequired();
        builder
            .HasOne(ag => ag.Article)
            .WithMany(a => a.ArticleGroups)
            .HasForeignKey(ag => ag.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}