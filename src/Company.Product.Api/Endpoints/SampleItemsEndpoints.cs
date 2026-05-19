using Company.Product.Application.Services;
using Company.Product.Contracts.SampleItems;

namespace Company.Product.Api.Endpoints;

internal static class SampleItemsEndpoints
{
    internal static IEndpointRouteBuilder MapSampleItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/sample-items")
            .RequireAuthorization()
            .WithTags("SampleItems");

        group.MapGet("/", async (SampleItemService service, CancellationToken ct) =>
            Results.Ok(await service.GetAllAsync(ct)))
            .WithName("GetAllSampleItems");

        group.MapGet("/{id:guid}", async (Guid id, SampleItemService service, CancellationToken ct) =>
        {
            var item = await service.GetByIdAsync(id, ct);
            return item is null ? Results.NotFound() : Results.Ok(item);
        }).WithName("GetSampleItemById");

        group.MapPost("/", async (CreateSampleItemRequest request, SampleItemService service, CancellationToken ct) =>
        {
            var item = await service.CreateAsync(request, ct);
            return Results.Created($"/api/v1/sample-items/{item.Id}", item);
        }).WithName("CreateSampleItem");

        group.MapPut("/{id:guid}", async (Guid id, UpdateSampleItemRequest request, SampleItemService service, CancellationToken ct) =>
        {
            var item = await service.UpdateAsync(id, request, ct);
            return item is null ? Results.NotFound() : Results.Ok(item);
        }).WithName("UpdateSampleItem");

        group.MapDelete("/{id:guid}", async (Guid id, SampleItemService service, CancellationToken ct) =>
        {
            var deleted = await service.DeleteAsync(id, ct);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).WithName("DeleteSampleItem");

        return app;
    }
}
