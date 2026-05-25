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
declare const firebaseAuth: FirebaseAuthModule;
export default firebaseAuth;
//# sourceMappingURL=firebaseAuth.d.ts.map