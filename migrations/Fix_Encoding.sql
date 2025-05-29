-- CARNICERÍA CRM - CORRECCIÓN DE CODIFICACIÓN UTF-8
-- Descripción: Script para corregir palabras mal codificadas en la base de datos
-- Autor: Sistema CRM
-- Fecha: 2025

PRINT '';
PRINT '=================================================';
PRINT 'CARNICERÍA CRM - CORRECCIÓN DE CODIFICACIÓN';
PRINT '=================================================';
PRINT '';

-- Configurar la base de datos para UTF-8
USE [CarniceriaCRM];
GO

PRINT '1. Corrigiendo codificación en tabla Patentes...';

-- Corregir descripciones en tabla Patentes que contengan caracteres mal codificados
UPDATE [dbo].[Patentes] 
SET [Descripcion] = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    [Descripcion], 
    'Ã¡', 'á'),
    'Ã©', 'é'),
    'Ã­', 'í'),
    'Ã³', 'ó'),
    'Ãº', 'ú')
WHERE [Descripcion] LIKE '%Ã%';

-- Corregir nombres en tabla Patentes
UPDATE [dbo].[Patentes] 
SET [Nombre] = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    [Nombre], 
    'Ã¡', 'á'),
    'Ã©', 'é'),
    'Ã­', 'í'),
    'Ã³', 'ó'),
    'Ãº', 'ú')
WHERE [Nombre] LIKE '%Ã%';

PRINT '✓ Tabla Patentes corregida.';

PRINT '2. Corrigiendo codificación en tabla Familias...';

-- Corregir descripciones en tabla Familias
UPDATE [dbo].[Familias] 
SET [Descripcion] = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    [Descripcion], 
    'Ã¡', 'á'),
    'Ã©', 'é'),
    'Ã­', 'í'),
    'Ã³', 'ó'),
    'Ãº', 'ú')
WHERE [Descripcion] LIKE '%Ã%';

-- Corregir nombres en tabla Familias
UPDATE [dbo].[Familias] 
SET [Nombre] = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    [Nombre], 
    'Ã¡', 'á'),
    'Ã©', 'é'),
    'Ã­', 'í'),
    'Ã³', 'ó'),
    'Ãº', 'ú')
WHERE [Nombre] LIKE '%Ã%';

PRINT '✓ Tabla Familias corregida.';

PRINT '3. Corrigiendo codificación en tabla Usuarios...';

-- Corregir nombres y apellidos en tabla Usuarios
UPDATE [dbo].[Usuarios] 
SET [Nombre] = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    [Nombre], 
    'Ã¡', 'á'),
    'Ã©', 'é'),
    'Ã­', 'í'),
    'Ã³', 'ó'),
    'Ãº', 'ú')
WHERE [Nombre] LIKE '%Ã%';

UPDATE [dbo].[Usuarios] 
SET [Apellido] = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    [Apellido], 
    'Ã¡', 'á'),
    'Ã©', 'é'),
    'Ã­', 'í'),
    'Ã³', 'ó'),
    'Ãº', 'ú')
WHERE [Apellido] LIKE '%Ã%';

PRINT '✓ Tabla Usuarios corregida.';

PRINT '4. Corrigiendo codificación en tabla Bitacora...';

-- Corregir acciones y descripciones en tabla Bitacora
UPDATE [dbo].[Bitacora] 
SET [Accion] = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    [Accion], 
    'Ã¡', 'á'),
    'Ã©', 'é'),
    'Ã­', 'í'),
    'Ã³', 'ó'),
    'Ãº', 'ú')
WHERE [Accion] LIKE '%Ã%';

UPDATE [dbo].[Bitacora] 
SET [Descripcion] = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    [Descripcion], 
    'Ã¡', 'á'),
    'Ã©', 'é'),
    'Ã­', 'í'),
    'Ã³', 'ó'),
    'Ãº', 'ú')
