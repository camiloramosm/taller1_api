# 🚀 Guía Rápida: Trabajar con Pull Requests

## ⚡ Flujo de Trabajo en 5 Pasos

### 1️⃣ Crear rama y hacer cambios

```bash
# Desde main actualizado
git checkout main
git pull origin main

# Crear nueva rama
git checkout -b feature/mi-nueva-funcionalidad

# Hacer tus cambios...
```

### 2️⃣ Ejecutar pruebas localmente

```bash
# ¡IMPORTANTE! Siempre ejecuta antes de hacer push
dotnet restore
dotnet build
dotnet test --verbosity normal
```

✅ **Si todas las pruebas pasan, continúa al siguiente paso**

❌ **Si alguna prueba falla, corrígela antes de continuar**

### 3️⃣ Commit y Push

```bash
git add .
git commit -m "feat: descripción clara del cambio"
git push origin feature/mi-nueva-funcionalidad
```

### 4️⃣ Crear Pull Request en GitHub

1. Ve a: https://github.com/camiloramosm/taller1_api
2. Click en **"Compare & pull request"** (banner amarillo)
3. Completa la plantilla del PR
4. Click en **"Create pull request"**

### 5️⃣ Esperar validación automática

```
┌─────────────────────────────────────────┐
│  🤖 GitHub Actions se ejecuta AUTO     │
├─────────────────────────────────────────┤
│  ⏳ Build and Run Tests (running...)   │
│  ⏳ Validate Pull Request (running...) │
│  ⏳ Code Quality Checks (running...)   │
└─────────────────────────────────────────┘
```

**Resultado posible:**

✅ **ÉXITO**: 
```
✓ Build and Run Tests
✓ Validate Pull Request
✓ Code Quality Checks

→ PR listo para revisión y merge
```

❌ **FALLO**:
```
✗ Build and Run Tests (failed)
  → 3 pruebas fallaron

→ PR BLOQUEADO hasta corregir
```

---

## 🔴 Si las Pruebas Fallan en GitHub

### Paso 1: Ver qué falló

1. En tu PR, click en **"Details"** del check que falló
2. Lee el log de error
3. Identifica qué prueba falló y por qué

### Paso 2: Corregir localmente

```bash
# En tu rama feature
# Corrige el código...

# Ejecuta las pruebas nuevamente
dotnet test
```

### Paso 3: Push de la corrección

```bash
git add .
git commit -m "fix: corregir prueba que fallaba"
git push
```

**✨ GitHub Actions se ejecutará automáticamente de nuevo**

---

## 🎯 Estados del Pull Request

### 🟡 En Proceso
```
⚠️ Some checks haven't completed yet
   • Build and Run Tests (in progress)
   • Validate Pull Request (queued)

→ Espera a que terminen todos los checks
```

### ✅ Listo para Merge
```
✓ All checks have passed
✓ This branch has no conflicts with the base branch
✓ 1 approval required

→ Solicita revisión o aprueba (si tienes permisos)
```

### ❌ Bloqueado
```
✗ Some checks were not successful
✗ 3 failing checks

→ Debes corregir los errores antes de merge
```

### 🔄 Desactualizado
```
⚠️ This branch is out-of-date with the base branch

→ Actualiza tu rama con main:
```

```bash
git checkout main
git pull origin main
git checkout feature/tu-rama
git merge main
git push
```

---

## 📝 Convenciones de Commits

Usa prefijos descriptivos en tus commits:

| Prefijo | Uso | Ejemplo |
|---------|-----|---------|
| `feat:` | Nueva funcionalidad | `feat: agregar validación de email` |
| `fix:` | Corrección de bug | `fix: corregir cálculo de fecha` |
| `docs:` | Documentación | `docs: actualizar README` |
| `test:` | Pruebas | `test: agregar pruebas para TodoService` |
| `refactor:` | Refactorización | `refactor: simplificar lógica de validación` |
| `style:` | Formato | `style: aplicar formato de código` |
| `chore:` | Tareas de mantenimiento | `chore: actualizar dependencias` |
| `ci:` | CI/CD | `ci: actualizar workflow de GitHub Actions` |

---

## 🚨 Problemas Comunes

### "Required status check is failing"

**Causa:** Las pruebas unitarias fallaron

**Solución:**
1. Ve a Actions → Click en el workflow fallido
2. Revisa el log
3. Corrige y push de nuevo

---

### "Branch is out of date"

**Causa:** Alguien hizo cambios en `main` después de que creaste tu rama

**Solución:**
```bash
git checkout main
git pull origin main
git checkout feature/tu-rama
git merge main
# Resuelve conflictos si hay
git push
```

---

### "At least 1 approving review is required"

**Causa:** Falta aprobación de revisión

**Solución:**
1. Pide a un compañero que revise tu PR
2. Espera la aprobación
3. Una vez aprobado, podrás hacer merge

---

## ✅ Checklist Pre-Push

Antes de hacer push, verifica:

- [ ] ✅ Pruebas unitarias pasan localmente
- [ ] ✅ El código compila sin errores
- [ ] ✅ No hay archivos innecesarios en el commit
- [ ] ✅ El mensaje de commit es descriptivo
- [ ] ✅ Actualicé la documentación si es necesario
- [ ] ✅ Agregué pruebas para el nuevo código

---

## 🎓 Comandos Útiles

```bash
# Ver estado de tu repositorio
git status

# Ver historial de commits
git log --oneline

# Ver qué cambió en un archivo
git diff <archivo>

# Descartar cambios no commiteados
git checkout -- <archivo>

# Ver ramas
git branch

# Cambiar de rama
git checkout <nombre-rama>

# Actualizar main
git checkout main && git pull origin main

# Ver diferencia entre ramas
git diff main..feature/mi-rama
```

---

## 📞 ¿Necesitas Ayuda?

1. **Revisa la documentación completa**: `CONFIGURACION_GITHUB.md`
2. **Ve los logs de GitHub Actions**: En tu PR → Details
3. **Pregunta al equipo**: Crea un comentario en tu PR

---

**Pro Tip:** Ejecuta `dotnet test` localmente antes de cada push. Te ahorrará tiempo y evitará fallos en CI/CD.

