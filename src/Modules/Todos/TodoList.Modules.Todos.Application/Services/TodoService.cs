using TodoList.Modules.Todos.Application.Common;
using TodoList.Modules.Todos.Application.DTOs;
using TodoList.Modules.Todos.Application.Interfaces;
using TodoList.Modules.Todos.Domain.Entities;
using TodoList.Modules.Todos.Domain.Exceptions;
using TodoList.Modules.Todos.Domain.Rules;

namespace TodoList.Modules.Todos.Application.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoItemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todoItem = await _repository.GetByIdAsync(id, cancellationToken);
        return todoItem == null ? null : MapToDto(todoItem);
    }

    public async Task<IEnumerable<TodoItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var todoItems = await _repository.GetAllAsync(cancellationToken);
        return todoItems.Select(MapToDto);
    }

    public async Task<PagedResult<TodoItemDto>> GetPagedAsync(TodoQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var pagedResult = await _repository.GetPagedAsync(parameters, cancellationToken);
        
        var dtos = pagedResult.Items.Select(MapToDto);
        
        return new PagedResult<TodoItemDto>(
            dtos,
            pagedResult.TotalCount,
            pagedResult.PageNumber,
            pagedResult.PageSize
        );
    }

    public async Task<IEnumerable<TodoItemDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var todoItems = await _repository.GetByUserIdAsync(userId, cancellationToken);
        return todoItems.Select(MapToDto);
    }

    public async Task<TodoItemDto> CreateAsync(CreateTodoItemDto createDto, CancellationToken cancellationToken = default)
    {
        // Validar reglas de negocio
        TodoBusinessRules.ValidateCreation(createDto.Title, createDto.Description);

        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = createDto.Title.Trim(),
            Description = createDto.Description?.Trim(),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UserId = createDto.UserId
        };

        var created = await _repository.CreateAsync(todoItem, cancellationToken);
        return MapToDto(created);
    }

    public async Task<TodoItemDto?> UpdateAsync(Guid id, UpdateTodoItemDto updateDto, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
            throw new TodoItemNotFoundException(id);

        // Validar reglas de negocio antes de actualizar
        existing.Title = updateDto.Title.Trim();
        existing.Description = updateDto.Description?.Trim();
        TodoBusinessRules.ValidateUpdate(existing, updateDto.Title, updateDto.Description);

        existing.IsCompleted = updateDto.IsCompleted;
        existing.UpdatedAt = DateTime.UtcNow;

        if (updateDto.IsCompleted && existing.CompletedAt == null)
        {
            existing.CompletedAt = DateTime.UtcNow;
        }
        else if (!updateDto.IsCompleted)
        {
            existing.CompletedAt = null;
        }

        var updated = await _repository.UpdateAsync(existing, cancellationToken);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
            throw new TodoItemNotFoundException(id);

        // Validar reglas de negocio antes de eliminar
        TodoBusinessRules.ValidateCanDelete(existing);

        return await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<bool> ToggleCompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
            throw new TodoItemNotFoundException(id);

        existing.IsCompleted = !existing.IsCompleted;
        existing.UpdatedAt = DateTime.UtcNow;

        if (existing.IsCompleted)
        {
            existing.CompletedAt = DateTime.UtcNow;
        }
        else
        {
            existing.CompletedAt = null;
        }

        await _repository.UpdateAsync(existing, cancellationToken);
        return true;
    }

    private static TodoItemDto MapToDto(TodoItem todoItem)
    {
        return new TodoItemDto
        {
            Id = todoItem.Id,
            Title = todoItem.Title,
            Description = todoItem.Description,
            IsCompleted = todoItem.IsCompleted,
            CreatedAt = todoItem.CreatedAt,
            UpdatedAt = todoItem.UpdatedAt,
            CompletedAt = todoItem.CompletedAt,
            UserId = todoItem.UserId
        };
    }
}

