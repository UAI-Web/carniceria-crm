using System;

namespace CarniceriaCRM.BE
{
    public enum PermisosEnum
    {
        // Gestión de Usuarios
        GestionarUsuarios = 1001,
        BloquearDesbloquearUsuarios = 1002,
        
        // Productos
        VerProductos = 2001,
        CrearProductos = 2002,
        ModificarProductos = 2003,
        EliminarProductos = 2004,
        GestionarStock = 2005,
        
        // Clientes
        VerClientes = 3001,
        CrearClientes = 3002,
        ModificarClientes = 3003,
        EliminarClientes = 3004,
        GestionarCredito = 3005,
        
        // Pedidos
        VerPedidos = 4001,
        CrearPedidos = 4002,
        ModificarPedidos = 4003,
        CancelarPedidos = 4004,
        ProcesarPedidos = 4005,
        
        // Ventas y Pagos
        RegistrarVentas = 5001,
        ProcesarPagos = 5002,
        VerHistorialVentas = 5003,
        GenerarFacturas = 5004,
        
        // Proveedores
        VerProveedores = 6001,
        CrearProveedores = 6002,
        ModificarProveedores = 6003,
        EliminarProveedores = 6004,
        
        // Reportes y Estadísticas
        VerReportes = 7001,
        GenerarReportes = 7002,
        VerEstadisticas = 7003,
        ExportarDatos = 7004,
        
        // Configuración del Sistema
        ConfigurarSistema = 8001,
        GestionarPermisos = 8002,
        VerBitacora = 8003,
        MantenimientoBD = 8004,
        
        // Categorías
        VerCategorias = 9001,
        CrearCategorias = 9002,
        ModificarCategorias = 9003,
        EliminarCategorias = 9004
    }
} 