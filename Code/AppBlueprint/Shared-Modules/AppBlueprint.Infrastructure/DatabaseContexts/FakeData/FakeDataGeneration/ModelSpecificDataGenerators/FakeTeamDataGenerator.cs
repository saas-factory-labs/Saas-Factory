using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using Bogus;

// using Shared.Models;

namespace AppBlueprint.Infrastructure.DatabaseContexts.FakeData.FakeDataGeneration.ModelSpecificDataGenerators;

public class FakeTeamDataGenerator
{
    private readonly Faker _faker;

    public FakeTeamDataGenerator()
    {
        _faker = new Faker();
    }

    public TeamEntity GenerateTeamModelFakeData()
    {
        var fakeTeam = new TeamEntity();

        fakeTeam.Name = _faker.Company.CompanyName();
        fakeTeam.CreatedAt = _faker.Date.Past();
        fakeTeam.Id = _faker.Random.Int(1, 1000);
        fakeTeam.Description = _faker.Lorem.Sentence();
        fakeTeam.IsActive = _faker.Random.Bool();
        // fakeTeam.OwnerId = _faker.Random.Int(1, 1000);
        fakeTeam.LastUpdatedAt = _faker.Date.Past();

        return fakeTeam;
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
