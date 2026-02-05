/**
 * Authentication module for token management
 */
const auth = {
    async fetchAndStoreToken() {
        try {
            const response = await fetch('/api/token');
            if (response.ok) {
                const data = await response.json();
                const token = data.accessToken;
                if (token) {
                    localStorage.setItem('auth_token', token);
                    console.log('Token stored in localStorage');
                }
            }
        }
        catch (error) {
            console.error('Error fetching or storing token:', error);
        }
    }
};
window.auth = auth;
export default auth;
//# sourceMappingURL=auth.js.map