namespace Company.Product.Application.Interfaces;

public interface IBlobService
{
    Task<Uri?> UploadAsync(string containerName, string blobName, Stream content, string? contentType = null, CancellationToken ct = default);
    Task<Stream?> DownloadAsync(string containerName, string blobName, CancellationToken ct = default);
    Task<bool> DeleteAsync(string containerName, string blobName, CancellationToken ct = default);
    Task<bool> ExistsAsync(string containerName, string blobName, CancellationToken ct = default);
    Task<IReadOnlyList<BlobInfo>> ListAsync(string containerName, string? prefix = null, CancellationToken ct = default);
}

public sealed record BlobInfo(string Name, Uri Uri, DateTimeOffset LastModified, long Size, string? ContentType);
