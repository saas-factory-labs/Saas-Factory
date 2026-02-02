# File Delete Fix - February 2, 2026

## Problem Summary
Delete file operation was failing with "Tenant context not available" error despite successful JWT authentication.

## Root Cause Analysis

### Issue 1: TenantMiddleware Bypassing API Endpoints with File Extensions
**Location:** `TenantMiddleware.ShouldBypassTenantValidation()` (line 164)

**Problem:** The middleware incorrectly treated API endpoints ending with file extensions (`.png`, `.jpg`, etc.) as static files, bypassing tenant validation.

**Example:**
- URL: `/api/v1/filestorage/tenant_xxx/folder/file.png`
- Expected: Tenant middleware should run and populate `HttpContext.Items["TenantId"]`
- Actual: Middleware bypassed the endpoint because `HasStaticFileExtension()` returned `true`

**Impact:** `TenantContextAccessor.TenantId` returned `null` because `HttpContext.Items["TenantId"]` was never populated.

### Issue 2: URL-Encoded FileKey Not Decoded
**Location:** `FileStorageController.DeleteFile()` (line 308)

**Problem:** The controller received URL-encoded fileKeys (e.g., `tenant_xxx%2Ffolder%2Ffile.png`) but queried the database with the encoded value instead of the decoded value (`tenant_xxx/folder/file.png`).

**Impact:** Database query failed to find the file metadata record, returning "File not found or access denied."

## Solutions Implemented

### Fix 1: TenantMiddleware - Check File Extensions Only for Non-API Paths

**File:** `AppBlueprint.Presentation.ApiModule/Middleware/TenantMiddleware.cs`

**Changes:**
```csharp
// Before
bool isDocumentationOrStatic = requestPath.Contains("swagger", StringComparison.OrdinalIgnoreCase) ||
                              requestPath.Contains("openapi", StringComparison.OrdinalIgnoreCase) ||
                              requestPath.Contains("nswag", StringComparison.OrdinalIgnoreCase) ||
                              requestPath.Contains("docs", StringComparison.OrdinalIgnoreCase) ||
                              requestPath.EndsWith("/v1", StringComparison.OrdinalIgnoreCase) ||
                              requestPath.StartsWith("/v1/", StringComparison.OrdinalIgnoreCase) ||
                              HasStaticFileExtension(requestPath); // ❌ Checked for ALL paths

if (isDocumentationOrStatic) return true;

// ... later ...
bool isApiEndpoint = requestPath.StartsWith("/api", StringComparison.OrdinalIgnoreCase);
return !isApiEndpoint;

// After
// Check if it's an API endpoint first
bool isApiEndpoint = requestPath.StartsWith("/api", StringComparison.OrdinalIgnoreCase);

// Check if it's a documentation or static file request
// IMPORTANT: Do not bypass tenant validation for API endpoints even if they have file extensions
bool isDocumentationOrStatic = requestPath.Contains("swagger", StringComparison.OrdinalIgnoreCase) ||
                              requestPath.Contains("openapi", StringComparison.OrdinalIgnoreCase) ||
                              requestPath.Contains("nswag", StringComparison.OrdinalIgnoreCase) ||
                              requestPath.Contains("docs", StringComparison.OrdinalIgnoreCase) ||
                              requestPath.EndsWith("/v1", StringComparison.OrdinalIgnoreCase) ||
                              requestPath.StartsWith("/v1/", StringComparison.OrdinalIgnoreCase) ||
                              (!isApiEndpoint && HasStaticFileExtension(requestPath)); // ✅ Only check file extensions for non-API paths

if (isDocumentationOrStatic) return true;

// ... later (reordered) ...
// Note: isApiEndpoint already computed above
return !isApiEndpoint;
```

**Result:** API endpoints like `/api/v1/filestorage/file.png` now correctly trigger tenant validation and database lookup.

### Fix 2: FileStorageController - URL Decode FileKey Parameter

**File:** `AppBlueprint.Presentation.ApiModule/Controllers/Baseline/FileStorageController.cs`

**Changes:**
```csharp
[HttpDelete("{*fileKey}")]
[RequireScope("write:files")]
public async Task<IActionResult> DeleteFile(string fileKey, CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(fileKey);

    // URL decode the fileKey (e.g., "tenant_xxx%2Ffolder%2Ffile.png" -> "tenant_xxx/folder/file.png")
    string decodedFileKey = Uri.UnescapeDataString(fileKey);

    try
    {
        await _fileStorageService.DeleteAsync(decodedFileKey, cancellationToken);
        _logger.LogInformation("File deleted successfully: {FileKey}", decodedFileKey);
        return NoContent();
    }
    catch (UnauthorizedAccessException ex)
    {
        _logger.LogWarning(ex, "Unauthorized file deletion attempt: {FileKey}", decodedFileKey);
        return NotFound(new { Message = "File not found or access denied" });
    }
}
```

**Result:** Database query uses the decoded fileKey, matching the stored value in the `FileMetadata` table.

## Verification Logs (After Fix)

```
[TenantMiddleware] No tenant claim in JWT. Looking up by ExternalAuthId (sub): 1gckjjlz1u4w
[TenantMiddleware] Database lookup result: TenantId=tenant_19B974A866F_fe8ac23bb8
TenantId from HttpContext.Items: tenant_19B974A866F_fe8ac23bb8
DeleteAsync called - FileKey: tenant_19B974A866F_fe8ac23bb8/adagag/cf65c872226d4fdda0b3246f2cff6350-boligportal-1.png, TenantId: tenant_19B974A866F_fe8ac23bb8, HasTenantContext: True
Proceeding with delete - TenantId: tenant_19B974A866F_fe8ac23bb8
Metadata query result - Found: True
File deleted from R2 and database successfully
```

## Architecture Notes

### Tenant Resolution Flow (Correct Implementation)
1. **JWT Authentication** → JWT token validated by Logto (no `tenant_id` claim in access token)
2. **TenantMiddleware** → Queries database by `sub` claim (ExternalAuthId) to get `TenantId`
3. **HttpContext.Items** → Populates `HttpContext.Items["TenantId"]` for downstream use
4. **TenantContextAccessor** → Retrieves `TenantId` from `HttpContext.Items`
5. **Row-Level Security** → Uses tenant ID to filter database queries

### Multi-Tenancy Security Principles
- **Tenant ID is NOT in JWT claims** (Logto design - access tokens don't include custom tenant claims)
- **Tenant ID stored in database** (`Users` table with `ExternalAuthId` foreign key)
- **Middleware populates tenant context** before controllers execute
- **Defense in depth**: Named Query Filters (EF Core) + Row-Level Security (PostgreSQL)

## Related Files Modified
- `AppBlueprint.Presentation.ApiModule/Middleware/TenantMiddleware.cs` (lines 134-152)
- `AppBlueprint.Presentation.ApiModule/Controllers/Baseline/FileStorageController.cs` (lines 303-308)

## Testing Recommendations
1. ✅ Test delete with file extensions: `.png`, `.jpg`, `.pdf`, `.json`, etc.
2. ✅ Test delete with URL-encoded paths (folders with special characters)
3. ✅ Verify tenant isolation (user cannot delete files from other tenants)
4. ✅ Test other authenticated endpoints with file extensions (download, update)

## Commit Message Suggestion
```
fix(api): Fix file deletion for API endpoints with file extensions

- TenantMiddleware now correctly validates tenant context for API endpoints
  ending with file extensions (e.g., /api/v1/filestorage/file.png)
- URL decode fileKey parameter in DeleteFile controller to match database values
- Resolves "Tenant context not available" error during authenticated file deletion

Fixes #[issue-number]
```
