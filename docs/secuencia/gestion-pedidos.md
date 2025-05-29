# Diagrama de Secuencia - Gestión de Pedidos

## 🛒 Sistema de Gestión de Pedidos

### Crear Pedido (Cliente)

```mermaid
sequenceDiagram
    participant CL as Cliente
    participant CP as CrearPedido.aspx
    participant PS as PedidoService
    participant PrS as ProductoService
    participant PD as PedidoDAL
    participant PrD as ProductoDAL
    participant BD as Base de Datos
    participant SS as StockService
    participant BIT as BitacoraDAL

    CL->>CP: Accede a crear pedido
    CP->>CP: VerificarPermisos() - Usuario autenticado
    CP->>PrS: ObtenerProductosDisponibles()
    PrS->>PrD: FiltrarActivos()
    PrD->>BD: SELECT * FROM Productos WHERE Activo = 1 AND Stock > 0
    BD-->>PrD: Productos disponibles
    PrD-->>PrS: Lista de productos
    PrS-->>CP: Productos para el pedido

    CP-->>CL: Formulario con productos disponibles

    CL->>CP: Selecciona productos y cantidades
    CL->>CP: Ingresa datos de entrega
    CL->>CP: Click "Agregar al Carrito"

    loop Para cada producto seleccionado
        CP->>SS: VerificarDisponibilidad(productoId, cantidad)
        SS->>PrD: ObtenerStockActual(productoId)
        PrD->>BD: SELECT StockActual FROM Stock

        alt Stock insuficiente
            SS-->>CP: Exception("Stock insuficiente")
            CP-->>CL: "Producto sin stock suficiente"
        end
    end

    CL->>CP: Click "Confirmar Pedido"
    CP->>PS: CrearPedido(cliente, productos, datosEntrega)

    PS->>PD: Insertar(pedido)
    PD->>BD: INSERT INTO Pedidos (ClienteId, FechaPedido, Estado, etc.)
    BD-->>PD: Pedido creado con ID

    loop Para cada item del pedido
        PS->>PD: InsertarDetallePedido(pedidoId, producto, cantidad, precio)
        PD->>BD: INSERT INTO PedidoDetalles

        PS->>SS: ReservarStock(productoId, cantidad)
        SS->>BD: UPDATE Stock SET StockReservado = StockReservado + @Cantidad
    end

    PS->>PS: CalcularTotal(pedido)
    PS->>PD: ActualizarTotal(pedidoId, total)
    PD->>BD: UPDATE Pedidos SET Total = @Total

    PS->>BIT: RegistrarCreacionPedido(pedido, cliente)
    BIT->>BD: INSERT INTO Bitacora (Accion: "Pedido creado")

    PS-->>CP: Pedido creado exitosamente
    CP-->>CL: "Pedido #${pedidoId} creado. En proceso de confirmación."
```

### Gestión de Pedidos (Carnicero/WebMaster)

