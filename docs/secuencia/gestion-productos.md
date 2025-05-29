# Diagrama de Secuencia - Gestión de Productos

## 📦 Sistema de Administración de Productos

### Crear Nuevo Producto

```mermaid
sequenceDiagram
    participant C as Carnicero/WebMaster
    participant GP as GestionProductos.aspx
    participant PS as ProductoService
    participant PD as ProductoDAL
    participant BD as Base de Datos
    participant BIT as BitacoraDAL
    participant MS as MovimientoStockDAL

    C->>GP: Click "Nuevo Producto"
    GP->>GP: VerificarPermisos() - Carnicero o WebMaster
    GP->>GP: CargarCategorias()
    GP->>PD: ObtenerCategorias()
    PD-->>GP: Lista de categorías
    GP-->>C: Formulario con categorías disponibles

    C->>GP: Completa datos del producto
    C->>GP: Nombre, Descripción, Precio, Categoría
    C->>GP: Stock inicial, Stock mínimo
    C->>GP: Click "Guardar"

    GP->>GP: ValidarFormulario()
    GP->>GP: ValidarPrecio() > 0
    GP->>GP: ValidarStock() >= 0

    alt Datos inválidos
        GP-->>C: Mostrar errores de validación
    end

    GP->>PS: CrearProducto(producto)
    PS->>PD: VerificarCodigoUnico(codigo)
    PD->>BD: SELECT COUNT(*) FROM Productos WHERE Codigo = @Codigo

    alt Código duplicado
        PD-->>PS: true
        PS-->>GP: Exception("Código ya existe")
        GP-->>C: "El código de producto ya está en uso"
    end

    PS->>PD: Insertar(producto)
    PD->>BD: INSERT INTO Productos (Nombre, Descripcion, Precio, etc.)
    BD-->>PD: Producto creado con ID

    alt Stock inicial > 0
        PS->>MS: RegistrarMovimientoStock(productoId, "Entrada inicial", stockInicial)
        MS->>BD: INSERT INTO MovimientosStock
    end

    PS->>BIT: RegistrarCreacionProducto(producto, usuario)
    BIT->>BD: INSERT INTO Bitacora (Accion: "Producto creado")

    PS-->>GP: Producto creado exitosamente
    GP->>GP: LimpiarFormulario()
    GP->>GP: RefrescarGrilla()
    GP-->>C: "Producto creado correctamente"
```

### Modificar Producto Existente

```mermaid
sequenceDiagram
    participant C as Carnicero/WebMaster
    participant GP as GestionProductos.aspx
    participant PS as ProductoService
    participant PD as ProductoDAL
    participant BD as Base de Datos
    participant BIT as BitacoraDAL

    C->>GP: Selecciona producto de la grilla
    C->>GP: Click "Editar"

    GP->>PD: ObtenerPorId(productoId)
    PD->>BD: SELECT * FROM Productos WHERE Id = @Id
    BD-->>PD: Datos del producto
    PD-->>GP: Producto completo

    GP->>GP: CargarDatosEnFormulario(producto)
    GP-->>C: Formulario con datos actuales

    C->>GP: Modifica datos (precio, descripción, etc.)
    C->>GP: Click "Actualizar"

    GP->>GP: ValidarCambios()
    GP->>PS: ActualizarProducto(productoModificado)

    PS->>PD: VerificarCambiosPrecio(producto)

    alt Cambio de precio significativo
        PS->>BIT: RegistrarCambioPrecio(producto, precioAnterior, precioNuevo)
        BIT->>BD: INSERT INTO Bitacora (Accion: "Precio modificado")
    end

    PS->>PD: Modificar(producto)
    PD->>BD: UPDATE Productos SET Nombre=@Nombre, Precio=@Precio, etc.

    PS->>BIT: RegistrarModificacionProducto(producto, cambios)
    BIT->>BD: INSERT INTO Bitacora (Accion: "Producto modificado")

    PS-->>GP: Producto actualizado
    GP->>GP: RefrescarGrilla()
    GP-->>C: "Producto actualizado correctamente"
```

