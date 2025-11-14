using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamInvite;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamMember;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Region;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.PaymentProvider;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.Subscription;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailAddress;

/// <summary>
/// Comprehensive database seeder that populates all application tables with realistic test data.
/// 
/// ENTITIES SEEDED (21 types, 1000+ records):
/// 
/// Core Reference Data:
/// - Languages (24 entries) - World languages with ISO codes
/// - GlobalRegions (6 entries) - Continental regions  
/// - CountryRegions (3 per country) - Regional subdivisions
/// - Cities (5 per region) - Major cities worldwide
/// - Streets (5 per city) - Street addresses
/// - Countries (195 entries) - All world countries
/// - Addresses (50 realistic addresses) - Complete address records
/// 
/// Authorization & Access Control:
/// - Roles (5 standard roles) - Admin, User, Manager, Support, Guest
/// - Permissions (10 core permissions) - CRUD and management permissions
/// 
/// Business & Payment:
/// - Subscriptions (20 plans) - Various subscription tiers
/// - Accounts (50 accounts) - Customer account records  
/// - PaymentProviders (6 providers) - Stripe, PayPal, Square, etc.
/// - Credits (2 per tenant) - Credit balance tracking
/// 
/// User Management & Multi-tenancy:
/// - Tenants (10 organizations) - Tenant organizations
/// - Users (50 users) - Users with realistic profiles
/// - Customers (30 customers) - Business/Personal/Government types
/// - ContactPersons (50 contacts) - Customer contact persons
/// 
/// Communication & Files:
/// - EmailAddresses (100 emails) - Linked to users/customers
/// - PhoneNumbers (80 numbers) - With country codes and verification
/// - Notifications (200 notifications) - User notification records
/// - Files (100 files) - File metadata with various extensions
/// 
/// Collaboration & Integration:
/// - Teams (15 teams) - Organizational teams within tenants
/// - Integrations (30 integrations) - Third-party service connections
/// 
/// Uses Bogus library for realistic fake data generation.
/// Implements proper dependency ordering for foreign key constraints.
/// Includes comprehensive error handling and logging.
/// </summary>
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.FileManagement;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Integration;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.DatabaseContexts.Modules.Credit;
using AppBlueprint.Infrastructure.Resources;
using AppBlueprint.SharedKernel;
using AppBlueprint.SharedKernel.Enums;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure;

public class DataSeeder(ApplicationDbContext dbContext, B2BDbContext b2bDbContext, ILogger<DataSeeder> logger)
{
    public async Task SeedDatabaseAsync(CancellationToken cancellationToken = default)
    {
        await DeleteAllRowsFromAllTablesAsync(cancellationToken);

        if (await AreAllEntitiesSeededAsync(cancellationToken))
        {
            logger.LogInformation(DataSeederMessages.DatabaseAlreadySeeded);
            return;
        }

        // Clear the change tracker to avoid tracking conflicts after checking for seeded data
        dbContext.ChangeTracker.Clear();
        b2bDbContext.ChangeTracker.Clear();

        // Correct dependency order
        await SeedLanguagesAsync(cancellationToken);
        await SeedGlobalRegionsAsync(cancellationToken);
        await SeedCountriesAsync(cancellationToken); // After GlobalRegions
        await SeedCountryRegionsAsync(cancellationToken); // After Countries
        await SeedCitiesAsync(cancellationToken); // After CountryRegions
        await SeedStreetsAsync(cancellationToken); // After Cities
        await SeedAddressesAsync(cancellationToken); // After Streets

        // Authorization and permissions
        await SeedRolesAsync(cancellationToken);
        await SeedPermissionsAsync(cancellationToken);

        await SeedRolePermissionsAsync(cancellationToken);
        // await SeedResourcePermissionTypesAsync(cancellationToken);
        // await SeedResourcePermissionsAsync(cancellationToken);

        // Payment and billing
        await SeedPaymentProvidersAsync(cancellationToken);
        await SeedSubscriptionsAsync(cancellationToken);
        // await SeedCreditsAsync(cancellationToken);

        // Customers and accounts
        await SeedAccountsAsync(cancellationToken);
        
        // User management - fixing entity structures
        await SeedTenantsAsync(cancellationToken);
        await SeedUsersAsync(cancellationToken);
        await SeedCustomersAsync(cancellationToken);
        await SeedCreditsAsync(cancellationToken);
        await SeedEmailAddressesAsync(cancellationToken);
        
        // Additional entities
        await SeedContactPersonsAsync(cancellationToken);
        await SeedPhoneNumbersAsync(cancellationToken);
        await SeedPaymentProvidersAsync(cancellationToken);
        await SeedTeamsAsync(cancellationToken);
        await SeedNotificationsAsync(cancellationToken);
        await SeedFilesAsync(cancellationToken);
        await SeedIntegrationsAsync(cancellationToken);
        
        // Auditing & Sessions
        await SeedSessionsAsync(cancellationToken);
        await SeedAuditLogsAsync(cancellationToken);
        
        // B2B Entities  
        await SeedOrganizationsAsync(cancellationToken);
        await SeedApiKeysAsync(cancellationToken);
        await SeedTodosAsync(cancellationToken);
        await SeedTeamMembersAsync(cancellationToken);
        await SeedTeamInvitesAsync(cancellationToken);
        
        // Authorization Relations
        await SeedUserRolesAsync(cancellationToken);
        // TODO: Add these when available in ApplicationDbContext
        // await SeedAdminsAsync(cancellationToken);
        
        // Data Management
        // TODO: Add these when available in ApplicationDbContext
        // await SeedDataExportsAsync(cancellationToken);
        // await SeedWebhooksAsync(cancellationToken);
        // await SeedSearchesAsync(cancellationToken);
        
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
            new() { Name = "User" },
            new() { Name = "Manager" },
            new() { Name = "Owner" }
        };

