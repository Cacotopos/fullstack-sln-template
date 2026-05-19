using Company.Product.Application.Interfaces;
using Company.Product.Domain.Entities;
using Company.Product.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Company.Product.Infrastructure.Persistence;

public sealed class ReadOnlyAppDbContext(DbContextOptions<ReadOnlyAppDbContext> options)
    : DbContext(options), IReadOnlyAppDbContext
{
    public IQueryable<SampleItem> SampleItems => Set<SampleItem>().AsNoTracking();

    protected override void OnModelCreating(ModelBuilder builder) =>
        builder.ApplyConfigurationsFromAssembly(typeof(SampleItemConfiguration).Assembly);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) return;
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        optionsBuilder.EnableDetailedErrors(false);
    }

    public override int SaveChanges() =>
        throw new InvalidOperationException("ReadOnlyAppDbContext does not support write operations.");

    public override int SaveChanges(bool acceptAllChangesOnSuccess) =>
        throw new InvalidOperationException("ReadOnlyAppDbContext does not support write operations.");

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("ReadOnlyAppDbContext does not support write operations.");

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("ReadOnlyAppDbContext does not support write operations.");
}
