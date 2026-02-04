# Firebase Push Notification Setup Guide

This guide walks through setting up Firebase Cloud Messaging (FCM) for push notifications in the AppBlueprint application.

## Prerequisites

- Firebase account (create at https://console.firebase.google.com)
- Firebase project created
- Admin access to the AppBlueprint deployment

## Setup Steps

### 1. Create Firebase Project

1. Go to https://console.firebase.google.com
2. Click "Add project" or select existing project
3. Follow the wizard to create your project
4. Enable Google Analytics (optional but recommended)

### 2. Register Web App

1. In Firebase Console, click the gear icon → Project settings
2. Scroll to "Your apps" section
3. Click the web icon (`</>`) to add a web app
4. Enter app nickname (e.g., "AppBlueprint Web")
5. **Important**: Check "Also set up Firebase Hosting" (optional)
6. Click "Register app"
7. Copy the Firebase configuration object - you'll need these values

### 3. Generate VAPID Key for Web Push

1. In Firebase Console → Project settings → Cloud Messaging tab
2. Scroll to "Web configuration" section
3. Click "Generate key pair" under "Web Push certificates"
4. Copy the VAPID key (starts with `B...`)
5. Store this key securely - you'll add it to environment variables

### 4. Get Firebase Admin SDK Service Account

1. In Firebase Console → Project settings → Service accounts tab
2. Click "Generate new private key"
3. Confirm and download the JSON file
4. **Important**: Store this file securely - it contains sensitive credentials
5. You'll need the entire JSON content for the FIREBASE_CREDENTIALS environment variable

### 5. Configure AppBlueprint.Web

#### Option A: Update firebase-messaging-sw.js (Service Worker)

Edit `/Code/AppBlueprint/AppBlueprint.Web/wwwroot/firebase-messaging-sw.js`:

```javascript
const firebaseConfig = {
  apiKey: "YOUR_API_KEY",
  authDomain: "YOUR_PROJECT_ID.firebaseapp.com",
  projectId: "YOUR_PROJECT_ID",
  storageBucket: "YOUR_PROJECT_ID.appspot.com",
  messagingSenderId: "YOUR_MESSAGING_SENDER_ID",
  appId: "YOUR_APP_ID",
  measurementId: "YOUR_MEASUREMENT_ID" // Optional
};
```

Replace the placeholder values with your actual Firebase configuration from Step 2.

#### Option B: Environment Variables (Recommended for production)

Instead of hardcoding in the service worker, you can inject these values via environment variables and generate the service worker dynamically.

### 6. Set Environment Variables

Add these environment variables to your deployment environment (Azure App Service, Docker, etc.):

```bash
# Firebase Server-Side (Admin SDK)
FIREBASE_CREDENTIALS='{"type":"service_account","project_id":"your-project","private_key_id":"...","private_key":"-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n","client_email":"...","client_id":"...","auth_uri":"...","token_uri":"...","auth_provider_x509_cert_url":"...","client_x509_cert_url":"..."}'

# VAPID Key for Web Push
FIREBASE_VAPID_KEY="YOUR_VAPID_KEY_HERE"

# Firebase Client Configuration (optional - can be hardcoded in JS)
FIREBASE_API_KEY="YOUR_API_KEY"
FIREBASE_AUTH_DOMAIN="YOUR_PROJECT_ID.firebaseapp.com"
FIREBASE_PROJECT_ID="YOUR_PROJECT_ID"
FIREBASE_STORAGE_BUCKET="YOUR_PROJECT_ID.appspot.com"
FIREBASE_MESSAGING_SENDER_ID="YOUR_MESSAGING_SENDER_ID"
FIREBASE_APP_ID="YOUR_APP_ID"
```

**Important Notes:**
- `FIREBASE_CREDENTIALS` must be the entire JSON content from the service account file (Step 4)
- For environment variables, use single quotes to preserve JSON structure
- For Docker, escape quotes properly or use Docker secrets

### 7. Verify Service Worker Registration

After deploying, verify the service worker is registered:

1. Open browser DevTools (F12)
2. Go to Application tab → Service Workers
3. Verify `firebase-messaging-sw.js` is registered
4. Check Console for any errors

### 8. Test Notifications

#### Using the Demo Page

1. Navigate to `/demo/notifications` in your app
2. Click "Register for Push Notifications" - grant browser permissions
3. Copy the FCM token displayed
4. Send a test notification using the form
5. Select "Push" channel and "Info" type
6. Enter your user ID and a message
7. Click "Send Notification"

#### Using Backend Service Directly

```csharp
using AppBlueprint.Application.Interfaces;

// Inject IPushNotificationService
var request = new PushNotificationRequest(
    userId: "usr_...",
    title: "Test Notification",
    body: "This is a test push notification",
    imageUrl: null,
    actionUrl: "/dashboard",
    data: new Dictionary<string, string>
    {
        ["type"] = "test",
        ["priority"] = "high"
    }
);

await pushNotificationService.SendAsync(request);
```

## Troubleshooting

### Service Worker Not Registering

- Check browser console for errors
- Verify service worker file is accessible at `/firebase-messaging-sw.js`
- Ensure HTTPS is enabled (required for service workers, except localhost)
- Clear browser cache and re-register

### Notifications Not Received

1. **Check FCM token is active**:
   - Tokens expire or become invalid if user clears browser data
   - Re-register to get a new token

2. **Verify FIREBASE_CREDENTIALS**:
   - Ensure JSON is valid and unescaped
   - Check service account has "Firebase Cloud Messaging API" permissions

3. **Check browser permissions**:
   - User must grant notification permission
   - Check Settings → Site Settings → Notifications

4. **Inspect logs**:
   - Check application logs for FCM errors
   - Look for "InvalidArgument", "Unregistered", or "SenderIdMismatch" errors

### CORS Issues

If using separate domains for API and web:
- Add Firebase Cloud Messaging domain to CORS policy
- Ensure service worker is served from same origin as app

## Security Best Practices

1. **Never commit Firebase credentials to source control**
   - Use environment variables or secrets management
   - Add service account JSON files to `.gitignore`

2. **Restrict Firebase API key**
   - In Firebase Console → Credentials → API Keys
   - Restrict key to specific HTTP referrers (your domain)

3. **Rotate service account keys regularly**
   - Generate new keys periodically
   - Revoke old keys after migration

4. **Validate notification requests**
   - Ensure users can only send notifications they're authorized for
   - Implement rate limiting to prevent abuse

## Production Checklist

- [ ] Firebase project created and configured
- [ ] VAPID key generated and added to environment variables
- [ ] Service account JSON downloaded and stored securely
- [ ] `FIREBASE_CREDENTIALS` environment variable set
- [ ] `firebase-messaging-sw.js` updated with correct config
- [ ] Service worker successfully registered in browser
- [ ] Test notifications sent and received successfully
- [ ] Browser permissions granted by test users
- [ ] Notification preferences working correctly
- [ ] Token registration and cleanup working
- [ ] Error handling and logging verified
- [ ] HTTPS enabled on production domain

## Additional Resources

- [Firebase Cloud Messaging Documentation](https://firebase.google.com/docs/cloud-messaging)
- [Web Push Notifications Guide](https://firebase.google.com/docs/cloud-messaging/js/client)
- [Service Workers API](https://developer.mozilla.org/en-US/docs/Web/API/Service_Worker_API)
- [Push API](https://developer.mozilla.org/en-US/docs/Web/API/Push_API)
- [Notification API](https://developer.mozilla.org/en-US/docs/Web/API/Notifications_API)

## Support

If you encounter issues:
1. Check application logs for detailed error messages
2. Review Firebase Console → Cloud Messaging → Logs
3. Verify all environment variables are set correctly
4. Test with the demo page at `/demo/notifications`
5. Check browser console for JavaScript errors
