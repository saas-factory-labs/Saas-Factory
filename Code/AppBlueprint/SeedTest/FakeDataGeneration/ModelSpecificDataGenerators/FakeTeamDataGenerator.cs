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
        var fakeTeam = new TeamEntity
        {
            Name = _faker.Company.CompanyName(),
            CreatedAt = _faker.Date.Past(),
            Id = PrefixedUlid.Generate("team"),
            Description = _faker.Lorem.Sentence(),
            IsActive = _faker.Random.Bool(),
            LastUpdatedAt = _faker.Date.Past()
        };

        return fakeTeam;
    }
}

