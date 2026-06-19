using AppBlueprint.TodoAppKernel.Controllers;
using AppBlueprint.TodoAppKernel.Domain;
using AppBlueprint.TodoAppKernel.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AppBlueprint.Tests.Web;

internal sealed class TodoControllerTests
{
    private const string TenantId = "tenant-123";
    private const string TodoId = "todo-123";

    private static (TodoController Controller, ITodoRepository Repository) CreateController()
    {
        ITodoRepository repository = Substitute.For<ITodoRepository>();
        ILogger<TodoController> logger = Substitute.For<ILogger<TodoController>>();

        var controller = new TodoController(logger, repository)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        controller.HttpContext.Items["TenantId"] = TenantId;

        return (controller, repository);
    }

    [Test]
    public async Task DeleteTodoAsync_WhenTodoDoesNotExist_ReturnsNotFound()
    {
        (TodoController controller, ITodoRepository repository) = CreateController();
        repository.GetByIdAsync(TodoId, TenantId, Arg.Any<CancellationToken>())
            .Returns((TodoEntity?)null);

        ActionResult result = await controller.DeleteTodoAsync(TodoId, CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().Be($"Todo with ID {TodoId} not found");
        await repository.DidNotReceive().DeleteAsync(TodoId, TenantId, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task DeleteTodoAsync_WhenTodoExists_DeletesTodoAndReturnsNoContent()
    {
        (TodoController controller, ITodoRepository repository) = CreateController();
        var todo = new TodoEntity("Ship fix", "Verify delete responses", TenantId, "user-123")
        {
            Id = TodoId
        };

        repository.GetByIdAsync(TodoId, TenantId, Arg.Any<CancellationToken>())
            .Returns(todo);

        ActionResult result = await controller.DeleteTodoAsync(TodoId, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
        await repository.Received(1).DeleteAsync(TodoId, TenantId, Arg.Any<CancellationToken>());
    }
}
