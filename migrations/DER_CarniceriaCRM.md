# Diagrama Entidad-Relación - Carnicería CRM

## DER Completo del Sistema

```mermaid
erDiagram
    USUARIOS {
        int Id PK
        nvarchar_50 Nombre
        nvarchar_50 Apellido
        nvarchar_100 Mail UK
        nvarchar_255 Clave
        int IntentosFallidos
        bit Bloqueado
        datetime FechaCreacion
        datetime FechaUltimaModificacion
        bit Activo
    }

    PATENTES {
        int Id PK
        nvarchar_100 Nombre UK
        nvarchar_500 Descripcion
        nvarchar_100 Permiso
        bit Activo
        datetime FechaCreacion
    }

    FAMILIAS {
        int Id PK
        nvarchar_100 Nombre UK
        nvarchar_500 Descripcion
        bit Activo
        datetime FechaCreacion
    }

    FAMILIA_PATENTE {
        int Id PK
        int FamiliaId FK
        int PatenteId FK
        datetime FechaAsignacion
    }

    FAMILIA_FAMILIA {
        int Id PK
        int FamiliaPadreId FK
        int FamiliaHijaId FK
        datetime FechaAsignacion
    }

    USUARIO_FAMILIA {
        int Id PK
        int UsuarioId FK
        int FamiliaId FK
        datetime FechaAsignacion
        bit Activo
    }

    BITACORA {
        int Id PK
        int UsuarioId FK
        nvarchar_100 Accion
        nvarchar_500 Descripcion
        datetime FechaHora
        nvarchar_45 DireccionIP
        nvarchar_200 UserAgent
    }

    CLIENTES {
        int Id PK
        nvarchar_50 Nombre
        nvarchar_50 Apellido
        nvarchar_100 Email
        nvarchar_20 Telefono
        nvarchar_200 Direccion
        nvarchar_50 Ciudad
        nvarchar_10 CodigoPostal
        datetime FechaRegistro
        bit Activo
        decimal_10_2 LimiteCredito
        decimal_10_2 SaldoActual
    }

    CATEGORIAS {
        int Id PK
        nvarchar_100 Nombre UK
        nvarchar_500 Descripcion
        bit Activo
        datetime FechaCreacion
    }

    PRODUCTOS {
        int Id PK
        nvarchar_100 Nombre
        nvarchar_500 Descripcion
        int CategoriaId FK
        int ProveedorId FK
        decimal_10_2 PrecioCompra
        decimal_10_2 PrecioVenta
        int StockMinimo
        int StockActual
        nvarchar_50 UnidadMedida
        bit Activo
        datetime FechaCreacion
        datetime FechaUltimaModificacion
    }

    PROVEEDORES {
        int Id PK
        nvarchar_100 NombreEmpresa
        nvarchar_50 ContactoNombre
        nvarchar_100 Email
        nvarchar_20 Telefono
        nvarchar_200 Direccion
        nvarchar_50 Ciudad
        nvarchar_10 CodigoPostal
        nvarchar_20 CUIT
        bit Activo
        datetime FechaRegistro
    }

    PEDIDOS {
        int Id PK
        int ClienteId FK
        int UsuarioId FK
        datetime FechaPedido
        datetime FechaEntrega
        nvarchar_20 Estado
        decimal_10_2 SubTotal
        decimal_10_2 Impuestos
        decimal_10_2 Total
        nvarchar_500 Observaciones
        nvarchar_200 DireccionEntrega
    }

    DETALLE_PEDIDOS {
        int Id PK
        int PedidoId FK
        int ProductoId FK
        int Cantidad
        decimal_10_2 PrecioUnitario
        decimal_10_2 Subtotal
    }

    MOVIMIENTOS_STOCK {
        int Id PK
        int ProductoId FK
        int UsuarioId FK
        nvarchar_20 TipoMovimiento
        int Cantidad
        int StockAnterior
        int StockActual
        nvarchar_500 Motivo
        datetime FechaMovimiento
        int PedidoId FK
    }

    PAGOS {
        int Id PK
        int PedidoId FK
        decimal_10_2 Monto
        nvarchar_20 MetodoPago
        datetime FechaPago
        nvarchar_100 Referencia
        nvarchar_20 Estado
        int UsuarioId FK
    }

    %% Relaciones Sistema de Permisos
    USUARIOS ||--o{ USUARIO_FAMILIA : "tiene_roles"
    FAMILIAS ||--o{ USUARIO_FAMILIA : "asignada_a"

    FAMILIAS ||--o{ FAMILIA_PATENTE : "contiene"
    PATENTES ||--o{ FAMILIA_PATENTE : "pertenece_a"

    FAMILIAS ||--o{ FAMILIA_FAMILIA : "padre"
    FAMILIAS ||--o{ FAMILIA_FAMILIA : "hija"

    %% Relaciones Existentes
    USUARIOS ||--o{ BITACORA : "registra"
    USUARIOS ||--o{ PEDIDOS : "procesa"
    USUARIOS ||--o{ MOVIMIENTOS_STOCK : "registra"
    USUARIOS ||--o{ PAGOS : "procesa"

    CLIENTES ||--o{ PEDIDOS : "realiza"

    CATEGORIAS ||--o{ PRODUCTOS : "contiene"
    PROVEEDORES ||--o{ PRODUCTOS : "suministra"

    PRODUCTOS ||--o{ DETALLE_PEDIDOS : "incluye"
    PRODUCTOS ||--o{ MOVIMIENTOS_STOCK : "afecta"

    PEDIDOS ||--o{ DETALLE_PEDIDOS : "contiene"
    PEDIDOS ||--o{ MOVIMIENTOS_STOCK : "genera"
    PEDIDOS ||--o{ PAGOS : "recibe"
```

