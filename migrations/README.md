# 🗄️ Carnicería CRM - Scripts de Base de Datos

Este directorio contiene todos los scripts necesarios para configurar completamente la base de datos del sistema CRM para carnicería.

## 📁 Archivos de Instalación

### 1. **Setup_Database.sql**

Script principal que crea toda la estructura de la base de datos:

- ✅ Base de datos `CarniceriaCRM`
- ✅ 15 tablas del sistema con todas sus relaciones
- ✅ Sistema de permisos con patrón Composite
- ✅ 13 índices para optimización de consultas
- ✅ Constraints y validaciones

**Ejecutar PRIMERO**

### 2. **Setup_InitialData.sql**

Script de carga de datos iniciales y configuración:

- ✅ 32 permisos (patentes) completos del sistema
- ✅ 3 roles predefinidos con permisos asignados
- ✅ 3 usuarios de prueba configurados
- ✅ Datos de ejemplo del negocio (categorías, proveedores, productos, clientes)

**Ejecutar SEGUNDO (después de Setup_Database.sql)**

### 3. **DER_CarniceriaCRM.md**

Documentación completa del diseño de la base de datos:

- 📊 Diagrama Entidad-Relación en formato Mermaid
- 📝 Descripción detallada de cada tabla
- 🔗 Explicación de las relaciones
- 🎭 Documentación del sistema de permisos

## 🚀 Instalación Rápida

```sql
-- 1. Crear estructura de base de datos
EXEC xp_cmdshell 'sqlcmd -S tu_servidor -d master -i Setup_Database.sql'

-- 2. Cargar datos iniciales
EXEC xp_cmdshell 'sqlcmd -S tu_servidor -d CarniceriaCRM -i Setup_InitialData.sql'
```

### Opción Manual

1. **Abrir SQL Server Management Studio**
2. **Ejecutar `Setup_Database.sql`** en el servidor
3. **Ejecutar `Setup_InitialData.sql`** en la base de datos `CarniceriaCRM`

## 🎭 Sistema de Permisos

### Roles Configurados

#### 🔴 **WebMaster** (Administrador Completo)

- **Permisos**: Todos los 32 permisos del sistema
- **Descripción**: Acceso total para administración
- **Usuario de prueba**: `admin@carniceria.com`

#### 🟡 **Carnicero** (Personal Operativo)

- **Permisos**: 22 permisos operativos
- **Funciones**: Gestión de productos, clientes, pedidos, ventas
- **Usuario de prueba**: `vendedor@carniceria.com`

#### 🟢 **Cliente** (Acceso Limitado)

- **Permisos**: 3 permisos de consulta básica
- **Funciones**: Ver productos, categorías y sus propios pedidos
- **Usuario de prueba**: `cliente@carniceria.com`

### Permisos Disponibles (32 Patentes)

| Categoría               | Permisos                                                                            | Descripción                            |
| ----------------------- | ----------------------------------------------------------------------------------- | -------------------------------------- |
| **Gestión de Usuarios** | GestionarUsuarios, BloquearDesbloquearUsuarios                                      | Administración de usuarios del sistema |
| **Productos**           | VerProductos, CrearProductos, ModificarProductos, EliminarProductos, GestionarStock | Gestión completa del inventario        |
| **Clientes**            | VerClientes, CrearClientes, ModificarClientes, EliminarClientes, GestionarCredito   | Administración de clientes             |
| **Pedidos**             | VerPedidos, CrearPedidos, ModificarPedidos, CancelarPedidos, ProcesarPedidos        | Gestión de pedidos                     |
| **Ventas y Pagos**      | RegistrarVentas, ProcesarPagos, VerHistorialVentas, GenerarFacturas                 | Operaciones comerciales                |
| **Proveedores**         | VerProveedores, CrearProveedores, ModificarProveedores, EliminarProveedores         | Gestión de proveedores                 |
| **Reportes**            | VerReportes, GenerarReportes, VerEstadisticas, ExportarDatos                        | Análisis y reportería                  |
| **Sistema**             | ConfigurarSistema, GestionarPermisos, VerBitacora, MantenimientoBD                  | Administración del sistema             |
| **Categorías**          | VerCategorias, CrearCategorias, ModificarCategorias, EliminarCategorias             | Gestión de categorías                  |

## 👥 Usuarios de Prueba

Todos los usuarios tienen la contraseña: **`admin123`**

