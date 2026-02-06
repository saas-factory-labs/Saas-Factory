# Quick Start: Testing JWT Authentication

## 🚀 Fastest Way to Test (5 minutes)

### Step 1: Generate a Test Token

```powershell
cd C:\Development\Development-Projects\saas-factory-labs\Code\AppBlueprint\JwtTokenGenerator
dotnet run
```

Follow the prompts (or just press Enter for defaults). Copy the generated JWT token.

### Step 2: Test with PowerShell

```powershell
# Set your token
$token = "paste-your-token-here"

# Test public endpoint (should work)
Invoke-RestMethod -Uri 'https://localhost:5002/api/v1/authtest/public' -SkipCertificateCheck

# Test protected endpoint without token (should fail with 401)
Invoke-RestMethod -Uri 'https://localhost:5002/api/v1/authtest/protected' -SkipCertificateCheck

# Test protected endpoint WITH token (should work)
Invoke-RestMethod -Uri 'https://localhost:5002/api/v1/authtest/protected' -Headers @{Authorization="Bearer $token"} -SkipCertificateCheck
```

### Step 3: Test with Swagger (Visual)

1. Navigate to `https://localhost:5002/swagger`
2. Click **"Authorize"** button (lock icon, top right)
3. Enter: `Bearer your-token-here` (include the word "Bearer")
4. Click **Authorize**
5. Try the `/api/v1/authtest/protected` endpoint
6. Should return 200 OK with user details

### Step 4: Run Automated Tests

```powershell
cd C:\Development\Development-Projects\saas-factory-labs\Code\AppBlueprint\AppBlueprint.Tests
dotnet test --filter "Category=JwtAuth"
```

## 📋 Test Scenarios Covered

| Endpoint | Auth Required | Expected Result |
|----------|---------------|-----------------|
| `/api/v1/authtest/public` | ❌ No | 200 OK |
| `/api/v1/authtest/protected` | ✅ Yes | 401 without token, 200 with token |
| `/api/v1/authtest/admin` | ✅ Admin role | 403 for User, 200 for Admin |
| `/api/v1/authtest/user` | ✅ User/Admin role | 200 for User or Admin |
| `/api/v1/authtest/echo` | ✅ Yes | 200 with user details |

## 🎯 What to Verify

### ✅ Success Indicators

1. **Public endpoint** returns 200 without token
2. **Protected endpoint** returns 401 without token
3. **Protected endpoint** returns 200 with valid token
4. **Admin endpoint** returns 403 with User role
5. **Admin endpoint** returns 200 with Admin role
6. **API logs** show "Token validated successfully"

### ❌ Common Issues

**401 Unauthorized:**
- Token not sent? Check Authorization header
- Token expired? Generate new token
- Wrong secret key? Check appsettings.json

**403 Forbidden:**
- Token valid but lacks required role
- Check token claims (use `/authtest/echo` endpoint)

## 🔧 Tools Created

1. **`AuthTestController.cs`** - Test endpoints in API
2. **`JwtTokenGenerator/`** - Console app to generate tokens
3. **`JwtAuthenticationTests.cs`** - Automated integration tests
4. **`JWT_TESTING_GUIDE.md`** - Detailed testing guide

## 🏃 Next Steps

Once basic testing works:

1. Add `[Authorize]` to your real controllers
2. Test from Blazor app (end-to-end)
3. Switch to real Auth0/Logto provider
4. Update secret key in production

## 📝 Example Output

**Successful authentication:**
```json
{
  "message": "Successfully authenticated!",
  "user": {
    "isAuthenticated": true,
    "name": "Test User",
    "userId": "test-user-123",
    "email": "test@example.com",
    "roles": ["User"]
  }
}
```

## 🎬 Full Test Workflow

```powershell
# 1. Start the API (if not already running)
# The AppHost should already be running in watch mode

# 2. Generate token
cd C:\Development\Development-Projects\saas-factory-labs\Code\AppBlueprint\JwtTokenGenerator
dotnet run

# 3. Copy the token and test with Swagger or run automated tests
$token = "your-generated-token"

# 4. Run automated tests
cd ..\AppBlueprint.Tests
dotnet test --filter "Category=JwtAuth" -v n
```

That's it! You're testing JWT authentication. 🎉

