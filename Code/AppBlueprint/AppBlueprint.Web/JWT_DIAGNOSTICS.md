# 🔍 JWT Token Verification - You're Logged In

Since you're getting 401 Unauthorized but you say you're logged in with JWT, let me help diagnose the issue.

## Quick Checks to Run

### 1. Verify You Have a Real JWT Token

**Open Browser Console (F12) and run:**

```javascript
const token = localStorage.getItem('auth_token');
console.log('=== TOKEN VERIFICATION ===');
console.log('Has token:', !!token);
console.log('Token length:', token?.length);
console.log('Starts with eyJ (JWT):', token?.startsWith('eyJ'));

if (token) {
    if (token.startsWith('eyJ')) {
        try {
            const parts = token.split('.');
            const payload = JSON.parse(atob(parts[1]));
            console.log('✅ REAL JWT TOKEN');
            console.log('Issuer:', payload.iss);
            console.log('Subject (User ID):', payload.sub);
            console.log('Audience:', payload.aud);
            console.log('Issued at:', new Date(payload.iat * 1000));
            console.log('Expires at:', new Date(payload.exp * 1000));
            console.log('Is expired:', Date.now() > payload.exp * 1000);
            console.log('Time until expiry:', Math.floor((payload.exp * 1000 - Date.now()) / 60000), 'minutes');
        } catch (e) {
            console.log('❌ Error parsing JWT:', e);
        }
    } else {
        console.log('❌ NOT A JWT TOKEN (Mock token)');
        console.log('Token preview:', token.substring(0, 50));
    }
} else {
    console.log('❌ NO TOKEN FOUND');
}
```

### Expected Output for Real JWT:
```
✅ REAL JWT TOKEN
Issuer: https://32nkyp.logto.app/oidc
Subject: user_xxxxx
Audience: uovd1gg5ef7i1c4w46mt6
Expires at: [future date]
Is expired: false
```

---

## Run Diagnostics on /todos Page

Click the "Run Tests" button and report:

1. **Token in Storage:** ✅ YES / ❌ NO
2. **Connection Test:** Result?
3. **Auth Test:** Result?
4. **Headers diagnostic:** What does it show?

---

## Possible Issues

### Issue 1: Token Expired
If the token shows `Is expired: true`, you need to:
- Log out
- Log back in via Logto
- Get fresh token

### Issue 2: Wrong Audience
If the token has `audience: null` or different audience, the API might reject it.

**Check what audience your token has:**
```javascript
const token = localStorage.getItem('auth_token');
const payload = JSON.parse(atob(token.split('.')[1]));
console.log('Token audience:', payload.aud);
console.log('API expects:', 'uovd1gg5ef7i1c4w46mt6');
console.log('Match:', payload.aud === 'uovd1gg5ef7i1c4w46mt6');
```

### Issue 3: Issuer Mismatch
Check if issuer matches exactly:
```javascript
const token = localStorage.getItem('auth_token');
const payload = JSON.parse(atob(token.split('.')[1]));
console.log('Token issuer:', payload.iss);
console.log('API expects:', 'https://32nkyp.logto.app/oidc');
console.log('Match:', payload.iss === 'https://32nkyp.logto.app/oidc');
```

### Issue 4: API Can't Download JWKS
The API needs to download Logto's public keys to validate the token.

**Check if Logto JWKS is accessible:**
Open in browser:
```
https://32nkyp.logto.app/oidc/.well-known/jwks.json
```

Should return JSON with public keys.

---

## Check API Logs

Look at your API console for authentication errors. You should see one of:

**Success:**
```
[Information] Token validated successfully. User: {...}, UserId: {...}, Issuer: {...}
```

**Failure - Common Errors:**

**Audience validation failed:**
```
[Error] IDX10214: Audience validation failed
```

**Issuer validation failed:**
```
[Error] IDX10205: Issuer validation failed
```

**Signature validation failed:**
```
[Error] IDX10503: Signature validation failed
```

**Token expired:**
```
[Error] IDX10223: Lifetime validation failed. The token is expired.
```

---

## What to Report Back

Please run the JavaScript checks above and tell me:

1. **Is it a real JWT token?** (starts with `eyJ`, 800+ chars)
2. **Issuer value:** What does `payload.iss` show?
3. **Audience value:** What does `payload.aud` show?
4. **Is expired:** True or False?
5. **API console error:** What specific error message?
6. **Diagnostic UI results:** All three test results

With this information, I can pinpoint the exact issue and fix it.

---

## Quick Fix Options

### If Token Expired:
```javascript
// Clear and re-login
localStorage.removeItem('auth_token');
// Then navigate to login page and complete Logto auth
```

### If Audience Issue:
We may need to disable audience validation in the API (already done, but verify it's active).

### If Issuer Issue:
We may need to accept multiple issuer formats (already configured, but verify).

---

**Run the diagnostics above and report back with the results!**

