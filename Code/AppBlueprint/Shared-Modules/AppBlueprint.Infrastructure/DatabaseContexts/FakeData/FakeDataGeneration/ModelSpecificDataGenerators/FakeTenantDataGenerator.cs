using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.SharedKernel;
using Bogus;

// using Shared.Models;

namespace AppBlueprint.Infrastructure.DatabaseContexts.FakeData.FakeDataGeneration.ModelSpecificDataGenerators;

public class FakeTenantDataGenerator
{
    private readonly Faker _faker;

    public FakeTenantDataGenerator()
    {
        _faker = new Faker();
    }

    public TenantEntity GenerateTenantModelFakeData()
    {
        var fakeTenant = new TenantEntity();

        fakeTenant.Name = _faker.Company.CompanyName();
        fakeTenant.CreatedAt = _faker.Date.Past();
        fakeTenant.Id = PrefixedUlid.Generate("tenant");
        fakeTenant.Description = _faker.Lorem.Sentence();
        fakeTenant.IsActive = _faker.Random.Bool();
        fakeTenant.LastUpdatedAt = _faker.Date.Past();

        return fakeTenant;
    }
}
