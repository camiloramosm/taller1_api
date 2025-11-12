using Postgrest.Attributes;
using Postgrest.Models;

namespace TodoList.Modules.Todos.Domain.Entities;

[Table("TodoItems")]
public class TodoItem : BaseModel
{
    [PrimaryKey("Id", shouldInsert: true)]
    public Guid Id { get; set; }
    
    [Column("Title")]
    public string Title { get; set; } = string.Empty;
    
    [Column("Description")]
    public string? Description { get; set; }
    
    [Column("IsCompleted")]
    public bool IsCompleted { get; set; }
    
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }
    
    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }
    
    [Column("CompletedAt")]
    public DateTime? CompletedAt { get; set; }
    
    [Column("UserId")]
    public Guid? UserId { get; set; }
}

