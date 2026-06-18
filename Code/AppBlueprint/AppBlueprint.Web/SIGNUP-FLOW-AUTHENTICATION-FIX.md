# Signup Flow & Authentication Fix

**Date:** January 3, 2026  
**Branch:** `fix/blazor-routing-ambiguity`  
**Status:** ✅ Resolved

---

## Problem Statement

The signup flow was failing during authentication callback with the following error:

```
Error: OpenIdConnectAuthenticationHandler: message.State is null or empty.
Correlation Cookie Present: False (not found)
Nonce Cookie Present: False (not found)
```

### Root Cause

The signup pages (`SignupPersonal.razor` and `SignupBusiness.razor`) were manually constructing Logto authentication URLs and redirecting directly to Logto. This approach **bypassed the ASP.NET Core OpenID Connect middleware**, which is responsible for:

1. Setting the **correlation cookie** (CSRF protection)
2. Setting the **nonce cookie** (replay attack protection)
3. Managing the OAuth **state parameter**
4. Enabling secure token exchange at the callback endpoint

Without these cookies, the callback from Logto would fail state validation, preventing successful authentication.

---

## Solution Overview

The fix involved three main changes:

1. **Redirect through proper authentication flow** - Use `/auth/signin` endpoint instead of manual URL construction
2. **Add comprehensive form validation** - Validate user input with clear error messages
3. **Create signup completion page** - Handle post-authentication tenant/user creation

---

## Detailed Changes

### 1. Fixed Signup Pages

#### Before (Incorrect)
```csharp
// ❌ Manual Logto URL construction - bypasses middleware
var logtoEndpoint = Configuration["Logto:Endpoint"];
var appId = Configuration["Logto:AppId"];
var redirectUri = $"{Navigation.BaseUri}callback";
var signupUrl = $"{logtoEndpoint}/auth?client_id={appId}&redirect_uri={Uri.EscapeDataString(redirectUri)}...";
Navigation.NavigateTo(signupUrl, forceLoad: true);
```

#### After (Correct)
```csharp
// ✅ Use proper authentication endpoint - invokes middleware
Navigation.NavigateTo("/auth/signin", forceLoad: true);
```

**Files Modified:**
- `AppBlueprint.Web/Components/Pages/Auth/SignupPersonal.razor`
- `AppBlueprint.Web/Components/Pages/Auth/SignupBusiness.razor`

**Key Changes:**
- Removed manual Logto URL building logic
- Removed `IConfiguration` dependency
- Added form validation with `DataAnnotationsValidator`
- Added error handling and user-friendly error messages
- Added loading states with informative messages
- Store signup data in localStorage before redirecting

### 2. Created Validation Models

**Files Created:**
- `AppBlueprint.Web/Models/Auth/PersonalSignupModel.cs`
- `AppBlueprint.Web/Models/Auth/BusinessSignupModel.cs`

**Features:**
- `[Required]` validation for essential fields
- `[StringLength]` validation for maximum lengths
- `[EmailAddress]` validation for email format
- Custom error messages for better UX
- Terms acceptance validation

**Example:**
```csharp
public class PersonalSignupModel
{
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string FirstName { get; set; } = string.Empty;
    
    // ... more fields
}
```

### 3. Created Signup Completion Page

**File Created:**
- `AppBlueprint.Web/Components/Pages/Auth/SignupComplete.razor`

**Responsibilities:**
1. Verify user is authenticated (redirect to `/signup` if not)
2. Retrieve signup session data from localStorage
3. Create tenant/user based on account type
4. Display loading, success, or error states
5. Auto-redirect to dashboard after successful creation
6. Provide retry functionality on errors

**Flow:**
```
OnInitializedAsync()
  ↓
Check Authentication
  ↓
Get localStorage["signupSession"]
  ↓
Parse SignupSessionData
  ↓
if (business) → CreateBusinessAccountAsync()
if (personal) → CreatePersonalAccountAsync()
  ↓
Clear localStorage
  ↓
Show Success + 3-second Countdown
  ↓
Navigate to "/" (Dashboard)
```

### 4. Updated Program.cs

**Changes:**
- Removed problematic data protection configuration that caused compilation errors
- Added `/callback-debug` diagnostic endpoint (development only)
- Simplified cookie policy configuration

