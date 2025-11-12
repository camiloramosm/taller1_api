# Reglas de Negocio - TodoList Service

## 📋 Resumen

Este documento describe todas las reglas de negocio, validaciones y políticas implementadas en el sistema de gestión de tareas (TodoList).

## 🎯 Reglas de Validación

### 1. Título de la Tarea

**Regla**: Todo elemento de tarea debe tener un título válido.

- **Mínimo**: 1 carácter
- **Máximo**: 200 caracteres
- **Requerido**: Sí
- **Formato**: Se eliminan espacios en blanco al inicio y final (trim)

**Excepciones**:
- `ValidationException` si está vacío
- `ValidationException` si excede 200 caracteres

**Implementación**:
```csharp
TodoBusinessRules.ValidateTitle(title)
```

### 2. Descripción de la Tarea

**Regla**: La descripción es opcional pero si existe debe cumplir límites.

- **Mínimo**: 0 caracteres (opcional)
- **Máximo**: 1000 caracteres
- **Requerido**: No
- **Formato**: Se eliminan espacios en blanco al inicio y final si existe

**Excepciones**:
- `ValidationException` si excede 1000 caracteres

**Implementación**:
```csharp
TodoBusinessRules.ValidateDescription(description)
```

### 3. Fechas del Sistema

**Reglas**:
- `CreatedAt`: Se establece automáticamente al crear (UTC)
- `UpdatedAt`: Se establece automáticamente al actualizar (UTC)
- `CompletedAt`: Se establece cuando `IsCompleted` es `true`
- `CompletedAt` debe ser posterior a `CreatedAt`
- `UpdatedAt` debe ser posterior a `CreatedAt`

**Excepciones**:
- `ValidationException` si las fechas son inconsistentes

**Implementación**:
```csharp
TodoBusinessRules.ValidateDates(todoItem)
```

## 🔒 Reglas de Operaciones

### 4. Crear Tarea

**Reglas**:
- El título es obligatorio y debe ser válido
- La descripción es opcional
- `IsCompleted` siempre inicia en `false`
- `CreatedAt` se establece en UTC
- Se genera un `Guid` único para `Id`

**Validaciones**:
```csharp
TodoBusinessRules.ValidateCreation(title, description)
```

### 5. Actualizar Tarea

**Reglas**:
- La tarea debe existir
- El título debe ser válido
- La descripción debe ser válida si existe
- Se actualiza `UpdatedAt` automáticamente
- Si se marca como completada, se establece `CompletedAt`
- Si se desmarca como completada, se elimina `CompletedAt`

**Excepciones**:
- `TodoItemNotFoundException` si no existe
- `ValidationException` si los datos no son válidos

**Validaciones**:
```csharp
TodoBusinessRules.ValidateUpdate(todoItem, title, description)
```

### 6. Eliminar Tarea

**Reglas**:
- La tarea debe existir
- Se permite eliminar cualquier tarea (extensible para futuras reglas)

**Excepciones**:
- `TodoItemNotFoundException` si no existe

**Validaciones**:
```csharp
TodoBusinessRules.ValidateCanDelete(todoItem)
```

### 7. Alternar Estado de Completado

**Reglas**:
- La tarea debe existir
- Cambia `IsCompleted` al estado opuesto
- Si se completa: establece `CompletedAt` en UTC
- Si se descompleta: elimina `CompletedAt`
- Actualiza `UpdatedAt` automáticamente

**Excepciones**:
- `TodoItemNotFoundException` si no existe

## 📊 Reglas de Consulta y Paginación

### 8. Paginación

**Reglas**:
- `PageNumber`: Mínimo 1 (default: 1)
- `PageSize`: Mínimo 1, Máximo 50 (default: 10)
- Si se solicita más de 50, se limita automáticamente a 50

**Implementación**:
```csharp
TodoQueryParameters
```

### 9. Filtros

**Filtros disponibles**:

#### `SearchTerm` (Búsqueda por texto)
- Busca en `Title` y `Description`
- No distingue mayúsculas/minúsculas
- Búsqueda parcial (contains)

#### `IsCompleted` (Estado)
- `true`: Solo tareas completadas
- `false`: Solo tareas pendientes
- `null`: Todas las tareas

#### `UserId` (Usuario)
- Filtra por usuario específico
- `null`: Todas las tareas de todos los usuarios

### 10. Ordenamiento

**Campos de ordenamiento** (`SortBy`):
- `CreatedAt` (default)
- `Title`
- `IsCompleted`
- `CompletedAt`
- `UpdatedAt`

**Dirección** (`SortDescending`):
- `true`: Descendente (default)
- `false`: Ascendente

## 🔐 Reglas de Seguridad

### 11. Rate Limiting (Límite de Peticiones)

**Reglas generales**:
- **Por minuto**: Máximo 60 peticiones
- **Por hora**: Máximo 1000 peticiones
- **Código de respuesta**: 429 (Too Many Requests)

