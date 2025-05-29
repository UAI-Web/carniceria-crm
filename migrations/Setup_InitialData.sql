-- ===============================================
-- CARNICERÍA CRM - DATOS INICIALES Y CONFIGURACIÓN
-- Descripción: Inserción de datos iniciales, permisos completos y configuración de roles
-- Nota: Ejecutar DESPUÉS de Setup_Database.sql
-- Fecha: 2024
-- ===============================================

USE [CarniceriaCRM];
GO

PRINT '=======================================================';
PRINT 'CARNICERÍA CRM - CARGANDO DATOS INICIALES';
PRINT '=======================================================';
PRINT '';
PRINT 'Este script insertará:';
PRINT '- Todos los permisos (patentes) del sistema';
PRINT '- Roles predefinidos (WebMaster, Carnicero, Cliente)';
PRINT '- Usuarios de prueba con roles asignados';
PRINT '- Datos de ejemplo para el negocio';
PRINT '';
PRINT 'Iniciando carga de datos...';
PRINT '';

-- ===============================================
-- 1. INSERTAR USUARIOS DE PRUEBA
-- ===============================================
PRINT '1. Creando usuarios de prueba...';

-- Usuario Administrador
IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Mail] = 'admin@carniceria.com')
BEGIN
    INSERT INTO [dbo].[Usuarios] 
    ([Nombre], [Apellido], [Mail], [Clave], [IntentosFallidos], [Bloqueado], [Activo])
    VALUES 
    ('Administrador', 'Sistema', 'admin@carniceria.com', 
     '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', -- admin123 en SHA256
     0, 0, 1);
    PRINT '✓ Usuario administrador creado exitosamente.';
END

-- Usuario Carnicero
IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Mail] = 'vendedor@carniceria.com')
BEGIN
    INSERT INTO [dbo].[Usuarios] 
    ([Nombre], [Apellido], [Mail], [Clave], [IntentosFallidos], [Bloqueado], [Activo])
    VALUES 
    ('Juan', 'Pérez', 'vendedor@carniceria.com', 
     '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', -- admin123 en SHA256
     0, 0, 1);
    PRINT '✓ Usuario vendedor creado exitosamente.';
END

-- Usuario Cliente
IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Mail] = 'cliente@carniceria.com')
BEGIN
    INSERT INTO [dbo].[Usuarios] 
    ([Nombre], [Apellido], [Mail], [Clave], [IntentosFallidos], [Bloqueado], [Activo])
    VALUES 
    ('María', 'Cliente', 'cliente@carniceria.com', 
     '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', -- admin123 en SHA256
     0, 0, 1);
    PRINT '✓ Usuario cliente creado exitosamente.';
END

-- ===============================================
-- 2. INSERTAR TODAS LAS PATENTES (PERMISOS)
-- ===============================================
PRINT '';
PRINT '2. Insertando todos los permisos del sistema...';

-- Gestión de Usuarios (1000-1999)
INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Gestionar Usuarios', 'Crear, modificar y eliminar usuarios del sistema', 'GestionarUsuarios', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'GestionarUsuarios');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Bloquear/Desbloquear Usuarios', 'Bloquear y desbloquear usuarios', 'BloquearDesbloquearUsuarios', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'BloquearDesbloquearUsuarios');

-- Productos (2000-2999)
INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Ver Productos', 'Consultar catálogo de productos', 'VerProductos', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'VerProductos');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Crear Productos', 'Agregar nuevos productos al sistema', 'CrearProductos', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'CrearProductos');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Modificar Productos', 'Editar información de productos existentes', 'ModificarProductos', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'ModificarProductos');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Eliminar Productos', 'Dar de baja productos del sistema', 'EliminarProductos', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'EliminarProductos');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Gestionar Stock', 'Administrar inventario y movimientos de stock', 'GestionarStock', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'GestionarStock');

