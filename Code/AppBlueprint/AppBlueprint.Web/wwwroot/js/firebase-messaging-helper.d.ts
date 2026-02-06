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
declare class FirebaseMessagingHelper {
    private messaging;
    private vapidKey;
    private firebaseConfig;
    loadConfigFromServer(): Promise<boolean>;
    initialize(firebaseConfig?: FirebaseConfig | null, vapidKey?: string | null): Promise<boolean>;
    requestPermission(): Promise<boolean>;
    getToken(): Promise<string | null>;
    registerServiceWorker(): Promise<ServiceWorkerRegistration | null>;
    setupForegroundMessageHandler(callback?: (payload: NotificationPayload) => void): Promise<void>;
    requestTokenAndPermission(): Promise<string | null>;
}
//# sourceMappingURL=firebase-messaging-helper.d.ts.map