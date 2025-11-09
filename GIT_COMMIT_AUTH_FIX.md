# Git Commit Message

```
fix: Enable authentication protection and fix logout functionality for Blazor pages

Fixed authentication flow where protected pages were accessible without login and logout button was not working.
The root causes were:
1. Missing cascading authentication state configuration in Blazor Server
2. Logout endpoint had RequireAuthorization preventing anonymous access
3. Missing OIDC logout event handlers for proper Logto logout flow

Changes:
- Added AddCascadingAuthenticationState() to Program.cs for Blazor Server authentication
- Added [Authorize] attribute to Dashboard page to require authentication
- Added [Authorize] attribute to AccountSettings page to require authentication  
- Added [Authorize] attribute to Checkout page to require authentication
- Fixed /signout-logto endpoint to use AllowAnonymous() instead of RequireAuthorization()
- Updated logout flow to sign out from cookie auth first, then OIDC with redirect
- Added OnRedirectToIdentityProviderForSignOut event handler for logout logging
- Added OnSignedOutCallbackRedirect event handler for post-logout callback logging
- Added proper error handling and fallback for logout failures

This ensures:
- Unauthenticated users are redirected to Logto login when accessing protected pages
- Dashboard and other protected pages are only accessible after successful authentication
- Authentication state properly cascades through Blazor component hierarchy
- Logout button properly signs out user from both application and Logto
- User is redirected back to home page after successful logout

Fixes: 
- Login button now properly redirects to Logto instead of directly to dashboard
- Logout button now successfully signs out user from both app and Logto identity provider
```


