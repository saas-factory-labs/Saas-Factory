window.auth = {
    fetchAndStoreToken: async function () {
        try {
            const response = await fetch('/api/token');
            if (response.ok) {
                const data = await response.json();
                const token = data.accessToken;
                if (token) {
                    localStorage.setItem('auth_token', token);
                    console.log('Token stored in localStorage.');
                }
            }
        } catch (e) {
            console.error('Error fetching or storing token:', e);
        }
    }
};
