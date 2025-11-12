# Configuración de Supabase

## Información del Proyecto

- **Nombre del Proyecto**: taller1
- **Project ID**: ivzqrlnrackqfjnizbza
- **Host de Base de Datos**: db.ivzqrlnrackqfjnizbza.supabase.co
- **URL del Proyecto**: https://ivzqrlnrackqfjnizbza.supabase.co
- **Región**: us-east-1

## Cómo Obtener la Contraseña de la Base de Datos

1. Ve a tu proyecto en Supabase: https://supabase.com/dashboard/project/ivzqrlnrackqfjnizbza
2. Navega a **Settings** → **Database**
3. Busca la sección **Connection string** o **Database password**
4. Si no recuerdas la contraseña, puedes:
   - **Resetear la contraseña**: En Settings → Database, hay una opción para resetear la contraseña de la base de datos
   - **Usar la Connection String URI**: Copia la URI completa y conviértela al formato de Npgsql

## Formato de la Cadena de Conexión

La cadena de conexión debe tener este formato:

```
Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=TU_PASSWORD;SSL Mode=Require;
```

## Alternativa: Usar Connection String URI

Si Supabase te proporciona una URI como:
```
postgresql://postgres:[PASSWORD]@db.ivzqrlnrackqfjnizbza.supabase.co:5432/postgres
```

Puedes convertirla manualmente o usar variables de entorno.

## Configuración Recomendada para Producción

Para producción, se recomienda usar **User Secrets** o **Variables de Entorno**:

### User Secrets (Desarrollo Local)
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=TU_PASSWORD;SSL Mode=Require;"
```

### Variables de Entorno
```bash
export ConnectionStrings__DefaultConnection="Host=db.ivzqrlnrackqfjnizbza.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=TU_PASSWORD;SSL Mode=Require;"
```

## Verificar la Conexión

Una vez configurada la contraseña, puedes verificar la conexión ejecutando:

```bash
dotnet run --project TodoList.API
```

Si la conexión es exitosa, la aplicación creará automáticamente la tabla `TodoItems` al iniciar.

