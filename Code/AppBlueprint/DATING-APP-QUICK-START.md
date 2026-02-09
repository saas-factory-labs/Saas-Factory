# Dating App Quick Start - Using Existing Logto Authentication

This guide shows how to integrate your dating app with AppBlueprint's **already-implemented** Logto JWT authentication.

## What's Already Built

✅ **Logto JWT authentication** - Fully working in AppBlueprint.Web
✅ **Email/password login** - `/login` route exists
✅ **Signup flow** - `/signup` with B2C/B2B options
✅ **User management** - UserEntity, TenantEntity, ProfileEntity
✅ **Multi-tenancy** - Row-Level Security (RLS) for data isolation
✅ **JWT token handling** - Automatic token refresh, claims extraction

## Integration Steps (15 minutes)

### Step 1: Reference AppBlueprint Packages (2 min)

```xml
<!-- DatingApp.csproj -->
<ItemGroup>
  <!-- Core packages - REQUIRED -->
  <ProjectReference Include="..\AppBlueprint\Shared-Modules\AppBlueprint.SharedKernel\AppBlueprint.SharedKernel.csproj" />
  <ProjectReference Include="..\AppBlueprint\Shared-Modules\AppBlueprint.Application\AppBlueprint.Application.csproj" />
  <ProjectReference Include="..\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure\AppBlueprint.Infrastructure.csproj" />
  
  <!-- Optional - for DTOs and API contracts -->
  <ProjectReference Include="..\AppBlueprint\Shared-Modules\AppBlueprint.Contracts\AppBlueprint.Contracts.csproj" />
  
  <!-- Optional - for pre-built UI components -->
  <ProjectReference Include="..\AppBlueprint\Shared-Modules\AppBlueprint.UiKit\AppBlueprint.UiKit.csproj" />
</ItemGroup>
```

### Step 2: Set Environment Variables (1 min)

AppBlueprint's authentication is already configured - you just need Logto credentials:

```powershell
# PowerShell - Use the SAME Logto instance as AppBlueprint.Web
$env:LOGTO_ENDPOINT="https://your-tenant.logto.app"
$env:LOGTO_APP_ID="your-app-id"
$env:LOGTO_APP_SECRET="your-app-secret"
$env:DATABASE_CONNECTION_STRING="Host=localhost;Port=5432;Database=dating_app;Username=postgres;Password=postgres"
```

**OR** use `appsettings.json`:

```jsonc
{
  "Logto": {
    "Endpoint": "https://your-tenant.logto.app",
    "AppId": "your-app-id",
    "Resource": "http://localhost:5000"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=dating_app;Username=postgres;Password=postgres"
  }
}
```

**Note:** `LOGTO_APP_SECRET` MUST be set as environment variable (not in appsettings.json for security).

### Step 3: Configure Program.cs (5 min)

```csharp
using AppBlueprint.Infrastructure.Extensions;
using AppBlueprint.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Add AppBlueprint Authentication (EXISTING IMPLEMENTATION)
// ========================================
builder.Services.AddAppBlueprintInfrastructure(
    builder.Configuration, 
    builder.Environment
);

// Add Logto JWT authentication (already implemented)
builder.Services.AddWebAuthentication(
    builder.Configuration, 
    builder.Environment
);

// ========================================
// Add Your Dating App DbContext
// ========================================
builder.Services.AddDbContext<DatingDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ??
                          builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

// ========================================
// Add MVC/Blazor Services
// ========================================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers(); // If using API

var app = builder.Build();

// ========================================
// Configure Middleware Pipeline
// ========================================
app.ConfigureAppBlueprintMiddleware(); // Includes tenant resolution

app.UseAuthentication(); // Logto JWT authentication
app.UseAuthorization();

// Map your routes
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
    
app.MapControllers(); // If using API

app.Run();
```

### Step 4: Create Your Custom DbContext (5 min)

Inherit from `ApplicationDbContext` to get all AppBlueprint entities + add your dating entities:

```csharp
using AppBlueprint.Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Infrastructure;

public class DatingDbContext : ApplicationDbContext
{
    public DatingDbContext(DbContextOptions<DatingDbContext> options) 
        : base(options) { }

    // Your dating-specific entities
    public DbSet<DatingProfile> DatingProfiles => Set<DatingProfile>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Like> Likes => Set<Like>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Block> Blocks => Set<Block>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // CRITICAL - keeps AppBlueprint entities

        // Configure your dating entities
        modelBuilder.Entity<DatingProfile>(entity =>
        {
            entity.ToTable("dating_profiles");
            entity.HasKey(e => e.Id);
            
            // Link to AppBlueprint's UserEntity
            entity.Property(e => e.UserId)
                .IsRequired();
                
            entity.Property(e => e.Bio)
                .HasMaxLength(500);
                
            entity.Property(e => e.DateOfBirth)
                .IsRequired();
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.ToTable("matches");
            entity.HasKey(e => e.Id);
            
            // Multi-tenancy: Matches are scoped to tenant
            entity.Property(e => e.TenantId).IsRequired();
        });

        // Add other entity configurations...
    }
}
```