**Configuración**:
```json
{
  "IpRateLimiting": {
    "GeneralRules": [
      { "Endpoint": "*", "Period": "1m", "Limit": 60 },
      { "Endpoint": "*", "Period": "1h", "Limit": 1000 }
    ]
  }
}
```

### 12. CORS (Cross-Origin Resource Sharing)

**Reglas actuales** (Desarrollo):
- Permite cualquier origen
- Permite cualquier método
- Permite cualquier header

⚠️ **Nota**: En producción, configurar orígenes específicos.

## 🏥 Health Checks

### 13. Monitoreo de Salud

**Endpoint**: `/health`

**Verificaciones**:
- Conexión a base de datos (PostgreSQL/Supabase)

**Respuestas**:
- `200 OK`: Sistema saludable
- `503 Service Unavailable`: Problemas detectados

## ⚠️ Manejo de Excepciones

### 14. Excepciones Personalizadas

#### `TodoItemNotFoundException`
- **Código HTTP**: 404 Not Found
- **Cuándo**: Tarea no encontrada
- **Información**: ID de la tarea buscada

#### `ValidationException`
- **Código HTTP**: 400 Bad Request
- **Cuándo**: Datos de entrada inválidos
- **Información**: Diccionario de errores por campo

#### `InvalidTodoOperationException`
- **Código HTTP**: 400 Bad Request
- **Cuándo**: Operación no permitida
- **Información**: Descripción del problema

### 15. Respuestas de Error

**Formato estándar**:
```json
{
  "statusCode": 400,
  "message": "Descripción del error",
  "errors": {
    "campo": ["error1", "error2"]
  }
}
```

## 📝 Logging

### 16. Registro de Eventos

**Niveles de log**:
- `Information`: Operaciones normales
- `Warning`: Situaciones inusuales
- `Error`: Errores y excepciones

**Eventos registrados**:
- Creación de tareas
- Actualización de tareas
- Eliminación de tareas
- Búsquedas y consultas
- Errores y excepciones
- Conexión a base de datos

## 🔄 Reglas de Integridad de Datos

### 17. Timestamps

**Automáticos**:
- `CreatedAt`: No se puede modificar después de crear
- `UpdatedAt`: Se actualiza en cada modificación
- `CompletedAt`: Sincronizado con `IsCompleted`

### 18. IDs y Claves

**Reglas**:
- `Id`: UUID/GUID generado automáticamente
- `UserId`: UUID/GUID opcional
- Índices en: `UserId`, `IsCompleted`

## 📈 Límites del Sistema

### 19. Límites Actuales

| Concepto | Límite |
|----------|--------|
| Título | 200 caracteres |
| Descripción | 1000 caracteres |
| Tamaño de página | 50 elementos |
| Peticiones por minuto | 60 |
| Peticiones por hora | 1000 |

## 🚀 Extensibilidad

### Reglas Futuras Sugeridas

1. **Límite de tareas activas por usuario**
   - Constante definida: `MaxActiveTasksPerUser = 100`
   - No implementada actualmente

2. **Prioridades de tareas**
   - Alta, Media, Baja
   - Ordenamiento por prioridad

3. **Categorías o etiquetas**
   - Clasificación de tareas
   - Filtrado por categoría

4. **Fechas de vencimiento**
   - `DueDate`
   - Alertas de vencimiento

5. **Tareas compartidas**
   - Múltiples usuarios
   - Permisos de edición

6. **Auditoría completa**
   - Historial de cambios
   - Usuario que realizó cada cambio

7. **Soft delete**
   - No eliminar físicamente
   - Campo `IsDeleted`

## 📚 Referencias

- `TodoBusinessRules.cs`: Implementación de reglas
- `TodoService.cs`: Aplicación de reglas
- `TodoItemsController.cs`: Validaciones de entrada
- `ExceptionHandlingMiddleware.cs`: Manejo de excepciones
- `Program.cs`: Configuración global

## ✅ Validación de Reglas

Para verificar que las reglas se aplican correctamente:

1. **Prueba de título vacío**:
   ```bash
   POST /api/todoitems con title=""
   # Debe retornar 400
   ```

2. **Prueba de título muy largo**:
   ```bash
   POST /api/todoitems con title de 201 caracteres
   # Debe retornar 400
   ```

3. **Prueba de tarea no existente**:
   ```bash
   GET /api/todoitems/00000000-0000-0000-0000-000000000000
   # Debe retornar 404
   ```

4. **Prueba de paginación**:
   ```bash
   GET /api/todoitems/paged?pageSize=100
   # pageSize debe limitarse a 50
   ```

5. **Prueba de rate limiting**:
   ```bash
   # Hacer 61 peticiones en 1 minuto
   # La petición 61 debe retornar 429
   ```

---

**Última actualización**: Noviembre 2025  
**Versión**: 1.0.0

