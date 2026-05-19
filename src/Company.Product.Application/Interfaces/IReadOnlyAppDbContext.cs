using Company.Product.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Company.Product.Application.Interfaces;

public interface IReadOnlyAppDbContext
{
    IQueryable<SampleItem> SampleItems { get; }
}
