using AppBlueprint.Application.Interfaces;
using AppBlueprint.Application.Signup;
using AppBlueprint.Application.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AppBlueprint.Tests.Application;

internal sealed class SignupServiceTests
{
    [Test]
    public async Task CreateTenantAndUserAsync_ShouldRejectWhenTermsAreNotAccepted()
    {
        var contextProvider = Substitute.For<ISignupDbContextProvider>();
        var logger = Substitute.For<ILogger<SignupService>>();
        var signupService = new SignupService(contextProvider, logger);
        var request = new SignupRequest
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            AcceptTerms = false,
            TenantType = SignupTenantType.Personal
        };
        var authenticatedIdentity = new AuthenticatedSignupIdentity
        {
            Email = "ada@example.com",
            ExternalAuthId = "logto|ada"
        };

        Func<Task> act = async () => await signupService.CreateTenantAndUserAsync(request, authenticatedIdentity);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Terms and conditions must be accepted.*");
        await contextProvider.DidNotReceive().GetDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task CreateTenantAndUserAsync_ShouldRejectWhenExternalAuthIdIsMissing()
    {
        var contextProvider = Substitute.For<ISignupDbContextProvider>();
        var logger = Substitute.For<ILogger<SignupService>>();
        var signupService = new SignupService(contextProvider, logger);
        var request = new SignupRequest
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            AcceptTerms = true,
            TenantType = SignupTenantType.Personal
        };
        var authenticatedIdentity = new AuthenticatedSignupIdentity
        {
            Email = "ada@example.com",
            ExternalAuthId = string.Empty
        };

        Func<Task> act = async () => await signupService.CreateTenantAndUserAsync(request, authenticatedIdentity);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*External auth ID is required.*");
        await contextProvider.DidNotReceive().GetDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task CreateTenantAndUserAsync_ShouldRejectBusinessSignupWithoutCompanyName()
    {
        var contextProvider = Substitute.For<ISignupDbContextProvider>();
        var logger = Substitute.For<ILogger<SignupService>>();
        var signupService = new SignupService(contextProvider, logger);
        var request = new SignupRequest
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            CompanyName = " ",
            Country = "Denmark",
            AcceptTerms = true,
            TenantType = SignupTenantType.Organization
        };
        var authenticatedIdentity = new AuthenticatedSignupIdentity
        {
            Email = "ada@example.com",
            ExternalAuthId = "logto|ada"
        };

        Func<Task> act = async () => await signupService.CreateTenantAndUserAsync(request, authenticatedIdentity);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Company name is required for organization signups.*");
        await contextProvider.DidNotReceive().GetDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task CreateTenantAndUserAsync_ShouldRejectBusinessSignupWithoutCountry()
    {
        var contextProvider = Substitute.For<ISignupDbContextProvider>();
        var logger = Substitute.For<ILogger<SignupService>>();
        var signupService = new SignupService(contextProvider, logger);
        var request = new SignupRequest
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            CompanyName = "Analytical Engines Ltd",
            Country = " ",
            AcceptTerms = true,
            TenantType = SignupTenantType.Organization
        };
        var authenticatedIdentity = new AuthenticatedSignupIdentity
        {
            Email = "ada@example.com",
            ExternalAuthId = "logto|ada"
        };

        Func<Task> act = async () => await signupService.CreateTenantAndUserAsync(request, authenticatedIdentity);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Country is required for organization signups.*");
        await contextProvider.DidNotReceive().GetDbContextAsync(Arg.Any<CancellationToken>());
    }
}
