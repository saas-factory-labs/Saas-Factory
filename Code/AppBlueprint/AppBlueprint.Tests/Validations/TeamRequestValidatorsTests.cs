using AppBlueprint.Application.Validations;
using AppBlueprint.Contracts.B2B.Contracts.Team.Requests;
using FluentAssertions;
using TUnit;

namespace AppBlueprint.Tests.Validations;

public class CreateTeamRequestValidatorTests
{
    private readonly CreateTeamRequestValidator _validator = new();

    [Test]
    public async Task ValidateWithValidRequestShouldPass()
    {
        // Arrange
        var request = new CreateTeamRequest
        {
            Name = "Engineering Team",
            Description = "Team responsible for product development"
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public async Task ValidateWithEmptyNameShouldFail()
    {
        // Arrange
        var request = new CreateTeamRequest
        {
            Name = "",
            Description = "Some description"
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Test]
    public async Task ValidateWithNullNameShouldFail()
    {
        // Arrange
        var request = new CreateTeamRequest
        {
            Name = null!,
            Description = "Some description"
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Test]
    public async Task ValidateWithNameExceedingMaxLengthShouldFail()
    {
        // Arrange
        var request = new CreateTeamRequest
        {
            Name = new string('A', 101), // 101 characters
            Description = "Some description"
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Test]
    public async Task ValidateWithMaxLengthNameShouldPass()
    {
        // Arrange
        var request = new CreateTeamRequest
        {
            Name = new string('A', 100), // Exactly 100 characters
            Description = "Some description"
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task ValidateWithNullDescriptionShouldPass()
    {
        // Arrange
        var request = new CreateTeamRequest
        {
            Name = "Valid Team Name",
            Description = null
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task ValidateWithDescriptionExceedingMaxLengthShouldFail()
    {
        // Arrange
        var request = new CreateTeamRequest
        {
            Name = "Valid Team Name",
            Description = new string('A', 501) // 501 characters
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }
}

public class UpdateTeamRequestValidatorTests
{
    private readonly UpdateTeamRequestValidator _validator = new();

    [Test]
    public async Task ValidateWithValidRequestShouldPass()
    {
        // Arrange
        var request = new UpdateTeamRequest
        {
            Name = "Updated Team Name",
            Description = "Updated description"
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public async Task ValidateWithBothNullPropertiesShouldFail()
    {
        // Arrange
        var request = new UpdateTeamRequest
        {
            Name = null,
            Description = null
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Test]
    public async Task ValidateWithEmptyNameShouldFail()
    {
        // Arrange
        var request = new UpdateTeamRequest
        {
            Name = "",
            Description = "Some description"
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Test]
    public async Task ValidateWithNameExceedingMaxLengthShouldFail()
    {
        // Arrange
        var request = new UpdateTeamRequest
        {
            Name = new string('A', 101), // 101 characters
            Description = null
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Test]
    public async Task ValidateWithDescriptionExceedingMaxLengthShouldFail()
    {
        // Arrange
        var request = new UpdateTeamRequest
        {
            Name = null,
            Description = new string('A', 501) // 501 characters
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Test]
    public async Task ValidateWithOnlyNameProvidedShouldPass()
    {
        // Arrange
        var request = new UpdateTeamRequest
        {
            Name = "Updated Name",
            Description = null
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task ValidateWithOnlyDescriptionProvidedShouldPass()
    {
        // Arrange
        var request = new UpdateTeamRequest
        {
            Name = null,
            Description = "Updated description"
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
