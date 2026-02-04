// Firebase Cloud Messaging Service Worker for Web Push Notifications
// Place this file in the wwwroot folder as firebase-messaging-sw.js

importScripts('https://www.gstatic.com/firebasejs/10.7.1/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/10.7.1/firebase-messaging-compat.js');

// Firebase config will be fetched from server endpoint
// This allows using environment variables without hardcoding values in static files
let messaging = null;
let isInitialized = false;

// Fetch Firebase config from server and initialize
async function initializeFirebase() {
  if (isInitialized) return;
  
  try {
    const response = await fetch('/api/firebase-config');
    if (!response.ok) {
      console.error('[firebase-messaging-sw.js] Failed to fetch Firebase config:', response.status);
      return;
    }
    
    const firebaseConfig = await response.json();
    console.log('[firebase-messaging-sw.js] Firebase config loaded from server');
    
    // Initialize Firebase with config from server
    firebase.initializeApp(firebaseConfig);
    messaging = firebase.messaging();
    isInitialized = true;
    
    // Set up message handler after initialization
    setupMessageHandler();
    
  } catch (error) {
    console.error('[firebase-messaging-sw.js] Error initializing Firebase:', error);
  }
}

// Set up the background message handler
function setupMessageHandler() {
  if (!messaging) return;

  // Handle background messages (when app is not in focus)
  messaging.onBackgroundMessage((payload) => {
    console.log('[firebase-messaging-sw.js] Received background message:', payload);

  const notificationTitle = payload.notification?.title || payload.data?.title || 'New Notification';
  const notificationOptions = {
    body: payload.notification?.body || payload.data?.body || 'You have a new notification',
    icon: payload.notification?.icon || '/favicon.png',
    badge: '/favicon.png',
    tag: payload.data?.notificationId || 'notification',
    data: {
      url: payload.data?.actionUrl || payload.fcmOptions?.link || '/',
      notificationId: payload.data?.notificationId
    },
    requireInteraction: false,
    vibrate: [200, 100, 200]
  };

  // Add image if provided
  if (payload.notification?.image || payload.data?.imageUrl) {
    notificationOptions.image = payload.notification.image || payload.data.imageUrl;
  }

    // Show notification
    return self.registration.showNotification(notificationTitle, notificationOptions);
  });
}

// Handle notification clicks
self.addEventListener('notificationclick', (event) => {
  console.log('[firebase-messaging-sw.js] Notification clicked:', event);

  event.notification.close();

  const urlToOpen = event.notification.data?.url || '/';

  event.waitUntil(
    clients.matchAll({ type: 'window', includeUncontrolled: true }).then((clientList) => {
      // Check if there's already a window open with this URL
      for (const client of clientList) {
        if (client.url === urlToOpen && 'focus' in client) {
          return client.focus();
        }
      }

      // If no window is open, open a new one
      if (clients.openWindow) {
        return clients.openWindow(urlToOpen);
      }
    })
  );
});

// Handle notification close
self.addEventListener('notificationclose', (event) => {
  console.log('[firebase-messaging-sw.js] Notification closed:', event);
});

// Service worker activation - initialize Firebase
self.addEventListener('activate', (event) => {
  console.log('[firebase-messaging-sw.js] Service Worker activated');
  event.waitUntil(
    Promise.all([
      clients.claim(),
      initializeFirebase()
    ])
  );
});

// Service worker installation
self.addEventListener('install', (event) => {
  console.log('[firebase-messaging-sw.js] Service Worker installed');
  self.skipWaiting();
});