-- Clientes (3000-3999)
INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Ver Clientes', 'Consultar información de clientes', 'VerClientes', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'VerClientes');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Crear Clientes', 'Registrar nuevos clientes', 'CrearClientes', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'CrearClientes');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Modificar Clientes', 'Editar información de clientes', 'ModificarClientes', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'ModificarClientes');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Eliminar Clientes', 'Dar de baja clientes', 'EliminarClientes', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'EliminarClientes');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Gestionar Crédito', 'Administrar límites y saldos de crédito', 'GestionarCredito', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'GestionarCredito');

-- Pedidos (4000-4999)
INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Ver Pedidos', 'Consultar pedidos del sistema', 'VerPedidos', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'VerPedidos');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Crear Pedidos', 'Generar nuevos pedidos', 'CrearPedidos', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'CrearPedidos');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Modificar Pedidos', 'Editar pedidos existentes', 'ModificarPedidos', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'ModificarPedidos');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Cancelar Pedidos', 'Cancelar pedidos en proceso', 'CancelarPedidos', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'CancelarPedidos');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Procesar Pedidos', 'Gestionar el estado de los pedidos', 'ProcesarPedidos', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'ProcesarPedidos');

-- Ventas y Pagos (5000-5999)
INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Registrar Ventas', 'Registrar ventas realizadas', 'RegistrarVentas', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'RegistrarVentas');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Procesar Pagos', 'Gestionar pagos de clientes', 'ProcesarPagos', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'ProcesarPagos');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Ver Historial Ventas', 'Consultar historial de ventas', 'VerHistorialVentas', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'VerHistorialVentas');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Generar Facturas', 'Crear facturas y comprobantes', 'GenerarFacturas', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'GenerarFacturas');

-- Proveedores (6000-6999)
INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Ver Proveedores', 'Consultar información de proveedores', 'VerProveedores', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'VerProveedores');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Crear Proveedores', 'Registrar nuevos proveedores', 'CrearProveedores', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'CrearProveedores');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Modificar Proveedores', 'Editar información de proveedores', 'ModificarProveedores', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'ModificarProveedores');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Eliminar Proveedores', 'Dar de baja proveedores', 'EliminarProveedores', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'EliminarProveedores');

-- Reportes y Estadísticas (7000-7999)
INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Ver Reportes', 'Consultar reportes del sistema', 'VerReportes', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'VerReportes');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Generar Reportes', 'Crear reportes personalizados', 'GenerarReportes', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'GenerarReportes');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Ver Estadísticas', 'Consultar estadísticas del negocio', 'VerEstadisticas', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'VerEstadisticas');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Exportar Datos', 'Exportar información a archivos', 'ExportarDatos', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'ExportarDatos');

-- Configuración del Sistema (8000-8999)
INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Configurar Sistema', 'Administrar configuración general', 'ConfigurarSistema', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'ConfigurarSistema');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Gestionar Permisos', 'Administrar roles y permisos', 'GestionarPermisos', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'GestionarPermisos');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Ver Bitácora', 'Consultar auditoría del sistema', 'VerBitacora', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'VerBitacora');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Mantenimiento BD', 'Realizar mantenimiento de base de datos', 'MantenimientoBD', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'MantenimientoBD');

-- Categorías (9000-9999)
INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Ver Categorías', 'Consultar categorías de productos', 'VerCategorias', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'VerCategorias');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Crear Categorías', 'Agregar nuevas categorías', 'CrearCategorias', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'CrearCategorias');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Modificar Categorías', 'Editar categorías existentes', 'ModificarCategorias', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'ModificarCategorias');

INSERT INTO [dbo].[Patentes] ([Nombre], [Descripcion], [Permiso], [Activo]) 
SELECT 'Eliminar Categorías', 'Dar de baja categorías', 'EliminarCategorias', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Patentes] WHERE [Permiso] = 'EliminarCategorias');

PRINT '✓ Todos los permisos (32 patentes) insertados exitosamente.';

