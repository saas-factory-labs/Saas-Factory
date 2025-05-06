using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using Bogus;

// using Shared.Models;

namespace AppBlueprint.Infrastructure.DatabaseContexts.FakeData.FakeDataGeneration.ModelSpecificDataGenerators;

public class FakeUserDataGenerator
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
            Profile = new ProfileEntity()
        };


        //fakeUser.Avatar = _faker.Internet.Avatar();
        //fakeUser.UserName = _faker.Person.UserName;
        //fakeUser.Slug = _faker.Person.UserName;
        //fakeUser.CreatedAt = _faker.Date.Past();
        //fakeUser.LastLogin = _faker.Date.Past();
        //fakeUser.Id = _faker.Random.Int(1, 1000);
        //fakeUser.LastLogin = _faker.Date.Past();
        //fakeUser.IsActive = _faker.Random.Bool();
        //fakeUser.Language = _faker.Random.String2(2);

        // mising person 
        // missing addresses
        // missing emails
        // missing phones
        // missing verifications

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
