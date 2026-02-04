using AppBlueprint.Application.Validations;
using AppBlueprint.Contracts.Baseline.Permissions.Requests;
using AppBlueprint.Contracts.Baseline.Role.Requests;
using FluentAssertions;
using TUnit;

namespace AppBlueprint.Tests.Validations;

public class CreateRoleRequestValidatorTests
{
    private readonly CreateRoleRequestValidator _validator = new();

    [Test]
    public async Task ValidateWithValidRequestShouldPass()
    {
        // Arrange
        var request = new CreateRoleRequest
        {
            Id = "role_123",
            Name = "Administrator",
            Description = "Full system access",
            Permissions = new List<CreatePermissionRequest>
            {
                new() { Name = "read", Description = "Read access" }
            }
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
        var request = new CreateRoleRequest
        {
            Id = "role_123",
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
        var request = new CreateRoleRequest
        {
            Id = "role_123",
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
        var request = new CreateRoleRequest
        {
            Id = "role_123",
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
        var request = new CreateRoleRequest
        {
            Id = "role_123",
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
        var request = new CreateRoleRequest
        {
            Id = "role_123",
            Name = "Valid Role Name",
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
        var request = new CreateRoleRequest
        {
            Id = "role_123",
            Name = "Valid Role Name",
            Description = new string('A', 501) // 501 characters
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Test]
    public async Task ValidateWithNullPermissionsShouldPass()
    {
        // Arrange
        var request = new CreateRoleRequest
        {
            Id = "role_123",
            Name = "Valid Role Name",
            Description = "Some description",
            Permissions = null
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task ValidateWithEmptyPermissionsListShouldPass()
    {
        // Arrange
        var request = new CreateRoleRequest
        {
            Id = "role_123",
            Name = "Valid Role Name",
            Description = "Some description",
            Permissions = new List<CreatePermissionRequest>()
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task ValidateWithPermissionHavingEmptyNameShouldFail()
    {
        // Arrange
        var request = new CreateRoleRequest
        {
            Id = "role_123",
            Name = "Valid Role Name",
            Description = "Some description",
            Permissions = new List<CreatePermissionRequest>
            {
                new() { Name = "", Description = "Some permission" }
            }
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Permissions"));
    }

    [Test]
    public async Task ValidateWithPermissionNameExceedingMaxLengthShouldFail()
    {
        // Arrange
        var request = new CreateRoleRequest
        {
            Id = "role_123",
            Name = "Valid Role Name",
            Description = "Some description",
            Permissions = new List<CreatePermissionRequest>
            {
                new() { Name = new string('A', 101), Description = "Some permission" }
            }
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Permissions"));
    }

    [Test]
    public async Task ValidateWithPermissionDescriptionExceedingMaxLengthShouldFail()
    {
        // Arrange
        var request = new CreateRoleRequest
        {
            Id = "role_123",
            Name = "Valid Role Name",
            Description = "Some description",
            Permissions = new List<CreatePermissionRequest>
            {
                new() { Name = "read", Description = new string('A', 501) }
            }
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Permissions"));
    }

    [Test]
    public async Task ValidateWithMultipleValidPermissionsShouldPass()
    {
        // Arrange
        var request = new CreateRoleRequest
        {
            Id = "role_123",
            Name = "Administrator",
            Description = "Full system access",
            Permissions = new List<CreatePermissionRequest>
            {
                new() { Name = "read", Description = "Read access" },
                new() { Name = "write", Description = "Write access" },
                new() { Name = "delete", Description = null }
            }
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task ValidateWithEmptyIdShouldPass()
    {
        // Arrange - Id defaults to string.Empty and is optional
        var request = new CreateRoleRequest
        {
            Id = string.Empty,
            Name = "Valid Role Name",
            Description = "Some description"
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
