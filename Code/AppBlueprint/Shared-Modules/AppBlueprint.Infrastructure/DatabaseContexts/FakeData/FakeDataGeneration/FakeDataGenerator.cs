using AutoBogus;
using Newtonsoft.Json;
// using Shared.Models;
using Assembly = System.Reflection.Assembly;

namespace AppBlueprint.Infrastructure.DatabaseContexts.FakeData.FakeDataGeneration;

public class FakeDataGenerator
{
    private readonly Dictionary<Type, object> _fakerConfigurations = new();

    public FakeDataGenerator()
    {
        IEnumerable<Type>? entityTypes = Assembly.GetAssembly(typeof(IEntity)).GetTypes()
            .Where(t => t.Namespace == "Shared.Models");

        foreach (Type? type in entityTypes)
        {
            Type? autoFakerType = typeof(AutoFaker<>).MakeGenericType(type);
            object? autoFaker = Activator.CreateInstance(autoFakerType);

            _fakerConfigurations[type] = autoFaker;
        }
    }

    public T GenerateFakeData<T>() where T : class
    {
        if (_fakerConfigurations.TryGetValue(typeof(T), out object? configuration))
        {
            object? autoFaker = configuration.GetType().GetMethod("Generate").Invoke(configuration, null);
            return (T)autoFaker;
        }

        return new AutoFaker<T>().Generate();
    }

    public void GenerateAndSaveFakeData<T>(string filePath) where T : class
    {
        T? fakeData = GenerateFakeData<T>();
        string? json = JsonConvert.SerializeObject(fakeData, Formatting.Indented);

        File.WriteAllText(filePath, json);
    }
}

internal interface IEntity
{
}

// public class FakeDataGenerator
// {
//     private readonly FakeUserDataGenerator _fakeUserDataGenerator;
// private readonly FakeTeamDataGenerator _fakeTeamDataGenerator;
// private readonly FakeTenantDataGenerator _fakeTenantDataGenerator;

// public FakeDataGenerator()
// {
//     _fakeUserDataGenerator = new FakeUserDataGenerator();
//     _fakeTeamDataGenerator = new FakeTeamDataGenerator();
//     _fakeTenantDataGenerator = new FakeTenantDataGenerator();
// }

// public T GenerateFakeData<T>() where T : class
// {
//     return new AutoFaker<T>().Generate();
// }

// public void AutoGenerateFakeData()
// {
//     // Generate a single instance of your class
//     var fakeTeam = AutoFaker.Generate<TeamModel>();

//     // Generate a list of instances of your class
//     var fakeUser = AutoFaker.Generate<UserModel>();

//     fakeUser.EmailAddresses = new List<EmailAddressModel>();

//     //         var customFaker = new AutoFaker<MyClass>()
//     // .RuleFor(x => x.MyProperty, faker => faker.Random.Word());

// }

// public List<UserModel> GenerateFakeUsers()
// {
//     List<UserModel> fakeUsers = new List<UserModel>();
//     for (int i = 0; i < 100; i++)
//     {
//         fakeUsers.Add(_fakeUserDataGenerator.GenerateUserModelFakeData());
//     }
//     return fakeUsers;
// }

// public List<TeamModel> GenerateFakeTeams()
// {
//     List<TeamModel> fakeTeams = new List<TeamModel>();
//     for (int i = 0; i < 100; i++)
//     {
//         fakeTeams.Add(_fakeTeamDataGenerator.GenerateTeamModelFakeData());
//     }
//     return fakeTeams;
// }

// public List<TenantModel> GenerateFakeTenants()
// {
//     List<TenantModel> fakeTenants = new List<TenantModel>();
//     for (int i = 0; i < 100; i++)
//     {
//         fakeTenants.Add(_fakeTenantDataGenerator.GenerateTenantModelFakeData());
//     }
//     return fakeTenants;
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