```mermaid
sequenceDiagram
    participant C as Carnicero/WebMaster
    participant GP as GestionPedidos.aspx
    participant PS as PedidoService
    participant PD as PedidoDAL
    participant BD as Base de Datos
    participant SS as StockService
    participant BIT as BitacoraDAL
    participant NS as NotificationService

    C->>GP: Accede a gestión de pedidos
    GP->>GP: VerificarPermisos() - Carnicero o WebMaster
    GP->>PS: ObtenerPedidosPendientes()
    PS->>PD: FiltrarPorEstado("Pendiente")
    PD->>BD: SELECT * FROM Pedidos WHERE Estado = 'Pendiente' ORDER BY FechaPedido
    BD-->>PD: Pedidos pendientes
    PD-->>PS: Lista de pedidos
    PS-->>GP: Pedidos para procesar

    GP-->>C: Grilla con pedidos pendientes

    Note over C,GP: Confirmar Pedido
    C->>GP: Selecciona pedido
    C->>GP: Click "Ver Detalle"

    GP->>PS: ObtenerConDetalles(pedidoId)
    PS->>PD: CargarDetallesCompletos(pedidoId)
    PD->>BD: SELECT p.*, pd.*, pr.Nombre FROM Pedidos p JOIN PedidoDetalles pd JOIN Productos pr
    BD-->>PD: Pedido con items completos
    PD-->>PS: Pedido detallado
    PS-->>GP: Información completa

    GP-->>C: Modal con detalles del pedido

    C->>GP: Click "Confirmar Pedido"
    GP->>PS: ConfirmarPedido(pedidoId)

    PS->>PD: CambiarEstado(pedidoId, "Confirmado")
    PD->>BD: UPDATE Pedidos SET Estado = 'Confirmado', FechaConfirmacion = GETDATE()

    PS->>SS: ConfirmarReservaStock(pedidoId)
    SS->>BD: UPDATE Stock SET StockActual = StockActual - StockReservado, StockReservado = 0

    PS->>BIT: RegistrarConfirmacionPedido(pedido, carnicero)
    BIT->>BD: INSERT INTO Bitacora (Accion: "Pedido confirmado")

    PS->>NS: EnviarNotificacionCliente(pedido, "Pedido confirmado")
    NS-->>C: Notificación enviada

    PS-->>GP: Pedido confirmado
    GP->>GP: RefrescarGrilla()
    GP-->>C: "Pedido confirmado y stock actualizado"
```

### Procesar Entrega

```mermaid
sequenceDiagram
    participant C as Carnicero/WebMaster
    participant GP as GestionPedidos.aspx
    participant PS as PedidoService
    participant PD as PedidoDAL
    participant BD as Base de Datos
    participant FS as FacturacionService
    participant BIT as BitacoraDAL
    participant NS as NotificationService

    C->>GP: Filtra pedidos confirmados
    GP->>PS: ObtenerPorEstado("Confirmado")
    PS-->>GP: Pedidos listos para entregar

    C->>GP: Selecciona pedido para entrega
    C->>GP: Click "Procesar Entrega"

    GP->>PS: PrepararEntrega(pedidoId)
    PS->>FS: GenerarFactura(pedidoId)
    FS->>BD: INSERT INTO Facturas (PedidoId, Numero, Total, etc.)

    FS->>FS: GenerarPDF(factura)
    FS-->>PS: Factura generada

    PS->>PD: CambiarEstado(pedidoId, "En Entrega")
    PD->>BD: UPDATE Pedidos SET Estado = 'En Entrega', FechaEntrega = GETDATE()

    PS->>BIT: RegistrarInicioEntrega(pedido, carnicero)
    BIT->>BD: INSERT INTO Bitacora (Accion: "Entrega iniciada")

    PS->>NS: EnviarNotificacionCliente(pedido, "Pedido en camino")

    PS-->>GP: Entrega iniciada
    GP-->>C: "Entrega procesada. Factura generada."

    Note over C,GP: Confirmar Entrega
    C->>GP: Click "Confirmar Entrega"
    GP->>PS: ConfirmarEntrega(pedidoId)

    PS->>PD: CambiarEstado(pedidoId, "Entregado")
    PD->>BD: UPDATE Pedidos SET Estado = 'Entregado', FechaEntregaReal = GETDATE()

    PS->>BIT: RegistrarEntregaCompletada(pedido)
    BIT->>BD: INSERT INTO Bitacora (Accion: "Pedido entregado")

    PS-->>GP: Entrega confirmada
    GP-->>C: "Pedido marcado como entregado"
```

### Cancelar Pedido