-- ===============================================
-- 3. INSERTAR FAMILIAS (ROLES)
-- ===============================================
PRINT '';
PRINT '3. Creando roles del sistema...';

-- Rol WebMaster
INSERT INTO [dbo].[Familias] ([Nombre], [Descripcion], [Activo]) 
SELECT 'WebMaster', 'Administrador completo del sistema con todos los permisos', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Familias] WHERE [Nombre] = 'WebMaster');

-- Rol Carnicero
INSERT INTO [dbo].[Familias] ([Nombre], [Descripcion], [Activo]) 
SELECT 'Carnicero', 'Personal operativo con permisos para gestionar productos, clientes y ventas', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Familias] WHERE [Nombre] = 'Carnicero');

-- Rol Cliente
INSERT INTO [dbo].[Familias] ([Nombre], [Descripcion], [Activo]) 
SELECT 'Cliente', 'Acceso limitado de consulta para clientes del negocio', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Familias] WHERE [Nombre] = 'Cliente');

PRINT '✓ Roles creados exitosamente.';

-- ===============================================
-- 4. ASIGNAR PATENTES A FAMILIAS
-- ===============================================
PRINT '';
PRINT '4. Configurando permisos por rol...';

-- Variables para IDs de familias
DECLARE @WebMasterId INT = (SELECT Id FROM [dbo].[Familias] WHERE [Nombre] = 'WebMaster');
DECLARE @CarniceroId INT = (SELECT Id FROM [dbo].[Familias] WHERE [Nombre] = 'Carnicero');
DECLARE @ClienteId INT = (SELECT Id FROM [dbo].[Familias] WHERE [Nombre] = 'Cliente');

-- WebMaster: TODOS LOS PERMISOS
INSERT INTO [dbo].[FamiliaPatente] ([FamiliaId], [PatenteId])
SELECT @WebMasterId, Id FROM [dbo].[Patentes] 
WHERE [Activo] = 1 
AND NOT EXISTS (SELECT 1 FROM [dbo].[FamiliaPatente] WHERE [FamiliaId] = @WebMasterId AND [PatenteId] = [Patentes].[Id]);

-- Carnicero: Permisos operativos (sin gestión de usuarios ni configuración)
INSERT INTO [dbo].[FamiliaPatente] ([FamiliaId], [PatenteId])
SELECT @CarniceroId, Id FROM [dbo].[Patentes] 
WHERE [Permiso] IN (
    'VerProductos', 'CrearProductos', 'ModificarProductos', 'EliminarProductos', 'GestionarStock',
    'VerClientes', 'CrearClientes', 'ModificarClientes', 'GestionarCredito',
    'VerPedidos', 'CrearPedidos', 'ModificarPedidos', 'CancelarPedidos', 'ProcesarPedidos',
    'RegistrarVentas', 'ProcesarPagos', 'VerHistorialVentas', 'GenerarFacturas',
    'VerProveedores', 'CrearProveedores', 'ModificarProveedores',
    'VerReportes', 'GenerarReportes', 'VerEstadisticas',
    'VerCategorias', 'CrearCategorias', 'ModificarCategorias'
)
AND NOT EXISTS (SELECT 1 FROM [dbo].[FamiliaPatente] WHERE [FamiliaId] = @CarniceroId AND [PatenteId] = [Patentes].[Id]);

-- Cliente: Solo permisos de consulta básicos
INSERT INTO [dbo].[FamiliaPatente] ([FamiliaId], [PatenteId])
SELECT @ClienteId, Id FROM [dbo].[Patentes] 
WHERE [Permiso] IN (
    'VerProductos',
    'VerCategorias',
    'VerPedidos'  -- Solo sus propios pedidos (se controla en lógica de negocio)
)
AND NOT EXISTS (SELECT 1 FROM [dbo].[FamiliaPatente] WHERE [FamiliaId] = @ClienteId AND [PatenteId] = [Patentes].[Id]);

