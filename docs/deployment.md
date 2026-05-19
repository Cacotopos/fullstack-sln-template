# Deployment Guide

Complete deployment guide for all environments: Local, Development, Pre-Production, and Production.

---

## Table of Contents

1. [Local Development](#local-development)
2. [Development Environment](#development-environment)
3. [Pre-Production Environment](#pre-production-environment)
4. [Production Environment](#production-environment)
5. [Minimal Cost Hosting](#minimal-cost-hosting)

---

## Local Development

### Prerequisites

- .NET SDK 8.0 (see `global.json`)
- Docker Desktop or Docker Engine
- PostgreSQL (via Docker)

### Quick Start

```bash
# 1. Copy environment file
cp .env.example .env

# 2. Start PostgreSQL and Seq
docker compose up -d

# 3. Run migrations
dotnet ef database update \
  --project src/Company.Product.Infrastructure \
  --startup-project src/Company.Product.Api

# 4. Run API
cd src/Company.Product.Api
dotnet run

# 5. Run Web (separate terminal)
cd src/Company.Product.Web
dotnet run

# 6. Run Worker (separate terminal)
cd src/Company.Product.Worker
dotnet run
```

### Verification

- API: https://localhost:5001/health
- Web: https://localhost:5002
- Swagger: https://localhost:5001/swagger
- Seq: http://localhost:5341

### Local Testing Checklist

- [ ] Health endpoint returns OK
- [ ] Database connects successfully
- [ ] Web page loads
- [ ] Authentication works (login/logout)
- [ ] Sample CRUD operations work
- [ ] Blob upload/download works
- [ ] Logs appear in Seq

---

## Development Environment

Use for integration checks and feature validation before promoting to pre-production.

### Docker Compose Deployment

```bash
# 1. Set environment
cp .env.example .env.dev
# Edit .env.dev with dev secrets

# 2. Build and run
docker compose -f docker-compose.dev.yml --env-file .env.dev up -d

# 3. Run migrations
docker compose -f docker-compose.dev.yml exec api \
  dotnet ef database update --project src/Company.Product.Infrastructure

# 4. Verify
curl -f https://dev-api.example.com/health
```

### Manual Deployment

```bash
# 1. Build
dotnet build -c Release

# 2. Run migrations
dotnet ef database update \
  --project src/Company.Product.Infrastructure \
  --startup-project src/Company.Product.Api \
  --configuration Release

# 3. Publish
dotnet publish src/Company.Product.Api -c Release -o ./publish/api
dotnet publish src/Company.Product.Web -c Release -o ./publish/web
dotnet publish src/Company.Product.Worker -c Release -o ./publish/worker

# 4. Transfer to server and run
```

### Environment Variables (.env.dev)

```env
ASPNETCORE_ENVIRONMENT=Development
CONNECTION_STRINGS__Default=Host=dev-db;Port=5432;Database=productdb_dev;Username=devuser;Password=***
Jwt__Key=dev-key-32-chars-minimum
Auth__Google__ClientId=dev-google-client-id
Auth__Google__ClientSecret=***
```

### Dev Checklist

- [ ] Build succeeds
- [ ] Migrations applied
- [ ] API and Web deployed
- [ ] Smoke tests pass
- [ ] Webhooks working
- [ ] Health checks pass

---

## Pre-Production Environment

Mirror of production for final validation and load testing.

### Docker Compose Deployment

```bash
# 1. Set environment
cp .env.example .env.preprod
# Edit .env.preprod with preprod secrets

# 2. Build and run
docker compose -f docker-compose.preprod.yml --env-file .env.preprod up -d

# 3. Run migrations
docker compose -f docker-compose.preprod.yml exec api \
  dotnet ef database update --project src/Company.Product.Infrastructure

# 4. Run smoke tests
./scripts/verify.sh preprod
```

### Environment Variables (.env.preprod)

```env
ASPNETCORE_ENVIRONMENT=Staging
CONNECTION_STRINGS__Default=Host=preprod-db;Port=5432;Database=productdb_preprod;...
CONNECTION_STRINGS__ReadOnly=Host=preprod-db-readonly;Port=5432;Database=productdb_preprod;...
Jwt__Key=preprod-key-32-chars-min
Auth__Google__ClientId=preprod-google-client-id
Auth__Google__ClientSecret=***
STRIPE__PUBLISHABLE_KEY=pk_test_...
STRIPE__SECRET_KEY=sk_test_...
```

### Pre-Prod Checklist

- [ ] Confirm release candidate approval
- [ ] Snapshot database
- [ ] Apply migrations
- [ ] Deploy artifact
- [ ] Run full smoke test suite
- [ ] Load testing
- [ ] Security scan
- [ ] Verify all integrations
- [ ] Confirm with stakeholders

### Load Testing

```bash
# Example using k6
k6 run --vus 100 --duration 30s load-tests/api-test.js
```

---

## Production Environment

Controlled, repeatable, rollback-friendly production deployment.

### Docker Compose Deployment

```bash
# 1. Set environment
cp .env.example .env.prod
# Edit .env.prod with production secrets

# 2. Build and run
docker compose -f docker-compose.prod.yml --env-file .env.prod up -d

# 3. Run migrations
docker compose -f docker-compose.prod.yml exec api \
  dotnet ef database update --project src/Company.Product.Infrastructure

# 4. Verify
./scripts/verify.sh prod
```

### Environment Variables (.env.prod)

```env
ASPNETCORE_ENVIRONMENT=Production
CONNECTION_STRINGS__Default=Host=prod-db;Port=5432;Database=productdb;...
CONNECTION_STRINGS__ReadOnly=Host=prod-db-readonly;Port=5432;Database=productdb;...
Jwt__Key=prod-key-32-chars-minimum-secure
Auth__Google__ClientId=prod-google-client-id
Auth__Google__ClientSecret=***
Auth__Microsoft__ClientId=prod-ms-client-id
Auth__Microsoft__ClientSecret=***
STRIPE__PUBLISHABLE_KEY=pk_live_...
STRIPE__SECRET_KEY=sk_live_...
STRIPE__WEBHOOK_SECRET=whsec_...
STRIPE__PRICE_ID_BASIC=price_...
STRIPE__PRICE_ID_PRO=price_...
```

### Production Checklist

- [ ] Confirm release approval
- [ ] Back up database
- [ ] Apply migrations
- [ ] Deploy promoted artifact
- [ ] Run smoke checks
- [ ] Verify health endpoints
- [ ] Verify auth flows
- [ ] Verify webhook endpoints
- [ ] Monitor logs and metrics
- [ ] Notify stakeholders

### Smoke Tests

```bash
# API health
curl -f https://api.example.com/health

# Web health
curl -f https://example.com/health

# Auth flow (manual)
# - Navigate to https://example.com
# - Sign in with Google
# - Create a Sample Item
# - Delete the Sample Item

# Stripe flow (manual)
# - Navigate to /billing/plans
# - Initiate checkout
# - Verify webhook receipt
# - Cancel subscription

# Localization
curl -f https://example.com?culture=fr

# DB connectivity
docker compose -f docker-compose.prod.yml exec postgres \
  psql -U postgres -d productdb -c "SELECT COUNT(*) FROM SampleItems;"
```

### Rollback Procedure

```bash
# Docker rollback
docker compose -f docker-compose.prod.yml down
docker compose -f docker-compose.prod.yml up -d --force-recreate

# Database rollback (if safe)
dotnet ef database update <previous-migration> \
  --project src/Company.Product.Infrastructure \
  --startup-project src/Company.Product.Api

# Restore config from versioned deployment records
```

---

## Minimal Cost Hosting

For detailed minimal-cost hosting options ($10-25/month), see [hosting-minimal-cost.md](hosting-minimal-cost.md).

### Quick Summary

**Recommended Stack:**
- **Database:** Supabase PostgreSQL (Free tier: 500MB)
- **API:** Railway ($5/month)
- **Worker:** Railway ($5/month)
- **Web:** Railway ($5/month)
- **Blob Storage:** Supabase Storage (Free tier: 1GB)
- **Logging:** Papertrail (Free)

**Total: ~$15/month**

See the full guide for detailed setup instructions, environment configuration, and scaling considerations.

---

## Common Tasks

### Running Migrations

```bash
# Local
dotnet ef database update \
  --project src/Company.Product.Infrastructure \
  --startup-project src/Company.Product.Api

# Remote (with connection string)
dotnet ef database update \
  --project src/Company.Product.Infrastructure \
  --startup-project src/Company.Product.Api \
  --connection "Host=remote-db;Port=5432;Database=productdb;Username=user;Password=***"
```

### Creating Migrations

```bash
dotnet ef migrations add MigrationName \
  --project src/Company.Product.Infrastructure \
  --startup-project src/Company.Product.Api
```

### Database Backup

```bash
# PostgreSQL backup
pg_dump -h hostname -U username -d database > backup.sql

# Restore
psql -h hostname -U username -d database < backup.sql
```

### Viewing Logs

```bash
# Docker logs
docker compose logs -f api
docker compose logs -f web
docker compose logs -f worker

# Seq (local)
# Open http://localhost:5341
```

---

## Troubleshooting

### Connection Issues

**Database connection fails:**
- Verify connection string in environment variables
- Check PostgreSQL is running: `docker compose ps`
- Check network connectivity
- Verify credentials

**API not accessible:**
- Check firewall rules
- Verify port bindings
- Check HTTPS certificates
- Review logs for startup errors

### Migration Issues

**Migration fails:**
- Ensure database is accessible
- Check for conflicting migrations
- Verify EF Core tools are installed
- Run with `--verbose` flag for details

### Authentication Issues

**OAuth login fails:**
- Verify redirect URIs in provider console
- Check client ID and secret
- Ensure HTTPS is enabled
- Review OAuth provider logs

### Performance Issues

**Slow queries:**
- Check database indexes
- Review query execution plans
- Enable query logging
- Consider read replicas

**High memory usage:**
- Check for memory leaks
- Review connection pooling settings
- Monitor with dotnet-counters
- Adjust container limits

---

## Security Best Practices

- [ ] Use HTTPS everywhere
- [ ] Rotate secrets regularly
- [ ] Use strong JWT keys (32+ characters)
- [ ] Enable database SSL
- [ ] Configure CORS properly
- [ ] Use environment variables for secrets
- [ ] Enable rate limiting
- [ ] Set up Web Application Firewall
- [ ] Regular security updates
- [ ] Monitor for vulnerabilities

---

## Monitoring & Alerts

### Health Checks

- API: `/health`
- Web: `/health`
- Database: Connection test
- Blob Storage: Upload test

### Metrics to Monitor

- Request rate and latency
- Error rate
- Database connection pool
- Memory and CPU usage
- Disk space
- Queue depth (Worker)

### Recommended Tools

- **Logs:** Seq, Papertrail, or Datadog
- **Metrics:** Railway dashboard, Grafana
- **Uptime:** UptimeRobot, Pingdom
- **APM:** Application Insights, New Relic

---

## Support

For additional deployment scenarios and platform-specific guides:
- [Minimal Cost Hosting](hosting-minimal-cost.md) - Railway, Supabase, Netlify
- [Architecture](ARCHITECTURE.md) - System design and patterns
- [Stripe Payments](stripe-payments.md) - Payment integration
- [Blob Storage](blob-storage.md) - File storage configuration