| Email                     | Nombre                | Rol       | Permisos                |
| ------------------------- | --------------------- | --------- | ----------------------- |
| `admin@carniceria.com`    | Administrador Sistema | WebMaster | 32 permisos (completo)  |
| `vendedor@carniceria.com` | Juan Pérez            | Carnicero | 22 permisos (operativo) |
| `cliente@carniceria.com`  | María Cliente         | Cliente   | 3 permisos (consulta)   |

## 📊 Estructura de Tablas

### Tablas Principales

1. **Usuarios** - Gestión de usuarios del sistema
2. **Patentes** - Permisos individuales (32 patentes)
3. **Familias** - Roles que agrupan permisos
4. **FamiliaPatente** - Asignación de permisos a roles
5. **FamiliaFamilia** - Jerarquía de roles (Composite Pattern)
6. **UsuarioFamilia** - Asignación de roles a usuarios
7. **Bitacora** - Auditoría de acciones del sistema
8. **Clientes** - Información de clientes
9. **Categorias** - Categorías de productos
10. **Proveedores** - Información de proveedores
11. **Productos** - Catálogo de productos
12. **Pedidos** - Pedidos realizados
13. **DetallePedidos** - Detalle de items por pedido
14. **MovimientosStock** - Historial de movimientos de inventario
15. **Pagos** - Registro de pagos realizados

### Datos de Ejemplo Incluidos

- **6 Categorías**: Carnes Rojas, Carnes Blancas, Embutidos, Mariscos, Congelados, Condimentos
- **4 Proveedores**: Frigorífico La Pampa, Avícola San Fernando, Embutidos El Criollo, Pesquera del Sur
- **3 Productos**: Bife de Chorizo, Asado de Tira, Pollo Entero
- **2 Clientes**: María Rodríguez, Carlos González

## 🔧 Configuración de Conexión

### Cadena de Conexión Recomendada (SQL Server Express)

```xml
<connectionStrings>
  <add name="CarniceriaCRM"
       connectionString="Server=.\SQLEXPRESS;Database=CarniceriaCRM;Integrated Security=true;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### Para SQL Server Completo

```xml
<connectionStrings>
  <add name="CarniceriaCRM"
       connectionString="Data Source=tu_servidor;Initial Catalog=CarniceriaCRM;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### Para SQL Server Express LocalDB

```xml
<connectionStrings>
  <add name="CarniceriaCRM"
       connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\CarniceriaCRM.mdf;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

## 🧪 Verificación de Instalación

### Consultas de Verificación

```sql
-- Verificar tablas creadas
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

-- Verificar usuarios y roles
SELECT u.Nombre, u.Mail, f.Nombre as Rol
FROM Usuarios u
JOIN UsuarioFamilia uf ON u.Id = uf.UsuarioId
JOIN Familias f ON uf.FamiliaId = f.Id;

-- Verificar permisos por rol
SELECT f.Nombre as Rol, COUNT(fp.PatenteId) as CantidadPermisos
FROM Familias f
LEFT JOIN FamiliaPatente fp ON f.Id = fp.FamiliaId
GROUP BY f.Id, f.Nombre;

-- Verificar datos de ejemplo
SELECT 'Categorias' as Tabla, COUNT(*) as Registros FROM Categorias
UNION ALL
SELECT 'Proveedores', COUNT(*) FROM Proveedores
UNION ALL
SELECT 'Productos', COUNT(*) FROM Productos
UNION ALL
SELECT 'Clientes', COUNT(*) FROM Clientes;
```

## 📝 Notas Importantes

- ⚠️ **Requisitos**: SQL Server 2016 o superior
- 🔒 **Seguridad**: Cambiar contraseñas de usuarios de prueba en producción
- 🗂️ **Orden de ejecución**: Primero `Setup_Database.sql`, luego `Setup_InitialData.sql`
- 📊 **Auditoría**: Todos los logins se registran automáticamente en la tabla Bitacora
- 🎯 **Compatibilidad**: Diseñado para integrar con aplicación .NET Web Forms existente

## 🆘 Solución de Problemas

### Error: "Database already exists"

- El script detecta si la base de datos ya existe y no la recrea
- Para recrear completamente: eliminar la base de datos manualmente primero

### Error: "Cannot insert duplicate key"

- Los scripts verifican existencia antes de insertar
- Safe para ejecutar múltiples veces

### Error de permisos

- Ejecutar como administrador del SQL Server
- Verificar permisos de creación de base de datos

---

**🎉 ¡El sistema está listo para usar!**

Para comenzar a trabajar con la aplicación, utilice cualquiera de los usuarios de prueba y la contraseña `admin123`.
