#!/bin/sh
set -e

# Default values
API_BASE_URL=${API_BASE_URL:-http://localhost:5000}
SIGNALR_HUB_URL=${SIGNALR_HUB_URL:-${API_BASE_URL}/hubs/updates}
ENVIRONMENT=${ENVIRONMENT:-production}
VERSION=${VERSION:-1.0.0}

# Create config.json from template with environment variable substitution
cat > /usr/share/nginx/html/assets/config.json <<EOF
{
  "apiBaseUrl": "${API_BASE_URL}",
  "signalrHub": "${SIGNALR_HUB_URL}",
  "environment": "${ENVIRONMENT}",
  "version": "${VERSION}",
  "features": {}
}
EOF

echo "Configuration file created:"
cat /usr/share/nginx/html/assets/config.json

# Execute the main command
exec "$@"
