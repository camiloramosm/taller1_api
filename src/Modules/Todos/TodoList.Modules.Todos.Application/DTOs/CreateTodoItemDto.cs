using System.ComponentModel.DataAnnotations;

namespace TodoList.Modules.Todos.Application.DTOs;

public class CreateTodoItemDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public Guid? UserId { get; set; }
}

