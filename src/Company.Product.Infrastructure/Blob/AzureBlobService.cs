using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Company.Product.Application.Interfaces;

namespace Company.Product.Infrastructure.Blob;

public sealed class AzureBlobService(BlobServiceClient client) : IBlobService
{
    public async Task<Uri?> UploadAsync(string containerName, string blobName, Stream content, string? contentType = null, CancellationToken ct = default)
    {
        var container = client.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, ct);

        var blob = container.GetBlobClient(blobName);
        var httpHeaders = contentType is not null ? new BlobHttpHeaders { ContentType = contentType } : null;
        await blob.UploadAsync(content, httpHeaders, default, default, default, default, default, ct);
        return blob.Uri;
    }

    public async Task<Stream?> DownloadAsync(string containerName, string blobName, CancellationToken ct = default)
    {
        var container = client.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);
        if (!await blob.ExistsAsync(ct)) return null;
        var response = await blob.DownloadAsync(ct);
        return response.Value.Content;
    }

    public async Task<bool> DeleteAsync(string containerName, string blobName, CancellationToken ct = default)
    {
        var container = client.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);
        var response = await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, ct);
        return response.Value;
    }

    public async Task<bool> ExistsAsync(string containerName, string blobName, CancellationToken ct = default)
    {
        var container = client.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);
        return await blob.ExistsAsync(ct);
    }

    public async Task<IReadOnlyList<Company.Product.Application.Interfaces.BlobInfo>> ListAsync(string containerName, string? prefix = null, CancellationToken ct = default)
    {
        var container = client.GetBlobContainerClient(containerName);
        var blobs = new List<Company.Product.Application.Interfaces.BlobInfo>();
        await foreach (BlobItem item in container.GetBlobsAsync(BlobTraits.Metadata, BlobStates.None, prefix, ct))
        {
            blobs.Add(new Company.Product.Application.Interfaces.BlobInfo(
                item.Name,
                new Uri(container.Uri, item.Name),
                item.Properties.LastModified ?? DateTimeOffset.MinValue,
                item.Properties.ContentLength ?? 0,
                item.Properties.ContentType));
        }
        return blobs;
    }
}
