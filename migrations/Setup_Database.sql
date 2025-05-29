-- ===============================================
-- CARNICERÍA CRM - ESTRUCTURA DE BASE DE DATOS
-- Descripción: Script que crea la estructura completa de la base de datos
-- Incluye: Base de datos, tablas e índices del sistema
-- Nota: Ejecutar Setup_InitialData.sql después para cargar datos
-- Fecha: 2024
-- ===============================================

PRINT '=======================================================';
PRINT 'CARNICERÍA CRM - CREANDO ESTRUCTURA DE BASE DE DATOS';
PRINT '=======================================================';
PRINT '';
PRINT 'Este script creará:';
PRINT '- Base de datos CarniceriaCRM';
PRINT '- Todas las tablas del sistema CRM';
PRINT '- Sistema de permisos con patrón Composite';
PRINT '- Índices para optimización';
PRINT '';
PRINT 'Nota: Ejecute Setup_InitialData.sql después para cargar datos iniciales';
PRINT '';
PRINT 'Iniciando creación de estructura...';
PRINT '';

-- ===============================================
-- 1. CREAR BASE DE DATOS
-- ===============================================
PRINT '1. Creando base de datos...';

-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'CarniceriaCRM')
BEGIN
    CREATE DATABASE [CarniceriaCRM];
    PRINT 'Base de datos CarniceriaCRM creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La base de datos CarniceriaCRM ya existe.';
END
GO

-- Usar la base de datos
USE [CarniceriaCRM];
GO

-- ===============================================
-- 2. CREAR TABLAS
-- ===============================================
PRINT '';
PRINT '2. Creando tablas del sistema...';

-- TABLA: USUARIOS
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Usuarios' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Usuarios] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Nombre] NVARCHAR(50) NOT NULL,
        [Apellido] NVARCHAR(50) NOT NULL,
        [Mail] NVARCHAR(100) NOT NULL,
        [Clave] NVARCHAR(255) NOT NULL,
        [IntentosFallidos] INT NOT NULL DEFAULT(0),
        [Bloqueado] BIT NOT NULL DEFAULT(0),
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        [FechaUltimaModificacion] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        [Activo] BIT NOT NULL DEFAULT(1),
        
        CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UK_Usuarios_Mail] UNIQUE ([Mail])
    );
    PRINT '✓ Tabla Usuarios creada exitosamente.';
END

-- TABLA: PATENTES (Permisos individuales)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Patentes' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Patentes] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Nombre] NVARCHAR(100) NOT NULL,
        [Descripcion] NVARCHAR(500) NULL,
        [Permiso] NVARCHAR(100) NOT NULL,
        [Activo] BIT NOT NULL DEFAULT(1),
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        
        CONSTRAINT [PK_Patentes] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UK_Patentes_Nombre] UNIQUE ([Nombre]),
        CONSTRAINT [UK_Patentes_Permiso] UNIQUE ([Permiso])
    );
    PRINT '✓ Tabla Patentes creada exitosamente.';
END

-- TABLA: FAMILIAS (Roles que agrupan permisos)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Familias' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Familias] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Nombre] NVARCHAR(100) NOT NULL,
        [Descripcion] NVARCHAR(500) NULL,
        [Activo] BIT NOT NULL DEFAULT(1),
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        
        CONSTRAINT [PK_Familias] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UK_Familias_Nombre] UNIQUE ([Nombre])
    );
    PRINT '✓ Tabla Familias creada exitosamente.';
END

-- TABLA: FAMILIA_PATENTE (Relación N:M entre Familias y Patentes)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='FamiliaPatente' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[FamiliaPatente] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [FamiliaId] INT NOT NULL,
        [PatenteId] INT NOT NULL,
        [FechaAsignacion] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        
        CONSTRAINT [PK_FamiliaPatente] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_FamiliaPatente_Familia] FOREIGN KEY ([FamiliaId]) 
            REFERENCES [dbo].[Familias] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FamiliaPatente_Patente] FOREIGN KEY ([PatenteId]) 
            REFERENCES [dbo].[Patentes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [UK_FamiliaPatente] UNIQUE ([FamiliaId], [PatenteId])
    );
    PRINT '✓ Tabla FamiliaPatente creada exitosamente.';
END

