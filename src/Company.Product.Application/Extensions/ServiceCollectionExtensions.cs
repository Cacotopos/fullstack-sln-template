using Company.Product.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Company.Product.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<SampleItemService>();
        return services;
    }
}
