using Microsoft.EntityFrameworkCore;
using UserManagement.Data.DBConfigs;
using UserManagement.Data.Entities;

namespace UserManagement.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new GroupConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserGroupConfiguration());
    }
}