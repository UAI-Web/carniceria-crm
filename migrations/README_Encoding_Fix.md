# Corrección de Codificación UTF-8

## Problema Identificado

Las palabras con acentos en la base de datos se ven mal codificadas:

- "Administración" aparece como "AdministraciÃ³n"
- "Gestión" aparece como "GestiÃ³n"
- "información" aparece como "informaciÃ³n"

## Solución Implementada

### 1. Configuración Web.config

Se agregó la configuración de globalización para UTF-8:

```xml
<globalization
    requestEncoding="utf-8"
    responseEncoding="utf-8"
    fileEncoding="utf-8"
    culture="es-ES"
    uiCulture="es-ES" />
```

### 2. Script de Corrección de Base de Datos

**Archivo:** `Fix_Encoding.sql`

Este script corrige automáticamente:

- ✅ Tabla `Patentes` (nombres y descripciones)
- ✅ Tabla `Familias` (nombres y descripciones)
- ✅ Tabla `Usuarios` (nombres y apellidos)
- ✅ Tabla `Bitacora` (acciones y descripciones)

### 3. Caracteres Corregidos

El script reemplaza:

- `Ã¡` → `á`
- `Ã©` → `é`
- `Ã­` → `í`
- `Ã³` → `ó`
- `Ãº` → `ú`
- `Ã±` → `ñ`
- `â€` → `"`

## Cómo Ejecutar la Corrección

### Opción 1: SQL Server Management Studio (SSMS)

1. Abre SQL Server Management Studio
2. Conéctate a tu instancia `.\SQLEXPRESS`
3. Abre el archivo `Fix_Encoding.sql`
4. Ejecuta el script completo (F5)

### Opción 2: Línea de comandos

```bash
sqlcmd -S .\SQLEXPRESS -d CarniceriaCRM -i Fix_Encoding.sql
```

### Opción 3: Visual Studio

1. Abre Visual Studio
2. Ve a View → SQL Server Object Explorer
3. Conéctate a `(localdb)\MSSQLLocalDB` o `.\SQLEXPRESS`
4. Haz clic derecho en la base de datos → New Query
5. Copia y pega el contenido de `Fix_Encoding.sql`
6. Ejecuta el script

## Verificación

Después de ejecutar el script, deberías ver:

- ✅ Palabras con acentos correctamente mostradas
- ✅ Mensaje de confirmación en el output del SQL
- ✅ Contadores de registros corregidos

## Prevención Futura

Para evitar que este problema vuelva a ocurrir:

1. **Asegúrate de que todos los archivos .sql se guarden en UTF-8**
2. **Verifica la collation de la base de datos:** `SQL_Latin1_General_CP1_CI_AS` o `Modern_Spanish_CI_AS`
3. **Usa la configuración correcta en Web.config** (ya implementada)

## Páginas Afectadas

Las siguientes páginas mostrarán los textos corregidos:

- 🔧 Dashboard.aspx (roles y permisos)
- 🔧 Bitacora.aspx (acciones y descripciones)
- 🔧 Cualquier página que muestre datos de base de datos

## Soporte

Si después de ejecutar el script sigues viendo caracteres mal codificados:

1. Verifica que el script se ejecutó sin errores
2. Revisa los contadores finales (deberían ser 0)
3. Comprueba la collation de la base de datos
4. Reinicia IIS o el servidor web

---

**Nota:** Este script es seguro de ejecutar múltiples veces. Solo actualiza registros que contengan caracteres mal codificados.
