using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailAddress;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailInvite;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailVerification;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline;

public partial class BaselineDbContext
{
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<EmailVerificationEntity> EmailVerifications { get; set; } = null!;
    public DbSet<EmailInviteEntity> EmailInvites { get; set; } = null!;
    public DbSet<PhoneNumberEntity> PhoneNumbers { get; set; } = null!;
    public DbSet<EmailAddressEntity> EmailAddresses { get; set; } = null!;

    partial void OnModelCreating_Users(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EmailAddressEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PhoneNumberEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new EmailVerificationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new EmailInviteEntityConfiguration());
    }
}