PRINT '✓ Permisos asignados a roles exitosamente.';

-- ===============================================
-- 5. ASIGNAR ROLES A USUARIOS
-- ===============================================
PRINT '';
PRINT '5. Asignando roles a usuarios...';

-- Variables para IDs de usuarios
DECLARE @AdminId INT = (SELECT Id FROM [dbo].[Usuarios] WHERE [Mail] = 'admin@carniceria.com');
DECLARE @VendedorId INT = (SELECT Id FROM [dbo].[Usuarios] WHERE [Mail] = 'vendedor@carniceria.com');
DECLARE @ClienteUserId INT = (SELECT Id FROM [dbo].[Usuarios] WHERE [Mail] = 'cliente@carniceria.com');

-- Admin como WebMaster
INSERT INTO [dbo].[UsuarioFamilia] ([UsuarioId], [FamiliaId], [Activo])
SELECT @AdminId, @WebMasterId, 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[UsuarioFamilia] WHERE [UsuarioId] = @AdminId AND [FamiliaId] = @WebMasterId);

-- Vendedor como Carnicero
INSERT INTO [dbo].[UsuarioFamilia] ([UsuarioId], [FamiliaId], [Activo])
SELECT @VendedorId, @CarniceroId, 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[UsuarioFamilia] WHERE [UsuarioId] = @VendedorId AND [FamiliaId] = @CarniceroId);

-- Cliente como Cliente
INSERT INTO [dbo].[UsuarioFamilia] ([UsuarioId], [FamiliaId], [Activo])
SELECT @ClienteUserId, @ClienteId, 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[UsuarioFamilia] WHERE [UsuarioId] = @ClienteUserId AND [FamiliaId] = @ClienteId);

PRINT '✓ Roles asignados a usuarios exitosamente.';

-- ===============================================
-- 6. INSERTAR DATOS DE EJEMPLO DEL NEGOCIO
-- ===============================================
PRINT '';
PRINT '6. Insertando datos de ejemplo del negocio...';

-- Categorías iniciales
INSERT INTO [dbo].[Categorias] ([Nombre], [Descripcion], [Activo])
SELECT 'Carnes Rojas', 'Cortes de res, ternera y cordero', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Categorias] WHERE [Nombre] = 'Carnes Rojas');

INSERT INTO [dbo].[Categorias] ([Nombre], [Descripcion], [Activo])
SELECT 'Carnes Blancas', 'Pollo, pavo y otras aves', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Categorias] WHERE [Nombre] = 'Carnes Blancas');

INSERT INTO [dbo].[Categorias] ([Nombre], [Descripcion], [Activo])
SELECT 'Embutidos', 'Chorizos, morcillas, salchichas', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Categorias] WHERE [Nombre] = 'Embutidos');

INSERT INTO [dbo].[Categorias] ([Nombre], [Descripcion], [Activo])
SELECT 'Mariscos', 'Pescados y frutos del mar', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Categorias] WHERE [Nombre] = 'Mariscos');

INSERT INTO [dbo].[Categorias] ([Nombre], [Descripcion], [Activo])
SELECT 'Congelados', 'Productos congelados diversos', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Categorias] WHERE [Nombre] = 'Congelados');

INSERT INTO [dbo].[Categorias] ([Nombre], [Descripcion], [Activo])
SELECT 'Condimentos', 'Especias y condimentos para carnes', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Categorias] WHERE [Nombre] = 'Condimentos');

-- Proveedores iniciales
INSERT INTO [dbo].[Proveedores] ([NombreEmpresa], [ContactoNombre], [Email], [Telefono], [Direccion], [Ciudad], [CodigoPostal], [CUIT], [Activo])
SELECT 'Frigorífico La Pampa S.A.', 'Carlos Rodriguez', 'ventas@frigopampa.com.ar', '011-4567-8901', 'Av. Industrial 1250', 'Buenos Aires', '1414', '20-12345678-9', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Proveedores] WHERE [NombreEmpresa] = 'Frigorífico La Pampa S.A.');

