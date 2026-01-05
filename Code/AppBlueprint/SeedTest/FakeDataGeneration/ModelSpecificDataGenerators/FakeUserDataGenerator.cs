using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using Bogus;

namespace AppBlueprint.SeedTest.FakeDataGeneration.ModelSpecificDataGenerators;

internal sealed class FakeUserDataGenerator
{
    private readonly Faker _faker;

    public FakeUserDataGenerator()
    {
        _faker = new Faker();
    }

    public UserEntity GenerateUserModelFakeData()
    {
        var fakeUser = new UserEntity
        {
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            Email = _faker.Person.Email,
            UserName = _faker.Person.UserName,
            Profile = new ProfileEntity(),
            LastLogin = _faker.Date.Past(),
            IsActive = _faker.Random.Bool()
        };
        return fakeUser;
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
