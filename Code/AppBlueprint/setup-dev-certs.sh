#!/bin/bash

# Create development certificates for each service
dotnet dev-certs https --clean
dotnet dev-certs https --trust

# Create directory for certificates if it doesn't exist
mkdir -p ${APPDATA}/ASP.NET/Https/

# Generate certificates for each service
dotnet dev-certs https -ep "${APPDATA}/ASP.NET/Https/web-service.pfx" -p "dev-cert-password"
dotnet dev-certs https -ep "${APPDATA}/ASP.NET/Https/api-service.pfx" -p "dev-cert-password"
dotnet dev-certs https -ep "${APPDATA}/ASP.NET/Https/app-gateway.pfx" -p "dev-cert-password"

echo "Development certificates have been created and trusted."