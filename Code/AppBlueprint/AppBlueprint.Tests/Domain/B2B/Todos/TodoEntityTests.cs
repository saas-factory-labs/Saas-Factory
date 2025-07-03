using AppBlueprint.TodoAppKernel.Domain;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppBlueprint.Tests.Domain.B2B.Todos;

[TestClass]
public class TodoEntityTests
{
    [TestMethod]
    public void CreateTodoEntityWithValidDataShouldInitializeCorrectly()
    {
        // Arrange
        var title = "Complete project documentation";
        var description = "Write comprehensive documentation for the todo system";
        var tenantId = "tenant_01ARZ3NDEKTSV4RRFFQ69G5FAV";
        var createdById = "user_01ARZ3NDEKTSV4RRFFQ69G5FAV";

        // Act
        var todo = new TodoEntity(title, description, tenantId, createdById);

        // Assert
        todo.Id.Should().StartWith("todo_");
        todo.Title.Should().Be(title);
        todo.Description.Should().Be(description);
        todo.TenantId.Should().Be(tenantId);
        todo.CreatedById.Should().Be(createdById);
        todo.AssignedToId.Should().Be(createdById); // Should default to creator
        todo.IsCompleted.Should().BeFalse();
        todo.Priority.Should().Be(TodoPriority.Medium);
        todo.CompletedAt.Should().BeNull();
        todo.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void MarkAsCompletedWhenNotCompletedShouldCompleteTask()
    {
        // Arrange
        var todo = new TodoEntity("Test task", null, "tenant_123", "user_123");
        var initialUpdateTime = todo.LastUpdatedAt;

        // Act
        todo.MarkAsCompleted();

        // Assert
        todo.IsCompleted.Should().BeTrue();
        todo.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        todo.LastUpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        todo.LastUpdatedAt.Should().NotBe(initialUpdateTime);
    }

    [TestMethod]
    public void MarkAsCompletedWhenAlreadyCompletedShouldNotChangeState()
    {
        // Arrange
        var todo = new TodoEntity("Test task", null, "tenant_123", "user_123");
        todo.MarkAsCompleted();
        var originalCompletedAt = todo.CompletedAt;
        var originalLastUpdated = todo.LastUpdatedAt;

        // Act
        todo.MarkAsCompleted();

        // Assert
        todo.IsCompleted.Should().BeTrue();
        todo.CompletedAt.Should().Be(originalCompletedAt);
        todo.LastUpdatedAt.Should().Be(originalLastUpdated);
    }

    [TestMethod]
    public void MarkAsIncompleteWhenCompletedShouldMarkIncomplete()
    {
        // Arrange
        var todo = new TodoEntity("Test task", null, "tenant_123", "user_123");
        todo.MarkAsCompleted();

        // Act
        todo.MarkAsIncomplete();

        // Assert
        todo.IsCompleted.Should().BeFalse();
        todo.CompletedAt.Should().BeNull();
        todo.LastUpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void UpdateDetailsWithValidDataShouldUpdateProperties()
    {
        // Arrange
        var todo = new TodoEntity("Original task", "Original description", "tenant_123", "user_123");
        var newTitle = "Updated task";
        var newDescription = "Updated description";
        var newPriority = TodoPriority.High;
        var newDueDate = DateTime.UtcNow.AddDays(7);

        // Act
        todo.UpdateDetails(newTitle, newDescription, newPriority, newDueDate);

        // Assert
        todo.Title.Should().Be(newTitle);
        todo.Description.Should().Be(newDescription);
        todo.Priority.Should().Be(newPriority);
        todo.DueDate.Should().Be(newDueDate);
        todo.LastUpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void AssignToWithValidUserIdShouldUpdateAssignment()
    {
        // Arrange
        var todo = new TodoEntity("Test task", null, "tenant_123", "user_123");
        var newAssigneeId = "user_456";

        // Act
        todo.AssignTo(newAssigneeId);

        // Assert
        todo.AssignedToId.Should().Be(newAssigneeId);
        todo.LastUpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void AssignToWithNullUserIdShouldThrowArgumentException()
    {
        // Arrange
        var todo = new TodoEntity("Test task", null, "tenant_123", "user_123");

        // Act & Assert
        var action = () => todo.AssignTo(null!);
        action.Should().Throw<ArgumentException>()
            .WithMessage("User ID cannot be null or empty. (Parameter 'userId')");
    }

    [TestMethod]
    public void TodoPriorityEnumValuesShouldBeCorrect()
    {
        // Assert
        ((int)TodoPriority.Low).Should().Be(0);
        ((int)TodoPriority.Medium).Should().Be(1);
        ((int)TodoPriority.High).Should().Be(2);
        ((int)TodoPriority.Urgent).Should().Be(3);
    }
}
