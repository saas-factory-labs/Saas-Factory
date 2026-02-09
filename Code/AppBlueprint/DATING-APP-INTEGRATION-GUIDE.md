# Dating App Integration Guide - Email/Password Authentication

This guide shows how to integrate AppBlueprint into a dating app with email/password authentication.

## Current Authentication Status

AppBlueprint currently uses **Logto** (OAuth/OpenID Connect) for authentication. To enable email/password login for your dating app, you have **3 options**:

---

## 🎯 **OPTION 1: Use Logto with Email/Password** (Recommended - Works Today)

Logto **DOES** support email/password authentication. You just need to enable it in your Logto Console.

### Setup Steps

1. **Create Logto Account** (Free tier available)
   - Go to https://logto.io
   - Create a new tenant
   - Copy your endpoint URL (e.g., `https://your-tenant.logto.app`)

2. **Configure Logto Application**
   ```bash
   # In Logto Console:
   # 1. Create a "Traditional Web" application
   # 2. Enable "Email & Password" sign-in method
   # 3. Add redirect URIs:
   #    - http://localhost:5000/callback
   #    - http://localhost:5000/signout-callback-logto
   # 4. Copy App ID and App Secret
   ```

3. **Set Environment Variables**
   ```bash
   # PowerShell
   $env:LOGTO_ENDPOINT="https://your-tenant.logto.app"
   $env:LOGTO_APP_ID="your-app-id"
   $env:LOGTO_APP_SECRET="your-app-secret"
   $env:DATABASE_CONNECTION_STRING="Host=localhost;Port=5432;Database=dating_app;Username=postgres;Password=postgres"
   ```

4. **DatingApp Program.cs**
   ```csharp
   using AppBlueprint.Infrastructure.Extensions;
   using AppBlueprint.Infrastructure.Authentication;
   using Microsoft.EntityFrameworkCore;

   var builder = WebApplication.CreateBuilder(args);

   // Add AppBlueprint Infrastructure
   builder.Services.AddAppBlueprintInfrastructure(
       builder.Configuration, 
       builder.Environment
   );

   // Add Authentication (Logto)
   builder.Services.AddWebAuthentication(
       builder.Configuration, 
       builder.Environment
   );

   // Add your custom DbContext
   builder.Services.AddDbContext<DatingDbContext>(options =>
   {
       var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ??
                             builder.Configuration.GetConnectionString("DefaultConnection");
       options.UseNpgsql(connectionString);
   });

   builder.Services.AddControllers();
   builder.Services.AddRazorPages(); // If using Blazor

   var app = builder.Build();

   // Configure middleware
   app.ConfigureAppBlueprintMiddleware();

   app.UseAuthentication();
   app.UseAuthorization();

   app.MapControllers();
   app.MapRazorPages();
   app.Run();
   ```

5. **Test Login**
   - Navigate to `http://localhost:5000/login`
   - Users will see Logto's hosted login page with email/password fields
   - After successful login, they're redirected back to your app

### ✅ **Pros:**
- **Works immediately** - No code changes needed
- **Secure** - Industry-standard OAuth/OIDC
- **Includes features** - Password reset, email verification, MFA, social login
- **Free tier** - Good for development/small apps
- **No password storage** - Logto handles hashing, salting, breach detection

### ❌ **Cons:**
- **External dependency** - Requires internet connection
- **Hosted UI** - Users see Logto branding (can customize in paid plans)

---

## 🔧 **OPTION 2: Use Mock Provider** (Development Only)

For local development/testing, AppBlueprint includes a **Mock Authentication Provider** that accepts any email/password.

### Setup Steps

1. **Update appsettings.json**
   ```json
   {
     "Authentication": {
       "Provider": "Mock"
     },
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=dating_app;Username=postgres;Password=postgres"
     }
   }
   ```

