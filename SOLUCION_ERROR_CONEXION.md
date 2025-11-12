# Solución al Error de Conexión con Supabase

## Error
```
System.Net.Sockets.SocketException: 'The requested name is valid, but no data of the requested type was found.'
```

## Causa
Este error ocurre cuando hay problemas con la resolución DNS o la configuración SSL al conectarse a Supabase.

## Soluciones Aplicadas

### 1. Cadena de Conexión Actualizada

**Antes**:
```
Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=!ulgxbcTqpjgcl2;SSL Mode=Require;
```

**Después**:
```
Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres.ivzqrlnrackqfjnizbza;Password=!ulgxbcTqpjgcl2;SSL Mode=Prefer;Trust Server Certificate=true;Pooling=true;
```

### Cambios Realizados:

1. **Username completo**: `postgres.ivzqrlnrackqfjnizbza` (incluye el ID del proyecto)
2. **SSL Mode**: Cambiado de `Require` a `Prefer`
3. **Trust Server Certificate**: Agregado `true` para aceptar certificados
4. **Pooling**: Agregado `true` para mejor rendimiento

## Alternativas si Persiste el Error

### Opción 1: Sin SSL (Solo Desarrollo)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=!ulgxbcTqpjgcl2;SSL Mode=Disable;Pooling=true;"
  }
}
```

### Opción 2: Con IPv6
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres.ivzqrlnrackqfjnizbza;Password=!ulgxbcTqpjgcl2;SSL Mode=VerifyFull;Trust Server Certificate=true;Pooling=true;Include Error Detail=true;"
  }
}
```

### Opción 3: Con Timeout Extendido
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=!ulgxbcTqpjgcl2;SSL Mode=Prefer;Trust Server Certificate=true;Pooling=true;Timeout=30;Command Timeout=30;"
  }
}
```

### Opción 4: URI Format (Alternativa)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "postgresql://postgres.ivzqrlnrackqfjnizbza:!ulgxbcTqpjgcl2@db.ivzqrlnrackqfjnizbza.supabase.co:5432/postgres?sslmode=prefer"
  }
}
```

## Verificación

### 1. Probar la Conexión
```bash
dotnet run --project TodoList.API
```

### 2. Verificar Health Check
```bash
curl http://localhost:5000/health
```

### 3. Ver Logs Detallados
Agrega esto temporalmente en `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Debug",
      "Npgsql": "Debug"
    }
  }
}
```

## Diagnóstico Adicional

### Verificar DNS
```bash
# Windows
nslookup db.ivzqrlnrackqfjnizbza.supabase.co

# PowerShell
Resolve-DnsName db.ivzqrlnrackqfjnizbza.supabase.co
```

### Verificar Conectividad
```bash
# Windows
Test-NetConnection db.ivzqrlnrackqfjnizbza.supabase.co -Port 5432

# Alternativa
telnet db.ivzqrlnrackqfjnizbza.supabase.co 5432
```

## Problemas Comunes

### 1. Firewall Bloqueando Puerto 5432
- Verifica tu firewall de Windows
- Permite conexiones salientes en puerto 5432

### 2. Antivirus Bloqueando
- Algunos antivirus bloquean conexiones SSL
- Temporalmente desactiva el antivirus para probar

### 3. VPN o Proxy
- Si estás detrás de VPN o proxy corporativo
- Puede estar bloqueando conexiones directas

### 4. IPv6 vs IPv4
- Windows puede intentar IPv6 primero
- Supabase puede tener mejor soporte para IPv4

## Solución Temporal con Connection Pooling Deshabilitado

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=!ulgxbcTqpjgcl2;SSL Mode=Prefer;Trust Server Certificate=true;Pooling=false;"
  }
}
```

## Configuración del DbContext (Si persiste)

En `DependencyInjection.cs`, intenta:

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(60);
    });
});
```

## Testing desde C#

Crea un archivo de prueba rápido:

```csharp
using Npgsql;

var connectionString = "Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=!ulgxbcTqpjgcl2;SSL Mode=Prefer;Trust Server Certificate=true;";

try
{
    using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();
    Console.WriteLine("✅ Conexión exitosa!");
    
    using var cmd = new NpgsqlCommand("SELECT version()", conn);
    var version = await cmd.ExecuteScalarAsync();
    Console.WriteLine($"PostgreSQL Version: {version}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine($"Detalles: {ex.InnerException?.Message}");
}
```

## Contacto con Supabase

Si ninguna solución funciona:
1. Ve al Dashboard de Supabase
2. Settings → Database
3. Verifica la Connection String que proporciona Supabase
4. Usa exactamente esa cadena

## Estado Actual

✅ **Aplicado**: Username con sufijo del proyecto  
✅ **Aplicado**: SSL Mode=Prefer  
✅ **Aplicado**: Trust Server Certificate=true  
✅ **Aplicado**: Pooling=true  

**Siguiente paso**: Ejecuta la aplicación y prueba nuevamente.

