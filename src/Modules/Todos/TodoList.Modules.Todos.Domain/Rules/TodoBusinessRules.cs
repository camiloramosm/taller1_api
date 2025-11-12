using TodoList.Modules.Todos.Domain.Entities;
using TodoList.Modules.Todos.Domain.Exceptions;

namespace TodoList.Modules.Todos.Domain.Rules;

/// <summary>
/// Reglas de negocio para los elementos de tareas (TodoItems)
/// </summary>
public static class TodoBusinessRules
{
    // Constantes de reglas
    public const int MinTitleLength = 1;
    public const int MaxTitleLength = 200;
    public const int MaxDescriptionLength = 1000;
    public const int MaxActiveTasksPerUser = 100;

    /// <summary>
    /// Valida que el título no esté vacío y no exceda el máximo permitido
    /// </summary>
    public static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ValidationException("Title", "El título no puede estar vacío");
        }

        if (title.Length < MinTitleLength)
        {
            throw new ValidationException("Title", $"El título debe tener al menos {MinTitleLength} carácter");
        }

        if (title.Length > MaxTitleLength)
        {
            throw new ValidationException("Title", $"El título no puede exceder {MaxTitleLength} caracteres");
        }
    }

    /// <summary>
    /// Valida que la descripción no exceda el máximo permitido
    /// </summary>
    public static void ValidateDescription(string? description)
    {
        if (description != null && description.Length > MaxDescriptionLength)
        {
            throw new ValidationException("Description", $"La descripción no puede exceder {MaxDescriptionLength} caracteres");
        }
    }

    /// <summary>
    /// Valida que un elemento puede ser marcado como completado
    /// </summary>
    public static void ValidateCanComplete(TodoItem todoItem)
    {
        if (todoItem.IsCompleted)
        {
            throw new InvalidTodoOperationException($"El elemento '{todoItem.Title}' ya está marcado como completado");
        }
    }

    /// <summary>
    /// Valida que un elemento puede ser marcado como incompleto
    /// </summary>
    public static void ValidateCanUncomplete(TodoItem todoItem)
    {
        if (!todoItem.IsCompleted)
        {
            throw new InvalidTodoOperationException($"El elemento '{todoItem.Title}' ya está marcado como incompleto");
        }
    }

    /// <summary>
    /// Valida que un elemento puede ser eliminado
    /// </summary>
    public static void ValidateCanDelete(TodoItem todoItem)
    {
        // Aquí puedes agregar reglas adicionales, por ejemplo:
        // - No permitir eliminar tareas de ciertos usuarios
        // - No permitir eliminar tareas antiguas
        // - etc.
        
        // Por ahora, siempre se permite eliminar
    }

    /// <summary>
    /// Valida las fechas del elemento
    /// </summary>
    public static void ValidateDates(TodoItem todoItem)
    {
        if (todoItem.UpdatedAt.HasValue && todoItem.UpdatedAt < todoItem.CreatedAt)
        {
            throw new ValidationException("UpdatedAt", "La fecha de actualización no puede ser anterior a la fecha de creación");
        }

        if (todoItem.CompletedAt.HasValue && todoItem.CompletedAt < todoItem.CreatedAt)
        {
            throw new ValidationException("CompletedAt", "La fecha de completado no puede ser anterior a la fecha de creación");
        }

        if (todoItem.CompletedAt.HasValue && !todoItem.IsCompleted)
        {
            throw new ValidationException("CompletedAt", "No puede haber fecha de completado si el elemento no está completado");
        }
    }

    /// <summary>
    /// Valida la creación de un nuevo elemento
    /// </summary>
    public static void ValidateCreation(string title, string? description)
    {
        ValidateTitle(title);
        ValidateDescription(description);
    }

    /// <summary>
    /// Valida la actualización de un elemento
    /// </summary>
    public static void ValidateUpdate(TodoItem todoItem, string title, string? description)
    {
        ValidateTitle(title);
        ValidateDescription(description);
        ValidateDates(todoItem);
    }
}

