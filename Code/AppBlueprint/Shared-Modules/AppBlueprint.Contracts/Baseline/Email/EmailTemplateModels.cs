namespace AppBlueprint.Contracts.Baseline.Email;

/// <summary>
/// Email template model for sending welcome emails to new users.
/// </summary>
public sealed record WelcomeEmailModel(
    string UserName,
    string EmailAddress,
    string TenantName,
    string? ActivationLink = null);

/// <summary>
/// Email template model for sending password reset emails.
/// </summary>
public sealed record PasswordResetEmailModel(
    string UserName,
    string EmailAddress,
    string ResetLink,
    DateTime ExpiresAt);

/// <summary>
/// Email template model for sending booking confirmation emails.
/// </summary>
public sealed record BookingConfirmationEmailModel(
    string UserName,
    string PropertyName,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    decimal TotalPrice,
    string BookingReference);

/// <summary>
/// Email template model for sending order confirmation emails.
/// </summary>
public sealed record OrderConfirmationEmailModel(
    string CustomerName,
    string OrderId,
    DateTime OrderDate,
    decimal TotalAmount,
    string OrderDetailsLink);

/// <summary>
/// Email template model for sending invoices/receipts.
/// </summary>
public sealed record InvoiceEmailModel(
    string CustomerName,
    string InvoiceNumber,
    DateTime InvoiceDate,
    DateTime DueDate,
    decimal TotalAmount,
    string InvoiceLink);

/// <summary>
/// Email template model for sending weekly digest emails.
/// </summary>
public sealed record WeeklyDigestEmailModel(
    string UserName,
    DateTime WeekStartDate,
    DateTime WeekEndDate,
    int NewNotifications,
    int NewMessages,
    string[] HighlightedActivities);

/// <summary>
/// Email template model for sending admin notifications.
/// </summary>
public sealed record AdminNotificationEmailModel(
    string AdminName,
    string NotificationType,
    string Title,
    string Message,
    string ActionLink,
    DateTime OccurredAt);
