using AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
using AppBlueprint.Infrastructure.Services;
using AppBlueprint.Contracts.Baseline.Payment.Requests;
using AppBlueprint.Contracts.Baseline.Payment.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TUnit;
using Stripe;

namespace AppBlueprint.Tests.ApiControllers;

public class PaymentControllerTests
{
    private readonly Mock<StripeSubscriptionService> _stripeServiceMock;
    private readonly Mock<ILogger<PaymentController>> _loggerMock;
    private readonly PaymentController _controller;

    public PaymentControllerTests()
    {
        _stripeServiceMock = new Mock<StripeSubscriptionService>();
        _loggerMock = new Mock<ILogger<PaymentController>>();
        _controller = new PaymentController(_stripeServiceMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task CreateCustomer_ShouldReturnCreated_WhenValidRequest()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Email = "test@example.com",
            PaymentMethodId = "pm_123456"
        };

        var customerEntity = new CustomerEntity
        {
            Id = "cus_123456",
            Email = "test@example.com",
            Name = "Test Customer",
            PhoneNumber = "",
            CreatedAt = DateTime.UtcNow
        };

        _stripeServiceMock.Setup(s => s.CreateCustomerAsync(request.Email, request.PaymentMethodId))
                         .ReturnsAsync(customerEntity);

        // Act
        var result = await _controller.CreateCustomer(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.Value.Should().BeOfType<CustomerResponse>();
        
        var response = createdResult.Value as CustomerResponse;
        response!.Id.Should().Be(customerEntity.Id);
        response.Email.Should().Be(customerEntity.Email);
    }

    [Test]
    public async Task CreateCustomer_ShouldReturnBadRequest_WhenServiceReturnsNull()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Email = "test@example.com",
            PaymentMethodId = "pm_123456"
        };

        _stripeServiceMock.Setup(s => s.CreateCustomerAsync(request.Email, request.PaymentMethodId))
                         .ReturnsAsync((CustomerEntity?)null);

        // Act
        var result = await _controller.CreateCustomer(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<ProblemDetails>();
    }

    [Test]
    public async Task CreateCustomer_ShouldReturnBadRequest_WhenArgumentException()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Email = "",
            PaymentMethodId = "pm_123456"
        };

        _stripeServiceMock.Setup(s => s.CreateCustomerAsync(request.Email, request.PaymentMethodId))
                         .ThrowsAsync(new ArgumentException("Email cannot be null or empty"));

        // Act
        var result = await _controller.CreateCustomer(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<ProblemDetails>();
    }

    [Test]
    public async Task CreateSubscription_ShouldReturnCreated_WhenValidRequest()
    {
        // Arrange
        var request = new CreateSubscriptionRequest
        {
            CustomerId = "cus_123456",
            PriceId = "price_123456"
        };

        var stripeSubscription = CreateMockSubscription("sub_123456", "cus_123456", "price_123456");

        _stripeServiceMock.Setup(s => s.CreateSubscriptionAsync(request.CustomerId, request.PriceId))
                         .ReturnsAsync(stripeSubscription);

        // Act
        var result = await _controller.CreateSubscription(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.Value.Should().BeOfType<SubscriptionResponse>();
        
        var response = createdResult.Value as SubscriptionResponse;
        response!.Id.Should().Be(stripeSubscription.Id);
        response.CustomerId.Should().Be(stripeSubscription.CustomerId);
    }

    [Test]
    public async Task CancelSubscription_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        var request = new CancelSubscriptionRequest
        {
            SubscriptionId = "sub_123456"
        };

        var stripeSubscription = CreateMockSubscription("sub_123456", "cus_123456", "price_123456");
        stripeSubscription.Status = "canceled";
        stripeSubscription.CanceledAt = DateTime.UtcNow;

        _stripeServiceMock.Setup(s => s.CancelSubscriptionAsync(request.SubscriptionId))
                         .ReturnsAsync(stripeSubscription);

        // Act
        var result = await _controller.CancelSubscription(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<SubscriptionResponse>();
        
        var response = okResult.Value as SubscriptionResponse;
        response!.Id.Should().Be(stripeSubscription.Id);
        response.Status.Should().Be("canceled");
    }

    private static Subscription CreateMockSubscription(string id, string customerId, string priceId)
    {
        var subscription = new Subscription
        {
            Id = id,
            CustomerId = customerId,
            Status = "active",
            Created = DateTime.UtcNow,
            CurrentPeriodStart = DateTime.UtcNow,
            CurrentPeriodEnd = DateTime.UtcNow.AddMonths(1)
        };

        // Mock subscription items
        var price = new Price
        {
            Id = priceId,
            UnitAmount = 1999,
            Currency = "usd"
        };

        var subscriptionItem = new SubscriptionItem
        {
            Price = price
        };

        subscription.Items = new StripeList<SubscriptionItem>
        {
            Data = new List<SubscriptionItem> { subscriptionItem }
        };

        return subscription;
    }
}