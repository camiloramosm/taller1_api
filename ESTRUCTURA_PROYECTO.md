# Estructura del Proyecto TodoList

## Descripción General

Este proyecto ha sido migrado a una arquitectura modular monolítica siguiendo el patrón de Evently. La estructura permite una mejor separación de responsabilidades, escalabilidad y mantenibilidad.

## Estructura de Carpetas

```
TodoList/
├── Directory.Build.props          # Configuración compartida de .NET
├── TodoList.sln                   # Archivo de solución principal
├── src/
│   ├── API/
│   │   └── TodoList.Api/          # Punto de entrada de la aplicación
│   │       ├── Program.cs
│   │       ├── appsettings.json
│   │       └── Properties/
│   │
│   ├── Common/                    # Código compartido entre módulos
│   │   ├── TodoList.Common.Domain/
│   │   │   ├── Entity.cs
│   │   │   ├── DomainEvent.cs
│   │   │   ├── IDomainEvent.cs
│   │   │   ├── Result.cs
│   │   │   ├── Error.cs
│   │   │   └── ErrorType.cs
│   │   │
│   │   ├── TodoList.Common.Application/
│   │   │   └── (Clases compartidas de aplicación)
│   │   │
│   │   ├── TodoList.Common.Infrastructure/
│   │   │   └── (Servicios de infraestructura compartidos)
│   │   │
│   │   └── TodoList.Common.Presentation/
│   │       └── (Clases compartidas de presentación)
│   │
│   └── Modules/                   # Módulos de la aplicación
│       └── Todos/                 # Módulo de gestión de tareas
│           ├── TodoList.Modules.Todos.Domain/
│           │   ├── Entities/
│           │   │   └── TodoItem.cs
│           │   ├── Exceptions/
│           │   │   ├── TodoItemNotFoundException.cs
│           │   │   ├── InvalidTodoOperationException.cs
│           │   │   └── ValidationException.cs
│           │   └── Rules/
│           │       └── TodoBusinessRules.cs
│           │
│           ├── TodoList.Modules.Todos.Application/
│           │   ├── Common/
│           │   │   ├── PagedResult.cs
│           │   │   └── TodoQueryParameters.cs
│           │   ├── DTOs/
│           │   │   ├── TodoItemDto.cs
│           │   │   ├── CreateTodoItemDto.cs
│           │   │   └── UpdateTodoItemDto.cs
│           │   ├── Interfaces/
│           │   │   └── ITodoRepository.cs
│           │   └── Services/
│           │       ├── ITodoService.cs
│           │       └── TodoService.cs
│           │
│           ├── TodoList.Modules.Todos.Infrastructure/
│           │   ├── Repositories/
│           │   │   └── SupabaseTodoRepository.cs
│           │   └── TodosModule.cs
│           │
│           └── TodoList.Modules.Todos.Presentation/
│               ├── Controllers/
│               │   └── TodoItemsController.cs
│               └── Middleware/
│                   └── ExceptionHandlingMiddleware.cs
│
└── [Proyectos antiguos - pendientes de eliminación]
```

## Capas de la Arquitectura

### 1. **Domain** (Dominio)
- **Responsabilidad**: Contiene las entidades de negocio, reglas de negocio y excepciones específicas del dominio.
- **Dependencias**: Ninguna (capa más interna)
- **Ejemplos**:
  - `TodoItem` - Entidad principal del módulo
  - `TodoBusinessRules` - Reglas de validación del negocio
  - Excepciones personalizadas

### 2. **Application** (Aplicación)
- **Responsabilidad**: Lógica de aplicación, casos de uso, DTOs e interfaces de repositorio.
- **Dependencias**: Domain
- **Ejemplos**:
  - `ITodoService` / `TodoService` - Servicios de aplicación
  - DTOs - Objetos de transferencia de datos
  - `ITodoRepository` - Interfaz del repositorio

### 3. **Infrastructure** (Infraestructura)
- **Responsabilidad**: Implementación de acceso a datos, servicios externos, configuración.
- **Dependencias**: Application, Domain
- **Ejemplos**:
  - `SupabaseTodoRepository` - Implementación de repositorio con Supabase
  - `TodosModule` - Registro de dependencias del módulo

### 4. **Presentation** (Presentación)
- **Responsabilidad**: Controllers, middleware, y todo lo relacionado con la API web.
- **Dependencias**: Application
- **Ejemplos**:
  - `TodoItemsController` - Controlador de API
  - `ExceptionHandlingMiddleware` - Middleware de manejo de excepciones

## Ventajas de esta Estructura

### ✅ Separación de Responsabilidades
Cada capa tiene una responsabilidad específica y bien definida.

### ✅ Independencia de Módulos
Los módulos pueden desarrollarse, probarse y desplegarse de forma independiente.

### ✅ Escalabilidad
Fácil agregar nuevos módulos sin afectar los existentes.

### ✅ Testabilidad
La arquitectura facilita la creación de pruebas unitarias e integración.

### ✅ Mantenibilidad
El código está organizado de forma lógica y es fácil de navegar.

### ✅ Reutilización
Las clases comunes se comparten entre módulos.

## Cómo Ejecutar el Proyecto

### Prerrequisitos
- .NET 9.0 SDK
- Cuenta de Supabase configurada

### Pasos

1. **Restaurar dependencias:**
   ```bash
   dotnet restore TodoList.sln
   ```

2. **Configurar Supabase:**
   Editar `src/API/TodoList.Api/appsettings.json` con tus credenciales:
   ```json
   {
     "Supabase": {
       "Url": "https://tu-proyecto.supabase.co",
       "Key": "tu-anon-key"
     }
   }
   ```

3. **Compilar:**
   ```bash
   dotnet build TodoList.sln
   ```

4. **Ejecutar:**
   ```bash
   dotnet run --project src/API/TodoList.Api/TodoList.Api.csproj
   ```

5. **Acceder a Swagger:**
   Navegar a `https://localhost:5001/swagger`

## Agregar Nuevos Módulos

Para agregar un nuevo módulo, sigue esta estructura:

```
src/Modules/NuevoModulo/
├── TodoList.Modules.NuevoModulo.Domain/
├── TodoList.Modules.NuevoModulo.Application/
├── TodoList.Modules.NuevoModulo.Infrastructure/
└── TodoList.Modules.NuevoModulo.Presentation/
```

Luego, registra el módulo en `Program.cs`:

```csharp
builder.Services.AddNuevoModulo(builder.Configuration);
```

## Próximos Pasos

- [ ] Eliminar carpetas antiguas (TodoList.API, TodoList.Application, TodoList.Domain, TodoList.Infrastructure)
- [ ] Agregar tests unitarios e integración
- [ ] Implementar MediatR para CQRS
- [ ] Agregar más módulos según sea necesario
- [ ] Configurar CI/CD

## Referencias

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Modular Monolith](https://www.kamilgrzybek.com/blog/posts/modular-monolith-primer)
- [Vertical Slice Architecture](https://jimmybogard.com/vertical-slice-architecture/)

