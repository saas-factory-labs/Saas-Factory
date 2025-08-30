using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Affiliate.Entities.Affiliate;

public class Affiliate : BaseEntity
{
    private readonly List<Commission> _commissions = new();
    private readonly List<Referral> _referrals = new();
    private readonly List<PayoutRequest> _payoutRequests = new();

    public Affiliate()
    {
        Id = PrefixedUlid.Generate("aff");
        ReferralCode = string.Empty;
        Email = string.Empty;
    }

    public required string ReferralCode { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal CommissionRate { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal PendingCommissions { get; set; }
    public DateTime? LastPayoutDate { get; set; }

    // Navigation properties
    public IReadOnlyCollection<Commission> Commissions => _commissions.AsReadOnly();
    public IReadOnlyCollection<Referral> Referrals => _referrals.AsReadOnly();
    public IReadOnlyCollection<PayoutRequest> PayoutRequests => _payoutRequests.AsReadOnly();

    // Domain methods for controlled collection management
    public void AddCommission(Commission commission)
    {
        ArgumentNullException.ThrowIfNull(commission);

        if (_commissions.Any(c => c.Id == commission.Id))
            return; // Commission already exists

        _commissions.Add(commission);
        commission.AffiliateId = Id;
    }

    public void RemoveCommission(Commission commission)
    {
        ArgumentNullException.ThrowIfNull(commission);
        _commissions.Remove(commission);
    }

    public void AddReferral(Referral referral)
    {
        ArgumentNullException.ThrowIfNull(referral);

        if (_referrals.Any(r => r.Id == referral.Id))
            return; // Referral already exists

        _referrals.Add(referral);
        referral.AffiliateId = Id;
    }

    public void RemoveReferral(Referral referral)
    {
        ArgumentNullException.ThrowIfNull(referral);
        _referrals.Remove(referral);
    }

    public void AddPayoutRequest(PayoutRequest payoutRequest)
    {
        ArgumentNullException.ThrowIfNull(payoutRequest);

        if (_payoutRequests.Any(pr => pr.Id == payoutRequest.Id))
            return; // Payout request already exists

        _payoutRequests.Add(payoutRequest);
        payoutRequest.AffiliateId = Id;
    }

    public void RemovePayoutRequest(PayoutRequest payoutRequest)
    {
        ArgumentNullException.ThrowIfNull(payoutRequest);
        _payoutRequests.Remove(payoutRequest);
    }
}