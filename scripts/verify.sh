#!/usr/bin/env bash
set -euo pipefail

echo "=== Verification ==="

echo "1. Docker Compose services:"
docker compose ps

echo -e "\n2. API health (http://localhost:5001/health):"
curl -sf http://localhost:5001/health && echo " OK" || echo " FAILED"

echo -e "\n3. Web health (http://localhost:5002/health):"
curl -sf http://localhost:5002/health && echo " OK" || echo " FAILED"

echo -e "\n4. PostgreSQL connection:"
docker compose exec -T postgres psql -U postgres -d template1 -c "SELECT 1;" >/dev/null 2>&1 && echo " OK" || echo " FAILED"

echo -e "\nVerification complete."
