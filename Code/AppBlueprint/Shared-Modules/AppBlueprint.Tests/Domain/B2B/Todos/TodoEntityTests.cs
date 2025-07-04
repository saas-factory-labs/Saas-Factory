using AppBlueprint.Domain.B2B.Todos;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppBlueprint.Tests.Domain.B2B.Todos;

[TestClass]
public class TodoEntityTests
{
    private const string TestTaskTitle = "Test task";
    private const string TestTenantId = "tenant_123";
    private const string TestUserId = "user_123";
    [TestMethod]
    public void CreateTodoEntity_WithValidData_ShouldInitializeCorrectly()
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
    public void MarkAsCompleted_WhenNotCompleted_ShouldCompleteTask()
    {
        // Arrange
        var todo = new TodoEntity(TestTaskTitle, null, TestTenantId, TestUserId);
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
    public void MarkAsCompleted_WhenAlreadyCompleted_ShouldNotChangeState()
    {
        // Arrange
        var todo = new TodoEntity(TestTaskTitle, null, TestTenantId, TestUserId);
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
    public void MarkAsIncomplete_WhenCompleted_ShouldMarkIncomplete()
    {
        // Arrange
        var todo = new TodoEntity(TestTaskTitle, null, TestTenantId, TestUserId);
        todo.MarkAsCompleted();

        // Act
        todo.MarkAsIncomplete();

        // Assert
        todo.IsCompleted.Should().BeFalse();
        todo.CompletedAt.Should().BeNull();
        todo.LastUpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void UpdateDetails_WithValidData_ShouldUpdateProperties()
    {
        // Arrange
        var todo = new TodoEntity("Original task", "Original description", TestTenantId, TestUserId);
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
    public void AssignTo_WithValidUserId_ShouldUpdateAssignment()
    {
        // Arrange
        var todo = new TodoEntity(TestTaskTitle, null, TestTenantId, TestUserId);
        var newAssigneeId = "user_456";

        // Act
        todo.AssignTo(newAssigneeId);

        // Assert
        todo.AssignedToId.Should().Be(newAssigneeId);
        todo.LastUpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void AssignTo_WithNullUserId_ShouldThrowArgumentException()
    {
        // Arrange
        var todo = new TodoEntity(TestTaskTitle, null, TestTenantId, TestUserId);

        // Act & Assert
        var action = () => todo.AssignTo(null!);
        action.Should().Throw<ArgumentException>()
            .WithMessage("User ID cannot be null or empty. (Parameter 'userId')");
    }

    [TestMethod]
    public void TodoPriority_EnumValues_ShouldBeCorrect()
    {
        // Assert
        ((int)TodoPriority.Low).Should().Be(0);
        ((int)TodoPriority.Medium).Should().Be(1);
        ((int)TodoPriority.High).Should().Be(2);
        ((int)TodoPriority.Urgent).Should().Be(3);
    }
}
