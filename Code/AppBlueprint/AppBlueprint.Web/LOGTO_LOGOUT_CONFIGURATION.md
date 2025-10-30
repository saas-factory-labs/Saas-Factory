# Logto Logout Configuration Guide

## Current Status ✅

**The post-logout redirect URI has been configured in Logto Console!**

The logout flow is now fully enabled and works as follows:

1. User clicks "Sign Out" button
2. Local tokens are cleared from browser storage
3. User is redirected to: `https://32nkyp.logto.app/oidc/session/end?client_id=uovd1gg5ef7i1c4w46mt6&post_logout_redirect_uri=http://localhost:8092`
4. Logto ends the session on the IdP side
5. Logto redirects back to `http://localhost:8092`
6. User sees the home page and can log in again

## Configuration

The following URI is registered in Logto Console:
- **Post sign-out redirect URI**: `http://localhost:8092`

## Testing the Logout Flow

1. Log in to your application
2. Click the "Sign Out" button in the account menu (Person icon → Sign Out)
3. You should be briefly redirected to Logto
4. Then automatically redirected back to `http://localhost:8092`
5. You should see the home page in a logged-out state
6. Try logging in again - you should need to enter credentials (not auto-logged in)

## What's Different Now

✅ **Full OIDC Logout**: Sessions are cleared both in your app AND in Logto  
✅ **No Hanging**: Proper redirect flow with registered URI  
✅ **Clean State**: Refresh tokens and sessions are revoked on Logto's side  

## Console Logs

Watch the browser console for detailed logout flow:
- `[Appbar] LOGOUT BUTTON CLICKED!`
- `[Appbar] Step 1: Calling AuthenticationProvider.LogoutAsync()`
- `[Appbar] Step 2: LogoutAsync completed - local tokens cleared`
- `[Appbar] Step 3: Getting Logto logout URL...`
- `[Appbar] Step 4: Redirecting to Logto logout URL: https://...`
- `[Appbar] Step 5: Navigation initiated`

## Production Configuration

For production deployments, remember to add your production URLs to Logto Console:

```
https://yourdomain.com
https://www.yourdomain.com
```

Both variants (with and without www) should be registered if you support both.

## Troubleshooting

If logout doesn't work as expected:

1. **Clear browser cache and cookies**: Sometimes old session data can interfere
2. **Check the exact URI matches**: No trailing slashes, correct port, correct protocol (http vs https)
3. **Verify in Logto Console**: Applications → Your App → Post sign-out redirect URIs
4. **Check browser console logs**: Look for the detailed step-by-step logs
5. **Try incognito/private window**: Rules out browser extension interference
