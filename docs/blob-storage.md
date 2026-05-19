# Blob Storage

The template supports file uploads and blob storage via:

- **Azure Blob Storage** (production)
- **Local filesystem** (development fallback)

## Configuration

### Environment Variables

```env
# Azure Blob Storage (optional; if omitted, uses local filesystem)
CONNECTION_STRINGS__AzureBlob=DefaultEndpointsProtocol=https;AccountName=mystorageaccount;AccountKey=myaccountkey;EndpointSuffix=core.windows.net
```

### Azure Setup

1. Create a Storage Account in Azure Portal
2. Under **Access keys**, copy a connection string
3. Add the connection string to `CONNECTION_STRINGS__AzureBlob`
4. Optionally configure CORS for direct browser uploads

### Local Development

If `CONNECTION_STRINGS__AzureBlob` is empty or missing, the app uses a local `blobs/` folder:

```
src/Company.Product.Api/bin/Debug/net8.0/blobs/
  uploads/
    abc123.png
    def456.pdf
  backups/
    backup-2024-03-29.zip
```

## API Endpoints

All endpoints require authentication (`Authorization: Bearer <jwt>`).

### Upload

```http
POST /api/v1/blobs/{containerName}
Content-Type: multipart/form-data

--boundary
Content-Disposition: form-data; name="file"; filename="photo.png"
Content-Type: image/png

<binary data>
--boundary--
```

Response:

```json
{
  "uri": "https://myaccount.blob.core.windows.net/uploads/abc123.png",
  "blobName": "abc123.png"
}
```

### Download

```http
GET /api/v1/blobs/{containerName}/{blobName}
```

Returns the file with `Content-Type: application/octet-stream` (or original type if stored).

### Delete

```http
DELETE /api/v1/blobs/{containerName}/{blobName}
```

Returns `204 No Content` on success.

### List

```http
GET /api/v1/blobs/{containerName}?prefix=2024-03
```

Response:

```json
[
  {
    "name": "2024-03-29-photo.png",
    "uri": "https://myaccount.blob.core.windows.net/uploads/2024-03-29-photo.png",
    "lastModified": "2024-03-29T12:00:00Z",
    "size": 12345,
    "contentType": "image/png"
  }
]
```

## Usage in Code

```csharp
public class MyService(IBlobService blobService)
{
    public async Task<Uri?> SaveUserAvatarAsync(string userId, Stream avatar)
    {
        var blobName = $"{userId}/avatar.png";
        return await blobService.UploadAsync("avatars", blobName, avatar, "image/png");
    }

    public async Task<Stream?> GetAvatarAsync(string userId)
    {
        var blobName = $"{userId}/avatar.png";
        return await blobService.DownloadAsync("avatars", blobName);
    }
}
```

## Container Naming

- Use lowercase, alphanumeric, hyphens only.
- Examples: `uploads`, `avatars`, `documents`, `backups`.
- Avoid dots; Azure allows them but they can cause issues.

## Security

- Files are private by default (`PublicAccessType.None`).
- Access is gated by JWT auth in the API.
- Consider generating SAS tokens for direct client uploads/downloads in production.

## Limits

- Azure Blob: 5,000 MB per blob (default), 500 TB per account.
- Local: limited by disk space.
- Recommended max file size: 100 MB for API uploads; larger files should use SAS or direct uploads.

## Testing

```bash
# Upload
curl -X POST "https://localhost:5001/api/v1/blobs/uploads" \
  -H "Authorization: Bearer $TOKEN" \
  -F "file=@/path/to/file.png"

# Download
curl -H "Authorization: Bearer $TOKEN" \
  "https://localhost:5001/api/v1/blobs/uploads/<blob-name>" \
  --output downloaded.png

# List
curl -H "Authorization: Bearer $TOKEN" \
  "https://localhost:5001/api/v1/blobs/uploads"
```

## Monitoring

- Enable Azure Storage logging and metrics in production.
- Consider Azure Monitor alerts for high error rates or throttling.

## Alternatives

- For S3-compatible services, replace `AzureBlobService` with an S3 client.
- For CDN, serve files via Azure CDN after upload.
