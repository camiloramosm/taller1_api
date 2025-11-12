using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Common;
using TodoList.Application.DTOs;
using TodoList.Application.Services;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TodoItemsController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly ILogger<TodoItemsController> _logger;

    public TodoItemsController(ITodoService todoService, ILogger<TodoItemsController> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los elementos de la lista de tareas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obteniendo todos los elementos de la lista de tareas");
        var items = await _todoService.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    /// <summary>
    /// Obtiene los elementos de la lista de tareas con paginación y filtros
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<TodoItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<TodoItemDto>>> GetPaged(
        [FromQuery] TodoQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obteniendo elementos paginados. Página: {PageNumber}, Tamaño: {PageSize}",
            parameters.PageNumber, parameters.PageSize);
        
        var pagedResult = await _todoService.GetPagedAsync(parameters, cancellationToken);
        return Ok(pagedResult);
    }

    /// <summary>
    /// Obtiene un elemento de la lista de tareas por su ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItemDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obteniendo elemento con ID: {TodoItemId}", id);
        var item = await _todoService.GetByIdAsync(id, cancellationToken);
        
        if (item == null)
        {
            _logger.LogWarning("Elemento con ID {TodoItemId} no encontrado", id);
            return NotFound(new { message = $"No se encontró el elemento con ID: {id}" });
        }
        
        return Ok(item);
    }

    /// <summary>
    /// Obtiene todos los elementos de la lista de tareas de un usuario específico
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<TodoItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obteniendo elementos del usuario con ID: {UserId}", userId);
        var items = await _todoService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(items);
    }

    /// <summary>
    /// Crea un nuevo elemento en la lista de tareas
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoItemDto>> Create(
        [FromBody] CreateTodoItemDto createDto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Solicitud de creación con modelo inválido");
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creando nuevo elemento: {Title}", createDto.Title);
        var created = await _todoService.CreateAsync(createDto, cancellationToken);
        
        _logger.LogInformation("Elemento creado exitosamente con ID: {TodoItemId}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Actualiza un elemento existente en la lista de tareas
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoItemDto>> Update(
        Guid id,
        [FromBody] UpdateTodoItemDto updateDto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Solicitud de actualización con modelo inválido para ID: {TodoItemId}", id);
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Actualizando elemento con ID: {TodoItemId}", id);
        var updated = await _todoService.UpdateAsync(id, updateDto, cancellationToken);
        
        _logger.LogInformation("Elemento con ID {TodoItemId} actualizado exitosamente", id);
        return Ok(updated);
    }

    /// <summary>
    /// Alterna el estado de completado de un elemento
    /// </summary>
    [HttpPatch("{id}/toggle-complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ToggleComplete(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Alternando estado de completado para elemento con ID: {TodoItemId}", id);
        await _todoService.ToggleCompleteAsync(id, cancellationToken);
        
        _logger.LogInformation("Estado de completado alternado exitosamente para ID: {TodoItemId}", id);
        return Ok(new { message = "Estado de completado actualizado correctamente" });
    }

    /// <summary>
    /// Elimina un elemento de la lista de tareas
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Eliminando elemento con ID: {TodoItemId}", id);
        await _todoService.DeleteAsync(id, cancellationToken);
        
        _logger.LogInformation("Elemento con ID {TodoItemId} eliminado exitosamente", id);
        return NoContent();
    }
}
