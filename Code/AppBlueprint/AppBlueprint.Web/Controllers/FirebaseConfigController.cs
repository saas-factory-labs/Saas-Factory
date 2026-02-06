using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Web.Controllers;

/// <summary>
/// Provides Firebase client configuration from server environment variables.
/// This allows the service worker to access Firebase config without hardcoding values.
/// </summary>
[ApiController]
[Route("api/firebase-config")]
[Microsoft.AspNetCore.Authorization.AllowAnonymous]
internal sealed class FirebaseConfigController(IConfiguration configuration) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Get Firebase client configuration for browser initialization.
    /// Does NOT expose the service account credentials (those are server-only).
    /// </summary>
    [HttpGet]
    public IActionResult GetFirebaseConfig()
    {
        string? apiKey = _configuration["FIREBASE_API_KEY"];
        string? authDomain = _configuration["FIREBASE_AUTH_DOMAIN"];
        string? projectId = _configuration["FIREBASE_PROJECT_ID"];
        string? storageBucket = _configuration["FIREBASE_STORAGE_BUCKET"];
        string? messagingSenderId = _configuration["FIREBASE_MESSAGING_SENDER_ID"];
        string? appId = _configuration["FIREBASE_APP_ID"];

        // Check if any required values are missing
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(appId))
        {
            return BadRequest(new
            {
                error = "Firebase configuration is incomplete. Please set these environment variables: FIREBASE_API_KEY, FIREBASE_PROJECT_ID, FIREBASE_APP_ID, FIREBASE_MESSAGING_SENDER_ID, FIREBASE_STORAGE_BUCKET, FIREBASE_AUTH_DOMAIN"
            });
        }

        // Return public client config (safe to expose)
        return Ok(new
        {
            apiKey,
            authDomain = authDomain ?? $"{projectId}.firebaseapp.com",
            projectId,
            storageBucket = storageBucket ?? $"{projectId}.appspot.com",
            messagingSenderId = messagingSenderId ?? "",
            appId
        });
    }

    /// <summary>
    /// Get VAPID key for web push notifications.
    /// </summary>
    [HttpGet("vapid-key")]
    public IActionResult GetVapidKey()
    {
        string? vapidKey = _configuration["FIREBASE_VAPID_KEY"];

        if (string.IsNullOrEmpty(vapidKey))
        {
            return BadRequest(new
            {
                error = "FIREBASE_VAPID_KEY environment variable is not set"
            });
        }

        return Ok(new
        {
            vapidKey
        });
    }
}