2. **Program.cs**
   ```csharp
   using AppBlueprint.Infrastructure.Extensions;
   using AppBlueprint.Infrastructure.Authorization;
   using Microsoft.EntityFrameworkCore;

   var builder = WebApplication.CreateBuilder(args);

   // Add AppBlueprint Infrastructure
   builder.Services.AddAppBlueprintInfrastructure(
       builder.Configuration, 
       builder.Environment
   );

   // Register Mock Authentication Provider
   builder.Services.AddScoped<IUserAuthenticationProvider, UserAuthenticationProviderAdapter>();

   builder.Services.AddDbContext<DatingDbContext>(options =>
   {
       var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ??
                             builder.Configuration.GetConnectionString("DefaultConnection");
       options.UseNpgsql(connectionString);
   });

   builder.Services.AddControllers();

   var app = builder.Build();

   app.ConfigureAppBlueprintMiddleware();

   app.MapControllers();
   app.Run();
   ```

3. **Login via API**
   ```http
   POST /api/v1/authentication/login
   Content-Type: application/json

   {
     "email": "john@example.com",
     "password": "any-password-works"
   }
   ```

### ✅ **Pros:**
- **No external dependencies**
- **Works offline**
- **Fast development**

### ❌ **Cons:**
- ⚠️ **NEVER use in production** - No real security
- **No password validation** - Accepts any credentials
- **No password storage** - Users can't actually register

---

## 🚀 **OPTION 3: Implement Custom Email/Password Authentication** (Advanced)

If you need full control over authentication, implement your own provider using ASP.NET Core Identity or custom JWT solution.

### Implementation Steps

1. **Add ASP.NET Core Identity**
   ```xml
   <!-- DatingApp.csproj -->
   <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" />
   ```

2. **Create Custom DbContext with Identity**
   ```csharp
   using AppBlueprint.Infrastructure.DatabaseContexts;
   using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore;

   public class DatingDbContext : IdentityDbContext<DatingUser>
   {
       public DatingDbContext(DbContextOptions<DatingDbContext> options) 
           : base(options) { }

       // Your custom entities
       public DbSet<Profile> Profiles => Set<Profile>();
       public DbSet<Match> Matches => Set<Match>();
       public DbSet<Message> Messages => Set<Message>();

       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           base.OnModelCreating(modelBuilder);

           // Configure Identity tables
           modelBuilder.Entity<DatingUser>().ToTable("AspNetUsers");
           
           // Your custom configurations
           modelBuilder.Entity<Profile>().ToTable("profiles");
       }
   }

   public class DatingUser : IdentityUser
   {
       public string FirstName { get; set; } = string.Empty;
       public string LastName { get; set; } = string.Empty;
       public DateTime DateOfBirth { get; set; }
       public string? ProfilePhotoUrl { get; set; }
   }
   ```

3. **Configure Identity in Program.cs**
   ```csharp
   using Microsoft.AspNetCore.Identity;

   var builder = WebApplication.CreateBuilder(args);

   // DO NOT add AppBlueprint authentication when using custom Identity
   // builder.Services.AddWebAuthentication(...); // SKIP THIS

   // Add custom Identity
   builder.Services.AddIdentity<DatingUser, IdentityRole>(options =>
   {
       options.Password.RequireDigit = true;
       options.Password.RequiredLength = 8;
       options.Password.RequireNonAlphanumeric = false;
       options.Password.RequireUppercase = true;
       options.Password.RequireLowercase = true;
       options.User.RequireUniqueEmail = true;
   })
   .AddEntityFrameworkStores<DatingDbContext>()
   .AddDefaultTokenProviders();

   // Add AppBlueprint Infrastructure (without authentication)
   builder.Services.AddDbContext<DatingDbContext>(options =>
   {
       var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
       options.UseNpgsql(connectionString);
   });

   // Add AppBlueprint repositories and services manually
   builder.Services.AddScoped<ITeamRepository, TeamRepository>();
   builder.Services.AddScoped<ITenantRepository, TenantRepository>();
   // Add other services as needed

   builder.Services.AddControllers();

   var app = builder.Build();

   app.UseAuthentication();
   app.UseAuthorization();

   app.MapControllers();
   app.Run();
   ```

