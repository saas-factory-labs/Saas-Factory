using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.SharedKernel;
using Bogus;

// using Shared.Models;

namespace AppBlueprint.SeedTest.FakeDataGeneration.ModelSpecificDataGenerators;

internal sealed class FakeTeamDataGenerator
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
        fakeTeam.Id = PrefixedUlid.Generate("team");
        fakeTeam.Description = _faker.Lorem.Sentence();
        fakeTeam.IsActive = _faker.Random.Bool();
        fakeTeam.LastUpdatedAt = _faker.Date.Past();

        return fakeTeam;
    }
}

