# Authentication Quick Setup Guide

> **Perfect for:** Dating apps, social platforms, and any consumer application that needs user login

## 🚀 5-Minute Setup

This guide shows you how to add authentication to your app in just a few lines of code.

---

## Choose Your Provider

| Provider | Best For | Free Tier | Self-Hosted |
|----------|----------|-----------|-------------|
| **Logto** | Modern apps, full control | ✅ Unlimited | ✅ Yes |
| **Auth0** | Enterprise features | ✅ 7,000 users | ❌ No |
| **Simple JWT** | Development/testing only | ✅ Yes | ✅ Yes |

---

## Option 1: Logto (Recommended)

### Step 1: Sign up for Logto Cloud

1. Go to [https://cloud.logto.io](https://cloud.logto.io)
2. Create a free account
3. Create a new application (choose "Traditional Web App")
4. Copy your credentials:
   - **Endpoint**: `https://your-tenant.logto.app`
   - **App ID** (Client ID)
   - **App Secret** (optional for API)

### Step 2: Add to Your Code

**In `Program.cs`:**

```csharp
using AppBlueprint.Presentation.ApiModule.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ✨ Just add this one line!
builder.Services.AddLogtoAuthentication(
    endpoint: "https://your-tenant.logto.app",
    clientId: "your-app-id"
);

var app = builder.Build();

// Add these middleware (order matters!)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
```

**Or use configuration:**

```csharp
// In Program.cs
builder.Services.AddQuickAuthentication(
    builder.Configuration, 
    AuthProvider.Logto
);
```

**In `appsettings.json`:**

```json
{
  "Authentication": {
    "Provider": "Logto",
    "Logto": {
      "Endpoint": "https://your-tenant.logto.app",
      "ClientId": "your-app-id",
      "ClientSecret": "your-app-secret"
    }
  }
}
```

### Step 3: Protect Your Endpoints

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    [HttpGet]
    [Authorize] // 🔒 This endpoint now requires authentication!
    public IActionResult GetProfile()
    {
        var userId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        
        return Ok(new { userId, email });
    }
}
```

### Done! 🎉

Your API now requires authentication. Users need to send a valid JWT token in the `Authorization` header:

```
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Option 2: Auth0

### Step 1: Set Up Auth0

1. Sign up at [https://auth0.com](https://auth0.com)
2. Create a new API
3. Copy your credentials:
   - **Domain**: `https://your-tenant.auth0.com`
   - **API Audience**: `https://your-api`

### Step 2: Add to Your Code

```csharp
using AppBlueprint.Presentation.ApiModule.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ✨ One line to add Auth0!
builder.Services.AddAuth0Authentication(
    domain: "https://your-tenant.auth0.com",
    audience: "https://your-api"
);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
```

---

## Option 3: Simple JWT (Development Only)

**⚠️ Warning:** This is for development and testing only. Use Logto or Auth0 in production.

```csharp
using AppBlueprint.Presentation.ApiModule.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ✨ Quick JWT setup for testing
builder.Services.AddSimpleJwtAuthentication(
    secretKey: "your-super-secret-key-at-least-32-characters-long!"
);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
```

---

## Testing Your Setup

### Method 1: Using PowerShell

```powershell
# Test public endpoint (should work)
Invoke-RestMethod -Uri 'http://localhost:5000/api/profile/public'

# Test protected endpoint (should fail with 401)
Invoke-RestMethod -Uri 'http://localhost:5000/api/profile'

# Test with token (should work)
$token = "your-jwt-token-here"
$headers = @{ Authorization = "Bearer $token" }
Invoke-RestMethod -Uri 'http://localhost:5000/api/profile' -Headers $headers
```

### Method 2: Using Swagger

1. Navigate to `/swagger` in your browser
2. Click the **"Authorize"** button (lock icon)
3. Enter: `Bearer your-jwt-token-here`
4. Click **"Authorize"**
5. Try your protected endpoints

---

## Common Scenarios

### Dating App Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    [HttpGet]
    [Authorize] // User must be logged in
    public IActionResult GetMatches()
    {
        var userId = User.FindFirst("sub")?.Value;
        // Return matches for this user
        return Ok(GetMatchesForUser(userId));
    }
    
    [HttpPost("like/{targetUserId}")]
    [Authorize]
    public IActionResult LikeProfile(string targetUserId)
    {
        var userId = User.FindFirst("sub")?.Value;
        // Process the like
        return Ok();
    }
}
```

### Role-Based Access

```csharp
[HttpDelete("{userId}")]
[Authorize(Roles = "Admin")] // Only admins can delete users
public IActionResult DeleteUser(string userId)
{
    // Delete user logic
    return NoContent();
}
```

### Optional Authentication

```csharp
[HttpGet("public-profiles")]
[AllowAnonymous] // Anyone can access, even without login
public IActionResult GetPublicProfiles()
{
    // Return public profiles
    return Ok(GetAllPublicProfiles());
}
```

---

## Advanced Configuration

### Custom Claims

Access user information from the JWT token:

```csharp
[HttpGet("me")]
[Authorize]
public IActionResult GetCurrentUser()
{
    var userId = User.FindFirst("sub")?.Value;
    var email = User.FindFirst("email")?.Value;
    var name = User.FindFirst("name")?.Value;
    var roles = User.FindAll("role").Select(c => c.Value).ToList();
    
    return Ok(new 
    { 
        userId, 
        email, 
        name, 
        roles 
    });
}
```

### Multiple Authentication Schemes

```csharp
// Support both Logto and API Keys
builder.Services.AddLogtoAuthentication(
    endpoint: "https://your-tenant.logto.app",
    clientId: "your-app-id"
);