4. **Create Login/Register Controllers**
   ```csharp
   using Microsoft.AspNetCore.Identity;
   using Microsoft.AspNetCore.Mvc;

   [ApiController]
   [Route("api/auth")]
   public class AuthController : ControllerBase
   {
       private readonly UserManager<DatingUser> _userManager;
       private readonly SignInManager<DatingUser> _signInManager;

       public AuthController(
           UserManager<DatingUser> userManager,
           SignInManager<DatingUser> signInManager)
       {
           _userManager = userManager;
           _signInManager = signInManager;
       }

       [HttpPost("register")]
       public async Task<IActionResult> Register([FromBody] RegisterRequest request)
       {
           var user = new DatingUser
           {
               UserName = request.Email,
               Email = request.Email,
               FirstName = request.FirstName,
               LastName = request.LastName,
               DateOfBirth = request.DateOfBirth
           };

           var result = await _userManager.CreateAsync(user, request.Password);

           if (!result.Succeeded)
               return BadRequest(result.Errors);

           await _signInManager.SignInAsync(user, isPersistent: false);
           return Ok(new { Message = "Registration successful" });
       }

       [HttpPost("login")]
       public async Task<IActionResult> Login([FromBody] LoginRequest request)
       {
           var result = await _signInManager.PasswordSignInAsync(
               request.Email,
               request.Password,
               request.RememberMe,
               lockoutOnFailure: false
           );

           if (!result.Succeeded)
               return Unauthorized(new { Message = "Invalid credentials" });

           return Ok(new { Message = "Login successful" });
       }

       [HttpPost("logout")]
       public async Task<IActionResult> Logout()
       {
           await _signInManager.SignOutAsync();
           return Ok(new { Message = "Logout successful" });
       }
   }

   public record RegisterRequest(
       string Email,
       string Password,
       string FirstName,
       string LastName,
       DateTime DateOfBirth
   );

   public record LoginRequest(
       string Email,
       string Password,
       bool RememberMe
   );
   ```

5. **Run Migrations**
   ```bash
   dotnet ef migrations add InitialIdentity --project DatingApp.Infrastructure
   dotnet ef database update --project DatingApp.Infrastructure
   ```

### ✅ **Pros:**
- **Full control** over authentication flow
- **No external dependencies**
- **Custom user fields** (DateOfBirth, ProfilePhoto, etc.)
- **Works offline**

### ❌ **Cons:**
- **More code to maintain** - You own password hashing, validation, reset flows
- **Security responsibility** - Must implement breach detection, rate limiting, etc.
- **Time investment** - 1-2 days to implement properly
- **Loses AppBlueprint's tenant features** - Must manually integrate multi-tenancy

---

## 📊 **Comparison Table**

| Feature | Option 1: Logto | Option 2: Mock | Option 3: Custom Identity |
|---------|----------------|----------------|---------------------------|
| **Works Today** | ✅ Yes | ✅ Yes | ❌ Requires implementation |
| **Production Ready** | ✅ Yes | ❌ No | ✅ Yes (if done right) |
| **Email/Password** | ✅ Yes | ✅ Yes | ✅ Yes |
| **Social Login** | ✅ Yes | ❌ No | ⚠️ Manual integration |
| **Password Reset** | ✅ Yes | ❌ No | ⚠️ Must implement |
| **Email Verification** | ✅ Yes | ❌ No | ⚠️ Must implement |
| **MFA** | ✅ Yes | ❌ No | ⚠️ Must implement |
| **Setup Time** | 30 minutes | 5 minutes | 1-2 days |
| **Cost** | Free tier + paid plans | Free | Free |
| **Branding Control** | Limited (paid plans) | Full control | Full control |
| **Offline Development** | ❌ No | ✅ Yes | ✅ Yes |


---

## 🎯 **Recommendation for Dating App**

**For fastest integration with production-ready security:**
→ **Use Option 1 (Logto)** with email/password enabled

