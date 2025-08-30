using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Affiliate;

public partial class AffiliateModuleDbContext
{
    private const string DecimalMoneyType = "decimal(18,2)";
    private const string DecimalRateType = "decimal(5,4)";

    public required DbSet<Entities.Affiliate.Affiliate> Affiliates { get; set; }
    public required DbSet<Entities.Affiliate.Commission> Commissions { get; set; }
    public required DbSet<Entities.Affiliate.Referral> Referrals { get; set; }
    public required DbSet<Entities.Affiliate.PayoutRequest> PayoutRequests { get; set; }

    partial void OnModelCreating_Affiliate(ModelBuilder modelBuilder)
    {
        // Affiliate entity configuration
        modelBuilder.Entity<Entities.Affiliate.Affiliate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReferralCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.CommissionRate).HasColumnType(DecimalRateType);
            entity.Property(e => e.TotalEarnings).HasColumnType(DecimalMoneyType);
            entity.Property(e => e.PendingCommissions).HasColumnType(DecimalMoneyType);
            entity.HasIndex(e => e.ReferralCode).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.ToTable("Affiliates");
        });

        // Commission entity configuration
        modelBuilder.Entity<Entities.Affiliate.Commission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType(DecimalMoneyType);
            entity.Property(e => e.Rate).HasColumnType(DecimalRateType);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TransactionId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.HasOne(e => e.Affiliate)
                  .WithMany(a => a.Commissions)
                  .HasForeignKey(e => e.AffiliateId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.ToTable("Commissions");
        });

        // Referral entity configuration
        modelBuilder.Entity<Entities.Affiliate.Referral>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReferredEmail).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.ConversionValue).HasColumnType(DecimalMoneyType);
            entity.Property(e => e.TrackingCode).HasMaxLength(100);
            entity.Property(e => e.Campaign).HasMaxLength(200);
            entity.HasOne(e => e.Affiliate)
                  .WithMany(a => a.Referrals)
                  .HasForeignKey(e => e.AffiliateId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.ToTable("Referrals");
        });

        // PayoutRequest entity configuration
        modelBuilder.Entity<Entities.Affiliate.PayoutRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType(DecimalMoneyType);
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TransactionId).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.HasOne(e => e.Affiliate)
                  .WithMany(a => a.PayoutRequests)
                  .HasForeignKey(e => e.AffiliateId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.ToTable("PayoutRequests");
        });
    }
}