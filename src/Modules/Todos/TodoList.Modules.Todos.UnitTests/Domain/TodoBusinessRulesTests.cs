using FluentAssertions;
using TodoList.Modules.Todos.Domain.Entities;
using TodoList.Modules.Todos.Domain.Exceptions;
using TodoList.Modules.Todos.Domain.Rules;
using TodoList.Modules.Todos.UnitTests.Abstractions;

namespace TodoList.Modules.Todos.UnitTests.Domain;

public class TodoBusinessRulesTests : BaseTest
{
    #region ValidateTitle Tests

    [Fact]
    public void ValidateTitle_ShouldNotThrow_WhenTitleIsValid()
    {
        // Arrange
        string validTitle = GenerateValidTitle();

        // Act
        Action act = () => TodoBusinessRules.ValidateTitle(validTitle);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateTitle_ShouldThrowValidationException_WhenTitleIsEmpty()
    {
        // Arrange
        string emptyTitle = string.Empty;

        // Act
        Action act = () => TodoBusinessRules.ValidateTitle(emptyTitle);

        // Assert
        act.Should().Throw<ValidationException>()
            .Where(ex => ex.Errors.ContainsKey("Title") && ex.Errors["Title"].Any());
    }

    [Fact]
    public void ValidateTitle_ShouldThrowValidationException_WhenTitleIsWhitespace()
    {
        // Arrange
        string whitespaceTitle = "   ";

        // Act
        Action act = () => TodoBusinessRules.ValidateTitle(whitespaceTitle);

        // Assert
        act.Should().Throw<ValidationException>()
            .Where(ex => ex.Errors.ContainsKey("Title") && ex.Errors["Title"].Any());
    }

    [Fact]
    public void ValidateTitle_ShouldThrowValidationException_WhenTitleExceedsMaxLength()
    {
        // Arrange
        string longTitle = GenerateLongTitle();

        // Act
        Action act = () => TodoBusinessRules.ValidateTitle(longTitle);

        // Assert
        act.Should().Throw<ValidationException>()
            .Where(ex => ex.Errors.ContainsKey("Title") && ex.Errors["Title"].Any());
    }

    #endregion

    #region ValidateDescription Tests

    [Fact]
    public void ValidateDescription_ShouldNotThrow_WhenDescriptionIsValid()
    {
        // Arrange
        string validDescription = GenerateValidDescription();

        // Act
        Action act = () => TodoBusinessRules.ValidateDescription(validDescription);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateDescription_ShouldNotThrow_WhenDescriptionIsNull()
    {
        // Arrange
        string? nullDescription = null;

        // Act
        Action act = () => TodoBusinessRules.ValidateDescription(nullDescription);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateDescription_ShouldThrowValidationException_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        string longDescription = GenerateLongDescription();

        // Act
        Action act = () => TodoBusinessRules.ValidateDescription(longDescription);

        // Assert
        act.Should().Throw<ValidationException>()
            .Where(ex => ex.Errors.ContainsKey("Description") && ex.Errors["Description"].Any());
    }

    #endregion

    #region ValidateCanComplete Tests

    [Fact]
    public void ValidateCanComplete_ShouldNotThrow_WhenTodoItemIsNotCompleted()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = GenerateValidTitle(),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        Action act = () => TodoBusinessRules.ValidateCanComplete(todoItem);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateCanComplete_ShouldThrowInvalidTodoOperationException_WhenTodoItemIsAlreadyCompleted()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = GenerateValidTitle(),
            IsCompleted = true,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        // Act
        Action act = () => TodoBusinessRules.ValidateCanComplete(todoItem);

        // Assert
        act.Should().Throw<InvalidTodoOperationException>()
            .WithMessage("*ya está marcado como completado*");
    }

    #endregion

    #region ValidateCanUncomplete Tests

    [Fact]
    public void ValidateCanUncomplete_ShouldNotThrow_WhenTodoItemIsCompleted()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = GenerateValidTitle(),
            IsCompleted = true,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        // Act
        Action act = () => TodoBusinessRules.ValidateCanUncomplete(todoItem);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateCanUncomplete_ShouldThrowInvalidTodoOperationException_WhenTodoItemIsNotCompleted()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = GenerateValidTitle(),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        Action act = () => TodoBusinessRules.ValidateCanUncomplete(todoItem);

        // Assert
        act.Should().Throw<InvalidTodoOperationException>()
            .WithMessage("*ya está marcado como incompleto*");
    }

