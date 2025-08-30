using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Affiliate.Entities.Affiliate;

public class PayoutRequest : BaseEntity
{
    public PayoutRequest()
    {
        Id = PrefixedUlid.Generate("payout");
        AffiliateId = string.Empty;
        PaymentMethod = string.Empty;
    }

    public required string AffiliateId { get; set; }
    public decimal Amount { get; set; }
    public required string PaymentMethod { get; set; } // PayPal, BankTransfer, Stripe, etc.
    public string? PaymentDetails { get; set; } // JSON or encrypted payment info
    public string Status { get; set; } = "Pending"; // Pending, Approved, Processed, Rejected
    public DateTime? ProcessedDate { get; set; }
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }

    // Navigation properties
    public Affiliate? Affiliate { get; set; }
}