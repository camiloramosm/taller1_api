namespace TodoList.Modules.Todos.Domain.Exceptions;

public class TodoItemNotFoundException : Exception
{
    public Guid TodoItemId { get; }

    public TodoItemNotFoundException(Guid todoItemId)
        : base($"No se encontró el elemento de tarea con ID: {todoItemId}")
    {
        TodoItemId = todoItemId;
    }

    public TodoItemNotFoundException(Guid todoItemId, string message)
        : base(message)
    {
        TodoItemId = todoItemId;
    }

    public TodoItemNotFoundException(Guid todoItemId, string message, Exception innerException)
        : base(message, innerException)
    {
        TodoItemId = todoItemId;
    }
}

