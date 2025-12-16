# Getting JWT Tokens in Blazor Server App

## 🎯 Your Blazor App Already Has Everything!

Your codebase has a complete authentication infrastructure with **automatic JWT token management**. Here's how to use it:

---

## Method 1: Interactive Demo Page (Easiest!)

I've created an **interactive demo page** at `/auth-demo` that:
- Shows your current JWT token
- Lets you login/logout
- Tests API calls with the token
- Provides copy-paste examples

### Access it:
1. Navigate to: `https://localhost:YOUR-PORT/auth-demo`
2. Login using the Mock provider
3. See your JWT token displayed
4. Copy and test it!

---

## Method 2: Programmatic Access in Your Blazor Components

### Get Token in Any Blazor Component

```razor
@inject ITokenStorageService TokenStorage
@inject IUserAuthenticationProvider AuthProvider

@code {
    private string? jwtToken;

    protected override async Task OnInitializedAsync()
    {
        // Method 1: Get token from storage
        jwtToken = await TokenStorage.GetTokenAsync();
        
        // Method 2: Check if authenticated
        bool isAuthenticated = AuthProvider.IsAuthenticated();
        
        if (isAuthenticated && !string.IsNullOrEmpty(jwtToken))
        {
            Console.WriteLine($"JWT Token: {jwtToken}");
            // Use the token...
        }
    }
}
```

### Use Token in API Calls

**Good News:** Your API client already adds the token automatically!

```razor
@inject ApiClient ApiClient

@code {
    private async Task CallProtectedEndpoint()
    {
        try
        {
            // Token is automatically added by IAuthenticationProvider!
            var response = await ApiClient.Api.V1.Authtest.Protected.GetAsync();
            
            // Success! The JWT token was validated
            Console.WriteLine("API call succeeded - token is valid");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API call failed: {ex.Message}");
        }
    }
}
```

---

## Method 3: Manual Token Extraction

### Display Token to User

```razor
@page "/show-token"
@inject ITokenStorageService TokenStorage

<h3>Your JWT Token</h3>

@if (!string.IsNullOrEmpty(token))
{
    <MudTextField @bind-Value="token" 
                 Label="JWT Token" 
                 Lines="10" 
                 ReadOnly="true" />
    
    <MudText>Token Length: @token.Length characters</MudText>
}
else
{
    <MudAlert Severity="Severity.Warning">No token found - please login</MudAlert>
}

@code {
    private string? token;

    protected override async Task OnInitializedAsync()
    {
        token = await TokenStorage.GetTokenAsync();
    }
}
```

---

## Method 4: Login and Get Token

### Complete Login Flow

```csharp
@page "/login"
@inject IUserAuthenticationProvider AuthProvider
@inject ITokenStorageService TokenStorage
@inject NavigationManager Navigation

<EditForm Model="loginModel" OnValidSubmit="HandleLogin">
    <MudTextField @bind-Value="loginModel.Email" Label="Email" />
    <MudTextField @bind-Value="loginModel.Password" 
                 Label="Password" 
                 InputType="InputType.Password" />
    <MudButton ButtonType="ButtonType.Submit">Login</MudButton>
</EditForm>

@if (!string.IsNullOrEmpty(errorMessage))
{
    <MudAlert Severity="Severity.Error">@errorMessage</MudAlert>
}

@code {
    private LoginModel loginModel = new();
    private string? errorMessage;

    private async Task HandleLogin()
    {
        // Login through your authentication provider
        bool success = await AuthProvider.LoginAsync(
            loginModel.Email, 
            loginModel.Password);
        
        if (success)
        {
            // Get the JWT token that was just created
            var token = await TokenStorage.GetTokenAsync();
            
            Console.WriteLine($"✅ Login successful!");
            Console.WriteLine($"JWT Token: {token}");
            
            // Token is now available for all API calls
            Navigation.NavigateTo("/dashboard");
        }
        else
        {
            errorMessage = "Login failed - invalid credentials";
        }
    }

    public class LoginModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
```

---

## How It Works Behind the Scenes

### 1. **Login Flow**
```
User Login → AuthProvider.LoginAsync() 
         → Gets token from Auth0/Logto/Mock
         → Stores in TokenStorage (localStorage)
         → Token available everywhere
```

### 2. **Automatic Token Injection**
```
API Call → ApiClient 
       → IAuthenticationProvider.AuthenticateRequestAsync()
       → Adds "Authorization: Bearer {token}" header
       → API validates token
       → Success!
```

