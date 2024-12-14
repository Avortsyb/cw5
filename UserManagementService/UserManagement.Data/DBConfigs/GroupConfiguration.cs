using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Data.Entities;

namespace UserManagement.Data.DBConfigs;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups");

        builder.HasKey(g => g.GroupId);

        builder.Property(g => g.GroupName)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(g => g.UserGroups)
            .WithOne(u => u.Group)
            .HasForeignKey(u => u.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(g => g.GroupName) 
            .IsUnique();
    }
}