## Descripción de Entidades

### SISTEMA DE PERMISOS Y ROLES

#### PATENTES

- Permisos individuales del sistema (hojas del composite)
- Cada patente representa una acción específica que se puede realizar
- Ejemplos: "VerProductos", "CrearClientes", "GestionarUsuarios"

#### FAMILIAS

- Roles que agrupan patentes y/o otras familias (composite)
- Permiten crear jerarquías de permisos
- Ejemplos de roles: "WebMaster", "Carnicero", "Cliente"

#### FAMILIA_PATENTE

- Relación many-to-many entre familias y patentes
- Una familia puede tener múltiples patentes
- Una patente puede estar en múltiples familias

#### FAMILIA_FAMILIA

- Relación jerárquica entre familias (composite pattern)
- Permite que una familia contenga otras familias
- Evita ciclos recursivos mediante validaciones

#### USUARIO_FAMILIA

- Asignación de roles (familias) a usuarios
- Un usuario puede tener múltiples roles
- Control de activación/desactivación de roles

### USUARIOS

- Gestión de usuarios del sistema con control de acceso y seguridad
- Control de intentos fallidos y bloqueo de usuarios
- Auditoría de cambios
- **Integración con sistema de permisos mediante USUARIO_FAMILIA**

### BITACORA

- Registro de todas las acciones importantes del sistema
- Rastrea inicios de sesión y actividades críticas
- Incluye información de contexto (IP, User Agent)

### CLIENTES

- Información completa de clientes
- Control de crédito y límites
- Gestión de contacto y ubicación

### PRODUCTOS

- Catálogo completo de productos de la carnicería
- Control de precios y stock
- Relación con categorías y proveedores

### CATEGORIAS

- Organización de productos por tipo
- Estructura jerárquica simple

### PROVEEDORES

- Gestión de proveedores
- Información fiscal y de contacto

### PEDIDOS

- Gestión completa de pedidos de clientes
- Estados de pedido y seguimiento
- Información de entrega

### DETALLE_PEDIDOS

- Líneas individuales de cada pedido
- Precios y cantidades específicas

### MOVIMIENTOS_STOCK

- Auditoría completa de movimientos de inventario
- Trazabilidad de cambios en stock
- Vinculación con pedidos

### PAGOS

- Gestión de pagos de pedidos
- Múltiples métodos de pago
- Estados de pago y referencias

## Roles Predefinidos del Sistema

### 🔧 WebMaster

- **Descripción**: Administrador completo del sistema
- **Permisos**: Todos los permisos disponibles
- **Acceso**: Configuración del sistema, gestión de usuarios, permisos, etc.

### 🥩 Carnicero

- **Descripción**: Personal operativo de la carnicería
- **Permisos**:
  - Gestión de productos, stock, categorías
  - Procesamiento de pedidos y ventas
  - Gestión de clientes
  - Generación de reportes operativos
- **Restricciones**: No puede gestionar usuarios ni configuración del sistema

### 👤 Cliente

- **Descripción**: Usuarios con acceso limitado de consulta
- **Permisos**:
  - Ver productos y precios
  - Consultar sus propios pedidos
  - Ver historial de compras
- **Restricciones**: Solo lectura, no puede modificar datos del sistema