### Gestión de Stock

```mermaid
sequenceDiagram
    participant C as Carnicero/WebMaster
    participant GS as GestionStock.aspx
    participant SS as StockService
    participant PD as ProductoDAL
    participant MS as MovimientoStockDAL
    participant BD as Base de Datos
    participant BIT as BitacoraDAL

    C->>GS: Accede a gestión de stock
    GS->>GS: CargarProductosConStock()
    GS->>PD: ObtenerTodosConStock()
    PD->>BD: SELECT p.*, s.StockActual FROM Productos p LEFT JOIN Stock s
    BD-->>PD: Productos con stock actual
    PD-->>GS: Lista de productos
    GS-->>C: Grilla con productos y niveles de stock

    Note over C,GS: Entrada de stock
    C->>GS: Selecciona producto
    C->>GS: Click "Entrada de Stock"
    C->>GS: Ingresa cantidad y motivo
    C->>GS: Click "Registrar Entrada"

    GS->>SS: RegistrarEntrada(productoId, cantidad, motivo)
    SS->>MS: CrearMovimiento("Entrada", cantidad, motivo)
    MS->>BD: INSERT INTO MovimientosStock (Tipo: "Entrada")

    SS->>PD: ActualizarStock(productoId, +cantidad)
    PD->>BD: UPDATE Stock SET StockActual = StockActual + @Cantidad

    SS->>BIT: RegistrarMovimientoStock(producto, "Entrada", cantidad)
    BIT->>BD: INSERT INTO Bitacora

    SS-->>GS: Entrada registrada
    GS->>GS: RefrescarStock()
    GS-->>C: "Entrada de stock registrada"

    Note over C,GS: Salida de stock
    C->>GS: Click "Salida de Stock"
    C->>GS: Ingresa cantidad y motivo

    GS->>SS: RegistrarSalida(productoId, cantidad, motivo)
    SS->>PD: VerificarStockSuficiente(productoId, cantidad)
    PD->>BD: SELECT StockActual FROM Stock WHERE ProductoId = @Id

    alt Stock insuficiente
        PD-->>SS: StockActual < cantidad
        SS-->>GS: Exception("Stock insuficiente")
        GS-->>C: "No hay suficiente stock disponible"
    end

    SS->>MS: CrearMovimiento("Salida", cantidad, motivo)
    SS->>PD: ActualizarStock(productoId, -cantidad)
    SS->>BIT: RegistrarMovimientoStock(producto, "Salida", cantidad)

    SS-->>GS: Salida registrada
    GS-->>C: "Salida de stock registrada"
```

### Alertas de Stock Bajo

```mermaid
sequenceDiagram
    participant SYS as Sistema (Timer/Scheduler)
    participant AS as AlertaService
    participant PD as ProductoDAL
    participant BD as Base de Datos
    participant NS as NotificationService
    participant C as Carnicero/WebMaster

    SYS->>AS: Ejecutar verificación diaria
    AS->>PD: ObtenerProductosStockBajo()
    PD->>BD: SELECT p.*, s.StockActual FROM Productos p INNER JOIN Stock s WHERE s.StockActual <= p.StockMinimo
    BD-->>PD: Productos con stock bajo
    PD-->>AS: Lista de productos críticos

    loop Para cada producto con stock bajo
        AS->>NS: CrearAlerta(producto, stockActual, stockMinimo)
        NS->>BD: INSERT INTO Alertas (Tipo: "Stock Bajo")

        AS->>NS: EnviarNotificacion(usuarios con rol Carnicero/WebMaster)
        NS->>C: Email/Dashboard notification
    end

    AS->>AS: GenerarReporteStockBajo()
    AS-->>SYS: Verificación completada
```

### Consulta de Catálogo (Cliente)

