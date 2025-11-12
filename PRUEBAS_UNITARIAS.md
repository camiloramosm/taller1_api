# Pruebas Unitarias - TodoList

## 📝 Resumen

Se han creado **36 pruebas unitarias** para el proyecto TodoList siguiendo el patrón de Evently con xUnit, FluentAssertions, NSubstitute y Bogus.

## 🎯 Resultados

✅ **Todas las pruebas pasaron exitosamente**
- **Total**: 36 pruebas
- **Pasadas**: 36
- **Fallidas**: 0
- **Omitidas**: 0
- **Duración**: ~700ms

## 📁 Estructura del Proyecto de Pruebas

```
src/Modules/Todos/TodoList.Modules.Todos.UnitTests/
├── Abstractions/
│   └── BaseTest.cs                          # Clase base con helpers de Faker
├── Domain/
│   └── TodoBusinessRulesTests.cs            # 19 pruebas
└── Application/
    └── TodoServiceTests.cs                  # 17 pruebas
```

## 🧪 Cobertura de Pruebas

### Domain Layer (19 pruebas)

#### TodoBusinessRules - Validación de Título (4 pruebas)
- ✅ ValidateTitle_ShouldNotThrow_WhenTitleIsValid
- ✅ ValidateTitle_ShouldThrowValidationException_WhenTitleIsEmpty
- ✅ ValidateTitle_ShouldThrowValidationException_WhenTitleIsWhitespace
- ✅ ValidateTitle_ShouldThrowValidationException_WhenTitleExceedsMaxLength

#### TodoBusinessRules - Validación de Descripción (3 pruebas)
- ✅ ValidateDescription_ShouldNotThrow_WhenDescriptionIsValid
- ✅ ValidateDescription_ShouldNotThrow_WhenDescriptionIsNull
- ✅ ValidateDescription_ShouldThrowValidationException_WhenDescriptionExceedsMaxLength

#### TodoBusinessRules - Validación de Completado (4 pruebas)
- ✅ ValidateCanComplete_ShouldNotThrow_WhenTodoItemIsNotCompleted
- ✅ ValidateCanComplete_ShouldThrowInvalidTodoOperationException_WhenTodoItemIsAlreadyCompleted
- ✅ ValidateCanUncomplete_ShouldNotThrow_WhenTodoItemIsCompleted
- ✅ ValidateCanUncomplete_ShouldThrowInvalidTodoOperationException_WhenTodoItemIsNotCompleted

#### TodoBusinessRules - Validación de Fechas (4 pruebas)
- ✅ ValidateDates_ShouldNotThrow_WhenDatesAreValid
- ✅ ValidateDates_ShouldThrowValidationException_WhenUpdatedAtIsBeforeCreatedAt
- ✅ ValidateDates_ShouldThrowValidationException_WhenCompletedAtIsBeforeCreatedAt
- ✅ ValidateDates_ShouldThrowValidationException_WhenCompletedAtExistsButNotCompleted

#### TodoBusinessRules - Validación de Creación y Actualización (4 pruebas)
- ✅ ValidateCreation_ShouldNotThrow_WhenTitleAndDescriptionAreValid
- ✅ ValidateCreation_ShouldThrowValidationException_WhenTitleIsInvalid
- ✅ ValidateUpdate_ShouldNotThrow_WhenAllDataIsValid
- ✅ ValidateUpdate_ShouldThrowValidationException_WhenTitleIsInvalid

### Application Layer (17 pruebas)

#### TodoService - GetByIdAsync (2 pruebas)
- ✅ GetByIdAsync_ShouldReturnTodoItemDto_WhenTodoItemExists
- ✅ GetByIdAsync_ShouldReturnNull_WhenTodoItemDoesNotExist

#### TodoService - GetAllAsync (2 pruebas)
- ✅ GetAllAsync_ShouldReturnAllTodoItems
- ✅ GetAllAsync_ShouldReturnEmptyList_WhenNoTodoItems

#### TodoService - CreateAsync (3 pruebas)
- ✅ CreateAsync_ShouldCreateTodoItem_WhenDataIsValid
- ✅ CreateAsync_ShouldThrowValidationException_WhenTitleIsEmpty
- ✅ CreateAsync_ShouldTrimTitleAndDescription

#### TodoService - UpdateAsync (3 pruebas)
- ✅ UpdateAsync_ShouldUpdateTodoItem_WhenDataIsValid
- ✅ UpdateAsync_ShouldThrowTodoItemNotFoundException_WhenTodoDoesNotExist
- ✅ UpdateAsync_ShouldSetCompletedAtToNull_WhenMarkingAsIncomplete

