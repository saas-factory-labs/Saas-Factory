# 🎯 EXACT CLICKS TO FIX LOGTO - VISUAL GUIDE

## WHAT YOU'RE LOOKING AT IN YOUR SCREENSHOTS

I can see you have TWO screenshots of Logto configuration. Let me tell you EXACTLY which URIs to delete.

## SCREENSHOT 1: Redirect URIs Section

### ❌ DELETE THESE (click the ⊖ button):
1. Row 1: `http://localhost:8092/callback` → Click ⊖ → Delete
2. Row 2: `https://localhost:443/callback` → Click ⊖ → Delete  
3. Row 4: `http://localhost:80/callback` → Click ⊖ → Delete
4. Row 5: `http://localhost/callback` → Click ⊖ → Delete

### ✅ KEEP THESE (do NOT delete):
- Row 3: `https://appblueprint-web-staging.up.railway.app/callback` ✅
- Row 6: `https://localhost:8083/callback` ✅
- Row 7: `http://localhost:8082/callback` ✅
- Row 8: `http://localhost:8082/callback` ✅ (duplicate is fine)
- Row 9: `https://localhost:8083/callback` ✅
- Row 10: `http://localhost:8082/Callback` ✅
- Row 11: `https://localhost:8083/Callback` ✅

## SCREENSHOT 2: Post sign-out redirect URIs Section

### ❌ DELETE THESE (click the ⊖ button):
1. Row 1: `http://localhost:8092` → Click ⊖ → Delete
2. Row 2: `https://appblueprint-web-staging.up.railway.app/signout-callback-logto` → Click ⊖ → Delete

### ✅ KEEP THESE (do NOT delete):
- Row 3: `https://localhost:8083/logout-complete` ✅
- Row 4: `http://localhost:8082/logout-complete` ✅
- Row 5: `https://localhost:8083/logout-complete` ✅ (duplicate is fine)
- Row 6: `http://localhost:8082/logout-complete` ✅ (duplicate is fine)

### ➕ ADD THIS ONE:
1. Click "+ Add another" at the bottom
2. Type: `https://appblueprint-web-staging.up.railway.app/logout-complete`
3. Press Enter

## AFTER CLEANUP - FINAL COUNT

**Redirect URIs: Should have 5-7 URIs** (some duplicates are OK)
**Post sign-out redirect URIs: Should have 3-5 URIs** (some duplicates are OK)

## THEN CLICK SAVE!

Look for the blue "Save" button and click it!

## VISUAL SUMMARY

```
REDIRECT URIs - Final State:
✅ https://appblueprint-web-staging.up.railway.app/callback
✅ https://localhost:8083/callback
✅ http://localhost:8082/callback
✅ http://localhost:8082/Callback
✅ https://localhost:8083/Callback

POST SIGN-OUT URIs - Final State:
✅ https://appblueprint-web-staging.up.railway.app/logout-complete
✅ https://localhost:8083/logout-complete
✅ http://localhost:8082/logout-complete
```

## THAT'S IT!

After you:
1. ❌ Delete the 6 wrong URIs
2. ➕ Add the 1 Railway logout-complete URI
3. 💾 Click Save
4. 🔄 Restart your app

**Logout will work!** 🎉

