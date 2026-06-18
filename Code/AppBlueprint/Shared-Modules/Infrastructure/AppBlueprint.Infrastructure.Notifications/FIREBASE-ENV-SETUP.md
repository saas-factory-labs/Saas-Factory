# Environment Variables Setup for Firebase

## Required Environment Variables

You need to set **TWO TYPES** of Firebase credentials:

### 1. Server-Side (Required for sending push notifications)

```bash
# Firebase Admin SDK Service Account (entire JSON from Firebase Console)
FIREBASE_CREDENTIALS='{"type":"service_account","project_id":"your-project","private_key_id":"...","private_key":"-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n","client_email":"firebase-adminsdk-xxx@your-project.iam.gserviceaccount.com","client_id":"...","auth_uri":"https://accounts.google.com/o/oauth2/auth","token_uri":"https://oauth2.googleapis.com/token","auth_provider_x509_cert_url":"https://www.googleapis.com/oauth2/v1/certs","client_x509_cert_url":"..."}'
```

**Where to get it:**
1. Firebase Console → ⚙️ Project Settings → **Service accounts** tab
2. Click "Generate new private key"
3. Download JSON file
4. Copy entire JSON content

### 2. Client-Side (Required for browser notifications)

```bash
# Firebase Web App Config (from Firebase Console → Project Settings → General)
FIREBASE_API_KEY="AIzaSyB..."
FIREBASE_AUTH_DOMAIN="your-project.firebaseapp.com"
FIREBASE_PROJECT_ID="your-project"
FIREBASE_STORAGE_BUCKET="your-project.appspot.com"
FIREBASE_MESSAGING_SENDER_ID="123456789"
FIREBASE_APP_ID="1:123456789:web:abc123"

# VAPID Key (from Firebase Console → Project Settings → Cloud Messaging)
FIREBASE_VAPID_KEY="BPXw9ILuFzVcF2yrKKvh0rZ..."
```

**Where to get these:**

#### Firebase Web Config:
1. Firebase Console → ⚙️ Project Settings → **General** tab
2. Scroll to "Your apps" → Click web app (`</>`)
3. Copy values from `firebaseConfig` object

#### VAPID Key:
1. Firebase Console → ⚙️ Project Settings → **Cloud Messaging** tab
2. Scroll to "Web Push certificates"
3. Click "Generate key pair" (if not already generated)
4. Copy the key

---

## Setup Methods

### Option 1: User Secrets (Development - Recommended)

```bash
cd AppBlueprint.Web

# Server-side credential (full JSON)
dotnet user-secrets set "FIREBASE_CREDENTIALS" '{"type":"service_account","project_id":"your-project",...}'

# Client-side config
dotnet user-secrets set "FIREBASE_API_KEY" "AIzaSyB..."
dotnet user-secrets set "FIREBASE_AUTH_DOMAIN" "your-project.firebaseapp.com"
dotnet user-secrets set "FIREBASE_PROJECT_ID" "your-project"
dotnet user-secrets set "FIREBASE_STORAGE_BUCKET" "your-project.appspot.com"
dotnet user-secrets set "FIREBASE_MESSAGING_SENDER_ID" "123456789"
dotnet user-secrets set "FIREBASE_APP_ID" "1:123456789:web:abc123"
dotnet user-secrets set "FIREBASE_VAPID_KEY" "BPXw9ILuFzVcF2yrKKvh0rZ..."
```

### Option 2: Environment Variables (PowerShell)

```powershell
# Set for current PowerShell session
$env:FIREBASE_CREDENTIALS='{"type":"service_account",...}'
$env:FIREBASE_API_KEY="AIzaSyB..."
$env:FIREBASE_AUTH_DOMAIN="your-project.firebaseapp.com"
$env:FIREBASE_PROJECT_ID="your-project"
$env:FIREBASE_STORAGE_BUCKET="your-project.appspot.com"
$env:FIREBASE_MESSAGING_SENDER_ID="123456789"
$env:FIREBASE_APP_ID="1:123456789:web:abc123"
$env:FIREBASE_VAPID_KEY="BPXw9ILuFzVcF2yrKKvh0rZ..."

# Run app
dotnet run
```

