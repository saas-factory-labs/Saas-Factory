using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;
using AppBlueprint.SharedKernel;
using Bogus;

// using Shared.Models;

namespace AppBlueprint.SeedTest.FakeDataGeneration.ModelSpecificDataGenerators;

internal sealed class FakeTenantDataGenerator
{
    private readonly Faker _faker;

    public FakeTenantDataGenerator()
    {
        _faker = new Faker();
    }

    public TenantEntity GenerateTenantModelFakeData()
    {
        var fakeTenant = new TenantEntity
        {
            Name = _faker.Company.CompanyName(),
            CreatedAt = _faker.Date.Past(),
            Id = PrefixedUlid.Generate("tenant"),
            Description = _faker.Lorem.Sentence(),
            IsActive = _faker.Random.Bool(),
            LastUpdatedAt = _faker.Date.Past()
        };

        return fakeTenant;
    }
}
