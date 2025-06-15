using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Region;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.Subscription;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel.Enums;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure;

public class DataSeeder(ApplicationDbContext dbContext)
{
    public async Task SeedDatabaseAsync(CancellationToken cancellationToken = default)
    {
        await DeleteAllRowsFromAllTablesAsync(cancellationToken).ConfigureAwait(false);

        if (await AreAllEntitiesSeededAsync(cancellationToken).ConfigureAwait(false))
        {
            Console.WriteLine("Database already seeded.");
            return;
        }

        // await SeedLanguagesAsync(cancellationToken);
        // await SeedCountriesAsync(cancellationToken);
        // await SeedAccountsAsync(cancellationToken);
        // await SeedSubscriptionsAsync(cancellationToken);
        // await SeedOrganizationsAsync(cancellationToken);
        // await SeedRolesAsync(cancellationToken);
        // await SeedApiKeysAsync(cancellationToken);
        // await SeedUsersAsync(cancellationToken);
        //await SeedTeamsAsync(cancellationToken);
        //await SeedTeamInvitesAsync(cancellationToken);
        //await SeedPhoneNumbersAsync(cancellationToken);
        //await SeedAuditLogsAsync(cancellationToken);

        Console.WriteLine("Database seeding completed.");
    }

    // private async Task SeedTeamInvitesAsync(CancellationToken cancellationToken)
    // {
    //     dbContext.TeamInvites.RemoveRange(dbContext.TeamInvites);

    //     var invites = new List<TeamInviteEntity>
    //         {
    //             new TeamInviteEntity
    //             {
    //                 Team = new TeamEntity { Name = "Team Alpha" }, Owner = new UserEntity { UserName = "user1" },
    //                 CreatedAt = DateTime.UtcNow, ExpireAt = DateTime.UtcNow.AddDays(7),
    //                 IsActive = true
    //             },
    //             new TeamInviteEntity
    //             {
    //                 Team = new TeamEntity { Name = "Team Beta" }, Owner = new UserEntity { UserName = "user2" },
    //                 CreatedAt = DateTime.UtcNow, ExpireAt = DateTime.UtcNow.AddDays(7),
    //                 IsActive = true
    //             }
    //         };

    //     await dbContext.TeamInvites.AddRangeAsync(invites);
    //     await dbContext.SaveChangesAsync();

    //     Console.WriteLine("Team invites seeded.");
    // }

    private async Task DeleteAllRowsFromAllTablesAsync(CancellationToken cancellationToken)
    {
        var tableNames = dbContext.Model.GetEntityTypes()
            .Select(t => t.GetTableName())
            .Distinct()
            .Where(name => !string.IsNullOrEmpty(name))
            .ToList();

        foreach (string? tableName in tableNames)
        {
            string sql = $"TRUNCATE TABLE \"{tableName}\" RESTART IDENTITY CASCADE;";
            await dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }

        Console.WriteLine("All tables truncated.");
    }

    // private async Task SeedTeamsAsync()
    // {
    //     var teams = new List<TeamEntity>
    //     {
    //         new TeamEntity { Name = "Team Alpha", Owner = new UserEntity { UserName = "owner1" } },
    //         new TeamEntity { Name = "Team Beta", Owner = new UserEntity { UserName = "owner2" } }
    //     };

    //     await dbContext.Teams.AddRangeAsync(teams);
    //     await dbContext.SaveChangesAsync();

    //     Console.WriteLine("Teams seeded.");
    // }

