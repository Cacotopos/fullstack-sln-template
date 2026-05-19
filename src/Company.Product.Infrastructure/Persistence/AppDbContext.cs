using Company.Product.Application.Interfaces;
using Company.Product.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Company.Product.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<IdentityUser>(options), IUnitOfWork, IReadOnlyAppDbContext
{
    public DbSet<SampleItem> SampleItems => Set<SampleItem>();

    IQueryable<SampleItem> IReadOnlyAppDbContext.SampleItems =>
        SampleItems.AsNoTracking();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
