# 🎯 Quick Reference: Get JWT Token in Blazor

## Fastest Way (3 Steps)

### 1. Navigate to Demo Page
```
https://localhost:YOUR-PORT/auth-demo
```

### 2. Login
- Email: `test@example.com`
- Password: `anything` (Mock provider accepts any credentials)
- Click "Login"

### 3. Copy Your Token
The JWT token is displayed on screen - copy it!

---

## In Your Code (Copy-Paste Ready)

### Get Token
```csharp
@inject ITokenStorageService TokenStorage

@code {
    private async Task GetMyToken()
    {
        var token = await TokenStorage.GetTokenAsync();
        Console.WriteLine($"My JWT: {token}");
    }
}
```

### Check If Logged In
```csharp
@inject IUserAuthenticationProvider AuthProvider

@code {
    private bool IsLoggedIn()
    {
        return AuthProvider.IsAuthenticated();
    }
}
```

### Make API Call (Token Added Automatically)
```csharp
@inject ApiClient ApiClient

@code {
    private async Task CallApi()
    {
        // Token is automatically included!
        var data = await ApiClient.Api.V1.Authtest.Protected.GetAsync();
    }
}
```

---

## Test Commands

### PowerShell (After Getting Token)
```powershell
$token = "your-token-here"
Invoke-RestMethod -Uri 'https://localhost:5002/api/v1/authtest/protected' `
    -Headers @{Authorization="Bearer $token"} -SkipCertificateCheck
```

### Swagger
1. Go to `https://localhost:5002/swagger`
2. Click "Authorize"
3. Enter: `Bearer your-token-here`

---

## Files Created

✅ **`AuthDemo.razor`** - Interactive demo page at `/auth-demo`  
✅ **`BLAZOR_JWT_GUIDE.md`** - Complete guide with examples  
✅ **`QUICKSTART_JWT_TESTING.md`** - API testing guide  

---

## The Magic ✨

Your Blazor app **automatically**:
- Gets tokens when you login
- Stores tokens in browser localStorage
- Adds tokens to every API call
- Refreshes tokens when needed

**You rarely need to handle tokens manually!**

---

## Commit Message

```
feat: Add JWT authentication demo page and documentation

- Created interactive /auth-demo page to display and test JWT tokens
- Added comprehensive Blazor JWT guide with code examples
- Implemented quick reference for token extraction
- All API calls automatically include JWT tokens via IAuthenticationProvider
```

