/**
 * Authentication module for token management
 */

interface AuthModule {
    fetchAndStoreToken(): Promise<void>;
}

interface TokenResponse {
    accessToken: string;
}

const auth: AuthModule = {
    async fetchAndStoreToken(): Promise<void> {
        try {
            const response = await fetch('/api/token');
            if (response.ok) {
                const data: TokenResponse = await response.json();
                const token = data.accessToken;
                if (token) {
                    localStorage.setItem('auth_token', token);
                    console.log('Token stored in localStorage');
                }
            }
        } catch (error) {
            console.error('Error fetching or storing token:', error);
        }
    }
};

// Attach to window for Blazor interop
declare global {
    interface Window {
        auth: AuthModule;
    }
}

(globalThis as unknown as Window).auth = auth;

export default auth;
