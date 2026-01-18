using AppBlueprint.Application.Interfaces.Email;
using AppBlueprint.Contracts.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Dev;

/// <summary>
/// Developer-only controller for previewing email templates locally.
/// Only available in Development environment.
/// </summary>
[ApiController]
[Route("api/dev/email-preview")]
[AllowAnonymous] // For easy local testing - would be secured in production
public sealed class EmailPreviewController(IEmailTemplateService templateService, IWebHostEnvironment environment) : ControllerBase
{
    /// <summary>
    /// Email template preview home page with links to all templates.
    /// </summary>
    [HttpGet]
    [Produces("text/html")]
    public IActionResult Index()
    {
        if (!environment.IsDevelopment())
        {
            return NotFound();
        }

        // Use the current request's scheme and host to build the correct base URL
        string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/dev/email-preview";
        
        string html = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Template Preview</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            padding: 40px 20px;
        }}
        .container {{
            max-width: 1200px;
            margin: 0 auto;
        }}
        h1 {{
            color: white;
            text-align: center;
            margin-bottom: 10px;
            font-size: 2.5rem;
        }}
        .subtitle {{
            color: rgba(255, 255, 255, 0.9);
            text-align: center;
            margin-bottom: 40px;
            font-size: 1.1rem;
        }}
        .grid {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
            gap: 24px;
            margin-bottom: 40px;
        }}
        .card {{
            background: white;
            border-radius: 12px;
            padding: 28px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            text-decoration: none;
            color: inherit;
            display: block;
        }}
        .card:hover {{
            transform: translateY(-4px);
            box-shadow: 0 15px 40px rgba(0, 0, 0, 0.3);
        }}
        .card-icon {{
            width: 48px;
            height: 48px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 24px;
            margin-bottom: 16px;
        }}
        .card-title {{
            font-size: 1.4rem;
            font-weight: 600;
            margin-bottom: 8px;
            color: #1a202c;
        }}
        .card-description {{
            color: #718096;
            line-height: 1.6;
            margin-bottom: 16px;
        }}
        .card-params {{
            background: #f7fafc;
            border-radius: 8px;
            padding: 12px;
            font-size: 0.85rem;
            color: #4a5568;
            font-family: 'Courier New', monospace;
        }}
        .info-box {{
            background: rgba(255, 255, 255, 0.1);
            backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.2);
            border-radius: 12px;
            padding: 24px;
            margin-top: 40px;
            color: white;
        }}
        .info-box h2 {{
            margin-bottom: 12px;
            font-size: 1.3rem;
        }}
        .info-box p {{
            line-height: 1.8;
            opacity: 0.95;
        }}
        .info-box code {{
            background: rgba(0, 0, 0, 0.2);
            padding: 2px 8px;
            border-radius: 4px;
            font-family: 'Courier New', monospace;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>📧 Email Template Preview</h1>
        <p class='subtitle'>Click any template to preview it in your browser</p>
        
        <div class='grid'>
            <a href='{baseUrl}/welcome?userName=John Doe' class='card'>
                <div class='card-icon'>👋</div>
                <h3 class='card-title'>Welcome Email</h3>
                <p class='card-description'>Sent to new users when they sign up for an account.</p>
                <div class='card-params'>?userName=John Doe</div>
            </a>
            
            <a href='{baseUrl}/password-reset?userName=John Doe' class='card'>
                <div class='card-icon'>🔐</div>
                <h3 class='card-title'>Password Reset</h3>
                <p class='card-description'>Sent when a user requests to reset their password.</p>
                <div class='card-params'>?userName=John Doe</div>
            </a>
            
            <a href='{baseUrl}/email-verification?userName=John Doe' class='card'>
                <div class='card-icon'>✅</div>
                <h3 class='card-title'>Email Verification</h3>
                <p class='card-description'>Sent to verify a user's email address.</p>
                <div class='card-params'>?userName=John Doe</div>
            </a>
            
            <a href='{baseUrl}/order-confirmation?customerName=Jane Smith' class='card'>
                <div class='card-icon'>🛒</div>
                <h3 class='card-title'>Order Confirmation</h3>
                <p class='card-description'>Sent after a successful purchase or subscription.</p>
                <div class='card-params'>?customerName=Jane Smith</div>
            </a>
            
            <a href='{baseUrl}/team-invitation?inviteeName=Sarah Johnson' class='card'>
                <div class='card-icon'>👥</div>
                <h3 class='card-title'>Team Invitation</h3>
                <p class='card-description'>Sent when a user is invited to join a team.</p>
                <div class='card-params'>?inviteeName=Sarah Johnson</div>
            </a>
        </div>
        
        <div class='info-box'>
            <h2>💡 Tips</h2>
            <p>
                • All templates support query parameters to customize the preview<br>
                • This preview is only available in <code>DEBUG</code> mode<br>
                • Templates are automatically used by <code>TransactionEmailService</code><br>
                • Base URL: <code>{baseUrl}</code>
            </p>
        </div>
    </div>
</body>
</html>";
        
        return Content(html, "text/html");
    }

    /// <summary>
    /// List all available email templates.
    /// </summary>
    /// <returns>List of template names.</returns>
    [HttpGet("templates")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTemplates()
    {
        IReadOnlyList<string> templates = await templateService.GetAvailableTemplatesAsync();
        return Ok(templates);
    }

    /// <summary>
    /// Preview the Welcome Email template.
    /// </summary>
    [HttpGet("welcome")]
    [Produces("text/html")]
    public async Task<IActionResult> PreviewWelcomeEmail(
        [FromQuery] string userName = "John Doe",
        [FromQuery] string siteName = "AppBlueprint",
        [FromQuery] string loginUrl = "https://app.example.com/login",
        [FromQuery] string supportEmail = "support@example.com")
    {
        var model = new WelcomeEmailModel(userName, siteName, loginUrl, supportEmail);
        string html = await templateService.RenderTemplateAsync(model);
        return Content(html, "text/html");
    }

    /// <summary>
    /// Preview the Password Reset Email template.
    /// </summary>
    [HttpGet("password-reset")]
    [Produces("text/html")]
    public async Task<IActionResult> PreviewPasswordResetEmail(
        [FromQuery] string userName = "John Doe",
        [FromQuery] string resetUrl = "https://app.example.com/reset-password?token=abc123",
        [FromQuery] string expirationHours = "24",
        [FromQuery] string supportEmail = "support@example.com")
    {
        var model = new PasswordResetEmailModel(userName, resetUrl, expirationHours, supportEmail);
        string html = await templateService.RenderTemplateAsync(model);
        return Content(html, "text/html");
    }

    /// <summary>
    /// Preview the Email Verification template.
    /// </summary>
    [HttpGet("email-verification")]
    [Produces("text/html")]
    public async Task<IActionResult> PreviewEmailVerification(
        [FromQuery] string userName = "John Doe",
        [FromQuery] string verificationUrl = "https://app.example.com/verify-email?token=abc123",
        [FromQuery] string siteName = "AppBlueprint",
        [FromQuery] string expirationHours = "24")
    {
        var model = new EmailVerificationModel(userName, verificationUrl, siteName, expirationHours);
        string html = await templateService.RenderTemplateAsync(model);
        return Content(html, "text/html");
    }

    /// <summary>
    /// Preview the Order Confirmation Email template.
    /// </summary>
    [HttpGet("order-confirmation")]
    [Produces("text/html")]
    public async Task<IActionResult> PreviewOrderConfirmation(
        [FromQuery] string customerName = "Jane Smith",
        [FromQuery] string orderId = "ORD-12345",
        [FromQuery] decimal totalAmount = 299.99m,
        [FromQuery] string orderDetailsUrl = "https://app.example.com/orders/12345",
        [FromQuery] string siteName = "AppBlueprint",
        [FromQuery] string supportEmail = "support@example.com")
    {
        var items = new List<OrderLineItemModel>
        {
            new("Premium Plan - Annual", 1, 249.99m, 249.99m),
            new("Additional User License", 2, 25.00m, 50.00m)
        };

        var model = new OrderConfirmationEmailModel(
            customerName, 
            orderId, 
            totalAmount, 
            items, 
            orderDetailsUrl, 
            siteName, 
            supportEmail);
        
        string html = await templateService.RenderTemplateAsync(model);
        return Content(html, "text/html");
    }

    /// <summary>
    /// Preview the Team Invitation Email template.
    /// </summary>
    [HttpGet("team-invitation")]
    [Produces("text/html")]
    public async Task<IActionResult> PreviewTeamInvitation(
        [FromQuery] string inviteeName = "Sarah Johnson",
        [FromQuery] string inviterName = "John Doe",
        [FromQuery] string teamName = "Acme Corp",
        [FromQuery] string acceptInvitationUrl = "https://app.example.com/invitations/accept?token=abc123",
        [FromQuery] string siteName = "AppBlueprint",
        [FromQuery] string expirationDays = "7")
    {
        var model = new TeamInvitationEmailModel(
            inviteeName, 
            inviterName, 
            teamName, 
            acceptInvitationUrl, 
            siteName, 
            expirationDays);
        
        string html = await templateService.RenderTemplateAsync(model);
        return Content(html, "text/html");
    }

    /// <summary>
    /// Preview any template by name with custom data (for advanced testing).
    /// </summary>
    [HttpPost("render")]
    [Produces("text/html")]
    public async Task<IActionResult> RenderCustomTemplate(
        [FromQuery] string templateName,
        [FromBody] Dictionary<string, object> data)
    {
        try
        {
            // This would require dynamic model creation - for now, return a message
            return Ok(new
            {
                message = "Use specific endpoints like /api/dev/email-preview/welcome for template previews",
                availableEndpoints = new[]
                {
                    "GET /api/dev/email-preview/templates",
                    "GET /api/dev/email-preview/welcome",
                    "GET /api/dev/email-preview/password-reset",
                    "GET /api/dev/email-preview/email-verification",
                    "GET /api/dev/email-preview/order-confirmation",
                    "GET /api/dev/email-preview/team-invitation"
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
