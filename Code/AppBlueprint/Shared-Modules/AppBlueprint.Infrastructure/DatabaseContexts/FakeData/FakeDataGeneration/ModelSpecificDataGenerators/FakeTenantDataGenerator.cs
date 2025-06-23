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

// Method to generate test data object for UserModel
// public UserModel GenerateUserModelTestData()
// {
//     UserModel fakeUser = new UserModel();

//     for (int i = 0; i < 2; i++)
//     {
//         fakeUser.Addresses.Add(new AddressModel()
//         {

//         });
//     }

// }

// // Add methods for other entity models

// // Method to generate test data objects for all entity models
// public Dictionary<string, object> GenerateAllTestData()
// {
//     Dictionary<string, object> testData = new Dictionary<string, object>();
//     testData.Add("UserModel", GenerateUserModelTestData());
//     // Add other entity models
//     return testData;
// }