### Step 5: Create Dating Entities (2 min)

```csharp
namespace DatingApp.Domain;

public class DatingProfile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    // Link to AppBlueprint's UserEntity
    public string UserId { get; set; } = string.Empty;
    
    // Dating-specific fields
    public string Bio { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string InterestedIn { get; set; } = string.Empty;
    public string[] PhotoUrls { get; set; } = Array.Empty<string>();
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    // Preferences
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 99;
    public int MaxDistance { get; set; } = 50; // km
}

public class Match
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty; // For multi-tenancy
    public string User1Id { get; set; } = string.Empty;
    public string User2Id { get; set; } = string.Empty;
    public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

public class Like
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string FromUserId { get; set; } = string.Empty;
    public string ToUserId { get; set; } = string.Empty;
    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string MatchId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
}
```

---

## How Authentication Works (Already Built)

### 1. **User Visits `/login`**
- AppBlueprint redirects to Logto login page
- User enters email/password in Logto's hosted UI
- Logto validates credentials

### 2. **Logto Returns JWT Token**
- Logto redirects to `/callback` with auth code
- AppBlueprint exchanges code for JWT access token
- Token stored in secure cookie

### 3. **Access Protected Pages**
```csharp
@page "/profile"
@attribute [Authorize] // <- AppBlueprint's existing auth

<h3>My Profile</h3>
<p>Logged in as: @context.User.Identity?.Name</p>
```

### 4. **Get Current User Info**
```csharp
@inject ICurrentUserService CurrentUserService
@inject ICurrentTenantService CurrentTenantService

@code {
    private string userId = string.Empty;
    private string tenantId = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        // AppBlueprint's existing services
        userId = await CurrentUserService.GetUserIdAsync();
        tenantId = await CurrentTenantService.GetTenantIdAsync();
        
        // Now fetch dating profile
        var profile = await _db.DatingProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }
}
```

### 5. **API Calls with JWT**
```csharp
// AppBlueprint automatically includes JWT in API calls
@inject HttpClient Http

await Http.PostAsJsonAsync("/api/matches", new { userId1, userId2 });
// JWT token automatically sent in Authorization header
```

---

## What You Get For Free

From AppBlueprint's existing implementation:

✅ **Authentication**
- `/login` - Login page (redirects to Logto)
- `/signup` - Signup with B2C (Personal) or B2B (Business) 
- `/callback` - OAuth callback handler
- `/signout` - Logout

✅ **User Management**
- `UserEntity` - FirstName, LastName, Email, Username
- `ProfileEntity` - Additional user profile data
- `TenantEntity` - Multi-tenancy support

✅ **Services Available**
- `ICurrentUserService` - Get current logged-in user
- `ICurrentTenantService` - Get current tenant
- `IUserService` - User CRUD operations
- `ITenantRepository` - Tenant operations

✅ **JWT Claims Automatically Available**
```csharp
@inject IHttpContextAccessor HttpContextAccessor

var claims = HttpContextAccessor.HttpContext.User.Claims;
var userId = claims.FirstOrDefault(c => c.Type == "sub")?.Value;
var email = claims.FirstOrDefault(c => c.Type == "email")?.Value;
var tenantId = claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;
```

✅ **Multi-Tenancy (RLS)**
- Automatic tenant isolation at database level
- Queries filtered by `TenantId` automatically
- No cross-tenant data leaks

---

## Example: Protected Dating Profile Page

