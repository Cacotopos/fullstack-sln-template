using Company.Product.Domain.Entities;

namespace Company.Product.Application.Interfaces;

public interface ISampleItemRepository
{
    Task<IReadOnlyList<SampleItem>> GetAllAsync(CancellationToken ct = default);
    Task<SampleItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(SampleItem item, CancellationToken ct = default);
    Task UpdateAsync(SampleItem item, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
