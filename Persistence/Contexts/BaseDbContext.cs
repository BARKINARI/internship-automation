using System.Reflection;
using Core.Persistence.Repositories;
using Core.Security.Entities;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;

namespace Persistence.Contexts;

public class BaseDbContext : DbContext
{
    protected IConfiguration Configuration { get; set; }

    public DbSet<StajBasvuru> StajBasvurulari { get; set; }
    public DbSet<Mesaj> Mesajlar { get; set; }
    public DbSet<Proje> Projeler { get; set; }
    public DbSet<StajyerProje> StajyerProjeler { get; set; }
    public DbSet<User<int>> Users { get; set; }
    public DbSet<OperationClaim<int>> OperationClaims { get; set; }
    public DbSet<UserOperationClaim<int, int, int>> UserOperationClaims { get; set; }
    public DbSet<RefreshToken<int, int>> RefreshTokens { get; set; }

    public BaseDbContext(DbContextOptions dbContextOptions, IConfiguration configuration)
        : base(dbContextOptions)
    {
        Configuration = configuration;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<EntityEntry<IEntityTimestamps>> entries = ChangeTracker
            .Entries<IEntityTimestamps>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (EntityEntry<IEntityTimestamps> entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedDate = DateTime.UtcNow;
            else if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedDate = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}