-- TABLA: FAMILIA_FAMILIA (Relación jerárquica entre Familias)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='FamiliaFamilia' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[FamiliaFamilia] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [FamiliaPadreId] INT NOT NULL,
        [FamiliaHijaId] INT NOT NULL,
        [FechaAsignacion] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        
        CONSTRAINT [PK_FamiliaFamilia] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_FamiliaFamilia_Padre] FOREIGN KEY ([FamiliaPadreId]) 
            REFERENCES [dbo].[Familias] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FamiliaFamilia_Hija] FOREIGN KEY ([FamiliaHijaId]) 
            REFERENCES [dbo].[Familias] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [UK_FamiliaFamilia] UNIQUE ([FamiliaPadreId], [FamiliaHijaId]),
        CONSTRAINT [CK_FamiliaFamilia_NoAutoRef] CHECK ([FamiliaPadreId] != [FamiliaHijaId])
    );
    PRINT '✓ Tabla FamiliaFamilia creada exitosamente.';
END

-- TABLA: USUARIO_FAMILIA (Asignación de roles a usuarios)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UsuarioFamilia' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[UsuarioFamilia] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UsuarioId] INT NOT NULL,
        [FamiliaId] INT NOT NULL,
        [FechaAsignacion] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        [Activo] BIT NOT NULL DEFAULT(1),
        
        CONSTRAINT [PK_UsuarioFamilia] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_UsuarioFamilia_Usuario] FOREIGN KEY ([UsuarioId]) 
            REFERENCES [dbo].[Usuarios] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UsuarioFamilia_Familia] FOREIGN KEY ([FamiliaId]) 
            REFERENCES [dbo].[Familias] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [UK_UsuarioFamilia] UNIQUE ([UsuarioId], [FamiliaId])
    );
    PRINT '✓ Tabla UsuarioFamilia creada exitosamente.';
END

-- TABLA: BITACORA
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Bitacora' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Bitacora] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UsuarioId] INT NOT NULL,
        [Accion] NVARCHAR(100) NOT NULL,
        [Descripcion] NVARCHAR(500) NULL,
        [FechaHora] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        [DireccionIP] NVARCHAR(45) NULL,
        [UserAgent] NVARCHAR(200) NULL,
        
        CONSTRAINT [PK_Bitacora] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Bitacora_Usuario] FOREIGN KEY ([UsuarioId]) 
            REFERENCES [dbo].[Usuarios] ([Id])
    );
    PRINT '✓ Tabla Bitacora creada exitosamente.';
END

-- TABLA: CLIENTES
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Clientes' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Clientes] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Nombre] NVARCHAR(50) NOT NULL,
        [Apellido] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(100) NULL,
        [Telefono] NVARCHAR(20) NULL,
        [Direccion] NVARCHAR(200) NULL,
        [Ciudad] NVARCHAR(50) NULL,
        [CodigoPostal] NVARCHAR(10) NULL,
        [FechaRegistro] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        [Activo] BIT NOT NULL DEFAULT(1),
        [LimiteCredito] DECIMAL(10,2) NOT NULL DEFAULT(0),
        [SaldoActual] DECIMAL(10,2) NOT NULL DEFAULT(0),
        
        CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT '✓ Tabla Clientes creada exitosamente.';
END

-- TABLA: CATEGORIAS
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categorias' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Categorias] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Nombre] NVARCHAR(100) NOT NULL,
        [Descripcion] NVARCHAR(500) NULL,
        [Activo] BIT NOT NULL DEFAULT(1),
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        
        CONSTRAINT [PK_Categorias] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UK_Categorias_Nombre] UNIQUE ([Nombre])
    );
    PRINT '✓ Tabla Categorias creada exitosamente.';
END

-- TABLA: PROVEEDORES
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Proveedores' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Proveedores] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [NombreEmpresa] NVARCHAR(100) NOT NULL,
        [ContactoNombre] NVARCHAR(50) NULL,
        [Email] NVARCHAR(100) NULL,
        [Telefono] NVARCHAR(20) NULL,
        [Direccion] NVARCHAR(200) NULL,
        [Ciudad] NVARCHAR(50) NULL,
        [CodigoPostal] NVARCHAR(10) NULL,
        [CUIT] NVARCHAR(20) NULL,
        [Activo] BIT NOT NULL DEFAULT(1),
        [FechaRegistro] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        
        CONSTRAINT [PK_Proveedores] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT '✓ Tabla Proveedores creada exitosamente.';
END