```razor
@page "/my-profile"
@attribute [Authorize]
@inject ICurrentUserService CurrentUserService
@inject DatingDbContext DbContext

<h3>My Dating Profile</h3>

@if (profile != null)
{
    <div>
        <p>Bio: @profile.Bio</p>
        <p>Age: @CalculateAge(profile.DateOfBirth)</p>
        <p>Photos: @profile.PhotoUrls.Length</p>
    </div>
}
else
{
    <p>Create your dating profile to get started!</p>
    <button @onclick="CreateProfile">Create Profile</button>
}

@code {
    private DatingProfile? profile;
    private string userId = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        // Get current user from AppBlueprint's auth
        userId = await CurrentUserService.GetUserIdAsync();
        
        // Load dating profile
        profile = await DbContext.DatingProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }
    
    private async Task CreateProfile()
    {
        profile = new DatingProfile
        {
            UserId = userId,
            DateOfBirth = DateTime.UtcNow.AddYears(-25),
            Bio = "New to this app!"
        };
        
        DbContext.DatingProfiles.Add(profile);
        await DbContext.SaveChangesAsync();
    }
    
    private int CalculateAge(DateTime dob)
    {
        var today = DateTime.Today;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--;
        return age;
    }
}
```

---

## Example: API Controller with Auth

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppBlueprint.Application.Services;

[ApiController]
[Route("api/matches")]
[Authorize] // <- AppBlueprint's existing auth
public class MatchesController : ControllerBase
{
    private readonly DatingDbContext _db;
    private readonly ICurrentUserService _currentUser;
    
    public MatchesController(
        DatingDbContext db, 
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMyMatches()
    {
        // AppBlueprint automatically provides userId from JWT
        string userId = await _currentUser.GetUserIdAsync();
        
        var matches = await _db.Matches
            .Where(m => m.User1Id == userId || m.User2Id == userId)
            .Where(m => m.IsActive)
            .ToListAsync();
            
        return Ok(matches);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateMatch([FromBody] CreateMatchRequest request)
    {
        string userId = await _currentUser.GetUserIdAsync();
        
        var match = new Match
        {
            User1Id = userId,
            User2Id = request.OtherUserId,
            MatchedAt = DateTime.UtcNow
        };
        
        _db.Matches.Add(match);
        await _db.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetMyMatches), new { id = match.Id }, match);
    }
}

public record CreateMatchRequest(string OtherUserId);
```

---

## Testing Your Integration

### 1. **Run AppBlueprint.Web First** (to verify Logto works)
```bash
cd Code/AppBlueprint/AppBlueprint.Web
dotnet run

# Navigate to http://localhost:5000/login
# Login with email/password
# If this works, Logto is configured correctly
```

### 2. **Run Your Dating App**
```bash
cd DatingApp
dotnet run

# Navigate to http://localhost:5001/login
# Should see same Logto login page
# After login, you're authenticated!
```

### 3. **Test Protected Routes**
```bash
# Without login -> Redirects to /login
curl http://localhost:5001/api/matches

# With login -> Returns data
# (JWT token automatically included in cookie)
```

---

## Troubleshooting

### "Redirect URI mismatch"
**Cause:** Your dating app URL not added to Logto
**Fix:** In Logto Console → Your App → Add redirect URI:
- `http://localhost:5001/callback`
- `http://localhost:5001/signout-callback-logto`

### "User not authenticated"
**Cause:** JWT token not present or expired
**Fix:** 
1. Check browser cookies for `.AspNetCore.Cookies`
2. Login again at `/login`
3. Verify `LOGTO_APP_SECRET` environment variable is set

### "Tenant not found"
**Cause:** User exists but no tenant assigned
**Fix:** Use AppBlueprint's `/signup` flow which creates tenant automatically

### "Cross-tenant data leak"
**Cause:** Forgot to add `TenantId` to entity
**Fix:** 
```csharp
// Add TenantId to all dating entities
public string TenantId { get; set; } = string.Empty;

// Configure RLS in OnModelCreating
modelBuilder.Entity<Match>()
    .HasQueryFilter(m => m.TenantId == _tenantProvider.GetCurrentTenantId());
```

---

## Next Steps

1. ✅ **Authentication works** - Users can login with email/password
2. ⬜ **Build dating features**:
   - Profile creation/editing
   - Photo upload (use AppBlueprint's `ObjectStorageService`)
   - Swipe/like functionality
   - Matching algorithm
   - Chat (add SignalR)
   - Geolocation search (add PostGIS)

3. ⬜ **UI Development**:
   - Profile cards
   - Match list
   - Chat interface
   - Settings page

---

## Summary

**You DON'T need to implement authentication** - it's already done in AppBlueprint!

Just:
1. Reference 3 packages (SharedKernel, Application, Infrastructure)
2. Set Logto credentials (same as AppBlueprint.Web)
3. Call `AddWebAuthentication()` in Program.cs
4. Use `[Authorize]` on protected pages/controllers

**Everything else (JWT handling, token refresh, user services) is handled automatically.**

---

**Ready to build your dating app features!** 🚀