    #endregion

    #region ValidateDates Tests

    [Fact]
    public void ValidateDates_ShouldNotThrow_WhenDatesAreValid()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = GenerateValidTitle(),
            IsCompleted = true,
            CreatedAt = createdAt,
            UpdatedAt = createdAt.AddMinutes(10),
            CompletedAt = createdAt.AddMinutes(20)
        };

        // Act
        Action act = () => TodoBusinessRules.ValidateDates(todoItem);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateDates_ShouldThrowValidationException_WhenUpdatedAtIsBeforeCreatedAt()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = GenerateValidTitle(),
            IsCompleted = false,
            CreatedAt = createdAt,
            UpdatedAt = createdAt.AddMinutes(-10)
        };

        // Act
        Action act = () => TodoBusinessRules.ValidateDates(todoItem);

        // Assert
        act.Should().Throw<ValidationException>()
            .Where(ex => ex.Errors.ContainsKey("UpdatedAt") && ex.Errors["UpdatedAt"].Any());
    }

    [Fact]
    public void ValidateDates_ShouldThrowValidationException_WhenCompletedAtIsBeforeCreatedAt()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = GenerateValidTitle(),
            IsCompleted = true,
            CreatedAt = createdAt,
            CompletedAt = createdAt.AddMinutes(-10)
        };

        // Act
        Action act = () => TodoBusinessRules.ValidateDates(todoItem);

        // Assert
        act.Should().Throw<ValidationException>()
            .Where(ex => ex.Errors.ContainsKey("CompletedAt") && ex.Errors["CompletedAt"].Any());
    }

    [Fact]
    public void ValidateDates_ShouldThrowValidationException_WhenCompletedAtExistsButNotCompleted()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = GenerateValidTitle(),
            IsCompleted = false,
            CreatedAt = createdAt,
            CompletedAt = createdAt.AddMinutes(10)
        };

        // Act
        Action act = () => TodoBusinessRules.ValidateDates(todoItem);

        // Assert
        act.Should().Throw<ValidationException>()
            .Where(ex => ex.Errors.ContainsKey("CompletedAt") && ex.Errors["CompletedAt"].Any());
    }

    #endregion

    #region ValidateCreation Tests

    [Fact]
    public void ValidateCreation_ShouldNotThrow_WhenTitleAndDescriptionAreValid()
    {
        // Arrange
        string title = GenerateValidTitle();
        string description = GenerateValidDescription();

        // Act
        Action act = () => TodoBusinessRules.ValidateCreation(title, description);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateCreation_ShouldThrowValidationException_WhenTitleIsInvalid()
    {
        // Arrange
        string invalidTitle = string.Empty;
        string description = GenerateValidDescription();

        // Act
        Action act = () => TodoBusinessRules.ValidateCreation(invalidTitle, description);

        // Assert
        act.Should().Throw<ValidationException>();
    }

    #endregion

    #region ValidateUpdate Tests

    [Fact]
    public void ValidateUpdate_ShouldNotThrow_WhenAllDataIsValid()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = "Old Title",
            IsCompleted = false,
            CreatedAt = createdAt,
            UpdatedAt = createdAt.AddMinutes(5)
        };
        string newTitle = GenerateValidTitle();
        string newDescription = GenerateValidDescription();

        // Act
        Action act = () => TodoBusinessRules.ValidateUpdate(todoItem, newTitle, newDescription);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateUpdate_ShouldThrowValidationException_WhenTitleIsInvalid()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = GenerateValidTitle(),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };
        string invalidTitle = string.Empty;
        string description = GenerateValidDescription();

        // Act
        Action act = () => TodoBusinessRules.ValidateUpdate(todoItem, invalidTitle, description);

        // Assert
        act.Should().Throw<ValidationException>();
    }

    #endregion
}