```mermaid
sequenceDiagram
    participant U as Usuario (Cliente/Carnicero)
    participant GP as GestionPedidos.aspx
    participant PS as PedidoService
    participant PD as PedidoDAL
    participant BD as Base de Datos
    participant SS as StockService
    participant BIT as BitacoraDAL
    participant NS as NotificationService

    U->>GP: Selecciona pedido
    U->>GP: Click "Cancelar Pedido"

    GP->>GP: VerificarPermisoCancelacion(pedido, usuario)

    alt Cliente cancelando su propio pedido
        GP->>GP: Verificar estado == "Pendiente"
        alt Pedido ya confirmado
            GP-->>U: "No se puede cancelar pedido confirmado"
        end
    end

    GP->>GP: ConfirmarCancelacion("¿Está seguro?")
    U->>GP: Confirma cancelación

    GP->>PS: CancelarPedido(pedidoId, motivo, usuario)
    PS->>PD: CambiarEstado(pedidoId, "Cancelado")
    PD->>BD: UPDATE Pedidos SET Estado = 'Cancelado', FechaCancelacion = GETDATE(), MotivoCancelacion = @Motivo

    PS->>SS: LiberarStockReservado(pedidoId)
    SS->>BD: UPDATE Stock SET StockReservado = StockReservado - @CantidadReservada

    PS->>BIT: RegistrarCancelacionPedido(pedido, usuario, motivo)
    BIT->>BD: INSERT INTO Bitacora (Accion: "Pedido cancelado")

    alt Cancelado por carnicero
        PS->>NS: EnviarNotificacionCliente(pedido, "Pedido cancelado", motivo)
    end

    PS-->>GP: Pedido cancelado
    GP->>GP: RefrescarGrilla()
    GP-->>U: "Pedido cancelado. Stock liberado."
```

### Consulta de Pedidos (Cliente)

```mermaid
sequenceDiagram
    participant CL as Cliente
    participant MP as MisPedidos.aspx
    participant PS as PedidoService
    participant PD as PedidoDAL
    participant BD as Base de Datos
    participant SS as SesionSingleton

    CL->>MP: Accede a "Mis Pedidos"
    MP->>MP: VerificarPermisos() - Usuario autenticado
    MP->>SS: Obtener usuario actual
    SS-->>MP: Usuario logueado

    MP->>PS: ObtenerPedidosDelCliente(clienteId)
    PS->>PD: FiltrarPorCliente(clienteId)
    PD->>BD: SELECT * FROM Pedidos WHERE ClienteId = @ClienteId ORDER BY FechaPedido DESC
    BD-->>PD: Pedidos del cliente
    PD-->>PS: Lista filtrada
    PS-->>MP: Pedidos del cliente

    MP-->>CL: Historial de pedidos

    Note over CL,MP: Ver detalle de pedido
    CL->>MP: Click "Ver Detalle" en pedido
    MP->>PS: ObtenerDetallesPedido(pedidoId, clienteId)

    PS->>PS: VerificarPropietario(pedidoId, clienteId)
    alt No es el propietario
        PS-->>MP: Exception("Acceso denegado")
        MP-->>CL: "No autorizado"
    end

    PS->>PD: CargarDetallesCompletos(pedidoId)
    PD->>BD: SELECT completo con productos y precios
    BD-->>PD: Detalles del pedido
    PD-->>PS: Información completa
    PS-->>MP: Detalles del pedido

    MP-->>CL: Modal con información detallada

    Note over CL,MP: Seguimiento de estado
    CL->>MP: Click "Seguir Pedido"
    MP->>PS: ObtenerEstadoPedido(pedidoId)
    PS->>PD: ConsultarEstado(pedidoId)
    PD->>BD: SELECT Estado, FechasProceso FROM Pedidos
    BD-->>PD: Estado actual y fechas
    PD-->>PS: Timeline del pedido
    PS-->>MP: Información de seguimiento

    MP-->>CL: Timeline con estados del pedido
```

### Reportes de Pedidos

