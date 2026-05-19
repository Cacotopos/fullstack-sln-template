# Logging with Serilog and Seq

This template uses Serilog for structured logging with Seq as a centralized log server.

## Quick Start

```bash
# Start Seq (Docker)
docker compose up -d seq

# Run any service (API, Web, Worker)
dotnet run --project src/Company.Product.Api

# Open Seq UI
open http://localhost:5341
```

## Architecture

- **Serilog**: Provides structured logging with sinks and enrichers
- **Seq**: Ingests, indexes, and provides a UI for searching logs
- **All services** (API, Web, Worker) send logs to Seq

## Configuration

### Environment Variables

```env
Seq__ServerUrl=http://localhost:5341
```

### appsettings.json

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://localhost:5341"
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"]
  }
}
```

## Using Serilog in Code

### Basic Logging

```csharp
public class MyService
{
    private readonly ILogger<MyService> _log;
    public MyService(ILogger<MyService> log) => _log = log;

    public void DoWork()
    {
        _log.LogInformation("Starting work at {Time}", DateTimeOffset.UtcNow);
        _log.LogWarning("Something might be wrong");
        _log.LogError(ex, "Failed to process item {ItemId}", itemId);
    }
}
```

### Structured Logging

```csharp
_log.LogInformation("User {UserId} uploaded file {FileName} ({Size} bytes)", userId, fileName, size);
```

### Enrichers

- `FromLogContext`: Adds `LogContext` properties
- `WithMachineName`: Adds machine name
- `WithThreadId`: Adds thread ID

Add contextual data:

```csharp
using (LogContext.PushProperty("UserId", userId))
{
    _log.LogInformation("Action performed");
}
```

## Seq Features

- **Search**: Full-text and property-based search
- **Filtering**: By level, message, properties
- **Dashboards**: Pin useful queries
- **Alerts**: Email/webhook on matches
- **Retention**: Configurable per-environment

### Example Queries

```
@Level = "Error"
@Message like "Failed%"
Contains(@UserId, "abc123")
@Timestamp > 2024-03-29T12:00:00Z
```

## Docker Compose

```yaml
services:
  seq:
    image: datalust/seq:latest
    environment:
      - ACCEPT_EULA=Y
    ports:
      - "5341:80"
    volumes:
      - seq_data:/data
```

## Production Considerations

- **Retention**: Set retention policies in Seq
- **API Keys**: Use Seq API keys for ingestion
- **HTTPS**: Put Seq behind a reverse proxy with TLS
- **Resource Limits**: Configure memory/CPU limits
- **Backups**: Backup `seq_data` volume

## Alternative Sinks

You can add additional sinks in `appsettings.json`:

```json
"WriteTo": [
  { "Name": "Console" },
  { "Name": "File", "Args": { "path": "logs/log-.txt" } },
  { "Name": "Seq", "Args": { "serverUrl": "http://localhost:5341" } }
]
```

## Performance

- Use async logging where possible
- Filter noisy namespaces (`Microsoft.*`, `System.*`)
- Avoid logging large objects; log identifiers instead
- Use `LogContext` for request/user correlation

## Troubleshooting

- **Seq not receiving logs**: Verify `Seq__ServerUrl` and network access
- **High memory usage**: Reduce minimum level or filter noisy logs
- **Missing properties**: Ensure `Enrich` and `LogContext` are configured

## Further Reading

- [Serilog Wiki](https://github.com/serilog/serilog/wiki)
- [Seq Documentation](https://docs.datalust.co/docs)
