// @ts-nocheck
/**
 * Firebase Authentication module for Blazor Server JSInterop.
 * Loads Firebase via CDN module imports and exposes sign-in/sign-out methods.
 */

interface FirebaseAuthConfig {
    apiKey: string;
    authDomain: string;
    projectId: string;
    storageBucket?: string;
    messagingSenderId?: string;
    appId?: string;
}

interface FirebaseSignInResult {
    success: boolean;
    idToken?: string;
    email?: string;
    displayName?: string;
    error?: string;
}

interface FirebaseExchangeResult {
    success: boolean;
    redirectTo?: string;
    error?: string;
}

interface FirebaseAuthModule {
    initialize(config: FirebaseAuthConfig): Promise<void>;
    signInWithGoogle(): Promise<FirebaseSignInResult>;
    exchangeIdToken(idToken: string): Promise<FirebaseExchangeResult>;
    signOut(): Promise<void>;
    getIdToken(): Promise<string | null>;
}

declare global {
    interface Window {
        firebaseAuth: FirebaseAuthModule;
    }
}

// Dynamically load Firebase SDK from CDN to keep the main bundle small
async function loadFirebaseModules(config: FirebaseAuthConfig) {
    const { initializeApp, getApps } = await import('https://www.gstatic.com/firebasejs/11.0.0/firebase-app.js');
    const { getAuth, signInWithPopup, GoogleAuthProvider, signOut: fbSignOut, getIdToken: fbGetIdToken } =
        await import('https://www.gstatic.com/firebasejs/11.0.0/firebase-auth.js');

    const app = getApps().length === 0 ? initializeApp(config) : getApps()[0];
    const auth = getAuth(app);
    return { auth, signInWithPopup, GoogleAuthProvider, fbSignOut, fbGetIdToken };
}

let firebaseModulesCache: Awaited<ReturnType<typeof loadFirebaseModules>> | null = null;
let firebaseConfig: FirebaseAuthConfig | null = null;

const firebaseAuth: FirebaseAuthModule = {
    async initialize(config: FirebaseAuthConfig): Promise<void> {
        firebaseConfig = config;
        firebaseModulesCache = await loadFirebaseModules(config);
        console.log('[Firebase] Initialized for project:', config.projectId);
    },

    async signInWithGoogle(): Promise<FirebaseSignInResult> {
        if (!firebaseModulesCache || !firebaseConfig) {
            return { success: false, error: 'Firebase not initialized. Call initialize() first.' };
        }

        const { auth, signInWithPopup, GoogleAuthProvider, fbGetIdToken } = firebaseModulesCache;

        try {
            const provider = new GoogleAuthProvider();
            const result = await signInWithPopup(auth, provider);
            const idToken = await fbGetIdToken(result.user, false);

            return {
                success: true,
                idToken,
                email: result.user.email ?? undefined,
                displayName: result.user.displayName ?? undefined
            };
        } catch (error: unknown) {
            const message = error instanceof Error ? error.message : 'Unknown error';
            console.error('[Firebase] Google sign-in failed:', message);
            return { success: false, error: message };
        }
    },

    async exchangeIdToken(idToken: string): Promise<FirebaseExchangeResult> {
        try {
            const response = await fetch('/auth/firebase/callback', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ idToken }),
                credentials: 'same-origin'
            });

            if (!response.ok) {
                const errorBody = await response.json().catch(() => ({ error: 'Unknown server error' }));
                return { success: false, error: errorBody.error || `Server error: ${response.status}` };
            }

            const data = await response.json();
            return { success: true, redirectTo: data.redirectTo || '/dashboard' };
        } catch (error: unknown) {
            const message = error instanceof Error ? error.message : 'Network error';
            console.error('[Firebase] Token exchange failed:', message);
            return { success: false, error: message };
        }
    },

    async signOut(): Promise<void> {
        if (!firebaseModulesCache) return;
        const { auth, fbSignOut } = firebaseModulesCache;
        await fbSignOut(auth);
        console.log('[Firebase] User signed out');
    },

    async getIdToken(): Promise<string | null> {
        if (!firebaseModulesCache) return null;
        const { auth } = firebaseModulesCache;
        const user = auth.currentUser;
        if (!user) return null;
        return firebaseModulesCache.fbGetIdToken(user, false);
    }
};

window.firebaseAuth = firebaseAuth;
export default firebaseAuth;
