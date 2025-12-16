# Docker Development Guide

## Prerequisites
- Docker and Docker Compose installed
- .NET SDK 9.0 or later
- Git

## Initial Setup

1. Clone the repository and navigate to the AppBlueprint directory
2. Copy `.env.example` to `.env` and fill in your values
3. Run the certificate setup script:
   ```bash
   ./setup-dev-certs.sh
   ```

## Development Environment

The development environment is configured with hot reload and debugging capabilities.

### Development URLs
- Web Service: 
  - HTTP: http://localhost:8080
  - HTTPS: https://localhost:8081
- API Service:
  - HTTP: http://localhost:8082
  - HTTPS: https://localhost:8083
- Gateway Service:
  - HTTP: http://localhost:8084
  - HTTPS: https://localhost:8085
- Database: localhost:5432

### Commands
Use the docker-manage.sh script for common operations:
```bash
# Start development environment
./docker-manage.sh dev-up

# View logs
./docker-manage.sh logs

# Stop development environment
./docker-manage.sh dev-down
```

## Production Environment

The production environment is configured with proper SSL handling, health checks, and container orchestration.

### Production Setup Requirements
1. SSL certificates for your domains
2. Environment variables set in `.env`:
   - WEB_DOMAIN
   - API_DOMAIN
   - GATEWAY_DOMAIN
   - SSL_CERTIFICATE_PATH
   - SSL_KEY_PATH
   - DB_NAME
   - DB_USER
   - DB_PASSWORD

### Commands
```bash
# Start production environment
./docker-manage.sh prod-up

# Stop production environment
./docker-manage.sh prod-down
```

## Troubleshooting

1. Certificate Issues
   - Ensure certificates are properly mounted
   - Check certificate permissions
   - Verify certificate paths in .env file

2. Database Connection Issues
   - Check if database container is running
   - Verify database credentials in .env file
   - Check network connectivity between services

3. Hot Reload Not Working
   - Ensure volume mounts are correct
   - Check if the correct Docker Compose override file is being used
   - Verify file permissions in mounted volumes

## Best Practices

1. Never commit sensitive information in Docker files
2. Use environment variables for configuration
3. Keep development and production configurations separate
4. Regularly update base images and dependencies
5. Monitor container logs for issues
6. Use health checks to ensure service availability