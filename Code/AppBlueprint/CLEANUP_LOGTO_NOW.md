# 🚨 CLEAN UP YOUR LOGTO CONFIGURATION NOW

## CURRENT PROBLEM

Looking at your screenshots, you have TOO MANY incorrect URIs still configured in Logto. This is causing the logout to fail.

## STEP-BY-STEP CLEANUP INSTRUCTIONS

### REDIRECT URIs SECTION

**REMOVE ALL of these (click the minus button next to each one):**
1. ❌ `http://localhost:8092/callback` 
2. ❌ `https://localhost:443/callback`
3. ❌ `http://localhost:80/callback`
4. ❌ `http://localhost/callback`

**KEEP ONLY these 5 URIs:**
1. ✅ `https://appblueprint-web-staging.up.railway.app/callback`
2. ✅ `https://localhost:8083/callback`
3. ✅ `http://localhost:8082/callback`
4. ✅ `http://localhost:8082/Callback` (capital C)
5. ✅ `https://localhost:8083/Callback` (capital C)

### POST SIGN-OUT REDIRECT URIs SECTION

**REMOVE ALL of these:**
1. ❌ `http://localhost:8092`
2. ❌ `https://appblueprint-web-staging.up.railway.app/signout-callback-logto`

**KEEP ONLY these 3 URIs:**
1. ✅ `https://localhost:8083/logout-complete`
2. ✅ `http://localhost:8082/logout-complete`
3. ✅ `https://appblueprint-web-staging.up.railway.app/logout-complete` (ADD THIS if missing)

## FINAL STATE - EXACTLY WHAT YOU SHOULD HAVE

After cleanup, your Logto should have EXACTLY these URIs and NOTHING ELSE:

### Redirect URIs (5 total):
```
https://appblueprint-web-staging.up.railway.app/callback
https://localhost:8083/callback
http://localhost:8082/callback
http://localhost:8082/Callback
https://localhost:8083/Callback
```

### Post sign-out redirect URIs (3 total):
```
https://appblueprint-web-staging.up.railway.app/logout-complete
https://localhost:8083/logout-complete
http://localhost:8082/logout-complete
```

## DO THIS RIGHT NOW

1. Go to https://32nkyp.logto.app/
2. Click on your application
3. **In Redirect URIs section:**
   - Remove all URIs with ports 8092, 443, 80, or no port
   - Keep only the 5 URIs listed above
   
4. **In Post sign-out redirect URIs section:**
   - Remove the `/signout-callback-logto` URI
   - Remove the `http://localhost:8092` URI
   - Keep/add only the 3 URIs listed above

5. Click **SAVE**

6. **Restart your application**

7. **Test logout again**

## WHY IT'S NOT WORKING

The browser console shows errors because Logto is trying to redirect to the wrong ports. Having multiple incorrect URIs confuses the redirect process.

**You MUST remove all the incorrect URIs for logout to work!**

