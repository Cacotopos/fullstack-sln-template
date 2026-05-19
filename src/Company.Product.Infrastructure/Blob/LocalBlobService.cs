using Company.Product.Application.Interfaces;

namespace Company.Product.Infrastructure.Blob;

public sealed class LocalBlobService(string basePath) : IBlobService
{
    private readonly string _basePath = Path.GetFullPath(basePath);

    public async Task<Uri?> UploadAsync(string containerName, string blobName, Stream content, string? contentType = null, CancellationToken ct = default)
    {
        var dir = GetContainerPath(containerName);
        Directory.CreateDirectory(dir);

        var fullPath = Path.Combine(dir, blobName);
        using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await content.CopyToAsync(fs, ct);
        return new Uri(fullPath);
    }

    public Task<Stream?> DownloadAsync(string containerName, string blobName, CancellationToken ct = default)
    {
        var fullPath = GetFullPath(containerName, blobName);
        if (!File.Exists(fullPath)) return Task.FromResult<Stream?>(null);
        return Task.FromResult<Stream?>(new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true));
    }

    public Task<bool> DeleteAsync(string containerName, string blobName, CancellationToken ct = default)
    {
        var fullPath = GetFullPath(containerName, blobName);
        if (!File.Exists(fullPath)) return Task.FromResult(false);
        File.Delete(fullPath);
        return Task.FromResult(true);
    }

    public Task<bool> ExistsAsync(string containerName, string blobName, CancellationToken ct = default)
    {
        var fullPath = GetFullPath(containerName, blobName);
        return Task.FromResult(File.Exists(fullPath));
    }

    public Task<IReadOnlyList<BlobInfo>> ListAsync(string containerName, string? prefix = null, CancellationToken ct = default)
    {
        var dir = GetContainerPath(containerName);
        if (!Directory.Exists(dir)) return Task.FromResult<IReadOnlyList<BlobInfo>>([]);

        var blobs = new List<BlobInfo>();
        var files = Directory.GetFiles(dir, prefix + "*", SearchOption.TopDirectoryOnly);
        foreach (var f in files)
        {
            var info = new FileInfo(f);
            var relative = Path.GetRelativePath(dir, f).Replace('\\', '/');
            blobs.Add(new BlobInfo(
                relative,
                new Uri(f),
                info.LastWriteTimeUtc,
                info.Length,
                null));
        }
        return Task.FromResult<IReadOnlyList<BlobInfo>>(blobs);
    }

    private string GetContainerPath(string containerName) => Path.Combine(_basePath, containerName);
    private string GetFullPath(string containerName, string blobName) => Path.Combine(GetContainerPath(containerName), blobName);
}
