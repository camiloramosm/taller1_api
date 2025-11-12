# Instrucciones para Configurar la Conexión a Supabase

## ✅ Estado Actual

- ✅ **Tabla creada**: La tabla `TodoItems` ya está creada en Supabase
- ✅ **Host configurado**: `db.ivzqrlnrackqfjnizbza.supabase.co`
- ⚠️ **Pendiente**: Configurar la contraseña de la base de datos

## 🔑 Obtener la Contraseña de Supabase

### Opción 1: Desde el Dashboard de Supabase

1. Ve a: https://supabase.com/dashboard/project/ivzqrlnrackqfjnizbza
2. Navega a **Settings** → **Database**
3. Busca la sección **Connection string** o **Database password**
4. Si no recuerdas la contraseña:
   - Haz clic en **Reset database password**
   - Guarda la nueva contraseña de forma segura

### Opción 2: Usar Connection String URI

Supabase también proporciona una URI de conexión. Puedes convertirla:

**Formato URI:**
```
postgresql://postgres:[PASSWORD]@db.ivzqrlnrackqfjnizbza.supabase.co:5432/postgres
```

**Formato Npgsql (para appsettings.json):**
```
Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=[PASSWORD];SSL Mode=Require;
```

## 📝 Configurar la Contraseña

### Método 1: Editar appsettings.json

Edita `TodoList.API/appsettings.json` y reemplaza `TU_PASSWORD_AQUI` con tu contraseña:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=TU_PASSWORD_REAL;SSL Mode=Require;"
  }
}
```

### Método 2: User Secrets (Recomendado para Desarrollo)

```bash
dotnet user-secrets init --project TodoList.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=TU_PASSWORD;SSL Mode=Require;" --project TodoList.API
```

### Método 3: Variables de Entorno

**Windows (PowerShell):**
```powershell
$env:ConnectionStrings__DefaultConnection = "Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=TU_PASSWORD;SSL Mode=Require;"
```

**Linux/Mac:**
```bash
export ConnectionStrings__DefaultConnection="Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=TU_PASSWORD;SSL Mode=Require;"
```

## 🧪 Verificar la Conexión

Una vez configurada la contraseña, ejecuta:

```bash
dotnet run --project TodoList.API
```

Si la conexión es exitosa, verás en los logs:
```
Conexión a la base de datos establecida correctamente
```

Y podrás acceder a Swagger en: `https://localhost:5001/swagger`

## 📊 Estado de la Base de Datos

- **Tabla**: `TodoItems` ✅ Creada
- **Índices**: 
  - `IX_TodoItems_UserId` ✅
  - `IX_TodoItems_IsCompleted` ✅
- **RLS (Row Level Security)**: Deshabilitado (aceptable para API con EF Core)

## 🔒 Nota de Seguridad

La tabla `TodoItems` tiene RLS deshabilitado. Esto es aceptable cuando usas Entity Framework Core directamente, ya que la seguridad se maneja a nivel de aplicación. Si planeas usar PostgREST de Supabase, deberías habilitar RLS.

