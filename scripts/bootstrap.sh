#!/usr/bin/env bash
set -euo pipefail

echo "=== Bootstrap: FullStack Template ==="
echo "Step 1: Start PostgreSQL via Docker Compose"
docker compose up -d postgres

sleep 5

echo "Step 2: Run EF Core migrations (API project context)"
dotnet ef database update \
    --project src/Company.Product.Infrastructure \
    --startup-project src/Company.Product.Api

echo "Step 3: Seed data (optional placeholder)"
# TODO: Add seeding command or script here

echo "Step 4: Verify services"
echo "PostgreSQL: docker compose ps postgres"
echo "API health: curl -f http://localhost:5001/health || echo 'API not running yet'"

echo "Bootstrap complete. You can now start API, Web, and Worker services."
