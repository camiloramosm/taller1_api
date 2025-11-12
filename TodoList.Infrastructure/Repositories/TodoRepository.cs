using Microsoft.EntityFrameworkCore;
using TodoList.Application.Common;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.Data;

namespace TodoList.Infrastructure.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly ApplicationDbContext _context;

    public TodoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TodoItems
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TodoItems
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<TodoItem>> GetPagedAsync(TodoQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = _context.TodoItems.AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(searchTerm) ||
                (t.Description != null && t.Description.ToLower().Contains(searchTerm))
            );
        }

        if (parameters.IsCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == parameters.IsCompleted.Value);
        }

        if (parameters.UserId.HasValue)
        {
            query = query.Where(t => t.UserId == parameters.UserId.Value);
        }

        // Aplicar ordenamiento
        query = parameters.SortBy.ToLower() switch
        {
            "title" => parameters.SortDescending
                ? query.OrderByDescending(t => t.Title)
                : query.OrderBy(t => t.Title),
            "iscompleted" => parameters.SortDescending
                ? query.OrderByDescending(t => t.IsCompleted)
                : query.OrderBy(t => t.IsCompleted),
            "completedat" => parameters.SortDescending
                ? query.OrderByDescending(t => t.CompletedAt)
                : query.OrderBy(t => t.CompletedAt),
            "updatedat" => parameters.SortDescending
                ? query.OrderByDescending(t => t.UpdatedAt)
                : query.OrderBy(t => t.UpdatedAt),
            _ => parameters.SortDescending
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt)
        };

        // Obtener el total de elementos
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar paginación
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TodoItem>(items, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IEnumerable<TodoItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.TodoItems
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TodoItem> CreateAsync(TodoItem todoItem, CancellationToken cancellationToken = default)
    {
        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync(cancellationToken);
        return todoItem;
    }

    public async Task<TodoItem> UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken = default)
    {
        _context.TodoItems.Update(todoItem);
        await _context.SaveChangesAsync(cancellationToken);
        return todoItem;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todoItem = await GetByIdAsync(id, cancellationToken);
        if (todoItem == null)
            return false;

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TodoItems
            .AnyAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.TodoItems
            .Where(t => t.UserId == userId && !t.IsCompleted)
            .CountAsync(cancellationToken);
    }
}