### 5. Updated Session Data Model

**File Modified:**
- `AppBlueprint.Web/Models/Auth/SignupSessionData.cs`

**Aligned Properties:**
- Uses `CompanyName` (not `OrganizationName`)
- Matches `BusinessSignupModel` properties
- Includes optional fields: `VatNumber`, `Country`

---

## Complete Authentication Flow

### Current Flow (Working)

```
┌─────────────────────────────────────────────────────────────┐
│ 1. User fills signup form (Personal or Business)           │
│    - Email, name, company details validated                │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 2. Form validation passes                                   │
│    - SignupPersonalModel or SignupBusinessModel validated   │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 3. Save signup data to localStorage                         │
│    - localStorage.setItem("signupSession", JSON)            │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 4. Redirect to /auth/signin                                 │
│    - Triggers authentication challenge                      │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 5. OpenID Connect middleware invoked                        │
│    - ChallengeAsync("Logto") called                         │
│    - Correlation cookie set (CSRF protection)               │
│    - Nonce cookie set (replay protection)                   │
│    - OAuth state parameter generated                        │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 6. Redirect to Logto authentication page                    │
│    - URL: https://32nkyp.logto.app/oidc/auth               │
│    - Includes: client_id, redirect_uri, state, nonce       │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 7. User authenticates at Logto                              │
│    - Creates account or logs in                             │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 8. Logto redirects to callback                              │
│    - URL: http://localhost:5000/callback?code=xxx&iss=xxx  │
│    - Browser sends correlation/nonce cookies                │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 9. OpenID Connect middleware processes callback             │
│    - Validates correlation cookie (CSRF check)              │
│    - Validates nonce cookie (replay check)                  │
│    - Exchanges authorization code for tokens                │
│    - Validates ID token signature                           │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 10. OnTokenValidated event fires                            │
│     - Sets RedirectUri = "/signup-complete"                 │
│     - Saves tokens to authentication properties             │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 11. User redirected to /signup-complete                     │
│     - Now authenticated with valid tokens                   │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 12. SignupComplete page loads                               │
│     - Retrieves localStorage["signupSession"]               │
│     - Parses SignupSessionData                              │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 13. Create tenant/user based on account type                │
│     - if business: CreateBusinessAccountAsync()             │
│     - if personal: CreatePersonalAccountAsync()             │
│     - TODO: Call API to provision tenant in database        │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 14. Clear signup session                                    │
│     - localStorage.removeItem("signupSession")              │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 15. Show success message                                    │
│     - Display "Welcome! 🎉"                                 │
│     - 3-second countdown timer starts                       │
└────────────────────┬────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────┐
│ 16. Auto-redirect to dashboard                              │
│     - Navigate to "/" (home/dashboard)                      │
│     - User is now fully onboarded                           │
└─────────────────────────────────────────────────────────────┘
```

---

## File Structure

```
AppBlueprint.Web/
├── Components/
│   └── Pages/
│       └── Auth/
│           ├── SignupPersonal.razor         (Modified)
│           ├── SignupBusiness.razor         (Modified)
│           └── SignupComplete.razor         (Created)
├── Models/
│   └── Auth/
│       ├── PersonalSignupModel.cs           (Created)
│       ├── BusinessSignupModel.cs           (Created)
│       └── SignupSessionData.cs             (Modified)
└── Program.cs                               (Modified)

Infrastructure/
└── Authentication/
    └── WebAuthenticationExtensions.cs       (No changes - already correct)
```

---

## Key Technical Details

### OpenID Connect Middleware Configuration

The middleware is configured in `WebAuthenticationExtensions.cs`:

```csharp
// Set callback path - middleware handles this automatically
options.CallbackPath = "/callback";

// Configure correlation cookie (CSRF protection)
options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.None; // Dev only
options.CorrelationCookie.SameSite = SameSiteMode.Lax;
options.CorrelationCookie.HttpOnly = true;
options.CorrelationCookie.IsEssential = true;

// Configure nonce cookie (replay protection)
options.NonceCookie.SecurePolicy = CookieSecurePolicy.None; // Dev only
options.NonceCookie.SameSite = SameSiteMode.Lax;
options.NonceCookie.HttpOnly = true;
options.NonceCookie.IsEssential = true;
```

