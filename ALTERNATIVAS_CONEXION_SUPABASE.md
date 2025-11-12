# Alternativas de Conexión a Supabase

## Problema
Error: `System.Net.Sockets.SocketException: 'The requested name is valid, but no data of the requested type was found.'`

## Soluciones

### ✅ Opción 1: Transaction Pooler (RECOMENDADO para APIs)

**Puerto**: 6543  
**Host**: `aws-0-us-east-1.pooler.supabase.com`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=aws-0-us-east-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.ivzqrlnrackqfjnizbza;Password=!ulgxbcTqpjgcl2;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;Timeout=15;Command Timeout=30;"
  }
}
```

**Ventajas**:
- Optimizado para APIs y aplicaciones web
- Mejor manejo de conexiones
- Evita problemas de DNS con el endpoint regional

### Opción 2: Session Pooler (Conexión Directa)

**Puerto**: 5432  
**Host**: `db.ivzqrlnrackqfjnizbza.supabase.co`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres.ivzqrlnrackqfjnizbza;Password=!ulgxbcTqpjgcl2;SSL Mode=Require;Trust Server Certificate=true;"
  }
}
```

### Opción 3: Conexión con IPv4 Explícito

Resuelve el DNS primero y usa la IP directamente:

```bash
# PowerShell
Resolve-DnsName db.ivzqrlnrackqfjnizbza.supabase.co
```

Luego usa la IP en la conexión:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=<IP_ADDRESS>;Port=5432;Database=postgres;Username=postgres.ivzqrlnrackqfjnizbza;Password=!ulgxbcTqpjgcl2;SSL Mode=Require;"
  }
}
```

### Opción 4: Sin SSL (Solo para pruebas locales)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=!ulgxbcTqpjgcl2;SSL Mode=Disable;"
  }
}
```

### Opción 5: Supabase REST API (Alternativa sin PostgreSQL directo)

Si la conexión directa no funciona, usa la API REST de Supabase:

1. Instalar el SDK:
```bash
dotnet add TodoList.Infrastructure package Supabase
```

2. Configurar:
```json
{
  "Supabase": {
    "Url": "https://ivzqrlnrackqfjnizbza.supabase.co",
    "Key": "TU_ANON_KEY_AQUI"
  }
}
```

## Información del Proyecto

- **Project ID**: ivzqrlnrackqfjnizbza
- **Region**: us-east-1
- **Direct Connection**: db.ivzqrlnrackqfjnizbza.supabase.co:5432
- **Transaction Pooler**: aws-0-us-east-1.pooler.supabase.com:6543
- **Session Pooler**: aws-0-us-east-1.pooler.supabase.com:5432

## Verificar Conexión desde PowerShell

```powershell
# Verificar DNS
Resolve-DnsName db.ivzqrlnrackqfjnizbza.supabase.co
Resolve-DnsName aws-0-us-east-1.pooler.supabase.com

# Verificar conectividad
Test-NetConnection -ComputerName aws-0-us-east-1.pooler.supabase.com -Port 6543
Test-NetConnection -ComputerName db.ivzqrlnrackqfjnizbza.supabase.co -Port 5432
```

## Problemas Comunes

### 1. Windows bloquea conexiones IPv6
**Solución**: Usar Transaction Pooler que generalmente resuelve mejor

### 2. Firewall corporativo
**Solución**: Verificar que los puertos 5432 y 6543 estén abiertos

### 3. Antivirus bloqueando SSL
**Solución**: Agregar excepción para dotnet.exe

### 4. VPN interfiriendo
**Solución**: Probar desconectando la VPN temporalmente

## Username Correcto

Para Supabase, el username debe incluir el sufijo del proyecto:
- ❌ `postgres`
- ✅ `postgres.ivzqrlnrackqfjnizbza`

## Testing

Después de cambiar la configuración:

```bash
# Ejecutar
dotnet run --project TodoList.API

# Probar health check
curl http://localhost:5000/health

# Ver logs detallados
# En appsettings.json agregar:
{
  "Logging": {
    "LogLevel": {
      "Npgsql": "Debug",
      "Microsoft.EntityFrameworkCore": "Debug"
    }
  }
}
```

## Dashboard de Supabase

Para verificar configuración correcta:
1. Ve a: https://supabase.com/dashboard/project/ivzqrlnrackqfjnizbza
2. Settings → Database
3. Connection String → Transaction pooler (recomendado)
4. Copia y adapta al formato Npgsql

