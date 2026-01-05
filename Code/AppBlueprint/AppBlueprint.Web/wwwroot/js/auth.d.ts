/**
 * Authentication module for token management
 */
interface AuthModule {
    fetchAndStoreToken(): Promise<void>;
}
declare const auth: AuthModule;
declare global {
    interface Window {
        auth: AuthModule;
    }
}
export default auth;
//# sourceMappingURL=auth.d.ts.map