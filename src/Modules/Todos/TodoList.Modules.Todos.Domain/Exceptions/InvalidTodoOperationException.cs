namespace TodoList.Modules.Todos.Domain.Exceptions;

public class InvalidTodoOperationException : Exception
{
    public InvalidTodoOperationException(string message)
        : base(message)
    {
    }

    public InvalidTodoOperationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

