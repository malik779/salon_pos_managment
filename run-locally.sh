#!/usr/bin/env bash
set -euo pipefail

COMPOSE_FILE=${COMPOSE_FILE:-docker-compose.dev.yml}
PROJECT_NAME=${PROJECT_NAME:-salon}

if ! command -v docker >/dev/null 2>&1; then
  echo "Docker is required to run the local stack." >&2
  exit 1
fi

if ! docker compose version >/dev/null 2>&1; then
  echo "Docker Compose v2 is required (docker compose plugin)." >&2
  exit 1
fi

echo "Starting Salon platform stack using ${COMPOSE_FILE}..."
docker compose -f "${COMPOSE_FILE}" -p "${PROJECT_NAME}" up --build "$@"