### Option 3: Azure App Service

1. Go to Azure Portal → App Service → Configuration → Application settings
2. Add each variable as a new application setting
3. For `FIREBASE_CREDENTIALS`, paste the entire JSON as the value
4. Save and restart the app

### Option 4: Docker Compose

```yaml
# docker-compose.yml
services:
  web:
    environment:
      - FIREBASE_CREDENTIALS={"type":"service_account",...}
      - FIREBASE_API_KEY=AIzaSyB...
      - FIREBASE_AUTH_DOMAIN=your-project.firebaseapp.com
      - FIREBASE_PROJECT_ID=your-project
      - FIREBASE_STORAGE_BUCKET=your-project.appspot.com
      - FIREBASE_MESSAGING_SENDER_ID=123456789
      - FIREBASE_APP_ID=1:123456789:web:abc123
      - FIREBASE_VAPID_KEY=BPXw9ILuFzVcF2yrKKvh0rZ...
```

---

## How It Works Now

### Before (Manual Config)
- ❌ Had to manually edit `firebase-messaging-sw.js` with Firebase config
- ❌ Config hardcoded in static JavaScript files
- ❌ Different configs for dev/staging/production required file edits

### After (Environment Variables)
- ✅ Firebase config fetched from `/api/firebase-config` endpoint
- ✅ VAPID key fetched from `/api/firebase-config/vapid-key` endpoint
- ✅ Service worker loads config dynamically on activation
- ✅ Blazor components call `initializeFirebaseMessagingFromServer()` (no params needed)
- ✅ Same files work across all environments (dev/staging/production)

### Architecture

```
┌──────────────────────────────────────────────────┐
│  Browser (Service Worker)                        │
│  firebase-messaging-sw.js                        │
│  Fetches: GET /api/firebase-config              │
└──────────────────────────────────────────────────┘
                     ↓
┌──────────────────────────────────────────────────┐
│  Server API Controller                           │
│  FirebaseConfigController.cs                     │
│  Reads: Environment Variables                    │
│  Returns: Public Firebase config (safe to expose)│
└──────────────────────────────────────────────────┘
                     ↑
┌──────────────────────────────────────────────────┐
│  Environment Variables                           │
│  - FIREBASE_API_KEY                              │
│  - FIREBASE_PROJECT_ID                           │
│  - FIREBASE_VAPID_KEY                            │
│  - etc.                                          │
└──────────────────────────────────────────────────┘
```

---

## Testing

1. **Set environment variables** (use Option 1 or 2 above)
2. **Run the app**: `dotnet run`
3. **Navigate to**: `/demo/notifications`
4. **Click**: "Register for Push Notifications"
5. **Check console**: Should see "Firebase config loaded from server"
6. **Send test notification**: Should receive both in-app and browser push

---

## Troubleshooting

### "Failed to fetch Firebase config"
- Check that environment variables are set correctly
- Verify `/api/firebase-config` endpoint is accessible
- Check browser console for 404 or 500 errors

### "Firebase not initialized"
- Service worker may not have activated yet
- Hard refresh browser (Ctrl+Shift+R)
- Unregister old service worker: DevTools → Application → Service Workers → Unregister

### "Permission denied"
- User must grant notification permission in browser
- Check browser site settings allow notifications
- Try in incognito/private window to reset permissions

---

## Security Notes

- ✅ `FIREBASE_CREDENTIALS` (service account) is **SERVER-ONLY** - never exposed to client
- ✅ Firebase client config (apiKey, projectId, etc.) is **SAFE to expose** - it's public by design
- ✅ VAPID key is **SAFE to expose** - it's meant for client-side use
- ⚠️ Still restrict Firebase API keys in Firebase Console → Project Settings → Restrictions

---

## What Changed

**Files Modified:**
- `firebase-messaging-sw.js` - Now fetches config from server instead of hardcoded values
- `firebase-messaging-helper.js` - Added `loadConfigFromServer()` and auto-initialize option

**Files Created:**
- `Controllers/FirebaseConfigController.cs` - API endpoint to serve Firebase config from environment variables

**No Breaking Changes:** Old manual initialization still works if you pass config explicitly.
