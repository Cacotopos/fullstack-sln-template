using Company.Product.Application.Extensions;
using Company.Product.Infrastructure.Extensions;
using Company.Product.Worker;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;

var builder = Host.CreateApplicationBuilder(args);

// Configure Serilog globally
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
