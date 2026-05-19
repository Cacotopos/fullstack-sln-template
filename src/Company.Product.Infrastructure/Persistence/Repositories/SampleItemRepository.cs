using Company.Product.Application.Interfaces;
using Company.Product.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Company.Product.Infrastructure.Persistence.Repositories;

internal sealed class SampleItemRepository(AppDbContext db) : ISampleItemRepository
{
    public async Task<IReadOnlyList<SampleItem>> GetAllAsync(CancellationToken ct = default) =>
        await db.SampleItems.ToListAsync(ct);

    public Task<SampleItem?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.SampleItems.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(SampleItem item, CancellationToken ct = default) =>
        await db.SampleItems.AddAsync(item, ct);

    public Task UpdateAsync(SampleItem item, CancellationToken ct = default)
    {
        db.SampleItems.Update(item);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var item = await db.SampleItems.FindAsync([id], ct);
        if (item is not null)
            db.SampleItems.Remove(item);
    }
}
