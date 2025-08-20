using AppBlueprint.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using TUnit;
using Stripe;

namespace AppBlueprint.Tests.Infrastructure;

public class StripeSubscriptionServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly StripeSubscriptionService _stripeSubscriptionService;
    private const string TestApiKey = "sk_test_123456789";

    public StripeSubscriptionServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(c => c.GetConnectionString("StripeApiKey"))
                         .Returns(TestApiKey);
        
        _stripeSubscriptionService = new StripeSubscriptionService(_configurationMock.Object);
    }

    [Test]
    public void Constructor_ShouldThrowException_WhenApiKeyIsNull()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c.GetConnectionString("StripeApiKey"))
                  .Returns((string?)null);

        // Act & Assert
        var act = () => new StripeSubscriptionService(configMock.Object);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("StripeApiKey connection string is not configured.");
    }

    [Test]
    public async Task CreateCustomerAsync_ShouldThrowArgumentException_WhenEmailIsNullOrEmpty()
    {
        // Act & Assert
        var act = async () => await _stripeSubscriptionService.CreateCustomerAsync("", "pm_123");
        await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Email cannot be null or empty*");

        var act2 = async () => await _stripeSubscriptionService.CreateCustomerAsync(null!, "pm_123");
        await act2.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("Email cannot be null or empty*");
    }

    [Test]
    public async Task CreateCustomerAsync_ShouldThrowArgumentException_WhenPaymentMethodIdIsNullOrEmpty()
    {
        // Act & Assert
        var act = async () => await _stripeSubscriptionService.CreateCustomerAsync("test@test.com", "");
        await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Payment method ID cannot be null or empty*");

        var act2 = async () => await _stripeSubscriptionService.CreateCustomerAsync("test@test.com", null!);
        await act2.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("Payment method ID cannot be null or empty*");
    }

    [Test]
    public async Task CreateSubscriptionAsync_ShouldThrowArgumentException_WhenCustomerIdIsNullOrEmpty()
    {
        // Act & Assert
        var act = async () => await _stripeSubscriptionService.CreateSubscriptionAsync("", "price_123");
        await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Customer ID cannot be null or empty*");

        var act2 = async () => await _stripeSubscriptionService.CreateSubscriptionAsync(null!, "price_123");
        await act2.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("Customer ID cannot be null or empty*");
    }

    [Test]
    public async Task CreateSubscriptionAsync_ShouldThrowArgumentException_WhenPriceIdIsNullOrEmpty()
    {
        // Act & Assert
        var act = async () => await _stripeSubscriptionService.CreateSubscriptionAsync("cus_123", "");
        await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Price ID cannot be null or empty*");

        var act2 = async () => await _stripeSubscriptionService.CreateSubscriptionAsync("cus_123", null!);
        await act2.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("Price ID cannot be null or empty*");
    }

    [Test]
    public async Task GetSubscriptionAsync_ShouldThrowArgumentException_WhenSubscriptionIdIsNullOrEmpty()
    {
        // Act & Assert
        var act = async () => await _stripeSubscriptionService.GetSubscriptionAsync("");
        await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Subscription ID cannot be null or empty*");

        var act2 = async () => await _stripeSubscriptionService.GetSubscriptionAsync(null!);
        await act2.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("Subscription ID cannot be null or empty*");
    }

    [Test]
    public async Task CancelSubscriptionAsync_ShouldThrowArgumentException_WhenSubscriptionIdIsNullOrEmpty()
    {
        // Act & Assert
        var act = async () => await _stripeSubscriptionService.CancelSubscriptionAsync("");
        await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Subscription ID cannot be null or empty*");

        var act2 = async () => await _stripeSubscriptionService.CancelSubscriptionAsync(null!);
        await act2.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("Subscription ID cannot be null or empty*");
    }

    [Test]
    public async Task GetCustomerSubscriptionsAsync_ShouldThrowArgumentException_WhenCustomerIdIsNullOrEmpty()
    {
        // Act & Assert
        var act = async () => await _stripeSubscriptionService.GetCustomerSubscriptionsAsync("");
        await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Customer ID cannot be null or empty*");

        var act2 = async () => await _stripeSubscriptionService.GetCustomerSubscriptionsAsync(null!);
        await act2.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("Customer ID cannot be null or empty*");
    }
}