**Why?**
- ✅ Works immediately (30 min setup)
- ✅ Production-ready security (password hashing, breach detection, rate limiting)
- ✅ Includes password reset, email verification, MFA out of the box
- ✅ Free tier supports up to 7,500 users
- ✅ Can add social login (Google, Facebook) later with zero code changes
- ✅ AppBlueprint's multi-tenancy works seamlessly

**Custom branding needed?**
- Logto Pro plan ($16/month) allows custom branding
- Or use Option 3 for full control (but 1-2 days extra work)

---

## 🚀 **Quick Start: Logto Email/Password Integration**

### 1. Create Logto Account (5 min)
```bash
# Visit https://logto.io and create account
# Create a "Traditional Web" application
# Enable "Email & Password" in Sign-in Methods
# Copy credentials
```

### 2. Set Environment Variables (1 min)
```powershell
# PowerShell
$env:LOGTO_ENDPOINT="https://your-tenant.logto.app"
$env:LOGTO_APP_ID="your-app-id"
$env:LOGTO_APP_SECRET="your-app-secret"
$env:DATABASE_CONNECTION_STRING="Host=localhost;Port=5432;Database=dating_app;Username=postgres;Password=postgres"
```

### 3. Create DatingApp Project (5 min)
```bash
# Create new Blazor Server app
dotnet new blazor -n DatingApp
cd DatingApp
```

### 4. Reference AppBlueprint (5 min)
```xml
<!-- DatingApp.csproj -->
<ItemGroup>
  <ProjectReference Include="..\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure\AppBlueprint.Infrastructure.csproj" />
  <ProjectReference Include="..\AppBlueprint\Shared-Modules\AppBlueprint.Application\AppBlueprint.Application.csproj" />
  <ProjectReference Include="..\AppBlueprint\Shared-Modules\AppBlueprint.SharedKernel\AppBlueprint.SharedKernel.csproj" />
</ItemGroup>
```

### 5. Configure Program.cs (5 min)
```csharp
using AppBlueprint.Infrastructure.Extensions;
using AppBlueprint.Infrastructure.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppBlueprintInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddWebAuthentication(builder.Configuration, builder.Environment);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.ConfigureAppBlueprintMiddleware();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

### 6. Add appsettings.json (2 min)
```json
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

### 7. Run Database Migrations (5 min)
```bash
dotnet ef migrations add Initial --project DatingApp
dotnet ef database update --project DatingApp
```

### 8. Test Login (2 min)
```bash
dotnet run

# Navigate to: http://localhost:5000/login
# Create account with email/password in Logto UI
# You're logged in!
```

---

## 📝 **Next Steps After Authentication Works**

1. **Add Dating-Specific Entities**
   - Profile (bio, photos, preferences)
   - Match
   - Like
   - Message
   - Block

2. **Implement Core Features**
   - Profile creation/editing
   - Photo upload (use AppBlueprint's ObjectStorageService)
   - Matching algorithm
   - Real-time chat (add SignalR)
   - Geolocation search (add PostGIS)

3. **UI Development**
   - Profile cards
   - Swipe interface
   - Chat UI
   - Match notifications

---

## 🆘 **Troubleshooting**

### "Redirect URI mismatch"
**Fix:** Add `http://localhost:5000/callback` to Logto Console → Application → Redirect URIs

### "Authentication failed"
**Fix:** Verify `LOGTO_APP_SECRET` environment variable is set correctly

### "Database connection failed"
**Fix:** Ensure PostgreSQL is running and connection string is correct

### "Users can't login"
**Fix:** Check Logto Console → Sign-in Experience → Enable "Email & Password"

---

## 📚 **Additional Resources**

- [Logto Documentation](https://docs.logto.io/)
- [AppBlueprint Authentication Guide](./Shared-Modules/AUTHENTICATION_GUIDE.md)
- [AppBlueprint Multi-Tenancy Guide](./Shared-Modules/MULTI_TENANCY_GUIDE.md)
- [Signup Flow Implementation](./SIGNUP-FLOW-IMPLEMENTATION.md)

---

**Need help?** Open an issue in the repository or reach out to the team.