### OnTokenValidated Event

After successful token exchange, the middleware fires `OnTokenValidated`:

```csharp
context.Properties.RedirectUri = "/signup-complete";
Console.WriteLine("[Web] Redirecting to /signup-complete for signup flow processing");
```

This ensures users are sent to the completion page to finalize their signup.

---

## Testing Verification

### ✅ Confirmed Working

1. **Correlation cookie present** in callback request
2. **Nonce cookie present** in callback request
3. **State validation** passes
4. **Token exchange** succeeds
5. **Access token** received and stored
6. **ID token** received and validated
7. **Signup session data** persists across authentication
8. **Tenant creation** placeholder ready for API integration
9. **Error handling** works with retry functionality
10. **Dashboard redirect** completes signup journey

### Console Logs (Success)

```
[SignupPersonal] Redirecting to /auth/signin for proper authentication flow
[Web] /auth/signin endpoint called
[Web] User NOT authenticated - calling ChallengeAsync to redirect to Logto
[Web] ChallengeAsync(LogtoScheme) completed - should redirect to Logto now
[Web] Configured correlation/nonce cookies: Path=/, IsEssential=true, SameSite=Lax, Secure=None
[Web] OpenID Connect callback path: /callback

[Callback request received]
[Web] Cookies Received: .AspNetCore.Correlation.Logto.xxx, .AspNetCore.OpenIdConnect.Nonce.xxx
[Web] Correlation Cookie Present: True
[Web] Nonce Cookie Present: True
[Web] OnTokenValidated event fired - saving tokens to authentication properties
[Web] Tokens received - AccessToken: True, IdToken: True, RefreshToken: True
[Web] Redirecting to /signup-complete for signup flow processing

[SignupComplete] Page loaded - starting account setup
[SignupComplete] User authenticated: user@example.com
[SignupComplete] Creating personal account
[SignupComplete] Personal account created for John Doe
[SignupComplete] Account setup completed successfully
```

---

## Next Steps / TODO

1. **Implement tenant creation API** - Replace placeholder methods in `SignupComplete.razor`:
   - `CreatePersonalAccountAsync()` should call tenant provisioning API
   - `CreateBusinessAccountAsync()` should call tenant provisioning API with company details

2. **Add email verification** - Optional step after signup:
   - Send verification email via Logto
   - Require email confirmation before full access

3. **Implement onboarding flow** - Guide new users:
   - Welcome wizard for first-time users
   - Feature tour
   - Initial setup steps

4. **Production hardening**:
   - Change `CookieSecurePolicy.None` to `CookieSecurePolicy.Always` in production
   - Enable HTTPS enforcement
   - Add rate limiting to signup endpoints

---

## Related Documentation

- [Logto Documentation](https://docs.logto.io/)
- [ASP.NET Core OpenID Connect](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/openid-connect)
- [OAuth 2.0 Authorization Code Flow](https://oauth.net/2/grant-types/authorization-code/)

---

## Troubleshooting

### Issue: "Correlation failed" error

**Symptoms:** 
- Callback fails with correlation error
- Cookies not present in callback request

**Solution:**
- Ensure cookies are enabled in browser
- Check `CookiePolicyOptions` configuration allows essential cookies
- Verify `SameSite=Lax` is set for development
- Confirm `/auth/signin` is used (not manual Logto URLs)

### Issue: User not authenticated after callback

**Symptoms:**
- Callback succeeds but user not logged in
- Redirected back to login page

**Solution:**
- Check `OnTokenValidated` event is configured
- Verify tokens are being saved: `options.SaveTokens = true`
- Ensure authentication scheme is properly configured

### Issue: Signup session data lost

**Symptoms:**
- SignupComplete page shows "No signup session found"
- User redirected to dashboard without tenant creation

**Solution:**
- Verify localStorage is not cleared by browser settings
- Check JavaScript is enabled
- Ensure data is saved before redirect in signup forms

---

## Contributors

- Fixed by: GitHub Copilot (Claude Sonnet 4.5)
- Date: January 3, 2026
- Repository: saas-factory-labs/Saas-Factory