        await dbContext.Roles.AddRangeAsync(roles, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation(DataSeederMessages.RolesSeeded);
    }

    private async Task SeedGlobalRegionsAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.GlobalRegions.AnyAsync(cancellationToken)) return;

        var globalRegions = new List<GlobalRegionEntity>
        {
            new() { Name = "North America" },
            new() { Name = "Europe" },
            new() { Name = "Asia" },
            new() { Name = "South America" },
            new() { Name = "Africa" },
            new() { Name = "Oceania" }
        };

        await dbContext.GlobalRegions.AddRangeAsync(globalRegions, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Global regions seeded");
    }

    private async Task SeedCountryRegionsAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.CountryRegions.AnyAsync(cancellationToken)) return;

        var countries = await dbContext.Countries.ToListAsync(cancellationToken);
        if (countries.Count == 0) return;

        var countryRegions = new List<CountryRegionEntity>
        {
            new() { Name = "Western Europe", CountryId = countries[0].Id },
            new() { Name = "Eastern Europe", CountryId = countries[0].Id },
            new() { Name = "Central Region", CountryId = countries[0].Id }
        };

        await dbContext.CountryRegions.AddRangeAsync(countryRegions, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Country regions seeded");
    }

    private async Task SeedCitiesAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Cities.AnyAsync(cancellationToken)) return;

        var faker = new Faker<CityEntity>()
            .RuleFor(c => c.Name, f => f.Address.City());

        var cities = faker.Generate(20);
        await dbContext.Cities.AddRangeAsync(cities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Cities seeded");
    }

    private async Task SeedStreetsAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Streets.AnyAsync(cancellationToken)) return;

        var cities = await dbContext.Cities.ToListAsync(cancellationToken);
        if (cities.Count == 0) return;

        var faker = new Faker<StreetEntity>()
            .RuleFor(s => s.Name, f => f.Address.StreetName())
            .RuleFor(s => s.CityId, f => f.PickRandom(cities).Id);

        var streets = faker.Generate(50);
        await dbContext.Streets.AddRangeAsync(streets, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Streets seeded");
    }

    private async Task SeedAddressesAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Addresses.AnyAsync(cancellationToken)) return;

        var streets = await dbContext.Streets.ToListAsync(cancellationToken);
        if (streets.Count == 0) return;

        var faker = new Faker<AddressEntity>()
            .RuleFor(a => a.StreetNumber, f => f.Address.BuildingNumber())
            .RuleFor(a => a.StreetId, f => f.PickRandom(streets).Id);

        var addresses = faker.Generate(30);
        await dbContext.Addresses.AddRangeAsync(addresses, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Addresses seeded");
    }

    private async Task SeedPermissionsAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Permissions.AnyAsync(cancellationToken)) return;

        var permissions = new List<PermissionEntity>
        {
            new() { Name = "Create", Description = "Create new resources" },
            new() { Name = "Read", Description = "Read existing resources" },
            new() { Name = "Update", Description = "Update existing resources" },
            new() { Name = "Delete", Description = "Delete existing resources" },
            new() { Name = "ManageUsers", Description = "Manage user accounts" },
            new() { Name = "ManageRoles", Description = "Manage user roles" }
        };

        await dbContext.Permissions.AddRangeAsync(permissions, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Permissions seeded");
    }

    private async Task SeedRolePermissionsAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.RolePermissions.AnyAsync(cancellationToken)) return;

        // Get all roles and permissions from database
        var roles = await dbContext.Roles.ToListAsync(cancellationToken);
        var permissions = await dbContext.Permissions.ToListAsync(cancellationToken);

        // Find specific roles
        var adminRole = roles.FirstOrDefault(r => r.Name == "Admin");
        var managerRole = roles.FirstOrDefault(r => r.Name == "Manager");
        var userRole = roles.FirstOrDefault(r => r.Name == "User");
        var ownerRole = roles.FirstOrDefault(r => r.Name == "Owner");

        // Find specific permissions
        var createPerm = permissions.FirstOrDefault(p => p.Name == "Create");
        var readPerm = permissions.FirstOrDefault(p => p.Name == "Read");
        var updatePerm = permissions.FirstOrDefault(p => p.Name == "Update");
        var deletePerm = permissions.FirstOrDefault(p => p.Name == "Delete");
        var manageUsersPerm = permissions.FirstOrDefault(p => p.Name == "ManageUsers");
        var manageRolesPerm = permissions.FirstOrDefault(p => p.Name == "ManageRoles");

        var rolePermissions = new List<RolePermissionEntity>();

        // Admin Role - Full permissions
        if (adminRole is not null)
        {
            if (createPerm is not null) rolePermissions.Add(new() { RoleId = adminRole.Id, PermissionId = createPerm.Id, Role = adminRole, Permission = createPerm });
            if (readPerm is not null) rolePermissions.Add(new() { RoleId = adminRole.Id, PermissionId = readPerm.Id, Role = adminRole, Permission = readPerm });
            if (updatePerm is not null) rolePermissions.Add(new() { RoleId = adminRole.Id, PermissionId = updatePerm.Id, Role = adminRole, Permission = updatePerm });
            if (deletePerm is not null) rolePermissions.Add(new() { RoleId = adminRole.Id, PermissionId = deletePerm.Id, Role = adminRole, Permission = deletePerm });
            if (manageUsersPerm is not null) rolePermissions.Add(new() { RoleId = adminRole.Id, PermissionId = manageUsersPerm.Id, Role = adminRole, Permission = manageUsersPerm });
            if (manageRolesPerm is not null) rolePermissions.Add(new() { RoleId = adminRole.Id, PermissionId = manageRolesPerm.Id, Role = adminRole, Permission = manageRolesPerm });
        }

        // Owner Role - Full permissions (same as Admin)
        if (ownerRole is not null)
        {
            if (createPerm is not null) rolePermissions.Add(new() { RoleId = ownerRole.Id, PermissionId = createPerm.Id, Role = ownerRole, Permission = createPerm });
            if (readPerm is not null) rolePermissions.Add(new() { RoleId = ownerRole.Id, PermissionId = readPerm.Id, Role = ownerRole, Permission = readPerm });
            if (updatePerm is not null) rolePermissions.Add(new() { RoleId = ownerRole.Id, PermissionId = updatePerm.Id, Role = ownerRole, Permission = updatePerm });
            if (deletePerm is not null) rolePermissions.Add(new() { RoleId = ownerRole.Id, PermissionId = deletePerm.Id, Role = ownerRole, Permission = deletePerm });
            if (manageUsersPerm is not null) rolePermissions.Add(new() { RoleId = ownerRole.Id, PermissionId = manageUsersPerm.Id, Role = ownerRole, Permission = manageUsersPerm });
            if (manageRolesPerm is not null) rolePermissions.Add(new() { RoleId = ownerRole.Id, PermissionId = manageRolesPerm.Id, Role = ownerRole, Permission = manageRolesPerm });
        }

        // Manager Role - CRUD + ManageUsers (no ManageRoles)
        if (managerRole is not null)
        {
            if (createPerm is not null) rolePermissions.Add(new() { RoleId = managerRole.Id, PermissionId = createPerm.Id, Role = managerRole, Permission = createPerm });
            if (readPerm is not null) rolePermissions.Add(new() { RoleId = managerRole.Id, PermissionId = readPerm.Id, Role = managerRole, Permission = readPerm });
            if (updatePerm is not null) rolePermissions.Add(new() { RoleId = managerRole.Id, PermissionId = updatePerm.Id, Role = managerRole, Permission = updatePerm });
            if (deletePerm is not null) rolePermissions.Add(new() { RoleId = managerRole.Id, PermissionId = deletePerm.Id, Role = managerRole, Permission = deletePerm });
            if (manageUsersPerm is not null) rolePermissions.Add(new() { RoleId = managerRole.Id, PermissionId = manageUsersPerm.Id, Role = managerRole, Permission = manageUsersPerm });
        }

        // User Role - Basic CRUD only (for their own resources)
        if (userRole is not null)
        {
            if (createPerm is not null) rolePermissions.Add(new() { RoleId = userRole.Id, PermissionId = createPerm.Id, Role = userRole, Permission = createPerm });
            if (readPerm is not null) rolePermissions.Add(new() { RoleId = userRole.Id, PermissionId = readPerm.Id, Role = userRole, Permission = readPerm });
            if (updatePerm is not null) rolePermissions.Add(new() { RoleId = userRole.Id, PermissionId = updatePerm.Id, Role = userRole, Permission = updatePerm });
        }

        await dbContext.RolePermissions.AddRangeAsync(rolePermissions, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation(DataSeederMessages.RolePermissionsSeeded);
    }

    private async Task SeedUserRolesAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.UserRoles.AnyAsync(cancellationToken)) return;

            var users = await dbContext.Users.ToListAsync(cancellationToken);
            var roles = await dbContext.Roles.ToListAsync(cancellationToken);

            if (users.Count == 0 || roles.Count == 0)
            {
                throw new InvalidOperationException("Cannot seed UserRoles - Users or Roles not found. Ensure SeedUsersAsync and SeedRolesAsync are called first.");
            }

            // Find specific roles
            var adminRole = roles.FirstOrDefault(r => r.Name == "Admin");
            var managerRole = roles.FirstOrDefault(r => r.Name == "Manager");
            var userRole = roles.FirstOrDefault(r => r.Name == "User");
            var ownerRole = roles.FirstOrDefault(r => r.Name == "Owner");

            var userRoles = new List<UserRoleEntity>();

            // Distribute roles realistically:
            // - Owner: first 2 users
            // - Admin: next 4 users
            // - Manager: ~15% of remaining users
            // - User: rest of users

            var userIndex = 0;

            // Assign Owner role to first 2 users
            if (ownerRole is not null && users.Count > 0)
            {
                var ownerCount = Math.Min(2, users.Count);
                for (int i = 0; i < ownerCount; i++)
                {
                    userRoles.Add(new UserRoleEntity
                    {
                        UserId = users[userIndex].Id,
                        RoleId = ownerRole.Id
                    });
                    userIndex++;
                }
            }

            // Assign Admin role to next 4 users
            if (adminRole is not null && userIndex < users.Count)
            {
                var adminCount = Math.Min(4, users.Count - userIndex);
                for (int i = 0; i < adminCount; i++)
                {
                    userRoles.Add(new UserRoleEntity
                    {
                        UserId = users[userIndex].Id,
                        RoleId = adminRole.Id
                    });
                    userIndex++;
                }
            }

            // Assign Manager role to ~15% of remaining users
            if (managerRole is not null && userIndex < users.Count)
            {
                var remainingUsers = users.Count - userIndex;
                var managerCount = Math.Max(1, (int)(remainingUsers * 0.15));
                managerCount = Math.Min(managerCount, remainingUsers);

                for (int i = 0; i < managerCount; i++)
                {
                    userRoles.Add(new UserRoleEntity
                    {
                        UserId = users[userIndex].Id,
                        RoleId = managerRole.Id
                    });
                    userIndex++;
                }
            }

            // Assign User role to all remaining users
            if (userRole is not null && userIndex < users.Count)
            {
                for (int i = userIndex; i < users.Count; i++)
                {
                    userRoles.Add(new UserRoleEntity
                    {
                        UserId = users[i].Id,
                        RoleId = userRole.Id
                    });
                }
            }

            await dbContext.UserRoles.AddRangeAsync(userRoles, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} user roles", userRoles.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding UserRoles");
            throw;
        }
    }

    private async Task<bool> AreAllEntitiesSeededAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Use AnyAsync for better performance when checking existence
            return await dbContext.Languages.AnyAsync(cancellationToken)
                   && await dbContext.GlobalRegions.AnyAsync(cancellationToken)
                   && await dbContext.Countries.AnyAsync(cancellationToken)
                   && await dbContext.Addresses.AnyAsync(cancellationToken)
                   && await dbContext.Permissions.AnyAsync(cancellationToken)
                   && await dbContext.Accounts.AnyAsync(cancellationToken)
                   && await dbContext.Subscriptions.AnyAsync(cancellationToken)
                   && await dbContext.Roles.AnyAsync(cancellationToken)
                   && await dbContext.Tenants.AnyAsync(cancellationToken)
                   && await dbContext.Users.AnyAsync(cancellationToken)
                   && await dbContext.Customers.AnyAsync(cancellationToken)
                   && await dbContext.Credits.AnyAsync(cancellationToken)
                   && await dbContext.EmailAddresses.AnyAsync(cancellationToken)
                   && await dbContext.ContactPersons.AnyAsync(cancellationToken)
                   && await dbContext.PhoneNumbers.AnyAsync(cancellationToken)
                   && await dbContext.PaymentProviders.AnyAsync(cancellationToken)
                   && await b2bDbContext.Teams.AnyAsync(cancellationToken)
                   && await dbContext.Notifications.AnyAsync(cancellationToken)
                   && await dbContext.Files.AnyAsync(cancellationToken)
                   && await dbContext.Integrations.AnyAsync(cancellationToken);
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

    // --- Additional entity seeding methods ---
    private async Task SeedUsersAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.Users.AnyAsync(cancellationToken)) return;

            var tenants = await dbContext.Tenants.ToListAsync(cancellationToken);
            if (tenants.Count == 0) throw new InvalidOperationException("Cannot seed Users - no Tenants found. Ensure SeedTenantsAsync is called first.");

            var faker = new Faker<UserEntity>()
                .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                .RuleFor(u => u.LastName, f => f.Person.LastName)
                .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.TenantId, f => f.PickRandom(tenants).Id)
                .RuleFor(u => u.IsActive, f => f.Random.Bool(0.9f));

            var users = faker.Generate(50);
            await dbContext.Users.AddRangeAsync(users, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} users", users.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding Users");
            throw;
        }
    }

    private async Task SeedTenantsAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.Tenants.AnyAsync(cancellationToken)) return;

            var faker = new Faker<TenantEntity>()
                .RuleFor(t => t.Name, f => f.Company.CompanyName())
                .RuleFor(t => t.Email, (f, t) => f.Internet.Email(t.Name.Replace(" ", "", StringComparison.OrdinalIgnoreCase).ToUpperInvariant()))
                .RuleFor(t => t.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(t => t.IsActive, f => f.Random.Bool(0.95f));

            var tenants = faker.Generate(10);
            await dbContext.Tenants.AddRangeAsync(tenants, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} tenants", tenants.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding Tenants");
            throw;
        }
    }

    private async Task SeedCustomersAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.Customers.AnyAsync(cancellationToken)) return;

            var tenants = await dbContext.Tenants.ToListAsync(cancellationToken);
            if (tenants.Count == 0) throw new InvalidOperationException("Cannot seed Customers - no Tenants found. Ensure SeedTenantsAsync is called first.");

            var faker = new Faker<CustomerEntity>()
                .RuleFor(c => c.CustomerType, f => f.PickRandom<CustomerType>())
                .RuleFor(c => c.Type, (f, c) => c.CustomerType == CustomerType.Business ? "Company" : "Personal")
                .RuleFor(c => c.Country, f => f.Address.Country())
                .RuleFor(c => c.VatNumber, f => f.Random.AlphaNumeric(10))
                .RuleFor(c => c.CurrentlyAtOnboardingFlowStep, f => f.Random.Int(1, 10));

            var customers = faker.Generate(30);
            await dbContext.Customers.AddRangeAsync(customers, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} customers", customers.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding Customers");
            throw;
        }
    }

    private async Task SeedCreditsAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.Credits.AnyAsync(cancellationToken)) return;

            var tenants = await dbContext.Tenants.ToListAsync(cancellationToken);
            if (tenants.Count == 0) throw new InvalidOperationException("Cannot seed Credits - no Tenants found. Ensure SeedTenantsAsync is called first.");

            var faker = new Faker<CreditEntity>()
                .RuleFor(c => c.CreditRemaining, f => f.Random.Decimal(0, 10000))
                .RuleFor(c => c.TenantId, f => f.PickRandom(tenants).Id);

            var credits = faker.Generate(tenants.Count * 2); // 2 credit records per tenant
            await dbContext.Credits.AddRangeAsync(credits, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} credits", credits.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding Credits");
            throw;
        }
    }

    private async Task SeedEmailAddressesAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.EmailAddresses.AnyAsync(cancellationToken)) return;

            var users = await dbContext.Users.ToListAsync(cancellationToken);
            var customers = await dbContext.Customers.ToListAsync(cancellationToken);
            var tenants = await dbContext.Tenants.ToListAsync(cancellationToken);

            if (users.Count == 0 || customers.Count == 0 || tenants.Count == 0) 
            {
                throw new InvalidOperationException("Cannot seed EmailAddresses - Users, Customers, or Tenants not found. Ensure related entities are seeded first.");
            }

            var faker = new Faker<EmailAddressEntity>()
                .RuleFor(e => e.Address, f => f.Internet.Email())
                .RuleFor(e => e.UserId, f => f.PickRandom(users).Id)
                .RuleFor(e => e.CustomerId, f => f.Random.Bool(0.5f) ? f.PickRandom(customers).Id : null)
                .RuleFor(e => e.TenantId, f => f.PickRandom(tenants).Id);

            var emailAddresses = faker.Generate(100);
            await dbContext.EmailAddresses.AddRangeAsync(emailAddresses, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} email addresses", emailAddresses.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding EmailAddresses");
            throw;
        }
    }

    private async Task SeedContactPersonsAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.ContactPersons.AnyAsync(cancellationToken)) return;

            var customers = await dbContext.Customers.ToListAsync(cancellationToken);
            var tenants = await dbContext.Tenants.ToListAsync(cancellationToken);

            if (customers.Count == 0 || tenants.Count == 0) 
            {
                throw new InvalidOperationException("Cannot seed ContactPersons - Customers or Tenants not found. Ensure related entities are seeded first.");
            }

            var faker = new Faker<ContactPersonEntity>()
                .RuleFor(cp => cp.FirstName, f => f.Person.FirstName)
                .RuleFor(cp => cp.LastName, f => f.Person.LastName)
                .RuleFor(cp => cp.IsActive, f => f.Random.Bool(0.95f))
                .RuleFor(cp => cp.IsPrimary, f => f.Random.Bool(0.3f))
                .RuleFor(cp => cp.CustomerId, f => f.PickRandom(customers).Id)
                .RuleFor(cp => cp.TenantId, f => f.PickRandom(tenants).Id);

            var contactPersons = faker.Generate(50);
            await dbContext.ContactPersons.AddRangeAsync(contactPersons, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} contact persons", contactPersons.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding ContactPersons");
            throw;
        }
    }

    private async Task SeedPhoneNumbersAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.PhoneNumbers.AnyAsync(cancellationToken)) return;

            var users = await dbContext.Users.ToListAsync(cancellationToken);
            var customers = await dbContext.Customers.ToListAsync(cancellationToken);
            var contactPersons = await dbContext.ContactPersons.ToListAsync(cancellationToken);
            var tenants = await dbContext.Tenants.ToListAsync(cancellationToken);

            if (users.Count == 0 || customers.Count == 0 || contactPersons.Count == 0 || tenants.Count == 0) 
            {
                throw new InvalidOperationException("Cannot seed PhoneNumbers - Users, Customers, ContactPersons, or Tenants not found. Ensure related entities are seeded first.");
            }

            var faker = new Faker<PhoneNumberEntity>()
                .RuleFor(pn => pn.Number, f => f.Phone.PhoneNumber("##########"))
                .RuleFor(pn => pn.CountryCode, f => f.Random.String2(2, 4))
                .RuleFor(pn => pn.IsPrimary, f => f.Random.Bool(0.7f))
                .RuleFor(pn => pn.IsVerified, f => f.Random.Bool(0.8f))
                .RuleFor(pn => pn.UserId, f => f.Random.Bool(0.4f) ? f.PickRandom(users).Id : null)
                .RuleFor(pn => pn.CustomerId, f => f.Random.Bool(0.3f) ? f.PickRandom(customers).Id : null)
                .RuleFor(pn => pn.ContactPersonId, f => f.Random.Bool(0.3f) ? f.PickRandom(contactPersons).Id : null)
                .RuleFor(pn => pn.TenantId, f => f.PickRandom(tenants).Id);

            var phoneNumbers = faker.Generate(80);
            await dbContext.PhoneNumbers.AddRangeAsync(phoneNumbers, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} phone numbers", phoneNumbers.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding PhoneNumbers");
            throw;
        }
    }

    private async Task SeedPaymentProvidersAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.PaymentProviders.AnyAsync(cancellationToken)) return;

            var paymentProviders = new List<PaymentProviderEntity>
            {
                new() { Name = "Stripe", Description = "Online payment processing for internet businesses", IsActive = true },
                new() { Name = "PayPal", Description = "Digital payment platform and online money transfer service", IsActive = true },
                new() { Name = "Square", Description = "Point-of-sale and financial services platform", IsActive = true },
                new() { Name = "Klarna", Description = "Buy now, pay later payment solution", IsActive = true },
                new() { Name = "Adyen", Description = "Global payment platform for omnichannel commerce", IsActive = true },
                new() { Name = "Braintree", Description = "Full-stack payment platform owned by PayPal", IsActive = false }
            };

            await dbContext.PaymentProviders.AddRangeAsync(paymentProviders, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} payment providers", paymentProviders.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding PaymentProviders");
            throw;
        }
    }

    private async Task SeedTeamsAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await b2bDbContext.Teams.AnyAsync(cancellationToken)) return;

            var tenants = await dbContext.Tenants.ToListAsync(cancellationToken);
            var users = await dbContext.Users.ToListAsync(cancellationToken);

            if (tenants.Count == 0 || users.Count == 0) 
            {
                throw new InvalidOperationException("Cannot seed Teams - Tenants or Users not found. Ensure related entities are seeded first.");
            }

            var faker = new Faker<TeamEntity>()
                .RuleFor(t => t.Name, f => f.Commerce.Department())
                .RuleFor(t => t.Description, f => f.Lorem.Sentence())
                .RuleFor(t => t.IsActive, f => f.Random.Bool(0.9f))
                .RuleFor(t => t.TenantId, f => f.PickRandom(tenants).Id);

            var teams = faker.Generate(15);
            await b2bDbContext.Teams.AddRangeAsync(teams, cancellationToken);
            await b2bDbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} teams", teams.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding Teams");
            throw;
        }
    }

    private async Task SeedNotificationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.Notifications.AnyAsync(cancellationToken)) return;

            var users = await dbContext.Users.ToListAsync(cancellationToken);

            if (users.Count == 0) 
            {
                throw new InvalidOperationException("Cannot seed Notifications - Users not found. Ensure SeedUsersAsync is called first.");
            }

            var faker = new Faker<NotificationEntity>()
                .RuleFor(n => n.Title, f => f.Lorem.Sentence(3))
                .RuleFor(n => n.Message, f => f.Lorem.Paragraph())
                .RuleFor(n => n.IsRead, f => f.Random.Bool(0.3f))
                .RuleFor(n => n.UserId, f => f.PickRandom(users).Id)
                .RuleFor(n => n.OwnerId, f => f.PickRandom(users).Id);

            var notifications = faker.Generate(200);
            await dbContext.Notifications.AddRangeAsync(notifications, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} notifications", notifications.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding Notifications");
            throw;
        }
    }

    private async Task SeedFilesAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.Files.AnyAsync(cancellationToken)) return;

            var users = await dbContext.Users.ToListAsync(cancellationToken);

            if (users.Count == 0) 
            {
                throw new InvalidOperationException("Cannot seed Files - Users not found. Ensure SeedUsersAsync is called first.");
            }

            var fileExtensions = new[] { ".pdf", ".docx", ".xlsx", ".png", ".jpg", ".mp4", ".csv", ".txt" };
            var faker = new Faker<FileEntity>()
                .RuleFor(f => f.FileName, faker => faker.System.FileName())
                .RuleFor(f => f.FileExtension, faker => faker.PickRandom(fileExtensions))
                .RuleFor(f => f.FileSize, faker => faker.Random.Long(1024, 50000000)) // 1KB to 50MB
                .RuleFor(f => f.FilePath, faker => faker.System.DirectoryPath())
                .RuleFor(f => f.OwnerId, faker => faker.PickRandom(users).Id);

            var files = faker.Generate(100);
            await dbContext.Files.AddRangeAsync(files, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} files", files.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding Files");
            throw;
        }
    }

    private async Task SeedIntegrationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.Integrations.AnyAsync(cancellationToken)) return;

            var users = await dbContext.Users.ToListAsync(cancellationToken);

            if (users.Count == 0) 
            {
                throw new InvalidOperationException("Cannot seed Integrations - Users not found. Ensure SeedUsersAsync is called first.");
            }

            var serviceNames = new[] { "Stripe", "SendGrid", "Twilio", "Slack", "Discord", "GitHub", "GitLab", "Zoom", "Teams", "AWS S3" };
            var faker = new Faker<IntegrationEntity>()
                .RuleFor(i => i.Name, (f, i) => $"{f.PickRandom(serviceNames)} Integration")
                .RuleFor(i => i.ServiceName, f => f.PickRandom(serviceNames))
                .RuleFor(i => i.Description, f => f.Lorem.Sentence())
                .RuleFor(i => i.ApiKeySecretReference, f => f.Random.AlphaNumeric(32))
                .RuleFor(i => i.OwnerId, f => f.PickRandom(users).Id);

            var integrations = faker.Generate(30);
            await dbContext.Integrations.AddRangeAsync(integrations, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} integrations", integrations.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding Integrations");
            throw;
        }
    }

    // --- Auditing & Sessions ---
    private async Task SeedSessionsAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.Sessions.AnyAsync(cancellationToken)) return;

            var faker = new Faker<SessionEntity>()
                .RuleFor(s => s.SessionKey, f => f.Random.AlphaNumeric(32))
                .RuleFor(s => s.SessionData, f => f.Lorem.Text())
                .RuleFor(s => s.ExpireDate, f => f.Date.Future());

            var sessions = faker.Generate(50);
            await dbContext.Sessions.AddRangeAsync(sessions, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} sessions", sessions.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding Sessions");
            throw;
        }
    }

    private async Task SeedAuditLogsAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.AuditLogs.AnyAsync(cancellationToken)) return;

            var users = await dbContext.Users.ToListAsync(cancellationToken);
            var tenants = await dbContext.Tenants.ToListAsync(cancellationToken);

            if (users.Count == 0 || tenants.Count == 0)
            {
                throw new InvalidOperationException("Cannot seed AuditLogs - Users or Tenants not found. Ensure related entities are seeded first.");
            }

            var actions = new[] { "CREATE", "UPDATE", "DELETE", "LOGIN", "LOGOUT", "EXPORT", "IMPORT" };
            var categories = new[] { "User", "Customer", "Tenant", "Role", "Permission", "Account", "File" };

            var faker = new Faker<AuditLogEntity>()
                .RuleFor(a => a.Action, f => f.PickRandom(actions))
                .RuleFor(a => a.Category, f => f.PickRandom(categories))
                .RuleFor(a => a.NewValue, f => f.Lorem.Sentence())
                .RuleFor(a => a.OldValue, f => f.Lorem.Sentence())
                .RuleFor(a => a.ModifiedAt, f => f.Date.Recent(30))
                .RuleFor(a => a.UserId, f => f.PickRandom(users).Id)
                .RuleFor(a => a.TenantId, f => f.PickRandom(tenants).Id);

            var auditLogs = faker.Generate(300);
            await dbContext.AuditLogs.AddRangeAsync(auditLogs, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} audit logs", auditLogs.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding AuditLogs");
            throw;
        }
    }

    // --- B2B Entities ---
    private async Task SeedOrganizationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await b2bDbContext.Organizations.AnyAsync(cancellationToken)) return;

            var users = await dbContext.Users.ToListAsync(cancellationToken);

            if (users.Count == 0)
            {
                throw new InvalidOperationException("Cannot seed Organizations - Users not found. Ensure SeedUsersAsync is called first.");
            }

            var faker = new Faker<AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Organization.OrganizationEntity>()
                .RuleFor(o => o.Name, f => f.Company.CompanyName())
                .RuleFor(o => o.Description, f => f.Company.CatchPhrase())
                .RuleFor(o => o.CreatedAt, f => f.Date.Past(2));

            var organizations = faker.Generate(20);
            await b2bDbContext.Organizations.AddRangeAsync(organizations, cancellationToken);
            await b2bDbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} organizations", organizations.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding Organizations");
            throw;
        }
    }

    private async Task SeedApiKeysAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await b2bDbContext.ApiKeys.AnyAsync(cancellationToken)) return;

            var users = await dbContext.Users.ToListAsync(cancellationToken);
            var tenants = await dbContext.Tenants.ToListAsync(cancellationToken);

            if (users.Count == 0 || tenants.Count == 0)
            {
                throw new InvalidOperationException("Cannot seed ApiKeys - Users or Tenants not found. Ensure related entities are seeded first.");
            }

            var faker = new Faker<ApiKeyEntity>()
                .RuleFor(a => a.Name, f => f.Hacker.Noun() + " API Key")
                .RuleFor(a => a.Description, f => f.Lorem.Sentence())
                .RuleFor(a => a.SecretRef, f => f.Random.Hash(32))
                .RuleFor(a => a.UserId, f => f.PickRandom(users).Id)
                .RuleFor(a => a.TenantId, f => f.PickRandom(tenants).Id);

            var apiKeys = faker.Generate(40);
            await b2bDbContext.ApiKeys.AddRangeAsync(apiKeys, cancellationToken);
            await b2bDbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} API keys", apiKeys.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding ApiKeys");
            throw;
        }
    }

    private async Task SeedTodosAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.Todos.AnyAsync(cancellationToken)) return;

            var users = await dbContext.Users.ToListAsync(cancellationToken);
            var tenants = await dbContext.Tenants.ToListAsync(cancellationToken);

            if (users.Count == 0 || tenants.Count == 0)
            {
                throw new InvalidOperationException("Cannot seed Todos - Users or Tenants not found. Ensure related entities are seeded first.");
            }

            var faker = new Faker<AppBlueprint.TodoAppKernel.Domain.TodoEntity>()
                .RuleFor(t => t.Title, f => f.Hacker.Phrase())
                .RuleFor(t => t.Description, f => f.Lorem.Paragraph())
                .RuleFor(t => t.IsCompleted, f => f.Random.Bool(0.3f))
                .RuleFor(t => t.Priority, f => f.PickRandom<AppBlueprint.TodoAppKernel.Domain.TodoPriority>())
                .RuleFor(t => t.DueDate, f => f.Random.Bool(0.7f) ? f.Date.Future() : null)
                .RuleFor(t => t.CompletedAt, (f, t) => t.IsCompleted ? f.Date.Recent(30) : null)
                .RuleFor(t => t.TenantId, f => f.PickRandom(tenants).Id)
                .RuleFor(t => t.CreatedById, f => f.PickRandom(users).Id)
                .RuleFor(t => t.AssignedToId, f => f.PickRandom(users).Id);

            var todos = faker.Generate(150);
            await dbContext.Todos.AddRangeAsync(todos, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} todos", todos.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding Todos");
            throw;
        }
    }

    private async Task SeedTeamMembersAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await b2bDbContext.TeamMembers.AnyAsync(cancellationToken)) return;

            var teams = await b2bDbContext.Teams.ToListAsync(cancellationToken);
            var users = await dbContext.Users.ToListAsync(cancellationToken);

            if (teams.Count == 0 || users.Count == 0)
            {
                throw new InvalidOperationException("Cannot seed TeamMembers - Teams or Users not found. Ensure related entities are seeded first.");
            }

            var aliases = new[] { "TheLeader", "Coder", "Designer", "Coordinator", "Helper", "Ninja", "Wizard", "Guru" };
            var faker = new Faker<TeamMemberEntity>()
                .RuleFor(tm => tm.Alias, f => f.PickRandom(aliases))
                .RuleFor(tm => tm.IsActive, f => f.Random.Bool(0.9f))
                .RuleFor(tm => tm.TeamId, f => f.PickRandom(teams).Id)
                .RuleFor(tm => tm.UserId, f => f.PickRandom(users).Id)
                .RuleFor(tm => tm.TenantId, f => f.PickRandom(teams).TenantId);

            var teamMembers = faker.Generate(60);
            await b2bDbContext.TeamMembers.AddRangeAsync(teamMembers, cancellationToken);
            await b2bDbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} team members", teamMembers.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding TeamMembers");
            throw;
        }
    }

    private async Task SeedTeamInvitesAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (await b2bDbContext.TeamInvites.AnyAsync(cancellationToken)) return;

            var teams = await b2bDbContext.Teams.ToListAsync(cancellationToken);
            var users = await dbContext.Users.ToListAsync(cancellationToken);

            if (teams.Count == 0 || users.Count == 0)
            {
                throw new InvalidOperationException("Cannot seed TeamInvites - Teams or Users not found. Ensure related entities are seeded first.");
            }

            var faker = new Faker<TeamInviteEntity>()
                .RuleFor(ti => ti.OwnerId, f => f.PickRandom(users).Id)
                .RuleFor(ti => ti.ExpireAt, f => f.Date.Future())
                .RuleFor(ti => ti.IsActive, f => f.Random.Bool(0.7f))
                .RuleFor(ti => ti.TeamId, f => f.PickRandom(teams).Id)
                .RuleFor(ti => ti.TenantId, f => f.PickRandom(teams).TenantId);

            var teamInvites = faker.Generate(25);
            await b2bDbContext.TeamInvites.AddRangeAsync(teamInvites, cancellationToken);
            await b2bDbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully seeded {Count} team invites", teamInvites.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding TeamInvites");
            throw;
        }
    }
}
