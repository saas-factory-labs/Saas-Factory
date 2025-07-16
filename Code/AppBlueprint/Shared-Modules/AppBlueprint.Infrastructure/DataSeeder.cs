using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Region;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.Subscription;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.Resources;
using AppBlueprint.SharedKernel;
using AppBlueprint.SharedKernel.Enums;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure;

public class DataSeeder(ApplicationDbContext dbContext, ILogger<DataSeeder> logger)
{
    public async Task SeedDatabaseAsync(CancellationToken cancellationToken = default)
    {
        await DeleteAllRowsFromAllTablesAsync(cancellationToken);

        if (await AreAllEntitiesSeededAsync(cancellationToken))
        {
            logger.LogInformation(DataSeederMessages.DatabaseAlreadySeeded);
            return;
        }

        await SeedLanguagesAsync(cancellationToken);
        await SeedCountriesAsync(cancellationToken);
        await SeedAccountsAsync(cancellationToken);
        await SeedSubscriptionsAsync(cancellationToken);
        await SeedRolesAsync(cancellationToken);
        // await SeedOrganizationsAsync(cancellationToken);
        // await SeedUsersAsync(cancellationToken); // TODO: Enable after implementing ULID generation

        logger.LogInformation(DataSeederMessages.DatabaseSeedingCompleted);
    }

    private async Task DeleteAllRowsFromAllTablesAsync(CancellationToken cancellationToken)
    {
        var tableNames = dbContext.Model.GetEntityTypes()
            .Select(t => t.GetTableName())
            .Distinct()
            .Where(name => !string.IsNullOrEmpty(name))
            .ToList();

        foreach (string? tableName in tableNames)
        {
            try
            {
                string sql = $"TRUNCATE TABLE \"{tableName}\" RESTART IDENTITY CASCADE;";
                await dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                logger.LogInformation("Successfully truncated table: {TableName}", tableName);
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01") // Table does not exist
            {
                logger.LogWarning("Table '{TableName}' does not exist, skipping truncation", tableName);
            }
            catch (Npgsql.PostgresException ex)
            {
                logger.LogWarning("Failed to truncate table '{TableName}': {ErrorMessage}. Continuing with next table", tableName, ex.Message);
            }
        }

        logger.LogInformation(DataSeederMessages.AllTablesProcessed);
    }

    private async Task SeedLanguagesAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Languages.AnyAsync(cancellationToken)) return;

        var languages = new List<LanguageEntity>
        {
            new() { Name = "English", Code = "en" },
            new() { Name = "Danish", Code = "da" },
            new() { Name = "Spanish", Code = "es" },
            new() { Name = "French", Code = "fr" }
        };

        await dbContext.Languages.AddRangeAsync(languages, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation(DataSeederMessages.LanguagesSeeded);
    }

    private async Task SeedCountriesAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Countries.AnyAsync(cancellationToken)) return;

        var countries = new List<CountryEntity>
        {
            new()
            {
                Name = "United States", 
                IsoCode = IsoCode.Us, 
                CityId = PrefixedUlid.Generate("city"),
                GlobalRegionId = PrefixedUlid.Generate("region"),
                GlobalRegion = new GlobalRegionEntity { Name = "America" }
            }
        };

        await dbContext.Countries.AddRangeAsync(countries, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation(DataSeederMessages.CountriesSeeded);
    }

    private async Task SeedAccountsAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Accounts.AnyAsync(cancellationToken)) return;

        var faker = new Faker<AccountEntity>()
            .RuleFor(a => a.Email, f => f.Internet.Email())
            .RuleFor(a => a.CustomerType, f => CustomerType.Business);

        var accounts = faker.Generate(10);
        await dbContext.Accounts.AddRangeAsync(accounts, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation(DataSeederMessages.AccountsSeeded);
    }

    private async Task SeedSubscriptionsAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Subscriptions.AnyAsync(cancellationToken)) return;

        var faker = new Faker<SubscriptionEntity>()
            .RuleFor(s => s.CreatedAt, f => f.Date.Past())
            .RuleFor(s => s.LastUpdatedAt, f => f.Date.Past())
            .RuleFor(s => s.Name, f => f.Random.AlphaNumeric(8));

        var subscriptions = faker.Generate(5);
        await dbContext.Subscriptions.AddRangeAsync(subscriptions, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation(DataSeederMessages.SubscriptionsSeeded);
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Roles.AnyAsync(cancellationToken)) return;

        var roles = new List<RoleEntity>
        {
            new() { Name = "Admin" },
            new() { Name = "User" }
        };

        await dbContext.Roles.AddRangeAsync(roles, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation(DataSeederMessages.RolesSeeded);
    }

    private async Task<bool> AreAllEntitiesSeededAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Use AnyAsync for better performance when checking existence
            return await dbContext.Languages.AnyAsync(cancellationToken)
                   && await dbContext.Countries.AnyAsync(cancellationToken)
                   && await dbContext.Accounts.AnyAsync(cancellationToken)
                   && await dbContext.Subscriptions.AnyAsync(cancellationToken)
                   && await dbContext.Roles.AnyAsync(cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Unable to check if entities are seeded due to database schema mismatch. Assuming no data exists");
            return false;
        }

        catch (Npgsql.PostgresException ex)
        {
            logger.LogWarning(ex, "Unable to check if entities are seeded due to PostgreSQL error. Assuming no data exists");
            return false;
        }
    }
}