INSERT INTO [dbo].[Proveedores] ([NombreEmpresa], [ContactoNombre], [Email], [Telefono], [Direccion], [Ciudad], [CodigoPostal], [CUIT], [Activo])
SELECT 'Avícola San Fernando', 'María González', 'pedidos@avicolasf.com.ar', '011-4567-8902', 'Ruta 202 Km 45', 'San Fernando', '1646', '20-23456789-0', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Proveedores] WHERE [NombreEmpresa] = 'Avícola San Fernando');

INSERT INTO [dbo].[Proveedores] ([NombreEmpresa], [ContactoNombre], [Email], [Telefono], [Direccion], [Ciudad], [CodigoPostal], [CUIT], [Activo])
SELECT 'Embutidos El Criollo', 'Roberto Fernández', 'info@elcriollo.com.ar', '011-4567-8903', 'Calle Industria 567', 'Lanús', '1824', '20-34567890-1', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Proveedores] WHERE [NombreEmpresa] = 'Embutidos El Criollo');

INSERT INTO [dbo].[Proveedores] ([NombreEmpresa], [ContactoNombre], [Email], [Telefono], [Direccion], [Ciudad], [CodigoPostal], [CUIT], [Activo])
SELECT 'Pesquera del Sur', 'Ana Martínez', 'ventas@pesquerasur.com.ar', '011-4567-8904', 'Puerto Madero 890', 'CABA', '1107', '20-45678901-2', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Proveedores] WHERE [NombreEmpresa] = 'Pesquera del Sur');

-- Productos de ejemplo (solo si no existen)
DECLARE @CategoriaCarnesRojas INT = (SELECT Id FROM [dbo].[Categorias] WHERE [Nombre] = 'Carnes Rojas');
DECLARE @CategoriaCarnesBlancas INT = (SELECT Id FROM [dbo].[Categorias] WHERE [Nombre] = 'Carnes Blancas');
DECLARE @CategoriaEmbutidos INT = (SELECT Id FROM [dbo].[Categorias] WHERE [Nombre] = 'Embutidos');
DECLARE @CategoriaMariscos INT = (SELECT Id FROM [dbo].[Categorias] WHERE [Nombre] = 'Mariscos');

DECLARE @ProveedorFrigo INT = (SELECT Id FROM [dbo].[Proveedores] WHERE [NombreEmpresa] = 'Frigorífico La Pampa S.A.');
DECLARE @ProveedorAvicola INT = (SELECT Id FROM [dbo].[Proveedores] WHERE [NombreEmpresa] = 'Avícola San Fernando');
DECLARE @ProveedorEmbutidos INT = (SELECT Id FROM [dbo].[Proveedores] WHERE [NombreEmpresa] = 'Embutidos El Criollo');
DECLARE @ProveedorPesquera INT = (SELECT Id FROM [dbo].[Proveedores] WHERE [NombreEmpresa] = 'Pesquera del Sur');

-- Productos de carnes rojas
INSERT INTO [dbo].[Productos] ([Nombre], [Descripcion], [CategoriaId], [ProveedorId], [PrecioCompra], [PrecioVenta], [StockMinimo], [StockActual], [UnidadMedida], [Activo])
SELECT 'Bife de Chorizo', 'Corte premium de res', @CategoriaCarnesRojas, @ProveedorFrigo, 2500.00, 3500.00, 10, 25, 'Kg', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Productos] WHERE [Nombre] = 'Bife de Chorizo');

INSERT INTO [dbo].[Productos] ([Nombre], [Descripcion], [CategoriaId], [ProveedorId], [PrecioCompra], [PrecioVenta], [StockMinimo], [StockActual], [UnidadMedida], [Activo])
SELECT 'Asado de Tira', 'Corte tradicional para asado', @CategoriaCarnesRojas, @ProveedorFrigo, 1800.00, 2600.00, 15, 30, 'Kg', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Productos] WHERE [Nombre] = 'Asado de Tira');

