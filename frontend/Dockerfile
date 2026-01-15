# syntax=docker/dockerfile:1.4

# Stage 1: Build Angular application
FROM node:22-alpine AS build

WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci --legacy-peer-deps

# Copy source code
COPY . .

# Build Angular application for production
RUN npm run build:prod

# Stage 2: Runtime with Nginx
FROM nginx:alpine

# Copy built application from build stage
COPY --from=build /app/dist/salon-pos-web /usr/share/nginx/html

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy entrypoint script for runtime configuration
COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh && \
    sed -i 's/\r$//' /docker-entrypoint.sh

# Create directory for config.json
RUN mkdir -p /usr/share/nginx/html/assets

# Expose port 80
EXPOSE 80

# Use custom entrypoint to handle environment variable substitution
ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]
