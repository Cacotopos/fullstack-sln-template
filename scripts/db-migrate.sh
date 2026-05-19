#!/usr/bin/env bash
set -euo pipefail

echo "=== Database Migration ==="
echo "Running EF Core database update..."

dotnet ef database update \
    --project src/Company.Product.Infrastructure \
    --startup-project src/Company.Product.Api

echo "Migration complete."