-- TABLA: PRODUCTOS
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Productos' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Productos] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Nombre] NVARCHAR(100) NOT NULL,
        [Descripcion] NVARCHAR(500) NULL,
        [CategoriaId] INT NOT NULL,
        [ProveedorId] INT NOT NULL,
        [PrecioCompra] DECIMAL(10,2) NOT NULL DEFAULT(0),
        [PrecioVenta] DECIMAL(10,2) NOT NULL DEFAULT(0),
        [StockMinimo] INT NOT NULL DEFAULT(0),
        [StockActual] INT NOT NULL DEFAULT(0),
        [UnidadMedida] NVARCHAR(50) NOT NULL DEFAULT('Unidad'),
        [Activo] BIT NOT NULL DEFAULT(1),
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        [FechaUltimaModificacion] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        
        CONSTRAINT [PK_Productos] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Productos_Categoria] FOREIGN KEY ([CategoriaId]) 
            REFERENCES [dbo].[Categorias] ([Id]),
        CONSTRAINT [FK_Productos_Proveedor] FOREIGN KEY ([ProveedorId]) 
            REFERENCES [dbo].[Proveedores] ([Id])
    );
    PRINT '✓ Tabla Productos creada exitosamente.';
END

-- TABLA: PEDIDOS
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Pedidos' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Pedidos] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ClienteId] INT NOT NULL,
        [UsuarioId] INT NOT NULL,
        [FechaPedido] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        [FechaEntrega] DATETIME2 NULL,
        [Estado] NVARCHAR(20) NOT NULL DEFAULT('Pendiente'),
        [SubTotal] DECIMAL(10,2) NOT NULL DEFAULT(0),
        [Impuestos] DECIMAL(10,2) NOT NULL DEFAULT(0),
        [Total] DECIMAL(10,2) NOT NULL DEFAULT(0),
        [Observaciones] NVARCHAR(500) NULL,
        [DireccionEntrega] NVARCHAR(200) NULL,
        
        CONSTRAINT [PK_Pedidos] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Pedidos_Cliente] FOREIGN KEY ([ClienteId]) 
            REFERENCES [dbo].[Clientes] ([Id]),
        CONSTRAINT [FK_Pedidos_Usuario] FOREIGN KEY ([UsuarioId]) 
            REFERENCES [dbo].[Usuarios] ([Id])
    );
    PRINT '✓ Tabla Pedidos creada exitosamente.';
END

-- TABLA: DETALLE_PEDIDOS
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DetallePedidos' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[DetallePedidos] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [PedidoId] INT NOT NULL,
        [ProductoId] INT NOT NULL,
        [Cantidad] INT NOT NULL,
        [PrecioUnitario] DECIMAL(10,2) NOT NULL,
        [Subtotal] DECIMAL(10,2) NOT NULL,
        
        CONSTRAINT [PK_DetallePedidos] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_DetallePedidos_Pedido] FOREIGN KEY ([PedidoId]) 
            REFERENCES [dbo].[Pedidos] ([Id]),
        CONSTRAINT [FK_DetallePedidos_Producto] FOREIGN KEY ([ProductoId]) 
            REFERENCES [dbo].[Productos] ([Id])
    );
    PRINT '✓ Tabla DetallePedidos creada exitosamente.';
END

-- TABLA: MOVIMIENTOS_STOCK
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MovimientosStock' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[MovimientosStock] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ProductoId] INT NOT NULL,
        [UsuarioId] INT NOT NULL,
        [TipoMovimiento] NVARCHAR(20) NOT NULL,
        [Cantidad] INT NOT NULL,
        [StockAnterior] INT NOT NULL,
        [StockActual] INT NOT NULL,
        [Motivo] NVARCHAR(500) NULL,
        [FechaMovimiento] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        [PedidoId] INT NULL,
        
        CONSTRAINT [PK_MovimientosStock] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_MovimientosStock_Producto] FOREIGN KEY ([ProductoId]) 
            REFERENCES [dbo].[Productos] ([Id]),
        CONSTRAINT [FK_MovimientosStock_Usuario] FOREIGN KEY ([UsuarioId]) 
            REFERENCES [dbo].[Usuarios] ([Id]),
        CONSTRAINT [FK_MovimientosStock_Pedido] FOREIGN KEY ([PedidoId]) 
            REFERENCES [dbo].[Pedidos] ([Id])
    );
    PRINT '✓ Tabla MovimientosStock creada exitosamente.';
END

-- TABLA: PAGOS
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Pagos' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Pagos] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [PedidoId] INT NOT NULL,
        [Monto] DECIMAL(10,2) NOT NULL,
        [MetodoPago] NVARCHAR(20) NOT NULL,
        [FechaPago] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        [Referencia] NVARCHAR(100) NULL,
        [Estado] NVARCHAR(20) NOT NULL DEFAULT('Completado'),
        [UsuarioId] INT NOT NULL,
        
        CONSTRAINT [PK_Pagos] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Pagos_Pedido] FOREIGN KEY ([PedidoId]) 
            REFERENCES [dbo].[Pedidos] ([Id]),
        CONSTRAINT [FK_Pagos_Usuario] FOREIGN KEY ([UsuarioId]) 
            REFERENCES [dbo].[Usuarios] ([Id])
    );
    PRINT '✓ Tabla Pagos creada exitosamente.';
