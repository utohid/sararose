#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
export DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}"
export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"

cd "$ROOT"
if command -v docker >/dev/null 2>&1; then
  echo "Starting MySQL (Docker)..."
  docker compose up -d
  for i in $(seq 1 40); do
    if docker compose exec -T mysql mysqladmin ping -h 127.0.0.1 -uroot -pSaraRose_Root_2024 --silent >/dev/null 2>&1; then
      break
    fi
    sleep 2
  done
elif mysql -h 127.0.0.1 -u sararose -pSaraRose_Dev_2024 -e "SELECT 1" sararose >/dev/null 2>&1; then
  echo "Using existing local MySQL."
else
  echo "Start MySQL first, or install Docker and run: docker compose up -d" >&2
  exit 1
fi

echo "API: http://127.0.0.1:43124  |  Site: http://127.0.0.1:43123"
cd "$ROOT/backend"
dotnet run --urls "http://127.0.0.1:43124" &
API_PID=$!
cd "$ROOT/frontend"
if [[ ! -d node_modules ]]; then
  npm install
fi
npm start &
UI_PID=$!
trap 'kill $API_PID $UI_PID 2>/dev/null || true' EXIT
wait
