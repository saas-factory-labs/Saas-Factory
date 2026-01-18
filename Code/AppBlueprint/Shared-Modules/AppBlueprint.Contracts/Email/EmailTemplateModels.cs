namespace AppBlueprint.Contracts.Email;

/// <summary>
/// Base interface for all email template models.
/// </summary>
public interface IEmailTemplateModel
{
    /// <summary>
    /// The name of the template to use for rendering.
    /// </summary>
    string TemplateName { get; }
}

/// <summary>
/// Model for welcome email sent after user signup.
/// </summary>
public sealed record WelcomeEmailModel(
    string UserName,
    string SiteName,
    string LoginUrl,
    string SupportEmail
) : IEmailTemplateModel
{
    public string TemplateName => "WelcomeEmail";
}

/// <summary>
/// Model for password reset email.
/// </summary>
public sealed record PasswordResetEmailModel(
    string UserName,
    string ResetUrl,
    string ExpirationHours,
    string SupportEmail
) : IEmailTemplateModel
{
    public string TemplateName => "PasswordResetEmail";
}

/// <summary>
/// Model for email verification.
/// </summary>
public sealed record EmailVerificationModel(
    string UserName,
    string VerificationUrl,
    string SiteName,
    string ExpirationHours
) : IEmailTemplateModel
{
    public string TemplateName => "EmailVerification";
}

/// <summary>
/// Model for order confirmation email.
/// </summary>
public sealed record OrderConfirmationEmailModel(
    string CustomerName,
    string OrderId,
    decimal TotalAmount,
    List<OrderLineItemModel> Items,
    string OrderDetailsUrl,
    string SiteName,
    string SupportEmail
) : IEmailTemplateModel
{
    public string TemplateName => "OrderConfirmation";
}

/// <summary>
/// Represents a line item in an order.
/// </summary>
public sealed record OrderLineItemModel(
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Total
);

/// <summary>
/// Model for team invitation email.
/// </summary>
public sealed record TeamInvitationEmailModel(
    string InviteeName,
    string InviterName,
    string TeamName,
    string AcceptInvitationUrl,
    string SiteName,
    string ExpirationDays
) : IEmailTemplateModel
{
    public string TemplateName => "TeamInvitation";
}
