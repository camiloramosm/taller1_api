using TodoList.Application.Common;
using TodoList.Domain.Entities;

namespace TodoList.Application.Interfaces;

public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<TodoItem>> GetPagedAsync(TodoQueryParameters parameters, CancellationToken cancellationToken = default);
    Task<IEnumerable<TodoItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TodoItem> CreateAsync(TodoItem todoItem, CancellationToken cancellationToken = default);
    Task<TodoItem> UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
