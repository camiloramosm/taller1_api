using TodoList.Modules.Todos.Application.Common;
using TodoList.Modules.Todos.Application.DTOs;

namespace TodoList.Modules.Todos.Application.Services;

public interface ITodoService
{
    Task<TodoItemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TodoItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<TodoItemDto>> GetPagedAsync(TodoQueryParameters parameters, CancellationToken cancellationToken = default);
    Task<IEnumerable<TodoItemDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TodoItemDto> CreateAsync(CreateTodoItemDto createDto, CancellationToken cancellationToken = default);
    Task<TodoItemDto?> UpdateAsync(Guid id, UpdateTodoItemDto updateDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ToggleCompleteAsync(Guid id, CancellationToken cancellationToken = default);
}

