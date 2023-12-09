using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Data;
public class CloudNativeDbContext : DbContext
{
    public CloudNativeDbContext(DbContextOptions<CloudNativeDbContext> options) : base(options)
    {
    }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(t => typeof(Entity).IsAssignableFrom(t.ClrType)))
        {
            entityType.AddISoftDeleteQueryFilter();
        }
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        InterceptChanges();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void InterceptChanges()
    {
        ChangeTracker.ApplySoftDeleteOverride();
    }
}