-- Productos de carnes blancas
INSERT INTO [dbo].[Productos] ([Nombre], [Descripcion], [CategoriaId], [ProveedorId], [PrecioCompra], [PrecioVenta], [StockMinimo], [StockActual], [UnidadMedida], [Activo])
SELECT 'Pollo Entero', 'Pollo fresco entero', @CategoriaCarnesBlancas, @ProveedorAvicola, 800.00, 1200.00, 20, 40, 'Unidad', 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Productos] WHERE [Nombre] = 'Pollo Entero');

-- Clientes de ejemplo
INSERT INTO [dbo].[Clientes] ([Nombre], [Apellido], [Email], [Telefono], [Direccion], [Ciudad], [CodigoPostal], [LimiteCredito], [SaldoActual], [Activo])
SELECT 'María', 'Rodríguez', 'maria.rodriguez@email.com', '11-2345-6789', 'Av. Corrientes 1234', 'CABA', '1043', 10000.00, 0.00, 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Clientes] WHERE [Email] = 'maria.rodriguez@email.com');

INSERT INTO [dbo].[Clientes] ([Nombre], [Apellido], [Email], [Telefono], [Direccion], [Ciudad], [CodigoPostal], [LimiteCredito], [SaldoActual], [Activo])
SELECT 'Carlos', 'González', 'carlos.gonzalez@email.com', '11-3456-7890', 'San Martín 567', 'Quilmes', '1878', 15000.00, 2500.00, 1
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Clientes] WHERE [Email] = 'carlos.gonzalez@email.com');

PRINT '✓ Datos de ejemplo del negocio insertados exitosamente.';

-- ===============================================
-- FINALIZACIÓN
-- ===============================================
PRINT '';
PRINT '=======================================================';
PRINT 'DATOS INICIALES CARGADOS EXITOSAMENTE';
PRINT '=======================================================';
PRINT '';
PRINT '✅ Sistema de permisos configurado completamente';
PRINT '✅ 32 permisos (patentes) creados';
PRINT '✅ 3 roles configurados con sus permisos correspondientes';
PRINT '✅ 3 usuarios de prueba creados y configurados';
PRINT '✅ Datos de ejemplo del negocio insertados';
PRINT '';
PRINT '👥 USUARIOS DISPONIBLES:';
PRINT '- admin@carniceria.com (WebMaster) - Contraseña: admin123';
PRINT '- vendedor@carniceria.com (Carnicero) - Contraseña: admin123';
PRINT '- cliente@carniceria.com (Cliente) - Contraseña: admin123';
PRINT '';
PRINT '🎭 ROLES CONFIGURADOS:';

-- Mostrar estadísticas de permisos por rol
DECLARE @WebMasterPermisos INT = (SELECT COUNT(*) FROM [dbo].[FamiliaPatente] WHERE [FamiliaId] = @WebMasterId);
DECLARE @CarniceroPermisos INT = (SELECT COUNT(*) FROM [dbo].[FamiliaPatente] WHERE [FamiliaId] = @CarniceroId);
DECLARE @ClientePermisos INT = (SELECT COUNT(*) FROM [dbo].[FamiliaPatente] WHERE [FamiliaId] = @ClienteId);

PRINT '- WebMaster: ' + CAST(@WebMasterPermisos AS NVARCHAR(10)) + ' permisos (acceso completo)';
PRINT '- Carnicero: ' + CAST(@CarniceroPermisos AS NVARCHAR(10)) + ' permisos (operaciones)';
PRINT '- Cliente: ' + CAST(@ClientePermisos AS NVARCHAR(10)) + ' permisos (solo consulta)';
PRINT '';
PRINT '🚀 El sistema está completamente configurado y listo para usar!';
GO 