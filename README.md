# TodoList Service - ASP.NET Core 9

[![CI - Build and Test](https://github.com/camiloramosm/taller1_api/actions/workflows/ci.yml/badge.svg)](https://github.com/camiloramosm/taller1_api/actions/workflows/ci.yml)

Servicio de To-Do List desarrollado con ASP.NET Core 9, siguiendo arquitectura de monolito modular y principios de Clean Code, conectado a Supabase (PostgreSQL).

## 📚 Documentación

- 📖 [Guía Rápida de Pull Requests](GUIA_RAPIDA_PR.md) - **¡Comienza aquí!**
- ⚙️ [Configuración de GitHub CI/CD](CONFIGURACION_GITHUB.md)
- 🧪 [Pruebas Unitarias](PRUEBAS_UNITARIAS.md)
- 🏗️ [Estructura del Proyecto](ESTRUCTURA_PROYECTO.md)
- 🔐 [Configuración de Supabase](CONFIGURACION_SUPABASE.md)
- 📋 [Reglas de Negocio](REGLAS_DE_NEGOCIO.md)

## 🏗️ Arquitectura

El proyecto sigue una arquitectura de **monolito modular** con separación de responsabilidades:

```
TodoListService/
├── TodoList.API/              # Capa de presentación (Controllers, Program.cs)
├── TodoList.Application/      # Capa de aplicación (Services, DTOs, Interfaces)
├── TodoList.Domain/           # Capa de dominio (Entities, Common)
└── TodoList.Infrastructure/  # Capa de infraestructura (DbContext, Repositories, DI)
```

### Capas

- **Domain**: Contiene las entidades del dominio y objetos de valor
- **Application**: Contiene la lógica de negocio, servicios, DTOs e interfaces de repositorios
- **Infrastructure**: Implementa el acceso a datos (Entity Framework Core + PostgreSQL), repositorios y configuración de dependencias
- **API**: Contiene los controladores REST y la configuración de la aplicación

## 🚀 Características

- ✅ CRUD completo para elementos de To-Do
- ✅ Filtrado por usuario
- ✅ Alternar estado de completado
- ✅ Validación de datos
- ✅ Swagger/OpenAPI para documentación
- ✅ CORS configurado
- ✅ Entity Framework Core con PostgreSQL
- ✅ Arquitectura limpia y modular

## 📋 Requisitos Previos

- .NET 9 SDK
- PostgreSQL (Supabase) o base de datos PostgreSQL local
- Visual Studio 2022, VS Code o cualquier editor compatible

## 🔧 Configuración

### 1. Clonar y restaurar dependencias

```bash
dotnet restore
```

### 2. Configurar la cadena de conexión

✅ **La tabla ya está creada en Supabase** - Solo necesitas configurar la contraseña.

Edita el archivo `TodoList.API/appsettings.json` y reemplaza `TU_PASSWORD_AQUI` con tu contraseña de Supabase:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=TU_PASSWORD_AQUI;SSL Mode=Require;"
  }
}
```

**Cómo obtener la contraseña:**
1. Ve a: https://supabase.com/dashboard/project/ivzqrlnrackqfjnizbza
2. Settings → Database
3. Busca "Database password" o "Connection string"
4. Si no la recuerdas, puedes resetearla desde el dashboard

**Información del proyecto Supabase:**
- Proyecto: `taller1`
- Host: `db.ivzqrlnrackqfjnizbza.supabase.co`
- Tabla `TodoItems`: ✅ Ya creada

Para más detalles, consulta `INSTRUCCIONES_CONEXION.md`

### 3. Verificar la conexión

La tabla `TodoItems` ya está creada en Supabase. Solo necesitas configurar la contraseña y ejecutar la aplicación.

**Nota:** Si necesitas crear migraciones adicionales en el futuro:

```bash
dotnet ef migrations add NombreMigracion --project TodoList.Infrastructure --startup-project TodoList.API
dotnet ef database update --project TodoList.Infrastructure --startup-project TodoList.API
```

## 🏃 Ejecutar la aplicación

```bash
dotnet run --project TodoList.API
```

La API estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## 📡 Endpoints de la API

### GET `/api/todoitems`
Obtiene todos los elementos de la lista de tareas.

### GET `/api/todoitems/{id}`
Obtiene un elemento específico por su ID.

### GET `/api/todoitems/user/{userId}`
Obtiene todos los elementos de un usuario específico.

### POST `/api/todoitems`
Crea un nuevo elemento.

**Body:**
```json
{
  "title": "Mi nueva tarea",
  "description": "Descripción opcional",
  "userId": "00000000-0000-0000-0000-000000000000"
}
```

### PUT `/api/todoitems/{id}`
Actualiza un elemento existente.

**Body:**
```json
{
  "title": "Tarea actualizada",
  "description": "Nueva descripción",
  "isCompleted": true
}
```

### PATCH `/api/todoitems/{id}/toggle-complete`
Alterna el estado de completado de un elemento.

### DELETE `/api/todoitems/{id}`
Elimina un elemento.

## 🧪 Ejemplos de uso

### Crear una tarea

```bash
curl -X POST https://localhost:5001/api/todoitems \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Completar proyecto",
    "description": "Finalizar el servicio de To-Do List",
    "userId": "123e4567-e89b-12d3-a456-426614174000"
  }'