builder.Services.AddAuthentication()
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
        "ApiKey", 
        options => { }
    );
```

### Environment-Based Configuration

```csharp
var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    // Use simple JWT in development
    builder.Services.AddSimpleJwtAuthentication(
        secretKey: builder.Configuration["DevJwtKey"]!
    );
}
else
{
    // Use Logto in production
    builder.Services.AddQuickAuthentication(
        builder.Configuration,
        AuthProvider.Logto
    );
}
```

---

## Troubleshooting

### 401 Unauthorized Errors

**Check:**
1. ✅ Is the token in the `Authorization` header?
2. ✅ Does it start with `Bearer `?
3. ✅ Is the token still valid (not expired)?
4. ✅ Are your credentials correct?

**Debug:**
```csharp
app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"];
    Console.WriteLine($"Auth Header: {authHeader}");
    await next();
});
```

### "No authenticationScheme was specified"

Make sure you called `app.UseAuthentication()` BEFORE `app.UseAuthorization()`:

```csharp
app.UseAuthentication();  // ← Must be first
app.UseAuthorization();   // ← Must be second
```

### Configuration Not Found

Make sure your configuration keys match exactly:

```jsonc
{
  "Authentication": {
    "Provider": "Logto",
    "Logto": {
      "Endpoint": "https://your-tenant.logto.app",  // Correct
      "ClientId": "your-app-id"                      // Correct
    }
  }
}
```

---

## Security Best Practices

### ✅ DO

- Use HTTPS in production
- Validate tokens on every request
- Keep secret keys secure (use Azure Key Vault, AWS Secrets Manager, etc.)
- Use strong, random secret keys (32+ characters)
- Implement token refresh for long-lived sessions
- Log authentication failures for monitoring

### ❌ DON'T

- Store tokens in localStorage (use secure cookies instead)
- Commit secrets to source control
- Use the same secret key in dev and production
- Accept tokens without validation
- Use simple JWT in production

---

## Migration from Other Providers

### From Custom JWT to Logto

**Before:**
```csharp
builder.Services.AddSimpleJwtAuthentication(
    secretKey: "my-secret-key"
);
```

**After:**
```csharp
builder.Services.AddLogtoAuthentication(
    endpoint: "https://your-tenant.logto.app",
    clientId: "your-app-id"
);
```

**That's it!** No code changes needed in your controllers.

---

## Real-World Example: Complete Dating App Setup

```csharp
using AppBlueprint.Presentation.ApiModule.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✨ Add authentication
builder.Services.AddLogtoAuthentication(
    endpoint: builder.Configuration["Logto:Endpoint"]!,
    clientId: builder.Configuration["Logto:ClientId"]!
);

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PremiumUser", policy => 
        policy.RequireClaim("subscription", "premium"));
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();    // ← Must be before UseAuthorization
app.UseAuthorization();
app.MapControllers();

app.Run();
```

**Controllers:**

```csharp
[ApiController]
[Route("api/[controller]")]
public class DatingController : ControllerBase
{
    [HttpGet("discover")]
    [Authorize] // All users must be logged in
    public IActionResult Discover()
    {
        return Ok(GetDiscoverProfiles());
    }
    
    [HttpGet("premium-matches")]
    [Authorize(Policy = "PremiumUser")] // Only premium users
    public IActionResult GetPremiumMatches()
    {
        return Ok(GetPremiumProfiles());
    }
    
    [HttpPost("message")]
    [Authorize]
    public IActionResult SendMessage([FromBody] MessageDto message)
    {
        var senderId = User.FindFirst("sub")?.Value;
        // Send message logic
        return Ok();
    }
}
```

---

## Next Steps

1. ✅ Set up your authentication provider (Logto recommended)
2. ✅ Add the authentication extension to your `Program.cs`
3. ✅ Add `[Authorize]` attributes to your controllers
4. ✅ Test with a valid JWT token
5. ✅ Add role-based authorization if needed
6. ✅ Implement token refresh for better UX
7. ✅ Set up monitoring and alerting

---

## Resources

- 📚 [Logto Documentation](https://docs.logto.io)
- 📚 [Auth0 Documentation](https://auth0.com/docs)
- 📚 [ASP.NET Core Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/)
- 🔧 [Full Logto Setup Guide](./LOGTO_SETUP_GUIDE.md)
- 🔧 [JWT Testing Guide](./JWT_TESTING_GUIDE.md)

---

## Support

Having issues? Check:
- [Existing guides](./README.md)
- [Test your JWT](./JWT_TESTING_GUIDE.md)
- Open an issue on GitHub

---

**Last Updated**: December 11, 2025  
**Tested With**: .NET 10.0, Logto 1.x, Auth0 Latest
