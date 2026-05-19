using System.ComponentModel.DataAnnotations;

namespace Company.Product.Contracts.SampleItems;

public sealed record CreateSampleItemRequest(
    [Required, MaxLength(200)] string Name,
    [MaxLength(1000)] string? Description);