### 3. **Token Storage**
- Stored in browser's `localStorage` (key: `auth_token`)
- Persists across page refreshes
- Automatically loaded on app startup
- Cleared on logout

---

## Supported Authentication Providers

Your app supports **3 providers** (configured in `appsettings.json`):

### Mock Provider (Development)
```json
{
  "Authentication": {
    "Provider": "Mock"
  }
}
```
- Accepts any email/password
- Generates fake JWT tokens
- Perfect for testing

### Auth0 Provider
```json
{
  "Authentication": {
    "Provider": "Auth0",
    "Auth0": {
      "Domain": "https://your-domain.auth0.com",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret",
      "Audience": "your-api-audience"
    }
  }
}
```
- Real OAuth2/OIDC tokens
- Production-ready

### Logto Provider
```json
{
  "Authentication": {
    "Provider": "Logto",
    "Logto": {
      "Endpoint": "https://your-instance.logto.app",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret"
    }
  }
}
```
- Real OAuth2/OIDC tokens
- Production-ready

---

## Quick Testing Workflow

### Step 1: Login via Blazor App
```
1. Go to your Blazor app
2. Login using any provider
3. You now have a JWT token stored
```

### Step 2: Extract Token
```razor
@inject ITokenStorageService TokenStorage

@code {
    var token = await TokenStorage.GetTokenAsync();
    Console.WriteLine(token); // See it in browser console
}
```

### Step 3: Test with Token
```powershell
# Copy token from browser console or demo page
$token = "eyJhbGc..."

# Test API
Invoke-RestMethod -Uri 'https://localhost:5002/api/v1/authtest/protected' `
    -Headers @{Authorization="Bearer $token"} -SkipCertificateCheck
```

---

## Common Patterns

### Pattern 1: Check Authentication in Any Component
```razor
@inject IUserAuthenticationProvider AuthProvider

@if (AuthProvider.IsAuthenticated())
{
    <p>✅ You are logged in</p>
    <p>Token is available for API calls</p>
}
else
{
    <p>⚠️ Please login</p>
}
```

### Pattern 2: Protect a Page
```razor
@page "/admin"
@attribute [Authorize] // Requires authentication

<h3>Admin Page</h3>
<p>You can only see this if you have a valid JWT token</p>
```

### Pattern 3: Use Token in Custom HTTP Calls
```razor
@inject ITokenStorageService TokenStorage

@code {
    private async Task CallExternalApi()
    {
        var token = await TokenStorage.GetTokenAsync();
        
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.GetAsync("https://external-api.com/data");
        // ...
    }
}
```

### Pattern 4: Token Refresh
```razor
@inject IUserAuthenticationProvider AuthProvider

@code {
    private async Task EnsureTokenIsValid()
    {
        if (!AuthProvider.IsAuthenticated())
        {
            // Token expired or invalid
            // Redirect to login or refresh
            Navigation.NavigateTo("/login");
        }
    }
}
```

---

## Troubleshooting

### "No token found"
✅ **Solution:** User needs to login first
```csharp
await AuthProvider.LoginAsync(email, password);
```

### "Token is null"
✅ **Solution:** Check authentication state
```csharp
if (AuthProvider.IsAuthenticated())
{
    var token = await TokenStorage.GetTokenAsync();
}
```

### "API returns 401"
✅ **Solution:** Token might be expired
- Login again to get new token
- Or implement token refresh

### "Token not sent to API"
✅ **Solution:** Ensure IAuthenticationProvider is registered
- Already configured in your app
- Check Program.cs has `AddAuthenticationServices()`

---

## Key Classes Reference

| Class | Purpose | Usage |
|-------|---------|-------|
| `ITokenStorageService` | Store/retrieve tokens | `await TokenStorage.GetTokenAsync()` |
| `IUserAuthenticationProvider` | Login/logout/check auth | `await AuthProvider.LoginAsync()` |
| `ApiClient` | API calls with auto-token | Inject and use normally |
| `IAuthenticationProviderFactory` | Create auth providers | Auto-configured |

---

## Next Steps

1. ✅ Use the `/auth-demo` page to see it in action
2. ✅ Login through your Blazor app
3. ✅ Extract token using `ITokenStorageService`
4. ✅ Test API calls - token added automatically!
5. ✅ Switch to Auth0/Logto for production

**Your Blazor app handles JWT tokens automatically - you rarely need to touch them directly!**

