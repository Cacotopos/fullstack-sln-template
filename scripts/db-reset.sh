#!/usr/bin/env bash
set -euo pipefail

echo "=== Database Reset ==="
echo "Dropping and recreating database..."

dotnet ef database drop \
    --project src/Company.Product.Infrastructure \
    --startup-project src/Company.Product.Api \
    --force

dotnet ef database update \
    --project src/Company.Product.Infrastructure \
    --startup-project src/Company.Product.Api

echo "Reset complete."
