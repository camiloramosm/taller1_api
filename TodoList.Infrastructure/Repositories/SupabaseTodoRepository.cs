using Supabase;
using TodoList.Application.Common;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Repositories;

public class SupabaseTodoRepository : ITodoRepository
{
    private readonly Client _supabaseClient;

    public SupabaseTodoRepository(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _supabaseClient
            .From<TodoItem>()
            .Where(x => x.Id == id)
            .Single();
        
        return response;
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await _supabaseClient
            .From<TodoItem>()
            .Order(x => x.CreatedAt, Postgrest.Constants.Ordering.Descending)
            .Get();
        
        return response.Models;
    }

    public async Task<PagedResult<TodoItem>> GetPagedAsync(TodoQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        // Por simplicidad, obtener todos y paginar en memoria
        // TODO: Implementar paginación en servidor cuando sea necesario
        var allItems = await GetAllAsync(cancellationToken);
        
        // Aplicar filtros
        var filtered = allItems.AsEnumerable();
        
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            filtered = filtered.Where(x => 
                x.Title.ToLower().Contains(searchTerm) || 
                (x.Description != null && x.Description.ToLower().Contains(searchTerm)));
        }
        
        if (parameters.IsCompleted.HasValue)
        {
            filtered = filtered.Where(x => x.IsCompleted == parameters.IsCompleted.Value);
        }
        
        if (parameters.UserId.HasValue)
        {
            filtered = filtered.Where(x => x.UserId == parameters.UserId.Value);
        }
        
        // Ordenar
        filtered = parameters.SortBy.ToLower() switch
        {
            "title" => parameters.SortDescending ? filtered.OrderByDescending(x => x.Title) : filtered.OrderBy(x => x.Title),
            "iscompleted" => parameters.SortDescending ? filtered.OrderByDescending(x => x.IsCompleted) : filtered.OrderBy(x => x.IsCompleted),
            "completedat" => parameters.SortDescending ? filtered.OrderByDescending(x => x.CompletedAt) : filtered.OrderBy(x => x.CompletedAt),
            "updatedat" => parameters.SortDescending ? filtered.OrderByDescending(x => x.UpdatedAt) : filtered.OrderBy(x => x.UpdatedAt),
            _ => parameters.SortDescending ? filtered.OrderByDescending(x => x.CreatedAt) : filtered.OrderBy(x => x.CreatedAt)
        };
        
        var filteredList = filtered.ToList();
        var totalCount = filteredList.Count;
        
        // Paginar
        var paged = filteredList
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize);
        
        return new PagedResult<TodoItem>(paged, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IEnumerable<TodoItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var response = await _supabaseClient
            .From<TodoItem>()
            .Where(x => x.UserId == userId)
            .Order(x => x.CreatedAt, Postgrest.Constants.Ordering.Descending)
            .Get();
        
        return response.Models;
    }

    public async Task<TodoItem> CreateAsync(TodoItem todoItem, CancellationToken cancellationToken = default)
    {
        var response = await _supabaseClient
            .From<TodoItem>()
            .Insert(todoItem);
        
        return response.Models.First();
    }

    public async Task<TodoItem> UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken = default)
    {
        var response = await _supabaseClient
            .From<TodoItem>()
            .Update(todoItem);
        
        return response.Models.First();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _supabaseClient
                .From<TodoItem>()
                .Where(x => x.Id == id)
                .Delete();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _supabaseClient
            .From<TodoItem>()
            .Where(x => x.Id == id)
            .Get();
        
        return response.Models.Any();
    }

    public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var response = await _supabaseClient
            .From<TodoItem>()
            .Where(x => x.UserId == userId && !x.IsCompleted)
            .Get();
        
        return response.Models.Count;
    }
}
