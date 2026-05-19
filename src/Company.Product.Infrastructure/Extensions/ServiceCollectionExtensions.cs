using Azure.Storage.Blobs;
using Company.Product.Application.Interfaces;
using Company.Product.Infrastructure.Auth;
using Company.Product.Infrastructure.Blob;
using Company.Product.Infrastructure.Persistence;
using Company.Product.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Company.Product.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var writeConnString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        var readConnString = configuration.GetConnectionString("ReadOnly") ?? writeConnString;

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(writeConnString));

        services.AddDbContext<ReadOnlyAppDbContext>(options =>
            options.UseNpgsql(readConnString)
                   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IReadOnlyAppDbContext>(sp => sp.GetRequiredService<ReadOnlyAppDbContext>());
        services.AddScoped<ISampleItemRepository, SampleItemRepository>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        var blobConnStr = configuration.GetConnectionString("AzureBlob");
        if (!string.IsNullOrWhiteSpace(blobConnStr))
        {
            var blobClient = new BlobServiceClient(blobConnStr);
            services.AddSingleton<IBlobService, AzureBlobService>();
            services.AddSingleton(blobClient);
        }
        else
        {
            var localPath = Path.Combine(AppContext.BaseDirectory, "blobs");
            services.AddSingleton<IBlobService>(new LocalBlobService(localPath));
        }

        return services;
    }
}