#### TodoService - DeleteAsync (2 pruebas)
- ✅ DeleteAsync_ShouldDeleteTodoItem_WhenTodoExists
- ✅ DeleteAsync_ShouldThrowTodoItemNotFoundException_WhenTodoDoesNotExist

#### TodoService - ToggleCompleteAsync (3 pruebas)
- ✅ ToggleCompleteAsync_ShouldMarkAsComplete_WhenTodoIsIncomplete
- ✅ ToggleCompleteAsync_ShouldMarkAsIncomplete_WhenTodoIsComplete
- ✅ ToggleCompleteAsync_ShouldThrowTodoItemNotFoundException_WhenTodoDoesNotExist

#### TodoService - GetPagedAsync y GetByUserIdAsync (2 pruebas)
- ✅ GetPagedAsync_ShouldReturnPagedResults
- ✅ GetByUserIdAsync_ShouldReturnUserTodos

## 📦 Paquetes NuGet Utilizados

```xml
<PackageReference Include="Bogus" Version="35.5.0" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
<PackageReference Include="NSubstitute" Version="5.1.0" />
<PackageReference Include="xunit" Version="2.7.1" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.8" />
<PackageReference Include="coverlet.collector" Version="6.0.2" />
```

## 🚀 Comandos para Ejecutar las Pruebas

### Ejecutar todas las pruebas
```bash
dotnet test src\Modules\Todos\TodoList.Modules.Todos.UnitTests\TodoList.Modules.Todos.UnitTests.csproj
```

### Ejecutar con verbosidad detallada
```bash
dotnet test src\Modules\Todos\TodoList.Modules.Todos.UnitTests\TodoList.Modules.Todos.UnitTests.csproj --verbosity normal
```

### Ejecutar con cobertura de código
```bash
dotnet test src\Modules\Todos\TodoList.Modules.Todos.UnitTests\TodoList.Modules.Todos.UnitTests.csproj /p:CollectCoverage=true
```

## 📋 Patrones de Prueba Utilizados

### 1. **Arrange-Act-Assert (AAA)**
Todas las pruebas siguen este patrón estándar:
```csharp
// Arrange: Preparar datos y dependencias
var todoId = Guid.NewGuid();

// Act: Ejecutar la operación a probar
var result = await _sut.GetByIdAsync(todoId);

// Assert: Verificar el resultado
result.Should().NotBeNull();
```

### 2. **Mocking con NSubstitute**
Se utiliza NSubstitute para simular el repositorio:
```csharp
_mockRepository.GetByIdAsync(todoId, Arg.Any<CancellationToken>())
    .Returns(todoItem);
```

### 3. **Assertions con FluentAssertions**
Validaciones expresivas y legibles:
```csharp
result.Should().NotBeNull();
result!.Title.Should().Be(expectedTitle);
act.Should().Throw<ValidationException>()
    .Where(ex => ex.Errors.ContainsKey("Title"));
```

### 4. **Generación de Datos con Bogus**
Datos de prueba aleatorios pero válidos:
```csharp
protected static string GenerateValidTitle()
{
    var sentence = Faker.Lorem.Sentence(3, 7);
    return sentence.Length <= 50 ? sentence : sentence.Substring(0, 50);
}
```

## 🎨 Características Destacadas

1. **BaseTest Abstracta**: Proporciona helpers comunes para todas las pruebas
2. **Validación de Excepciones**: Verifica tanto el tipo como el contenido del diccionario de errores
3. **Pruebas de Borde**: Cubre casos límite como strings vacíos, nulos, y longitudes máximas
4. **Mocking Efectivo**: Aísla la lógica de negocio de las dependencias externas
5. **Datos Realistas**: Utiliza Bogus para generar datos de prueba significativos

## ⚠️ Advertencias de Análisis de Código

Las advertencias CA1707 (underscores en nombres de métodos) y CA2007 (ConfigureAwait) son comunes en proyectos de prueba y no afectan la funcionalidad. Se pueden suprimir si se desea:

```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);CA1707;CA2007</NoWarn>
</PropertyGroup>
```

## ✅ Conclusión

El proyecto ahora cuenta con una suite completa de pruebas unitarias que valida:
- ✅ Reglas de negocio del dominio
- ✅ Lógica de servicio de aplicación
- ✅ Manejo de excepciones personalizadas
- ✅ Validaciones de datos
- ✅ Operaciones CRUD completas

Todas las pruebas pasan exitosamente y proporcionan una base sólida para el desarrollo continuo del proyecto.

