# HTTPS Certificate Setup

This document explains how to set up self-signed certificates for HTTPS in the AppBlueprint project.

## Windows Setup

1. Run the PowerShell script to create and trust development certificates:

```powershell
.\setup-dev-certs.ps1
```

This script will:
- Clean any existing dev certificates
- Create a new trusted development certificate
- Generate specific certificates for each service
- Place certificates in `%APPDATA%\ASP.NET\Https\` with password `dev-cert-password`

## What Changed

The web application has been configured to:

1. Listen on both HTTP (port 80) and HTTPS (port 443)
2. Use the self-signed certificate from `%APPDATA%\ASP.NET\Https\web-service.pfx`
3. Enable HTTPS redirection for secure connections

## Troubleshooting

If you still see certificate errors:

1. Verify the certificate was created successfully in `%APPDATA%\ASP.NET\Https\`
2. Check that you've run the application as the same user who installed the certificates
3. Try restarting your browser to clear any cached certificate errors
4. If using Docker, ensure the certificates are properly mounted to containers

## Note

For production deployments, you should replace these self-signed certificates with properly issued certificates from a trusted certificate authority.