WHERE [Descripcion] LIKE '%Ã%';

PRINT '✓ Tabla Bitacora corregida.';

-- Corregir otras codificaciones comunes
PRINT '5. Aplicando correcciones adicionales...';

-- Caracteres especiales adicionales
UPDATE [dbo].[Patentes] 
SET [Descripcion] = REPLACE(REPLACE(REPLACE(
    [Descripcion], 
    'Ã±', 'ñ'),
    'Ã‚', 'Â'),
    'â€', '"')
WHERE [Descripcion] LIKE '%Ã%' OR [Descripcion] LIKE '%â€%';

UPDATE [dbo].[Familias] 
SET [Descripcion] = REPLACE(REPLACE(REPLACE(
    [Descripcion], 
    'Ã±', 'ñ'),
    'Ã‚', 'Â'), 
    'â€', '"')
WHERE [Descripcion] LIKE '%Ã%' OR [Descripcion] LIKE '%â€%';

UPDATE [dbo].[Bitacora] 
SET [Descripcion] = REPLACE(REPLACE(REPLACE(
    [Descripcion], 
    'Ã±', 'ñ'),
    'Ã‚', 'Â'),
    'â€', '"')
WHERE [Descripcion] LIKE '%Ã%' OR [Descripcion] LIKE '%â€%';

UPDATE [dbo].[Bitacora] 
SET [Accion] = REPLACE(REPLACE(REPLACE(
    [Accion], 
    'Ã±', 'ñ'),
    'Ã‚', 'Â'),
    'â€', '"')
WHERE [Accion] LIKE '%Ã%' OR [Accion] LIKE '%â€%';

PRINT '✓ Correcciones adicionales aplicadas.';

-- Mostrar estadísticas
PRINT '';
PRINT '6. Verificando resultados...';

DECLARE @CountPatentes INT = (SELECT COUNT(*) FROM [dbo].[Patentes] WHERE [Descripcion] LIKE '%Ã%' OR [Nombre] LIKE '%Ã%');
DECLARE @CountFamilias INT = (SELECT COUNT(*) FROM [dbo].[Familias] WHERE [Descripcion] LIKE '%Ã%' OR [Nombre] LIKE '%Ã%');
DECLARE @CountUsuarios INT = (SELECT COUNT(*) FROM [dbo].[Usuarios] WHERE [Nombre] LIKE '%Ã%' OR [Apellido] LIKE '%Ã%');
DECLARE @CountBitacora INT = (SELECT COUNT(*) FROM [dbo].[Bitacora] WHERE [Accion] LIKE '%Ã%' OR [Descripcion] LIKE '%Ã%');

PRINT '- Patentes con caracteres mal codificados restantes: ' + CAST(@CountPatentes AS VARCHAR(10));
PRINT '- Familias con caracteres mal codificados restantes: ' + CAST(@CountFamilias AS VARCHAR(10));
PRINT '- Usuarios con caracteres mal codificados restantes: ' + CAST(@CountUsuarios AS VARCHAR(10));
PRINT '- Bitacora con caracteres mal codificados restantes: ' + CAST(@CountBitacora AS VARCHAR(10));

PRINT '';
PRINT '=================================================';
PRINT '✅ CORRECCIÓN DE CODIFICACIÓN COMPLETADA';
PRINT '=================================================';
PRINT '';
PRINT 'NOTA: Si aún aparecen caracteres mal codificados, es posible que necesite';
PRINT 'verificar la configuración de collation de la base de datos.';
PRINT '';

-- Mostrar algunas muestras para verificación
PRINT 'Muestras de datos corregidos:';
SELECT TOP 5 Nombre, Descripcion FROM [dbo].[Patentes] WHERE Descripcion LIKE '%ción%' OR Nombre LIKE '%ión%';
SELECT TOP 3 Nombre, Descripcion FROM [dbo].[Familias];

PRINT '';
PRINT 'Script ejecutado exitosamente.';
GO 