using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Affiliate.Entities.Affiliate;

public sealed class ReferralEntity : BaseEntity
{
    public ReferralEntity()
    {
        Id = PrefixedUlid.Generate("ref");
        AffiliateId = string.Empty;
        ReferredEmail = string.Empty;
    }

    public required string AffiliateId { get; set; }
    public required string ReferredEmail { get; set; }
    public string? ReferredUserId { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Converted, Cancelled
    public DateTime? ConvertedDate { get; set; }
    public decimal? ConversionValue { get; set; }
    public string? TrackingCode { get; set; }
    public Uri? SourceUrl { get; set; }
    public string? Campaign { get; set; }

    // Navigation properties
    public AffiliateEntity? Affiliate { get; set; }
}
