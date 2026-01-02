using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Affiliate.Entities.Affiliate;

public class Commission : BaseEntity
{
    public Commission()
    {
        Id = PrefixedUlid.Generate("comm");
        AffiliateId = string.Empty;
        ReferralId = string.Empty;
        TransactionId = string.Empty;
    }

    public required string AffiliateId { get; set; }
    public required string ReferralId { get; set; }
    public required string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public decimal Rate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Paid, Cancelled
    public DateTime? ProcessedDate { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public AffiliateEntity? Affiliate { get; set; }
    public ReferralEntity? Referral { get; set; }
}