    private async Task SeedUsersAsync(CancellationToken cancellationToken)
    {
        Faker<UserEntity>? faker = new Faker<UserEntity>()
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.IsActive, f => f.Random.Bool())
            .RuleFor(u => u.CreatedAt, f => f.Date.Past())
            .RuleFor(u => u.LastLogin, f => f.Date.Past())
            // .RuleFor(u => u.EmailAddresses, f => new List<EmailAddressEntity> { dbContext.EmailAddresses.FirstOrDefault() })
            .RuleFor(u => u.Addresses, f => dbContext.Addresses.ToList())
            .RuleFor(u => u.Roles, f => new List<RoleEntity> { dbContext.Roles.FirstOrDefault() });
    }

    private async Task SeedLanguagesAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Languages.AnyAsync(cancellationToken).ConfigureAwait(false)) return;

        var languages = new List<LanguageEntity>
        {
            new() { Name = "English", Code = "en" },
            new() { Name = "Danish", Code = "da" },
            new() { Name = "Spanish", Code = "es" },
            new() { Name = "French", Code = "fr" }
        };

        await dbContext.Languages.AddRangeAsync(languages, cancellationToken).ConfigureAwait(false);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        Console.WriteLine("Languages seeded.");
    }

    private async Task SeedCountriesAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Countries.AnyAsync(cancellationToken).ConfigureAwait(false)) return;

        var countries = new List<CountryEntity>
        {
            new()
            {
                Name = "United States", IsoCode = IsoCode.Us, GlobalRegion = new GlobalRegionEntity { Name = "America" }
            }
        };

        await dbContext.Countries.AddRangeAsync(countries, cancellationToken).ConfigureAwait(false);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        Console.WriteLine("Countries seeded.");
    }

    private async Task SeedAccountsAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Accounts.AnyAsync(cancellationToken).ConfigureAwait(false)) return;

        Faker<AccountEntity> faker = new Faker<AccountEntity>()
            .RuleFor(a => a.Email, f => f.Internet.Email())
            .RuleFor(a => a.CustomerType, f => CustomerType.Business);

        List<AccountEntity> accounts = faker.Generate(10);
        await dbContext.Accounts.AddRangeAsync(accounts, cancellationToken).ConfigureAwait(false);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        Console.WriteLine("Accounts seeded.");
    }

    private async Task SeedSubscriptionsAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Subscriptions.AnyAsync(cancellationToken).ConfigureAwait(false)) return;

        Faker<SubscriptionEntity> faker = new Faker<SubscriptionEntity>()
            .RuleFor(s => s.CreatedAt, f => f.Date.Past())
            .RuleFor(s => s.UpdatedAt, f => f.Date.Past())
            .RuleFor(s => s.Name, f => f.Random.AlphaNumeric(8));

        List<SubscriptionEntity> subscriptions = faker.Generate(5);
        await dbContext.Subscriptions.AddRangeAsync(subscriptions, cancellationToken).ConfigureAwait(false);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        Console.WriteLine("Subscriptions seeded.");
    }

    // private async Task SeedOrganizationsAsync(CancellationToken cancellationToken)
    // {
    //     if (await dbContext.Organizations.AnyAsync(cancellationToken)) return;

    //     var organizations = new List<OrganizationEntity>
    //     {
    //         new() { Name = "Org A", CreatedAt = DateTime.UtcNow, Owner = new UserEntity(), Customers = new List<CustomerEntity>(), Description = "adad", Teams = new List<TeamEntity>() },

    //     };

    //     await dbContext.Organizations.AddRangeAsync(organizations, cancellationToken);
    //     await dbContext.SaveChangesAsync(cancellationToken);
    //     Console.WriteLine("Organizations seeded.");
    // }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Roles.AnyAsync(cancellationToken).ConfigureAwait(false)) return;

        var roles = new List<RoleEntity> { new() { Name = "Admin" }, new() { Name = "User" } };

        await dbContext.Roles.AddRangeAsync(roles, cancellationToken).ConfigureAwait(false);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        Console.WriteLine("Roles seeded.");
    }

    // private async Task SeedApiKeysAsync(CancellationToken cancellationToken)
    // {
    //     if (await dbContext.ApiKeys.AnyAsync(cancellationToken)) return;

    //     var apiKeys = new List<ApiKeyEntity>
    //     {
    //         new() { Name = "Key1", Description = "Test Key 1", SecretRef = "Secret1", Owner = new UserEntity() },
    //     };

    //     await dbContext.ApiKeys.AddRangeAsync(apiKeys, cancellationToken);
    //     await dbContext.SaveChangesAsync(cancellationToken);
    //     Console.WriteLine("API Keys seeded.");
    // }

    private async Task<bool> AreAllEntitiesSeededAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Languages.AnyAsync(cancellationToken).ConfigureAwait(false)
               && await dbContext.Countries.AnyAsync(cancellationToken).ConfigureAwait(false)
               && await dbContext.Accounts.AnyAsync(cancellationToken).ConfigureAwait(false)
               && await dbContext.Subscriptions.AnyAsync(cancellationToken).ConfigureAwait(false);
        //    && await dbContext.Organizations.AnyAsync(cancellationToken)
        //    && await dbContext.Roles.AnyAsync(cancellationToken)
        //    && await dbContext.ApiKeys.AnyAsync(cancellationToken);
    }
}
