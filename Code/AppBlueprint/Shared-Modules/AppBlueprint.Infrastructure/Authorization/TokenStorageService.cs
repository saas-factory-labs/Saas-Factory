using Microsoft.JSInterop;
using System.Text.Json;

namespace AppBlueprint.Infrastructure.Authorization
{
    /// <summary>
    /// Interface for token storage service that handles persisting authentication tokens
    /// </summary>
    public interface ITokenStorageService
    {
        /// <summary>
        /// Store the token in the browser's local storage
        /// </summary>
        /// <param name="token">Authentication token to store</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task StoreTokenAsync(string token);

        /// <summary>
        /// Get the token from the browser's local storage
        /// </summary>
        /// <returns>The stored token or null if not found</returns>
        Task<string?> GetTokenAsync();

        /// <summary>
        /// Remove the token from the browser's local storage
        /// </summary>
        /// <returns>Task representing the asynchronous operation</returns>
        Task RemoveTokenAsync();

        /// <summary>
        /// Store a generic value in browser storage
        /// </summary>
        Task StoreValueAsync(string key, string value);

        /// <summary>
        /// Get a generic value from browser storage
        /// </summary>
        Task<string?> GetValueAsync(string key);

        /// <summary>
        /// Remove a generic value from browser storage
        /// </summary>
        Task RemoveValueAsync(string key);
    }

    /// <summary>
    /// Implementation of token storage service using browser local storage
    /// </summary>
    public class TokenStorageService : ITokenStorageService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string StorageKey = "auth_token";

        /// <summary>
        /// Initializes a new instance of the TokenStorageService class
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime for browser interop</param>
        public TokenStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }

        /// <inheritdoc />
        public async Task StoreTokenAsync(string token)
        {
            ArgumentNullException.ThrowIfNull(token);

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, token);
        }

        /// <inheritdoc />
        public async Task<string?> GetTokenAsync()
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        }

        /// <inheritdoc />
        public async Task RemoveTokenAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        }

        /// <inheritdoc />
        public async Task StoreValueAsync(string key, string value)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(value);

            // Use sessionStorage instead of localStorage for better persistence across OAuth redirects
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, value);
        }

        /// <inheritdoc />
        public async Task<string?> GetValueAsync(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            // Use sessionStorage instead of localStorage for better persistence across OAuth redirects
            return await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", key);
        }

        /// <inheritdoc />
        public async Task RemoveValueAsync(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            // Use sessionStorage instead of localStorage for better persistence across OAuth redirects
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", key);
        }
    }
}