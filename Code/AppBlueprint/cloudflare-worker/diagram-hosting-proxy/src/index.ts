/**
 * Welcome to Cloudflare Workers! This is your first worker.
 *
 * - Run `npm run dev` in your terminal to start a development server
 * - Open a browser tab at http://localhost:8787/ to see your worker in action
 * - Run `npm run deploy` to publish your worker
 *
 * Bind resources to your worker in `wrangler.json`. After adding bindings, a type definition for the
 * `Env` object can be regenerated with `npm run cf-typegen`.
 *
 * Learn more at https://developers.cloudflare.com/workers/
 */
export default {
	async fetch(request: Request, env: any): Promise<Response> {
		const url = new URL(request.url);

		if (url.pathname === '/schema.sql') {
			const GITHUB_RAW_URL = 'https://raw.githubusercontent.com/Trubador/SaaS-Factory/main/schema.sql';
			const GITHUB_TOKEN = env.GITHUB_TOKEN;


			try {
				// Fetch the file from GitHub
				const githubResponse = await fetch(GITHUB_RAW_URL, {
					headers: {
						Authorization: `Bearer ${GITHUB_TOKEN}`,
					},
				});

				if (!githubResponse.ok) {
					return new Response('Error fetching file from GitHub.', { status: githubResponse.status });
				}

				// Serve the file content with CORS headers
				const fileContent = await githubResponse.text();
				return new Response(fileContent, {
					headers: {
						'Content-Type': 'text/plain',
						'Access-Control-Allow-Origin': '*', // Allow any origin
						'Access-Control-Allow-Methods': 'GET, OPTIONS',
						'Access-Control-Allow-Headers': '*',
						'Cache-Control': 'no-cache',
					},
				});
			} catch (error) {
				console.error('Error fetching file:', error);
				return new Response('Internal Server Error', { status: 500 });
			}
		}

		// Default response for unknown paths
		return new Response('Not Found', { status: 404 });
	},
};




