using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Data.Entities;

namespace UserManagement.Data.DBConfigs;

public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        builder.ToTable("UserGroups");

        builder.HasKey(ur => new { ur.UserId, ur.GroupId });

        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserGroups)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Group)
            .WithMany(r => r.UserGroups)
            .HasForeignKey(ur => ur.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

