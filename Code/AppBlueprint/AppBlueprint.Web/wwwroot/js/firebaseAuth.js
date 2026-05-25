"use strict";
// @ts-nocheck
/**
 * Firebase Authentication module for Blazor Server JSInterop.
 * Loads Firebase via CDN module imports and exposes sign-in/sign-out methods.
 */
Object.defineProperty(exports, "__esModule", { value: true });
// Dynamically load Firebase SDK from CDN to keep the main bundle small
async function loadFirebaseModules(config) {
    const { initializeApp, getApps } = await import('https://www.gstatic.com/firebasejs/11.0.0/firebase-app.js');
    const { getAuth, signInWithPopup, GoogleAuthProvider, signOut: fbSignOut, getIdToken: fbGetIdToken } = await import('https://www.gstatic.com/firebasejs/11.0.0/firebase-auth.js');
    const app = getApps().length === 0 ? initializeApp(config) : getApps()[0];
    const auth = getAuth(app);
    return { auth, signInWithPopup, GoogleAuthProvider, fbSignOut, fbGetIdToken };
}
let firebaseModulesCache = null;
let firebaseConfig = null;
const firebaseAuth = {
    async initialize(config) {
        firebaseConfig = config;
        firebaseModulesCache = await loadFirebaseModules(config);
        console.log('[Firebase] Initialized for project:', config.projectId);
    },
    async signInWithGoogle() {
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
        }
        catch (error) {
            const message = error instanceof Error ? error.message : 'Unknown error';
            console.error('[Firebase] Google sign-in failed:', message);
            return { success: false, error: message };
        }
    },
    async exchangeIdToken(idToken) {
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
        }
        catch (error) {
            const message = error instanceof Error ? error.message : 'Network error';
            console.error('[Firebase] Token exchange failed:', message);
            return { success: false, error: message };
        }
    },
    async signOut() {
        if (!firebaseModulesCache)
            return;
        const { auth, fbSignOut } = firebaseModulesCache;
        await fbSignOut(auth);
        console.log('[Firebase] User signed out');
    },
    async getIdToken() {
        if (!firebaseModulesCache)
            return null;
        const { auth } = firebaseModulesCache;
        const user = auth.currentUser;
        if (!user)
            return null;
        return firebaseModulesCache.fbGetIdToken(user, false);
    }
};
window.firebaseAuth = firebaseAuth;
exports.default = firebaseAuth;
//# sourceMappingURL=firebaseAuth.js.map