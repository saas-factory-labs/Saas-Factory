# PostgreSQL Full-Text Search Integration Guide

This guide explains how to integrate PostgreSQL full-text search into your SaaS application using the AppBlueprint framework.

## 📋 Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Step-by-Step Integration](#step-by-step-integration)
- [Configuration Examples](#configuration-examples)
- [Usage Examples](#usage-examples)
- [Advanced Features](#advanced-features)
- [Performance Optimization](#performance-optimization)
- [Troubleshooting](#troubleshooting)

---

## Overview

The PostgreSQL full-text search implementation provides:

- ✅ **Zero-cost search** - Uses your existing PostgreSQL database
- ✅ **Automatic tenant isolation** - Respects Row-Level Security (RLS) and global query filters
- ✅ **ACID guarantees** - Search results always in sync with data
- ✅ **Multi-language support** - Supports English, Spanish, French, and 20+ other languages
- ✅ **High performance** - GIN indexes provide subsecond search for 100k-1M records per tenant
- ✅ **Clean architecture** - Follows Domain → Application → Infrastructure separation

**When to use this:**
- MVP/early-stage product (minimize infrastructure complexity)
- Dataset < 1M documents per tenant
- Budget-conscious (no external search service costs)
- Strong multi-tenancy requirements

**When to consider alternatives (Algolia/Typesense):**
- Search is core product feature
- Need typo tolerance and advanced autocomplete
- Individual tenants exceed 1M searchable records
- Global user base requiring CDN-level performance

---

## Prerequisites

- PostgreSQL 12+ (already configured in AppBlueprint)
- Entity Framework Core 10.0+ (already included)
- Npgsql.EntityFrameworkCore.PostgreSQL 10.0+ (already included)
- Your entity must inherit from `BaseEntity` or similar

---

## Step-by-Step Integration

### Step 1: Update Entity Configuration

Add a computed `SearchVector` column to your entity configuration. This column automatically updates when your entity data changes.

**Example: Making `ProductEntity` searchable**

```csharp
// Infrastructure/DatabaseContexts/YourFeature/Entities/EntityConfigurations/ProductEntityConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // ... existing configuration ...

        // ========================================
        // Full-Text Search Configuration
        // ========================================

        // Add computed tsvector column
        // Choose which fields to index (name, description, tags, etc.)
        builder.Property<string>("SearchVector")
            .HasComputedColumnSql(
                "to_tsvector('english', coalesce(\"Name\", '') || ' ' || coalesce(\"Description\", '') || ' ' || coalesce(\"Category\", ''))",
                stored: true)
            .HasComment("Full-text search vector for product search");

        // Create GIN index for fast full-text search
        builder.HasIndex("SearchVector")
            .HasMethod("GIN")
            .HasDatabaseName("IX_Products_SearchVector");
    }
}
```

**Field Selection Tips:**
- **Include:** User-facing text fields (name, description, tags, notes)
- **Include:** Searchable identifiers (SKU, reference numbers)
- **Exclude:** Sensitive data (passwords, tokens, API keys)
- **Exclude:** Binary data, dates, numeric IDs
- **Exclude:** Foreign keys and technical fields

**Language Options:**
```sql
-- English (default)
to_tsvector('english', ...)

-- Spanish
to_tsvector('spanish', ...)

-- French
to_tsvector('french', ...)

-- Portuguese
to_tsvector('portuguese', ...)

-- Multi-language (searches all, but less precise)
to_tsvector('simple', ...)
```

---

### Step 2: Generate Database Migration

After updating your entity configuration, generate an EF Core migration:

```powershell
# Navigate to your DbContext project
cd Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure

# Generate migration for your specific DbContext
dotnet ef migrations add AddProductFullTextSearch --context B2BDbContext --output-dir DatabaseContexts/B2B/Migrations

# Apply migration to database
dotnet ef database update --context B2BDbContext
```

**Verify migration SQL:**
```sql
-- The migration should create:

-- 1. Add computed column
ALTER TABLE "Products" 
ADD COLUMN "SearchVector" tsvector 
GENERATED ALWAYS AS (
    to_tsvector('english', 
        coalesce("Name", '') || ' ' || 
        coalesce("Description", '') || ' ' || 
        coalesce("Category", '')
    )
) STORED;

-- 2. Create GIN index
CREATE INDEX "IX_Products_SearchVector" 
ON "Products" USING GIN("SearchVector");
```

---

### Step 3: Register Search Service

Add the search service to your DI container in `Program.cs` or your API startup:

```csharp
// AppBlueprint.ApiService/Program.cs
using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;
using AppBlueprint.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register search services for each searchable entity
builder.Services.AddPostgreSqlFullTextSearch<UserEntity, B2BDbContext>();
builder.Services.AddPostgreSqlFullTextSearch<TenantEntity, BaselineDbContext>();
builder.Services.AddPostgreSqlFullTextSearch<ProductEntity, B2BDbContext>();

// ... rest of your services ...
```

**Important:** Use the correct `DbContext` for each entity:
- `BaselineDbContext` - Core entities (Users, Tenants, Emails)
- `B2BDbContext` - B2B-specific entities (Organizations, Teams, Products)
- `B2CDbContext` - B2C-specific entities (Profiles, Dating features)
- Your custom `DbContext` - Feature-specific entities

---

### Step 4: Inject and Use in Controllers

**API Controller Example:**

```csharp
// AppBlueprint.ApiService/Controllers/ProductSearchController.cs
using AppBlueprint.Application.Interfaces;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.ApiService.Controllers;

[Authorize]
[ApiController]
[Route("api/products/search")]
public sealed class ProductSearchController : ControllerBase
{
    private readonly ISearchService<ProductEntity> _searchService;
    private readonly ILogger<ProductSearchController> _logger;

    public ProductSearchController(
        ISearchService<ProductEntity> searchService,
        ILogger<ProductSearchController> logger)
    {
        ArgumentNullException.ThrowIfNull(searchService);
        ArgumentNullException.ThrowIfNull(logger);

        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// Searches products using full-text search.
    /// Automatically scoped to the authenticated user's tenant.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string q,           // Search query
        [FromQuery] int page = 1,       // Page number (1-based)
        [FromQuery] int pageSize = 20,  // Results per page
        [FromQuery] string? category = null,
        [FromQuery] bool? inStock = null)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Search query is required");

        // Build search query
        var searchQuery = new SearchQuery
        {
            QueryText = q,
            PageNumber = page,
            PageSize = Math.Min(pageSize, 100), // Max 100 results per page
            MinRelevanceScore = 0.1f // Filter low-relevance results
        };

        // Add optional filters
        if (!string.IsNullOrEmpty(category))
            searchQuery.Filters["Category"] = category;

        if (inStock.HasValue)
            searchQuery.Filters["InStock"] = inStock.Value;

        try
        {
            SearchResult<ProductEntity> results = await _searchService.SearchAsync(searchQuery);

            _logger.LogInformation(
                "Product search completed: query={Query}, results={Count}, time={Time}ms",
                q, results.TotalCount, results.ExecutionTimeMs);

            return Ok(new
            {
                query = results.Query,
                totalCount = results.TotalCount,
                pageNumber = results.PageNumber,
                pageSize = results.PageSize,
                totalPages = results.TotalPages,
                hasNextPage = results.HasNextPage,
                hasPreviousPage = results.HasPreviousPage,
                executionTimeMs = results.ExecutionTimeMs,
                items = results.Items.Select(item => new
                {
                    id = item.Entity.Id,
                    name = item.Entity.Name,
                    description = item.Entity.Description,
                    category = item.Entity.Category,
                    price = item.Entity.Price,
                    relevanceScore = item.RelevanceScore,
                    matchedTerms = item.MatchedTerms
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Product search failed: query={Query}", q);
            return StatusCode(500, "Search failed. Please try again.");
        }
    }
}
```

---

## Configuration Examples

### Basic Search (Simple Text)

```csharp
var query = new SearchQuery
{
    QueryText = "wireless headphones", // Searches for products containing both words
    PageSize = 20,
    PageNumber = 1
};

var results = await _searchService.SearchAsync(query);
```

### Search with Filters

```csharp
var query = new SearchQuery
{
    QueryText = "laptop",
    PageSize = 20,
    PageNumber = 1,
    Filters = new Dictionary<string, object>
    {
        ["Category"] = "Electronics",
        ["InStock"] = true,
        ["IsActive"] = true
    }
};

var results = await _searchService.SearchAsync(query);
```

### Advanced Search Operators

PostgreSQL supports boolean operators in search queries:

```csharp
// AND operator (both words must be present)
QueryText = "laptop & gaming"  // or "laptop gaming" (implicit AND)

// OR operator (either word can be present)
QueryText = "laptop | desktop"

// NOT operator (exclude words)
QueryText = "laptop & !refurbished"

// Phrase search (exact phrase)
QueryText = "gaming laptop"

// Complex query
QueryText = "(gaming | professional) & laptop & !refurbished"
```

### Pagination Example

```csharp
// Page 1
var page1Query = new SearchQuery
{
    QueryText = "laptop",
    PageNumber = 1,
    PageSize = 20
};
var page1Results = await _searchService.SearchAsync(page1Query);

// Check if there's a next page
if (page1Results.HasNextPage)
{
    // Fetch page 2
    var page2Query = new SearchQuery
    {
        QueryText = "laptop",
        PageNumber = 2,
        PageSize = 20
    };
    var page2Results = await _searchService.SearchAsync(page2Query);
}
```

### Relevance Filtering

```csharp
var query = new SearchQuery
{
    QueryText = "laptop",
    MinRelevanceScore = 0.3f // Only return results with 30%+ relevance
};

var results = await _searchService.SearchAsync(query);
```

---

## Usage Examples

### Example 1: User Search (Admin Panel)

```csharp
public sealed class UserSearchService
{
    private readonly ISearchService<UserEntity> _searchService;

    public async Task<List<UserEntity>> SearchUsersAsync(
        string searchText,
        bool activeOnly = true)
    {
        var query = new SearchQuery
        {
            QueryText = searchText,
            PageSize = 50,
            PageNumber = 1,
            Filters = activeOnly 
                ? new Dictionary<string, object> { ["IsActive"] = true }
                : new Dictionary<string, object>()
        };

        SearchResult<UserEntity> results = await _searchService.SearchAsync(query);
        
        return results.Items
            .Select(item => item.Entity)
            .ToList();
    }
}
```

### Example 2: Tenant Search (Super Admin)

```csharp
public sealed class TenantSearchService
{
    private readonly ISearchService<TenantEntity> _searchService;

    public async Task<SearchResult<TenantEntity>> SearchTenantsAsync(
        string companyName,
        TenantType? tenantType = null,
        int page = 1)
    {
        var query = new SearchQuery
        {
            QueryText = companyName,
            PageSize = 25,
            PageNumber = page
        };

        if (tenantType.HasValue)
            query.Filters["TenantType"] = tenantType.Value;

        return await _searchService.SearchAsync(query);
    }
}
```

### Example 3: Autocomplete/Typeahead

```csharp
[HttpGet("autocomplete")]
public async Task<IActionResult> Autocomplete([FromQuery] string q)
{
    if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
        return Ok(Array.Empty<string>());

    var query = new SearchQuery
    {
        QueryText = q,
        PageSize = 10, // Limit to top 10 suggestions
        PageNumber = 1
    };

    SearchResult<ProductEntity> results = await _searchService.SearchAsync(query);

    // Return just the names for autocomplete
    var suggestions = results.Items
        .Select(item => item.Entity.Name)
        .Distinct()
        .ToList();

    return Ok(suggestions);
}
```

### Example 4: Multi-Field Search Results

```csharp
public async Task<SearchResult<ProductEntity>> SearchProductsWithHighlightsAsync(string searchText)
{
    var query = new SearchQuery
    {
        QueryText = searchText,
        PageSize = 20,
        PageNumber = 1
    };

    SearchResult<ProductEntity> results = await _searchService.SearchAsync(query);

    // Results include:
    // - Entity: The full ProductEntity object
    // - RelevanceScore: 0.0 to 1.0 (higher = more relevant)
    // - MatchedTerms: List of search terms that matched
    // - Headline: (future) Highlighted snippet showing matches

    return results;
}
```

---

## Advanced Features

### Multi-Language Search

If your SaaS app serves multiple countries, configure language per tenant:

```csharp
// Entity configuration with dynamic language
builder.Property<string>("SearchVector")
    .HasComputedColumnSql(
        // Use tenant's language preference (store in tenant table)
        "to_tsvector(coalesce(tenant_language, 'english'), coalesce(\"Name\", '') || ' ' || coalesce(\"Description\", ''))",
        stored: true);
```

### Weighted Search (Prioritize Fields)

Make certain fields more important in ranking:

```csharp
// PostgreSQL weighted search (A=lowest, D=highest)
builder.Property<string>("SearchVector")
    .HasComputedColumnSql(
        @"setweight(to_tsvector('english', coalesce(""Name"", '')), 'A') || 
          setweight(to_tsvector('english', coalesce(""Description"", '')), 'B') || 
          setweight(to_tsvector('english', coalesce(""Tags"", '')), 'C')",
        stored: true);
```

### Custom Ranking Options

```csharp
var query = new SearchQuery
{
    QueryText = "laptop",
    Ranking = new SearchRankingOptions
    {
        UseNormalization = true, // Normalize scores to 0.0-1.0
        WeightLabels = new[] { 1.0f, 0.6f, 0.4f, 0.2f } // D, C, B, A weights
    }
};
```

---

## Performance Optimization

### 1. Index Only Necessary Fields

**Bad (too much data):**
```csharp
// Indexing everything increases index size and slows down updates
to_tsvector('english', coalesce("Name", '') || ' ' || 
    coalesce("Description", '') || ' ' || 
    coalesce("InternalNotes", '') || ' ' ||
    coalesce("TechnicalSpecs", '') || ' ' ||
    coalesce("LongFormContent", ''))
```

**Good (selective indexing):**
```csharp
// Only index user-searchable fields
to_tsvector('english', coalesce("Name", '') || ' ' || coalesce("Description", ''))
```

### 2. Monitor Index Size

```sql
-- Check search index size
SELECT 
    schemaname,
    tablename,
    indexname,
    pg_size_pretty(pg_relation_size(indexrelid)) AS index_size
FROM pg_stat_user_indexes
WHERE indexname LIKE '%SearchVector%'
ORDER BY pg_relation_size(indexrelid) DESC;
```

### 3. Vacuum and Analyze

PostgreSQL GIN indexes can become bloated. Run maintenance:

```sql
-- Rebuild index (reduces bloat)
REINDEX INDEX CONCURRENTLY "IX_Products_SearchVector";

-- Update statistics (improves query planning)
ANALYZE "Products";
```

### 4. Limit Results Per Page

```csharp
// Cap max page size to prevent performance issues
int safePage Size = Math.Min(requestedPageSize, 100);

var query = new SearchQuery
{
    QueryText = searchText,
    PageSize = safePageSize
};
```

### 5. Add Covering Indexes

If you frequently filter by specific fields, add composite indexes:

```csharp
// Composite index for common filter combinations
builder.HasIndex(e => new { e.Category, e.IsActive, e.TenantId })
    .HasDatabaseName("IX_Products_Category_Active_Tenant");
```

---

## Troubleshooting

### Issue: Migration Fails with "column already exists"

**Cause:** You ran the migration multiple times or manually added the column.

**Solution:**
```powershell
# Rollback the migration
dotnet ef migrations remove --context B2BDbContext

# Or manually drop the column
# DROP INDEX IF EXISTS "IX_Products_SearchVector";
# ALTER TABLE "Products" DROP COLUMN IF EXISTS "SearchVector";

# Then regenerate migration
dotnet ef migrations add AddProductFullTextSearch --context B2BDbContext
```

---

### Issue: Search Returns No Results

**Debugging steps:**

1. **Verify column exists:**
```sql
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'Products' AND column_name = 'SearchVector';
```

2. **Check if tsvector has data:**
```sql
SELECT "Id", "Name", "SearchVector" 
FROM "Products" 
LIMIT 5;
```

3. **Test search directly in SQL:**
```sql
SELECT "Id", "Name", 
       ts_rank("SearchVector", to_tsquery('english', 'laptop')) AS rank
FROM "Products"
WHERE "SearchVector" @@ to_tsquery('english', 'laptop')
ORDER BY rank DESC
LIMIT 10;
```

4. **Check tenant isolation:**
```csharp
// Temporarily disable tenant filter in debug
var allResults = await _dbContext.Products
    .IgnoreQueryFilters() // WARNING: Only use for debugging
    .ToListAsync();
```

---

### Issue: Search is Slow (>1 second)

**Cause:** Missing or unused GIN index.

**Solution:**

1. **Verify index exists and is used:**
```sql
EXPLAIN ANALYZE
SELECT * FROM "Products"
WHERE "SearchVector" @@ to_tsquery('english', 'laptop');

-- Should show: "Bitmap Index Scan on IX_Products_SearchVector"
-- Bad: "Seq Scan on Products" (means index not used)
```

2. **Rebuild index if needed:**
```sql
REINDEX INDEX CONCURRENTLY "IX_Products_SearchVector";
```

3. **Check index bloat:**
```sql
SELECT 
    schemaname,
    tablename,
    indexname,
    pg_size_pretty(pg_relation_size(indexrelid)) AS size,
    idx_scan AS scans,
    idx_tup_read AS tuples_read
FROM pg_stat_user_indexes
WHERE indexname = 'IX_Products_SearchVector';
```

---

### Issue: Tenant Sees Other Tenant's Data

**Cause:** Row-Level Security (RLS) not properly configured or tenant context not set.

**Solution:**

1. **Verify RLS is enabled:**
```sql
SELECT tablename, rowsecurity 
FROM pg_tables 
WHERE tablename = 'Products';
-- rowsecurity should be 't' (true)
```

2. **Check RLS policies:**
```sql
SELECT * FROM pg_policies WHERE tablename = 'Products';
```

3. **Verify tenant context is set:**
```csharp
// In your controller/middleware
var tenantId = User.FindFirst("tenant_id")?.Value;
if (string.IsNullOrEmpty(tenantId))
    return Unauthorized("Tenant ID not found");
```

---

### Issue: Updates to Entities Don't Update Search Index

**Cause:** SearchVector is a computed column and should update automatically, but caching might interfere.

**Solution:**

1. **Verify column is GENERATED ALWAYS:**
```sql
SELECT column_name, generation_expression
FROM information_schema.columns
WHERE table_name = 'Products' AND column_name = 'SearchVector';
```

2. **Force EF Core to reload:**
```csharp
await _dbContext.Entry(product).ReloadAsync();
```

3. **Clear any caching:**
```csharp
_dbContext.ChangeTracker.Clear();
```

---

## Best Practices

✅ **DO:**
- Index only user-searchable fields (name, description, tags)
- Use pagination (max 100 results per page)
- Add filters for common queries (category, status, date range)
- Monitor search performance with query execution time logging
- Use `ArgumentNullException.ThrowIfNull()` for parameter validation
- Follow clean architecture (controllers → services → repositories)

❌ **DON'T:**
- Index sensitive data (passwords, tokens, API keys)
- Return all results without pagination
- Use search for exact ID lookups (use direct queries instead)
- Ignore tenant isolation in test code
- Skip migration testing in staging environment
- Over-engineer with external search engines for simple use cases

---

## Migration Path to External Search (Future)

If you later need Algolia/Typesense/Elasticsearch:

1. **Keep the interface:** `ISearchService<TEntity>` remains the same
2. **Add new implementation:** `AlgoliaSearchService<TEntity> : ISearchService<TEntity>`
3. **Register conditionally:**
```csharp
if (configuration["Search:Provider"] == "Algolia")
    services.AddScoped<ISearchService<ProductEntity>, AlgoliaSearchService<ProductEntity>>();
else
    services.AddPostgreSqlFullTextSearch<ProductEntity, B2BDbContext>();
```
4. **No controller changes needed** - Dependency injection handles the swap

---

## Support & Further Reading

- **PostgreSQL Full-Text Search Docs:** https://www.postgresql.org/docs/current/textsearch.html
- **EF Core PostgreSQL Provider:** https://www.npgsql.org/efcore/
- **AppBlueprint Multi-Tenancy Guide:** `MULTI_TENANCY_GUIDE.md`
- **Clean Architecture Dependencies:** `.github/.ai-rules/baseline/clean-architecture-dependencies.md`

---

## License

This implementation is part of the AppBlueprint SaaS framework.

---

**Questions or Issues?**  
Check the troubleshooting section above or consult the AppBlueprint documentation.