```

### Obtener todas las tareas

```bash
curl https://localhost:5001/api/todoitems
```

### Marcar como completada

```bash
curl -X PATCH https://localhost:5001/api/todoitems/{id}/toggle-complete
```

## 🏗️ Estructura del Proyecto

```
TodoListService/
├── TodoList.API/
│   ├── Controllers/
│   │   └── TodoItemsController.cs
│   ├── Program.cs
│   └── appsettings.json
├── TodoList.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
├── TodoList.Domain/
│   ├── Entities/
│   └── Common/
└── TodoList.Infrastructure/
    ├── Data/
    ├── Repositories/
    └── DependencyInjection.cs
```

## 🔒 Seguridad

- La aplicación está configurada para desarrollo. Para producción:
  - Configurar CORS apropiadamente
  - Implementar autenticación y autorización
  - Usar migraciones de EF Core en lugar de `EnsureCreatedAsync`
  - Configurar HTTPS correctamente
  - Validar y sanitizar todas las entradas

## 📝 Notas

- El proyecto usa `EnsureCreatedAsync()` solo para desarrollo rápido
- Para producción, se recomienda usar migraciones de Entity Framework Core
- La validación se realiza mediante Data Annotations en los DTOs
- Los timestamps se manejan en UTC

## 🛠️ Tecnologías Utilizadas

- ASP.NET Core 9
- Entity Framework Core 9
- Npgsql (PostgreSQL)
- Swagger/OpenAPI
- Clean Architecture
- C# 13

## 🔄 CI/CD y Pull Requests

### Flujo de Trabajo

Este proyecto utiliza **GitHub Actions** para CI/CD automático:

1. **Crea una rama** para tu feature:
   ```bash
   git checkout -b feature/mi-funcionalidad
   ```

2. **Ejecuta las pruebas localmente**:
   ```bash
   dotnet test --verbosity normal
   ```

3. **Crea un Pull Request** en GitHub

4. **GitHub Actions ejecutará automáticamente**:
   - ✅ Build de la solución
   - ✅ 36 pruebas unitarias
   - ✅ Verificación de calidad de código

5. **El PR será aprobado o rechazado automáticamente** basado en los resultados

### Protección de Rama Main

La rama `main` está protegida:
- ❌ No se permiten push directos
- ✅ Solo se aceptan cambios mediante Pull Request
- ✅ Todas las pruebas deben pasar antes del merge
- ✅ Se requiere al menos 1 aprobación

📖 **Ver guía completa**: [GUIA_RAPIDA_PR.md](GUIA_RAPIDA_PR.md)

⚙️ **Configuración detallada**: [CONFIGURACION_GITHUB.md](CONFIGURACION_GITHUB.md)

## 📄 Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.