END

-- ===============================================
-- 3. CREAR ÍNDICES PRINCIPALES
-- ===============================================
PRINT '';
PRINT '3. Creando índices para optimización...';

-- Índices para sistema de permisos
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UsuarioFamilia_UsuarioId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_UsuarioFamilia_UsuarioId]
    ON [dbo].[UsuarioFamilia] ([UsuarioId]);
    PRINT '✓ Índice IX_UsuarioFamilia_UsuarioId creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FamiliaPatente_FamiliaId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_FamiliaPatente_FamiliaId]
    ON [dbo].[FamiliaPatente] ([FamiliaId]);
    PRINT '✓ Índice IX_FamiliaPatente_FamiliaId creado.';
END

-- Índices principales para consultas frecuentes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bitacora_FechaHora')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Bitacora_FechaHora]
    ON [dbo].[Bitacora] ([FechaHora] DESC);
    PRINT '✓ Índice IX_Bitacora_FechaHora creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bitacora_UsuarioId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Bitacora_UsuarioId]
    ON [dbo].[Bitacora] ([UsuarioId]);
    PRINT '✓ Índice IX_Bitacora_UsuarioId creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Productos_Nombre')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Productos_Nombre]
    ON [dbo].[Productos] ([Nombre]);
    PRINT '✓ Índice IX_Productos_Nombre creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Productos_CategoriaId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Productos_CategoriaId]
    ON [dbo].[Productos] ([CategoriaId]);
    PRINT '✓ Índice IX_Productos_CategoriaId creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Productos_ProveedorId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Productos_ProveedorId]
    ON [dbo].[Productos] ([ProveedorId]);
    PRINT '✓ Índice IX_Productos_ProveedorId creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pedidos_ClienteId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Pedidos_ClienteId]
    ON [dbo].[Pedidos] ([ClienteId]);
    PRINT '✓ Índice IX_Pedidos_ClienteId creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pedidos_FechaPedido')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Pedidos_FechaPedido]
    ON [dbo].[Pedidos] ([FechaPedido] DESC);
    PRINT '✓ Índice IX_Pedidos_FechaPedido creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DetallePedidos_PedidoId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_DetallePedidos_PedidoId]
    ON [dbo].[DetallePedidos] ([PedidoId]);
    PRINT '✓ Índice IX_DetallePedidos_PedidoId creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DetallePedidos_ProductoId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_DetallePedidos_ProductoId]
    ON [dbo].[DetallePedidos] ([ProductoId]);
    PRINT '✓ Índice IX_DetallePedidos_ProductoId creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MovimientosStock_ProductoId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_MovimientosStock_ProductoId]
    ON [dbo].[MovimientosStock] ([ProductoId]);
    PRINT '✓ Índice IX_MovimientosStock_ProductoId creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MovimientosStock_FechaMovimiento')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_MovimientosStock_FechaMovimiento]
    ON [dbo].[MovimientosStock] ([FechaMovimiento] DESC);
    PRINT '✓ Índice IX_MovimientosStock_FechaMovimiento creado.';
END

-- ===============================================
-- FINALIZACIÓN
-- ===============================================
PRINT '';
PRINT '=======================================================';
PRINT 'ESTRUCTURA DE BASE DE DATOS CREADA EXITOSAMENTE';
PRINT '=======================================================';
PRINT '';
PRINT '✅ Base de datos CarniceriaCRM creada';
PRINT '✅ 15 tablas del sistema creadas con sus relaciones';
PRINT '✅ Sistema de permisos con patrón Composite implementado';
PRINT '✅ 13 índices para optimización aplicados';
PRINT '';
PRINT '📋 TABLAS PRINCIPALES CREADAS:';
PRINT '- Usuarios, Patentes, Familias (Sistema de permisos)';
PRINT '- Bitacora (Auditoría del sistema)';
PRINT '- Clientes, Categorias, Proveedores, Productos';
PRINT '- Pedidos, DetallePedidos, MovimientosStock, Pagos';
PRINT '';
PRINT '⏭️  SIGUIENTE PASO:';
PRINT '   Ejecute Setup_InitialData.sql para cargar datos iniciales';
PRINT '   y configurar el sistema de permisos completo.';
PRINT '';
PRINT '🚀 La estructura está lista para recibir datos!';
GO 