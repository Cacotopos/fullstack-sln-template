namespace Company.Product.Contracts.SampleItems;

public sealed record SampleItemDto(
    Guid Id,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