```mermaid
sequenceDiagram
    participant C as Carnicero/WebMaster
    participant RP as ReportesPedidos.aspx
    participant RS as ReporteService
    participant PD as PedidoDAL
    participant BD as Base de Datos

    C->>RP: Accede a reportes de pedidos
    RP->>RP: VerificarPermisos() - Carnicero o WebMaster
    RP->>RP: ConfigurarFiltros(fechas, estado, cliente)

    C->>RP: Selecciona criterios de reporte
    C->>RP: Rango de fechas, estado, cliente
    C->>RP: Click "Generar Reporte"

    RP->>RS: GenerarReportePedidos(filtros)
    RS->>PD: ObtenerDatosReporte(filtros)
    PD->>BD: Query compleja con agregaciones

    Note over BD: Consulta optimizada
    BD->>BD: SELECT p.*, COUNT(*) as TotalPedidos, SUM(Total) as TotalVentas<br/>FROM Pedidos p<br/>GROUP BY Estado, MONTH(FechaPedido)<br/>WHERE [filtros aplicados]

    BD-->>PD: Datos agregados
    PD-->>RS: Dataset para reporte

    RS->>RS: CalcularEstadisticas(datos)
    RS->>RS: GenerarGraficos(ventas, tendencias)
    RS-->>RP: Reporte completo con gráficos

    RP-->>C: Dashboard con métricas y gráficos

    Note over C,RP: Exportar reporte
    C->>RP: Click "Exportar"
    RP->>RS: ExportarAExcel(datos)
    RS->>RS: GenerarArchivoExcel(reporte)
    RS-->>RP: Archivo generado

    RP->>RP: Response.Download(archivo)
    RP-->>C: Descarga del reporte Excel
```

## 🛒 Características del Sistema de Pedidos

### 1. **Flujo Completo de Pedidos**

- 📝 **Creación**: Cliente crea pedido con productos disponibles
- ✅ **Confirmación**: Carnicero valida y confirma pedido
- 🚛 **Entrega**: Procesamiento y entrega con facturación
- ✅ **Finalización**: Confirmación de entrega exitosa

### 2. **Gestión de Stock Integrada**

- 📦 **Reserva Automática**: Stock comprometido al crear pedido
- ✅ **Confirmación**: Stock definitivamente asignado
- 🔄 **Liberación**: Stock liberado en cancelaciones
- ⚠️ **Validación**: Verificación en tiempo real

### 3. **Control de Estados**

- 🆕 **Pendiente**: Esperando confirmación
- ✅ **Confirmado**: Aprobado por carnicero
- 🚛 **En Entrega**: Siendo entregado
- ✅ **Entregado**: Completado exitosamente
- ❌ **Cancelado**: Cancelado con motivo

### 4. **Facturación Automática**

- 🧾 **Generación**: Factura automática al confirmar entrega
- 📄 **PDF**: Documento descargable
- 💰 **Cálculos**: Totales e impuestos automáticos
- 📊 **Numeración**: Secuencial y controlada

### 5. **Notificaciones y Seguimiento**

- 📧 **Email**: Notificaciones automáticas al cliente
- 📱 **Dashboard**: Estado en tiempo real
- 📍 **Timeline**: Historial completo del pedido
- 🔔 **Alertas**: Para carniceros sobre pedidos pendientes

### 6. **Reportes y Analíticas**

- 📊 **Ventas**: Por período, cliente, producto
- 📈 **Tendencias**: Análisis temporal
- 💰 **Ingresos**: Totales y promedios
- 📱 **Dashboard**: Métricas en tiempo real
- 📤 **Exportación**: Excel para análisis externo

### 7. **Seguridad y Permisos**

- 🔒 **Segregación**: Clientes solo ven sus pedidos
- 👥 **Roles**: Diferentes permisos por tipo de usuario
- 📝 **Auditoría**: Todos los cambios registrados
- 🛡️ **Validación**: Verificación de propietario

---

_Diagrama generado para Carnicería CRM - Gestión de Pedidos_