```mermaid
sequenceDiagram
    participant CL as Cliente
    participant CAT as Catalogo.aspx
    participant PS as ProductoService
    participant PD as ProductoDAL
    participant BD as Base de Datos

    CL->>CAT: Accede al catálogo
    CAT->>CAT: VerificarPermisos() - Todos los roles
    CAT->>PS: ObtenerProductosActivos()
    PS->>PD: FiltrarProductosPublicos()
    PD->>BD: SELECT * FROM Productos WHERE Activo = 1 AND Visible = 1
    BD-->>PD: Productos disponibles
    PD-->>PS: Lista filtrada
    PS-->>CAT: Productos para mostrar

    CAT->>CAT: AplicarFiltrosCliente(categoria, precio)
    CAT-->>CL: Catálogo con productos disponibles

    Note over CL,CAT: Búsqueda de productos
    CL->>CAT: Ingresa término de búsqueda
    CL->>CAT: Selecciona filtros (categoría, rango precio)

    CAT->>PS: BuscarProductos(termino, categoria, precioMin, precioMax)
    PS->>PD: FiltrarConCriterios(filtros)
    PD->>BD: Query con WHERE condicionales
    BD-->>PD: Resultados filtrados
    PD-->>PS: Productos encontrados
    PS-->>CAT: Lista filtrada

    CAT-->>CL: Resultados de búsqueda

    Note over CL,CAT: Ver detalle de producto
    CL->>CAT: Click en producto
    CAT->>PS: ObtenerDetalleProducto(productoId)
    PS->>PD: ObtenerConDetalles(id)
    PD->>BD: SELECT p.*, c.Nombre as Categoria FROM Productos p JOIN Categorias c
    BD-->>PD: Producto con detalles completos
    PD-->>PS: Información detallada
    PS-->>CAT: Detalles del producto

    CAT-->>CL: Modal/página con información completa
```

## 📦 Características del Sistema de Productos

### 1. **Gestión Completa de Productos**

- ✅ **CRUD Completo**: Crear, leer, actualizar, eliminar
- 📊 **Categorización**: Organización por categorías
- 💰 **Gestión de Precios**: Control de cambios con auditoría
- 📸 **Imágenes**: Soporte para fotos de productos
- 🔍 **Búsqueda Avanzada**: Por múltiples criterios

### 2. **Control de Inventario**

- 📦 **Stock en Tiempo Real**: Actualización automática
- 📈 **Movimientos de Stock**: Entradas y salidas registradas
- ⚠️ **Alertas Automáticas**: Stock bajo y falta de inventario
- 📊 **Histórico**: Trazabilidad completa de movimientos
- 🔄 **Ajustes**: Correcciones de inventario

### 3. **Seguridad y Permisos**

- 🔒 **Control de Acceso**: Por roles (Carnicero/WebMaster)
- 👁️ **Visibilidad**: Productos públicos/privados
- 📝 **Auditoría**: Todos los cambios registrados
- 🛡️ **Validaciones**: Precios, stock, códigos únicos

### 4. **Experiencia del Cliente**

- 🛍️ **Catálogo Público**: Vista optimizada para clientes
- 🔍 **Filtros Intuitivos**: Por categoría, precio, disponibilidad
- 📱 **Responsive**: Adaptado a dispositivos móviles
- ⚡ **Rendimiento**: Carga rápida con paginación

### 5. **Gestión de Categorías**

- 📂 **Organización Jerárquica**: Categorías y subcategorías
- 🏷️ **Etiquetado**: Sistema de tags para productos
- 🔄 **Reasignación**: Cambio de categorías con historial
- 📊 **Estadísticas**: Productos por categoría

### 6. **Integración con Pedidos**

- 🛒 **Disponibilidad Real**: Stock validado en pedidos
- 📉 **Reserva Automática**: Stock comprometido en pedidos
- 🔄 **Liberación**: Stock liberado en cancelaciones
- 📈 **Estadísticas**: Productos más vendidos

---

_Diagrama generado para Carnicería CRM - Gestión de Productos_
