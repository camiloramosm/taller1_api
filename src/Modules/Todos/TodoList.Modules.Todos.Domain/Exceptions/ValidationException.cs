namespace TodoList.Modules.Todos.Domain.Exceptions;

public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("Ocurrieron uno o más errores de validación.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("Ocurrieron uno o más errores de validación.")
    {
        Errors = errors;
    }

    public ValidationException(string propertyName, string errorMessage)
        : base("Ocurrieron uno o más errores de validación.")
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        };
    }
}

