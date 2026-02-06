"use strict";
// Firebase Cloud Messaging Helper for Web Push Notifications
// This file helps initialize FCM and get push tokens in the browser
class FirebaseMessagingHelper {
    constructor() {
        this.messaging = null;
        this.vapidKey = null;
        this.firebaseConfig = null;
    }
    async loadConfigFromServer() {
        try {
            // Fetch Firebase config from server
            const configResponse = await fetch('/api/firebase-config');
            if (!configResponse.ok) {
                const errorData = await configResponse.json().catch(() => null);
                const errorMsg = (errorData === null || errorData === void 0 ? void 0 : errorData.error) || `HTTP ${configResponse.status}`;
                console.error('Failed to fetch Firebase config:', errorMsg);
                globalThis.lastFirebaseError = errorMsg;
                return false;
            }
            this.firebaseConfig = await configResponse.json();
            // Fetch VAPID key from server
            const vapidResponse = await fetch('/api/firebase-config/vapid-key');
            if (!vapidResponse.ok) {
                const errorData = await vapidResponse.json().catch(() => null);
                const errorMsg = (errorData === null || errorData === void 0 ? void 0 : errorData.error) || `HTTP ${vapidResponse.status}`;
                console.error('Failed to fetch VAPID key:', errorMsg);
                globalThis.lastFirebaseError = errorMsg;
                return false;
            }
            const vapidData = await vapidResponse.json();
            this.vapidKey = vapidData.vapidKey;
            console.log('Firebase config loaded from server');
            globalThis.lastFirebaseError = null;
            return true;
        }
        catch (error) {
            const errorMessage = error instanceof Error ? error.message : String(error);
            console.error('Failed to load Firebase config from server:', error);
            globalThis.lastFirebaseError = errorMessage;
            return false;
        }
    }
    async initialize(firebaseConfig = null, vapidKey = null) {
        try {
            // If config not provided, fetch from server
            if (!firebaseConfig || !vapidKey) {
                const loaded = await this.loadConfigFromServer();
                if (!loaded) {
                    console.error('Could not load Firebase config');
                    return false;
                }
            }
            else {
                this.firebaseConfig = firebaseConfig;
                this.vapidKey = vapidKey;
            }
            // Import Firebase modules
            const { initializeApp } = await import('https://www.gstatic.com/firebasejs/10.7.1/firebase-app.js');
            const { getMessaging } = await import('https://www.gstatic.com/firebasejs/10.7.1/firebase-messaging.js');
            // Initialize Firebase
            const app = initializeApp(this.firebaseConfig);
            this.messaging = getMessaging(app);
            console.log('Firebase initialized successfully');
            return true;
        }
        catch (error) {
            console.error('Failed to initialize Firebase:', error);
            return false;
        }
    }
    async requestPermission() {
        try {
            const permission = await Notification.requestPermission();
            console.log('Notification permission:', permission);
            return permission === 'granted';
        }
        catch (error) {
            console.error('Failed to request notification permission:', error);
            return false;
        }
    }
    async getToken() {
        if (!this.messaging) {
            console.error('Firebase messaging not initialized');
            return null;
        }
        try {
            const { getToken } = await import('https://www.gstatic.com/firebasejs/10.7.1/firebase-messaging.js');
            const currentToken = await getToken(this.messaging, {
                vapidKey: this.vapidKey,
                serviceWorkerRegistration: await this.registerServiceWorker()
            });
            if (currentToken) {
                console.log('FCM Token obtained:', currentToken.substring(0, 20) + '...');
                return currentToken;
            }
            console.log('No registration token available. Request permission to generate one.');
            return null;
        }
        catch (error) {
            console.error('An error occurred while retrieving token:', error);
            return null;
        }
    }
    async registerServiceWorker() {
        if (!('serviceWorker' in navigator)) {
            console.error('Service Worker not supported');
            return null;
        }
        try {
            const registration = await navigator.serviceWorker.register('/firebase-messaging-sw.js');
            console.log('Service Worker registered:', registration);
            return registration;
        }
        catch (error) {
            console.error('Service Worker registration failed:', error);
            return null;
        }
    }
    async setupForegroundMessageHandler(callback) {
        if (!this.messaging) {
            console.error('Firebase messaging not initialized');
            return;
        }
        try {
            const { onMessage } = await import('https://www.gstatic.com/firebasejs/10.7.1/firebase-messaging.js');
            onMessage(this.messaging, (payload) => {
                var _a, _b, _c, _d, _e, _f;
                console.log('Foreground message received:', payload);
                if (callback && typeof callback === 'function') {
                    callback(payload);
                }
                // Show browser notification if app is in foreground
                if (Notification.permission === 'granted') {
                    const notificationTitle = ((_a = payload.notification) === null || _a === void 0 ? void 0 : _a.title) || 'New Notification';
                    const notificationOptions = {
                        body: ((_b = payload.notification) === null || _b === void 0 ? void 0 : _b.body) || ((_c = payload.data) === null || _c === void 0 ? void 0 : _c.body) || '',
                        icon: ((_d = payload.notification) === null || _d === void 0 ? void 0 : _d.icon) || '/favicon.png',
                        badge: '/favicon.png',
                        tag: ((_e = payload.data) === null || _e === void 0 ? void 0 : _e.notificationId) || 'notification',
                        data: {
                            url: ((_f = payload.data) === null || _f === void 0 ? void 0 : _f.actionUrl) || '/'
                        }
                    };
                    new Notification(notificationTitle, notificationOptions);
                }
            });
        }
        catch (error) {
            console.error('Failed to setup foreground message handler:', error);
        }
    }
    async requestTokenAndPermission() {
        const hasPermission = await this.requestPermission();
        if (!hasPermission) {
            console.error('Notification permission denied');
            return null;
        }
        return await this.getToken();
    }
}
// Export for use in Blazor
globalThis.firebaseMessaging = new FirebaseMessagingHelper();
// Helper function for Blazor interop - Initialize with server config
globalThis.initializeFirebaseMessaging = async (config = null, vapidKey = null) => {
    // If no config provided, will fetch from server automatically
    return await globalThis.firebaseMessaging.initialize(config, vapidKey);
};
// Initialize automatically on page load (fetches config from server)
globalThis.initializeFirebaseMessagingFromServer = async () => {
    return await globalThis.firebaseMessaging.initialize();
};
globalThis.requestNotificationPermission = async () => {
    return await globalThis.firebaseMessaging.requestPermission();
};
globalThis.getFirebaseToken = async () => {
    return await globalThis.firebaseMessaging.getToken();
};
globalThis.requestFirebaseTokenAndPermission = async () => {
    return await globalThis.firebaseMessaging.requestTokenAndPermission();
};
globalThis.setupFirebaseForegroundHandler = async (dotnetHelper, methodName) => {
    await globalThis.firebaseMessaging.setupForegroundMessageHandler((payload) => {
        if (dotnetHelper && methodName) {
            dotnetHelper.invokeMethodAsync(methodName, payload);
        }
    });
};
//# sourceMappingURL=firebase-messaging-helper.js.map