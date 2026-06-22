# Firebase Environment Variables - Action Required

## ⚠️ Required Firebase Configuration

The notification system requires these environment variables to be set in **Doppler**:

### Firebase Client Config (for browser)
```bash
FIREBASE_API_KEY=your-api-key-here
FIREBASE_PROJECT_ID=your-project-id
FIREBASE_APP_ID=your-app-id
FIREBASE_MESSAGING_SENDER_ID=your-sender-id
FIREBASE_STORAGE_BUCKET=your-project-id.appspot.com
FIREBASE_AUTH_DOMAIN=your-project-id.firebaseapp.com
FIREBASE_VAPID_KEY=your-vapid-public-key
```

### Firebase Server Config (for FCM admin)
```bash
FIREBASE_CREDENTIALS={"type":"service_account","project_id":"...","private_key":"..."}
```

## Where to Get These Values

### Client Configuration (Firebase Console)
1. Go to https://console.firebase.google.com
2. Select your project (or create one)
3. Go to **Project Settings** (gear icon)
4. Scroll down to **Your apps** → **Web app**
5. Copy the config values:
   - `apiKey` → `FIREBASE_API_KEY`
   - `projectId` → `FIREBASE_PROJECT_ID`
   - `appId` → `FIREBASE_APP_ID`
   - `messagingSenderId` → `FIREBASE_MESSAGING_SENDER_ID`
   - `storageBucket` → `FIREBASE_STORAGE_BUCKET`
   - `authDomain` → `FIREBASE_AUTH_DOMAIN`

### VAPID Key (for Web Push)
1. In Firebase Console → **Project Settings**
2. Go to **Cloud Messaging** tab
3. Scroll to **Web configuration**
4. Under **Web Push certificates**, click **Generate key pair**
5. Copy the key → `FIREBASE_VAPID_KEY`

### Service Account (for server-side FCM)
1. In Firebase Console → **Project Settings**
2. Go to **Service Accounts** tab
3. Click **Generate new private key**
4. Download the JSON file
5. Copy the **entire JSON content** (as a single line) → `FIREBASE_CREDENTIALS`

## How to Set in Doppler

```bash
# Option 1: Doppler CLI
doppler secrets set FIREBASE_API_KEY "your-key" --project appblueprint --config dev
doppler secrets set FIREBASE_PROJECT_ID "your-project-id" --project appblueprint --config dev
# ... repeat for all variables

# Option 2: Doppler Dashboard
# 1. Go to https://dashboard.doppler.com
# 2. Select AppBlueprint project
# 3. Select environment (dev/staging/production)
# 4. Click "Add Secret" for each variable
```

## Verification

After setting the variables:
1. Restart the Web app
2. Navigate to `/demo/notifications`
3. Click "Request Permission & Get Token"
4. Check that Firebase initializes without errors in browser console
5. FCM token should be generated successfully

## Current Status

❌ **Not Configured** - FirebaseConfigController returns HTTP 400:
```
Firebase configuration is incomplete. Please set these environment variables:
FIREBASE_API_KEY, FIREBASE_PROJECT_ID, FIREBASE_APP_ID, 
FIREBASE_MESSAGING_SENDER_ID, FIREBASE_STORAGE_BUCKET, FIREBASE_AUTH_DOMAIN
```

Once configured, delete this file or mark as ✅ **Configured**.
