using Company.Product.Application.Interfaces;
using Company.Product.Contracts.SampleItems;
using Company.Product.Domain.Entities;

namespace Company.Product.Application.Services;

public sealed class SampleItemService(
    ISampleItemRepository repository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
{
    public async Task<IReadOnlyList<SampleItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await repository.GetAllAsync(ct);
        return items.Select(MapToDto).ToList();
    }

    public async Task<SampleItemDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await repository.GetByIdAsync(id, ct);
        return item is null ? null : MapToDto(item);
    }

    public async Task<SampleItemDto> CreateAsync(CreateSampleItemRequest request, CancellationToken ct = default)
    {
        var item = SampleItem.Create(request.Name, request.Description, currentUser.UserId);
        await repository.AddAsync(item, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return MapToDto(item);
    }

    public async Task<SampleItemDto?> UpdateAsync(Guid id, UpdateSampleItemRequest request, CancellationToken ct = default)
    {
        var item = await repository.GetByIdAsync(id, ct);
        if (item is null) return null;
        item.Update(request.Name, request.Description);
        await repository.UpdateAsync(item, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return MapToDto(item);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var item = await repository.GetByIdAsync(id, ct);
        if (item is null) return false;
        await repository.DeleteAsync(id, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    private static SampleItemDto MapToDto(SampleItem item) =>
        new(item.Id, item.Name, item.Description, item.CreatedAt, item.UpdatedAt);
}
