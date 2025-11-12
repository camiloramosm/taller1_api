# 🔧 Solución: Error NU1102 en GitHub Actions

## 📋 Problema Identificado

El pipeline de CI/CD en GitHub Actions estaba fallando en el paso de `dotnet restore` con los siguientes errores:

```
error NU1102: Unable to find package supabase-csharp with version (>= 0.20.1)
  - Found 74 version(s) in nuget.org [ Nearest version: 0.16.2 ]

warning NU1603: TodoList.Modules.Todos.Domain depends on postgrest-csharp (>= 3.4.4) but postgrest-csharp 3.4.4 was not found. postgrest-csharp 3.5.0 was resolved instead.
```

## 🔍 Análisis del Problema

1. **Versión inexistente de supabase-csharp**: El proyecto estaba intentando usar la versión `0.20.1`, pero NuGet.org solo tiene disponible hasta la versión `0.16.2`.

2. **Conflicto de versiones de postgrest-csharp**: 
   - `TodoList.Modules.Todos.Domain.csproj` especificaba postgrest-csharp `3.4.4`
   - Esta versión no existe, por lo que NuGet resolvía a `3.5.0`
   - Sin embargo, `supabase-csharp 0.16.2` **requiere** `postgrest-csharp >= 3.5.1`
   - Esto causaba un error NU1605 (package downgrade detected)

## ✅ Solución Implementada

### 1. Actualizar versión de postgrest-csharp

**Archivo**: `src/Modules/Todos/TodoList.Modules.Todos.Domain/TodoList.Modules.Todos.Domain.csproj`

```xml
<ItemGroup>
  <PackageReference Include="postgrest-csharp" Version="3.5.1" />
</ItemGroup>
```

### 2. Agregar referencia explícita en Infrastructure

**Archivo**: `src/Modules/Todos/TodoList.Modules.Todos.Infrastructure/TodoList.Modules.Todos.Infrastructure.csproj`

```xml
<ItemGroup>
  <PackageReference Include="supabase-csharp" Version="0.16.2" />
  <PackageReference Include="postgrest-csharp" Version="3.5.1" />
  <!-- ... otros paquetes ... -->
</ItemGroup>
```

### 3. Actualizar proyecto legacy

**Archivo**: `TodoList.Infrastructure/TodoList.Infrastructure.csproj`

```xml
<PackageReference Include="supabase-csharp" Version="0.16.2" />
<PackageReference Include="postgrest-csharp" Version="3.5.1" />
```

## 📊 Resultados

### Antes de la corrección:
- ❌ `dotnet restore` fallaba en GitHub Actions
- ❌ Build fallaba por dependencias no resueltas
- ❌ Tests no se ejecutaban

### Después de la corrección:
- ✅ `dotnet restore` exitoso (0 errores)
- ✅ `dotnet build` exitoso (0 errores, 137 warnings de análisis de código)
- ✅ `dotnet test` exitoso (36/36 pruebas pasando)

## 🎯 Verificación Local

Comandos ejecutados para verificar la solución:

```bash
# 1. Restaurar dependencias
dotnet restore TodoList.sln
# ✅ Exitoso: 0 errores, 0 warnings

# 2. Compilar solución
dotnet build TodoList.sln --no-restore
# ✅ Exitoso: 0 errores, 137 warnings

# 3. Ejecutar pruebas unitarias
dotnet test TodoList.sln --no-build --verbosity normal
# ✅ Exitoso: 36 pruebas pasadas
```

## 📦 Versiones Finales de Paquetes

| Paquete | Versión | Ubicación |
|---------|---------|-----------|
| `supabase-csharp` | `0.16.2` | Infrastructure |
| `postgrest-csharp` | `3.5.1` | Domain, Infrastructure |

## 🚀 Próximos Pasos

1. ✅ Verificar que el workflow de GitHub Actions pasa exitosamente
2. ✅ Confirmar que las pruebas unitarias se ejecutan en CI/CD
3. ✅ Validar que la protección de rama `main` funciona correctamente
4. 🔄 Monitorear futuras actualizaciones de `supabase-csharp` en NuGet.org

## 📝 Notas Importantes

- La versión `0.16.2` de `supabase-csharp` es la **última versión estable** disponible en NuGet.org al 12 de noviembre de 2025.
- La versión `3.5.1` de `postgrest-csharp` es **requerida** por `supabase-csharp 0.16.2`.
- Cualquier intento de usar versiones más recientes (como `1.0.0-rc.10` o `0.20.1`) fallará porque no existen en el registro oficial de NuGet.

## 🔗 Referencias

- [Paquete supabase-csharp en NuGet](https://www.nuget.org/packages/supabase-csharp/)
- [Paquete postgrest-csharp en NuGet](https://www.nuget.org/packages/postgrest-csharp/)
- [Documentación de NuGet Error Codes](https://docs.microsoft.com/nuget/reference/errors-and-warnings/)

---

**Fecha de corrección**: 12 de noviembre de 2025  
**Commit**: `63bd02a` - fix: actualizar versiones de paquetes NuGet para CI/CD

