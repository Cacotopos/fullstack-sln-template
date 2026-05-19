using Company.Product.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Company.Product.Api.Endpoints;

public static class BlobEndpoints
{
    public static void Map(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/blobs").RequireAuthorization();

        group.MapPost("/{containerName}", async (
            string containerName,
            IFormFile file,
            IBlobService blobService,
            CancellationToken ct) =>
        {
            if (file.Length == 0) return Results.BadRequest("No file uploaded.");

            var blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            using var stream = file.OpenReadStream();
            var uri = await blobService.UploadAsync(containerName, blobName, stream, file.ContentType, ct);
            return uri is not null ? Results.Ok(new { uri = uri.ToString(), blobName }) : Results.Problem("Upload failed.");
        })
        .DisableAntiforgery()
        .WithName("UploadBlob");

        group.MapGet("/{containerName}/{blobName}", async (
            string containerName,
            string blobName,
            IBlobService blobService,
            CancellationToken ct) =>
        {
            var stream = await blobService.DownloadAsync(containerName, blobName, ct);
            return stream is not null ? Results.File(stream, contentType: "application/octet-stream", enableRangeProcessing: true) : Results.NotFound();
        })
        .WithName("DownloadBlob");

        group.MapDelete("/{containerName}/{blobName}", async (
            string containerName,
            string blobName,
            IBlobService blobService,
            CancellationToken ct) =>
        {
            var deleted = await blobService.DeleteAsync(containerName, blobName, ct);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteBlob");

        group.MapGet("/{containerName}", async (
            string containerName,
            [FromQuery] string? prefix,
            IBlobService blobService,
            CancellationToken ct) =>
        {
            var blobs = await blobService.ListAsync(containerName, prefix, ct);
            return Results.Ok(blobs);
        })
        .WithName("ListBlobs");
    }

    public static void MapBlobEndpoints(this IEndpointRouteBuilder routes)
    {
        Map(routes);
    }
}
