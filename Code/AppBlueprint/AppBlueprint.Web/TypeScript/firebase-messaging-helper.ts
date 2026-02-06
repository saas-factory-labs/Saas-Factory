// Firebase Cloud Messaging Helper for Web Push Notifications
// This file helps initialize FCM and get push tokens in the browser

interface FirebaseConfig {
    apiKey: string;
    authDomain: string;
    projectId: string;
    storageBucket: string;
    messagingSenderId: string;
    appId: string;
    measurementId?: string;
}

interface VapidKeyResponse {
    vapidKey: string;
}

interface NotificationPayload {
    notification?: {
        title?: string;
        body?: string;
        icon?: string;
        image?: string;
    };
    data?: {
        [key: string]: string;
    };
}

class FirebaseMessagingHelper {
    private messaging: any = null;
    private vapidKey: string | null = null;
    private firebaseConfig: FirebaseConfig | null = null;

    async loadConfigFromServer(): Promise<boolean> {
        try {
            // Fetch Firebase config from server
            const configResponse = await fetch('/api/firebase-config');
            if (!configResponse.ok) {
                const errorData = await configResponse.json().catch(() => null);
                const errorMsg = errorData?.error || `HTTP ${configResponse.status}`;
                console.error('Failed to fetch Firebase config:', errorMsg);
                (window as any).lastFirebaseError = errorMsg;
                return false;
            }
            this.firebaseConfig = await configResponse.json();

            // Fetch VAPID key from server
            const vapidResponse = await fetch('/api/firebase-config/vapid-key');
            if (!vapidResponse.ok) {
                const errorData = await vapidResponse.json().catch(() => null);
                const errorMsg = errorData?.error || `HTTP ${vapidResponse.status}`;
                console.error('Failed to fetch VAPID key:', errorMsg);
                (window as any).lastFirebaseError = errorMsg;
                return false;
            }
            const vapidData: VapidKeyResponse = await vapidResponse.json();
            this.vapidKey = vapidData.vapidKey;

            console.log('Firebase config loaded from server');
            (window as any).lastFirebaseError = null;
            return true;
        } catch (error) {
            const errorMessage = error instanceof Error ? error.message : String(error);
            console.error('Failed to load Firebase config from server:', error);
            (window as any).lastFirebaseError = errorMessage;
            return false;
        }
    }

    async initialize(firebaseConfig: FirebaseConfig | null = null, vapidKey: string | null = null): Promise<boolean> {
        try {
            // If config not provided, fetch from server
            if (!firebaseConfig || !vapidKey) {
                const loaded = await this.loadConfigFromServer();
                if (!loaded) {
                    console.error('Could not load Firebase config');
                    return false;
                }
            } else {
                this.firebaseConfig = firebaseConfig;
                this.vapidKey = vapidKey;
            }

            // Import Firebase modules
            const { initializeApp } = await import('https://www.gstatic.com/firebasejs/10.7.1/firebase-app.js' as any);
            const { getMessaging } = await import('https://www.gstatic.com/firebasejs/10.7.1/firebase-messaging.js' as any);

            // Initialize Firebase
            const app = initializeApp(this.firebaseConfig!);
            this.messaging = getMessaging(app);

            console.log('Firebase initialized successfully');
            return true;
        } catch (error) {
            console.error('Failed to initialize Firebase:', error);
            return false;
        }
    }

    async requestPermission(): Promise<boolean> {
        try {
            const permission = await Notification.requestPermission();
            console.log('Notification permission:', permission);
            return permission === 'granted';
        } catch (error) {
            console.error('Failed to request notification permission:', error);
            return false;
        }
    }

    async getToken(): Promise<string | null> {
        if (!this.messaging) {
            console.error('Firebase messaging not initialized');
            return null;
        }

        try {
            const { getToken } = await import('https://www.gstatic.com/firebasejs/10.7.1/firebase-messaging.js' as any);
            
            const currentToken = await getToken(this.messaging, {
                vapidKey: this.vapidKey!,
                serviceWorkerRegistration: await this.registerServiceWorker()
            });

            if (currentToken) {
                console.log('FCM Token obtained:', currentToken.substring(0, 20) + '...');
                return currentToken;
            }
            console.log('No registration token available. Request permission to generate one.');
            return null;
        } catch (error) {
            console.error('An error occurred while retrieving token:', error);
            return null;
        }
    }

    async registerServiceWorker(): Promise<ServiceWorkerRegistration | null> {
        if (!('serviceWorker' in navigator)) {
            console.error('Service Worker not supported');
            return null;
        }

        try {
            const registration = await navigator.serviceWorker.register('/firebase-messaging-sw.js');
            console.log('Service Worker registered:', registration);
            return registration;
        } catch (error) {
            console.error('Service Worker registration failed:', error);
            return null;
        }
    }

    async setupForegroundMessageHandler(callback?: (payload: NotificationPayload) => void): Promise<void> {
        if (!this.messaging) {
            console.error('Firebase messaging not initialized');
            return;
        }

        try {
            const { onMessage } = await import('https://www.gstatic.com/firebasejs/10.7.1/firebase-messaging.js' as any);
            
            onMessage(this.messaging, (payload: NotificationPayload) => {
                console.log('Foreground message received:', payload);
                
                if (callback && typeof callback === 'function') {
                    callback(payload);
                }

                // Show browser notification if app is in foreground
                if (Notification.permission === 'granted') {
                    const notificationTitle = payload.notification?.title || 'New Notification';
                    const notificationOptions: NotificationOptions = {
                        body: payload.notification?.body || payload.data?.body || '',
                        icon: payload.notification?.icon || '/favicon.png',
                        badge: '/favicon.png',
                        tag: payload.data?.notificationId || 'notification',
                        data: {
                            url: payload.data?.actionUrl || '/'
                        }
                    };

                    new Notification(notificationTitle, notificationOptions);
                }
            });
        } catch (error) {
            console.error('Failed to setup foreground message handler:', error);
        }
    }

    async requestTokenAndPermission(): Promise<string | null> {
        const hasPermission = await this.requestPermission();
        if (!hasPermission) {
            console.error('Notification permission denied');
            return null;
        }

        return await this.getToken();
    }
}

// Export for use in Blazor
(window as any).firebaseMessaging = new FirebaseMessagingHelper();

// Helper function for Blazor interop - Initialize with server config
(window as any).initializeFirebaseMessaging = async (config: FirebaseConfig | null = null, vapidKey: string | null = null): Promise<boolean> => {
    // If no config provided, will fetch from server automatically
    return await (window as any).firebaseMessaging.initialize(config, vapidKey);
};

// Initialize automatically on page load (fetches config from server)
(window as any).initializeFirebaseMessagingFromServer = async (): Promise<boolean> => {
    return await (window as any).firebaseMessaging.initialize();
};

(window as any).requestNotificationPermission = async (): Promise<boolean> => {
    return await (window as any).firebaseMessaging.requestPermission();
};

(window as any).getFirebaseToken = async (): Promise<string | null> => {
    return await (window as any).firebaseMessaging.getToken();
};

(window as any).requestFirebaseTokenAndPermission = async (): Promise<string | null> => {
    return await (window as any).firebaseMessaging.requestTokenAndPermission();
};

(window as any).setupFirebaseForegroundHandler = async (dotnetHelper: any, methodName: string): Promise<void> => {
    await (window as any).firebaseMessaging.setupForegroundMessageHandler((payload: NotificationPayload) => {
        if (dotnetHelper && methodName) {
            dotnetHelper.invokeMethodAsync(methodName, payload);
        }
    });
};
