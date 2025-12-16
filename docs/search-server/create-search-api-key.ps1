# Create a fixed search-only API key for Typesense
# This key will be used in the browser client (Typesense.mjs)

# TODO: Replace these values with your actual Railway Typesense details
$RAILWAY_URL = ""
$ADMIN_API_KEY = ""

# Generate a fixed search-only key (or choose your own)
# This value will remain constant - save it for use in Typesense.mjs
$SEARCH_ONLY_KEY = "saas-factory-docs-search-$(Get-Random -Minimum 10000 -Maximum 99999)"

Write-Host "Creating search-only API key..." -ForegroundColor Cyan
Write-Host "Railway URL: $RAILWAY_URL" -ForegroundColor Gray
Write-Host "Search Key Value: $SEARCH_ONLY_KEY" -ForegroundColor Yellow
Write-Host ""

# Create the search-only API key with a fixed value
$body = @{
    value = $SEARCH_ONLY_KEY
    description = "Search only key for SaaS Factory documentation"
    actions = @("documents:search")
    collections = @("*")
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$RAILWAY_URL/keys" -Method Post `
        -Headers @{
            "X-TYPESENSE-API-KEY" = $ADMIN_API_KEY
            "Content-Type" = "application/json"
        } `
        -Body $body

    Write-Host "✓ Success! Search-only API key created:" -ForegroundColor Green
    Write-Host ""
    Write-Host "Key ID: $($response.id)" -ForegroundColor Gray
    Write-Host "Key Value: $($response.value)" -ForegroundColor Yellow
    Write-Host "Description: $($response.description)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Copy this key value: $($response.value)" -ForegroundColor Yellow
    Write-Host "2. Update Typesense.mjs line 64 with this key" -ForegroundColor Gray
    Write-Host "3. Update Typesense.mjs line 59 with: $RAILWAY_URL" -ForegroundColor Gray

} catch {
    Write-Host "✗ Error creating API key:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "- Check that RAILWAY_URL is correct (include https://)" -ForegroundColor Gray
    Write-Host "- Verify ADMIN_API_KEY is correct" -ForegroundColor Gray
    Write-Host "- Ensure your Railway Typesense service is running" -ForegroundColor Gray
}
