# ⚠️ LOGOUT TROUBLESHOOTING - IMMEDIATE ACTIONS

## PROBLEM DIAGNOSIS

Looking at your screenshots, I can see:
1. ✅ App is running on port 8082 (correct)
2. ❌ Logto still has WRONG URIs configured (ports 8092, 80, 443, etc.)
3. ❌ Browser shows Blazor WebSocket errors (normal with forceLoad, but...)
4. ❌ Logout is not completing

## ROOT CAUSE

**Your Logto configuration still has 7+ INCORRECT URIs!** This is preventing proper logout.

## IMMEDIATE FIX - DO THIS IN ORDER

### Step 1: Clean Up Logto (5 minutes)

Go to https://32nkyp.logto.app/ RIGHT NOW and do this:

**Redirect URIs - DELETE these 4:**
1. Click ⊖ next to `http://localhost:8092/callback` → Delete
2. Click ⊖ next to `https://localhost:443/callback` → Delete
3. Click ⊖ next to `http://localhost:80/callback` → Delete
4. Click ⊖ next to `http://localhost/callback` → Delete

**Post sign-out redirect URIs - DELETE these 2:**
1. Click ⊖ next to `http://localhost:8092` → Delete
2. Click ⊖ next to `https://appblueprint-web-staging.up.railway.app/signout-callback-logto` → Delete

**Post sign-out redirect URIs - ADD this 1:**
1. Click "+ Add another"
2. Type: `https://appblueprint-web-staging.up.railway.app/logout-complete`
3. Press Enter

**Click SAVE!**

### Step 2: Verify Final Configuration

After cleanup, you should have EXACTLY:

**Redirect URIs (5 total):**
- https://appblueprint-web-staging.up.railway.app/callback
- https://localhost:8083/callback
- http://localhost:8082/callback (might appear twice - that's OK)
- http://localhost:8082/Callback
- https://localhost:8083/Callback

**Post sign-out redirect URIs (3-4 total):**
- https://appblueprint-web-staging.up.railway.app/logout-complete
- https://localhost:8083/logout-complete (might appear twice - that's OK)
- http://localhost:8082/logout-complete (might appear twice - that's OK)

### Step 3: Restart Your Application

Even though watch mode should reload, do a FULL restart:
1. Stop the app (Ctrl+C)
2. Clear the terminal
3. Start again: `dotnet run --project Code/AppBlueprint/AppBlueprint.AppHost`

### Step 4: Test Logout

1. Navigate to http://localhost:8082
2. Sign in (if not already signed in)
3. Click the sign-out button
4. **Watch what happens**

**Expected behavior:**
- You see "Signing out..." page briefly
- Redirected back to /login
- You are logged out
- Must sign in again

### Step 5: If Still Not Working

If logout still doesn't work after cleaning Logto:

**Check server console output:**
- Do you see `[Appbar] LOGOUT BUTTON CLICKED!`?
- Do you see `[Web] SignOut endpoint called`?
- Do you see `[Web] Redirecting to Logto end session:`?
- Do you see `[Web] Logout complete callback`?

**Copy and paste the ENTIRE server console output** after clicking logout.

## WHY THIS WILL FIX IT

The incorrect URIs in Logto are causing redirect confusion. When you have:
- ❌ `http://localhost:8092` (wrong port)
- ❌ `/signout-callback-logto` (old endpoint)

Logto tries to redirect to these instead of the correct `/logout-complete` endpoint on port 8082.

**Remove ALL incorrect URIs and logout WILL work!**

## CHECKLIST

Do these in order:
- [ ] Remove 4 incorrect redirect URIs from Logto
- [ ] Remove 2 incorrect post-logout URIs from Logto
- [ ] Add Railway logout-complete URI to Logto
- [ ] Save Logto configuration
- [ ] Restart application
- [ ] Test logout
- [ ] If still broken, copy server console output

**DO THIS NOW!**

