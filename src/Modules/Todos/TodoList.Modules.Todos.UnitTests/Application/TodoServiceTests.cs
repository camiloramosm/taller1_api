using FluentAssertions;
using NSubstitute;
using TodoList.Modules.Todos.Application.Common;
using TodoList.Modules.Todos.Application.DTOs;
using TodoList.Modules.Todos.Application.Interfaces;
using TodoList.Modules.Todos.Application.Services;
using TodoList.Modules.Todos.Domain.Entities;
using TodoList.Modules.Todos.Domain.Exceptions;
using TodoList.Modules.Todos.UnitTests.Abstractions;

namespace TodoList.Modules.Todos.UnitTests.Application;

public class TodoServiceTests : BaseTest
{
    private readonly ITodoRepository _mockRepository;
    private readonly TodoService _sut;

    public TodoServiceTests()
    {
        _mockRepository = Substitute.For<ITodoRepository>();
        _sut = new TodoService(_mockRepository);
    }

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTodoItemDto_WhenTodoItemExists()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var todoItem = new TodoItem
        {
            Id = todoId,
            Title = GenerateValidTitle(),
            Description = GenerateValidDescription(),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.GetByIdAsync(todoId, Arg.Any<CancellationToken>())
            .Returns(todoItem);

        // Act
        var result = await _sut.GetByIdAsync(todoId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(todoId);
        result.Title.Should().Be(todoItem.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTodoItemDoesNotExist()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        _mockRepository.GetByIdAsync(todoId, Arg.Any<CancellationToken>())
            .Returns((TodoItem?)null);

        // Act
        var result = await _sut.GetByIdAsync(todoId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTodoItems()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Task 1", IsCompleted = false, CreatedAt = DateTime.UtcNow },
            new TodoItem { Id = Guid.NewGuid(), Title = "Task 2", IsCompleted = true, CreatedAt = DateTime.UtcNow },
            new TodoItem { Id = Guid.NewGuid(), Title = "Task 3", IsCompleted = false, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(todoItems);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoTodoItems()
    {
        // Arrange
        _mockRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<TodoItem>());

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ShouldCreateTodoItem_WhenDataIsValid()
    {
        // Arrange
        var createDto = new CreateTodoItemDto
        {
            Title = GenerateValidTitle(),
            Description = GenerateValidDescription(),
            UserId = Guid.NewGuid()
        };

        _mockRepository.CreateAsync(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<TodoItem>());

        // Act
        var result = await _sut.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(createDto.Title.Trim());
        result.Description.Should().Be(createDto.Description?.Trim());
        result.IsCompleted.Should().BeFalse();
        result.UserId.Should().Be(createDto.UserId);

        await _mockRepository.Received(1).CreateAsync(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenTitleIsEmpty()
    {
        // Arrange
        var createDto = new CreateTodoItemDto
        {
            Title = string.Empty,
            Description = GenerateValidDescription()
        };

        // Act
        Func<Task> act = async () => await _sut.CreateAsync(createDto);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.ContainsKey("Title") && ex.Errors["Title"].Any());
    }

    [Fact]
    public async Task CreateAsync_ShouldTrimTitleAndDescription()
    {
        // Arrange
        var createDto = new CreateTodoItemDto
        {
            Title = "  Test Title  ",
            Description = "  Test Description  "
        };

        _mockRepository.CreateAsync(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<TodoItem>());

        // Act
        var result = await _sut.CreateAsync(createDto);

        // Assert
        result.Title.Should().Be("Test Title");
        result.Description.Should().Be("Test Description");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTodoItem_WhenDataIsValid()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var existingTodo = new TodoItem
        {
            Id = todoId,
            Title = "Old Title",
            Description = "Old Description",
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        var updateDto = new UpdateTodoItemDto
        {
            Title = GenerateValidTitle(),
            Description = GenerateValidDescription(),
            IsCompleted = true
        };

        _mockRepository.GetByIdAsync(todoId, Arg.Any<CancellationToken>())
            .Returns(existingTodo);

        _mockRepository.UpdateAsync(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<TodoItem>());

        // Act
        var result = await _sut.UpdateAsync(todoId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be(updateDto.Title.Trim());
        result.Description.Should().Be(updateDto.Description?.Trim());
        result.IsCompleted.Should().Be(updateDto.IsCompleted);
        result.UpdatedAt.Should().NotBeNull();
        result.CompletedAt.Should().NotBeNull();

        await _mockRepository.Received(1).UpdateAsync(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowTodoItemNotFoundException_WhenTodoDoesNotExist()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var updateDto = new UpdateTodoItemDto
        {
            Title = GenerateValidTitle(),
            IsCompleted = false
        };

        _mockRepository.GetByIdAsync(todoId, Arg.Any<CancellationToken>())
            .Returns((TodoItem?)null);

        // Act
        Func<Task> act = async () => await _sut.UpdateAsync(todoId, updateDto);

        // Assert
        await act.Should().ThrowAsync<TodoItemNotFoundException>()
            .Where(ex => ex.TodoItemId == todoId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetCompletedAtToNull_WhenMarkingAsIncomplete()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var existingTodo = new TodoItem
        {
            Id = todoId,
            Title = "Test Title",
            IsCompleted = true,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        var updateDto = new UpdateTodoItemDto
        {
            Title = "Updated Title",
            IsCompleted = false
        };

        _mockRepository.GetByIdAsync(todoId, Arg.Any<CancellationToken>())
            .Returns(existingTodo);

        _mockRepository.UpdateAsync(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<TodoItem>());

        // Act
        var result = await _sut.UpdateAsync(todoId, updateDto);

        // Assert
        result!.IsCompleted.Should().BeFalse();
        result.CompletedAt.Should().BeNull();
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ShouldDeleteTodoItem_WhenTodoExists()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var existingTodo = new TodoItem
        {
            Id = todoId,
            Title = GenerateValidTitle(),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.GetByIdAsync(todoId, Arg.Any<CancellationToken>())
            .Returns(existingTodo);

        _mockRepository.DeleteAsync(todoId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _sut.DeleteAsync(todoId);

        // Assert
        result.Should().BeTrue();
        await _mockRepository.Received(1).DeleteAsync(todoId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowTodoItemNotFoundException_WhenTodoDoesNotExist()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        _mockRepository.GetByIdAsync(todoId, Arg.Any<CancellationToken>())
            .Returns((TodoItem?)null);

        // Act
        Func<Task> act = async () => await _sut.DeleteAsync(todoId);

        // Assert
        await act.Should().ThrowAsync<TodoItemNotFoundException>()
            .Where(ex => ex.TodoItemId == todoId);
    }

    #endregion

    #region ToggleCompleteAsync Tests

    [Fact]
    public async Task ToggleCompleteAsync_ShouldMarkAsComplete_WhenTodoIsIncomplete()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var existingTodo = new TodoItem
        {
            Id = todoId,
            Title = GenerateValidTitle(),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.GetByIdAsync(todoId, Arg.Any<CancellationToken>())
            .Returns(existingTodo);

        _mockRepository.UpdateAsync(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<TodoItem>());

        // Act
        var result = await _sut.ToggleCompleteAsync(todoId);

        // Assert
        result.Should().BeTrue();
        await _mockRepository.Received(1).UpdateAsync(
            Arg.Is<TodoItem>(t => t.IsCompleted && t.CompletedAt != null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ToggleCompleteAsync_ShouldMarkAsIncomplete_WhenTodoIsComplete()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var existingTodo = new TodoItem
        {
            Id = todoId,
            Title = GenerateValidTitle(),
            IsCompleted = true,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        _mockRepository.GetByIdAsync(todoId, Arg.Any<CancellationToken>())
            .Returns(existingTodo);

        _mockRepository.UpdateAsync(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<TodoItem>());

        // Act
        var result = await _sut.ToggleCompleteAsync(todoId);

        // Assert
        result.Should().BeTrue();
        await _mockRepository.Received(1).UpdateAsync(
            Arg.Is<TodoItem>(t => !t.IsCompleted && t.CompletedAt == null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ToggleCompleteAsync_ShouldThrowTodoItemNotFoundException_WhenTodoDoesNotExist()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        _mockRepository.GetByIdAsync(todoId, Arg.Any<CancellationToken>())
            .Returns((TodoItem?)null);

        // Act
        Func<Task> act = async () => await _sut.ToggleCompleteAsync(todoId);

        // Assert
        await act.Should().ThrowAsync<TodoItemNotFoundException>()
            .Where(ex => ex.TodoItemId == todoId);
    }

    #endregion

    #region GetPagedAsync Tests

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedResults()
    {
        // Arrange
        var parameters = new TodoQueryParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Task 1", IsCompleted = false, CreatedAt = DateTime.UtcNow },
            new TodoItem { Id = Guid.NewGuid(), Title = "Task 2", IsCompleted = true, CreatedAt = DateTime.UtcNow }
        };

        var pagedResult = new PagedResult<TodoItem>(todoItems, 2, 1, 10);

        _mockRepository.GetPagedAsync(parameters, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        // Act
        var result = await _sut.GetPagedAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    #endregion

    #region GetByUserIdAsync Tests

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnUserTodos()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Task 1", UserId = userId, IsCompleted = false, CreatedAt = DateTime.UtcNow },
            new TodoItem { Id = Guid.NewGuid(), Title = "Task 2", UserId = userId, IsCompleted = true, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(todoItems);

        // Act
        var result = await _sut.GetByUserIdAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(dto => dto.UserId.Should().Be(userId));
    }

    #endregion
}
