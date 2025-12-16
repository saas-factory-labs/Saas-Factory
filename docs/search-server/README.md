# Typesense Search Server Configuration

This directory contains configuration files for setting up Typesense search for the documentation site.

## Directory Structure

```
search-server/
├── typesense-scraper/
│   ├── typesense-scraper.env          # Scraper environment configuration
│   └── typesense-scraper-config.json  # Scraper rules and selectors
└── typesense-server/
    └── Dockerfile                      # Docker image for read-only Typesense server
```

## Setup Steps

### 1. Configure Environment Variables

Edit `typesense-scraper/typesense-scraper.env`:
- Set `TYPESENSE_HOST` to your Railway Typesense URL (without https://)
- Set `TYPESENSE_API_KEY` environment variable with your admin API key

### 2. Configure Scraper

Edit `typesense-scraper/typesense-scraper-config.json`:
- Replace `YOUR_DOMAIN_HERE` with your actual documentation domain
- Adjust selectors if your HTML structure differs from the defaults

### 3. Create Fixed Search-Only API Key

**Important**: Create a search-only API key with a fixed `value` so it remains consistent across deployments:

```bash
curl 'https://YOUR_RAILWAY_URL/keys' -X POST \
  -H "X-TYPESENSE-API-KEY: YOUR_ADMIN_API_KEY" \
  -H 'Content-Type: application/json' \
  -d '{"value":"YOUR_FIXED_SEARCH_KEY","description":"Search only","actions":["documents:search"],"collections":["*"]}'
```

Save the `YOUR_FIXED_SEARCH_KEY` value - this goes in your `Typesense.mjs` component.

### 4. Update Typesense.mjs Component

Edit `docs/RazorPress/RazorPress/wwwroot/mjs/components/Typesense.mjs`:
- Line 60: Replace `YOUR_RAILWAY_TYPESENSE_URL` with your Railway URL
- Line 64: Replace `YOUR_SEARCH_ONLY_API_KEY` with the fixed key from step 3
- Line 59: Update collection name to `saas_factory_docs` (or your chosen name)

## Local Testing

### Run Local Typesense Server

```powershell
# Create data directory
mkdir -p C:\temp\typesense-data

# Run Typesense server
docker run -p 8108:8108 -v C:\temp\typesense-data:/data `
  typesense/typesense:29.0 `
  --data-dir /data --api-key=YOUR_TEMP_KEY --enable-cors
```

### Run Scraper Locally

```powershell
cd docs/search-server/typesense-scraper

# Update .env file with localhost:8108 for local testing

# Run scraper
docker run -it --env-file typesense-scraper.env `
  -e "CONFIG=$(Get-Content typesense-scraper-config.json -Raw | ConvertTo-Json -Compress)" `
  typesense/docsearch-scraper
```

## GitHub Actions Workflow (Future)

For automated index updates, create a GitHub Actions workflow that:

1. Spins up a local Typesense server
2. Scrapes your published docs site
3. Builds a Docker image with the index data baked in
4. Deploys the read-only Docker image to your hosting platform

See [ServiceStack's workflow](https://github.com/ServiceStack/docs/blob/master/.github/workflows/search-index-update.yml) for reference.

## Railway Deployment

For Railway, you can:

1. Deploy Typesense directly as a service
2. Use persistent volumes for the data directory
3. Configure environment variables in Railway dashboard
4. Run the scraper manually or via GitHub Actions

## Notes

- Collection name: `saas_factory_docs` (configurable in scraper config)
- The scraper creates collections with timestamps, e.g., `saas_factory_docs_1234567890`
- Search query uses: `hierarchy.lvl0,hierarchy.lvl1,content,hierarchy.lvl2,hierarchy.lvl3`
- Results are grouped by: `hierarchy.lvl0` (page title)
