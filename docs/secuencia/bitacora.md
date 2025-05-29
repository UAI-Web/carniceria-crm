# Diagrama de Secuencia - Consulta de Bitácora

## 📋 Sistema de Auditoría y Consulta de Bitácora

### Acceso a Bitácora con Control de Permisos

```mermaid
sequenceDiagram
    participant U as Usuario WebMaster
    participant D as Dashboard.aspx
    participant B as Bitacora.aspx
    participant SS as SesionSingleton
    participant BS as BitacoraService
    participant BD as BitacoraDAL
    participant DB as Base de Datos
    participant US as UsuarioService

    U->>D: Click "Bitácora"
    D->>D: Verificar pnlBitacora.Visible (solo WebMaster)
    D->>B: Response.Redirect("Bitacora.aspx")

    B->>B: Page_Load()
    B->>B: VerificarPermisos()
    B->>SS: Obtener usuario actual
    SS-->>B: Usuario con roles

    alt No es WebMaster
        B->>B: !usuarioActual.Familias.Any(f => f.Nombre == "WebMaster")
        B->>D: Response.Redirect("Login.aspx") - Acceso denegado
        D-->>U: Redirección por falta de permisos
    end

    B->>B: CargarInformacionUsuario()
    B->>B: CargarUsuariosFiltro()
    B->>US: ObtenerTodos()
    US-->>B: Lista de usuarios para filtro

    B->>B: ConfigurarFechasPorDefecto() (últimos 7 días)
    B->>B: CargarBitacora() - Carga inicial

    B->>BS: ObtenerConFiltros(null, null, fechaDesde, fechaHasta)
    BS->>BD: FiltrarRegistros()
    BD->>DB: SELECT * FROM Bitacora con JOINs y filtros
    DB-->>BD: Registros de bitácora
    BD-->>BS: Lista filtrada
    BS-->>B: Registros para mostrar

    B->>B: CrearDataTable(registros)
    B->>B: gvBitacora.DataBind()
    B-->>U: Bitácora cargada con datos
```

### Filtrado Avanzado de Bitácora

```mermaid
sequenceDiagram
    participant U as Usuario WebMaster
    participant B as Bitacora.aspx
    participant BS as BitacoraService
    participant BD as BitacoraDAL
    participant DB as Base de Datos

    U->>B: Configura filtros (usuario, acción, fechas)
    U->>B: Click "Filtrar"

    B->>B: btnFiltrar_Click()
    B->>B: Obtener valores de filtros

    Note over B: Preparar parámetros
    B->>B: int? usuarioId = ddlUsuarios.SelectedValue
    B->>B: string accion = ddlAcciones.SelectedValue
    B->>B: DateTime? fechaDesde, fechaHasta

    B->>BS: ObtenerConFiltros(usuarioId, accion, fechaDesde, fechaHasta)

    BS->>BD: FiltrarRegistros(parámetros)
    BD->>DB: Query SQL con filtros dinámicos

    Note over DB: Consulta optimizada
    DB->>DB: SELECT b.*, u.Nombre, u.Apellido, u.Mail<br/>FROM Bitacora b<br/>LEFT JOIN Usuarios u ON b.UsuarioId = u.Id<br/>WHERE [filtros aplicados]<br/>ORDER BY b.FechaHora DESC

    DB-->>BD: Resultados filtrados
    BD-->>BS: Lista de registros
    BS-->>B: Datos filtrados

    B->>B: CrearDataTable(registrosFiltrados)
    B->>B: Actualizar GridView
    B->>B: Actualizar estadísticas (lblTotalRegistros)

    B-->>U: Bitácora filtrada mostrada
```

### Ver Detalle de Registro

```mermaid
sequenceDiagram
    participant U as Usuario WebMaster
    participant B as Bitacora.aspx
    participant BS as BitacoraService
    participant BD as BitacoraDAL
    participant DB as Base de Datos

    U->>B: Click "Ver Detalle" en registro
    B->>B: btnVerDetalle_Click()
    B->>B: Obtener ID del registro (CommandArgument)

    B->>BS: ObtenerPorId(registroId)
    BS->>BD: BuscarPorId(id)
    BD->>DB: SELECT * FROM Bitacora WHERE Id = @Id
    DB-->>BD: Registro específico
    BD-->>BS: Objeto Bitacora completo
    BS-->>B: Detalles del registro

    B->>B: Cargar datos en modal
    B->>B: lblDetalleId.Text = registro.Id
    B->>B: lblDetalleFecha.Text = registro.FechaHora
    B->>B: lblDetalleUsuario.Text = usuario.NombreCompleto
    B->>B: lblDetalleAccion.Text = registro.Accion
    B->>B: lblDetalleDescripcion.Text = registro.Descripcion
    B->>B: lblDetalleIP.Text = registro.DireccionIP
    B->>B: lblDetalleUserAgent.Text = registro.UserAgent

    B->>B: ClientScript.RegisterStartupScript("showModal")
    B-->>U: Modal con detalles completos del registro
```

### Exportación de Datos (Futuro)

```mermaid
sequenceDiagram
    participant U as Usuario WebMaster
    participant B as Bitacora.aspx
    participant ES as ExportService
    participant BD as BitacoraDAL
    participant DB as Base de Datos

    U->>B: Click "Exportar"
    B->>B: btnExportar_Click()

    Note over B: Funcionalidad en desarrollo
    B->>B: Obtener filtros actuales
    B->>ES: ExportarAExcel(filtros)

    ES->>BD: ObtenerDatosParaExportar(filtros)
    BD->>DB: SELECT con formato para exportación
    DB-->>BD: Datos formateados
    BD-->>ES: Dataset completo

    ES->>ES: Generar archivo Excel/CSV
    ES->>ES: Aplicar formato y headers
    ES-->>B: Archivo generado

    B->>B: Response.Download(archivo)
    B-->>U: Descarga del archivo Excel/CSV
```

## 🔍 Características del Sistema de Bitácora

### 1. **Control de Acceso**

- ✅ Solo usuarios con rol WebMaster pueden acceder
- ✅ Verificación en Page_Load y redirección automática
- ✅ Validación de permisos antes de cada operación

### 2. **Filtros Implementados**

- 📅 **Fechas**: Rango configurable (por defecto últimos 7 días)
- 👤 **Usuario**: Dropdown con todos los usuarios del sistema
- 🎯 **Acción**: Filtro por tipo de acción (Login, Logout, etc.)
- 🔄 **Combinados**: Múltiples filtros aplicables simultáneamente

### 3. **Tipos de Registros Auditados**

- 🔐 **Autenticación**: Login exitoso, login fallido, logout
- 🚫 **Seguridad**: Bloqueo de usuarios, intentos fallidos
- 🔑 **Cambios**: Modificación de contraseñas
- 👥 **Usuarios**: Creación, modificación, eliminación
- 📝 **Operaciones**: Todas las acciones críticas del sistema

### 4. **Información Capturada**

- 🕐 **Timestamp**: Fecha y hora exacta
- 👤 **Usuario**: Quien realizó la acción
- 🎯 **Acción**: Qué se hizo
- 📝 **Descripción**: Detalles adicionales
- 🌐 **IP**: Dirección IP del cliente
- 💻 **UserAgent**: Información del navegador

### 5. **Funcionalidades de la Interfaz**

- 📊 **Paginación**: Para manejar grandes volúmenes
- 🔄 **Actualización**: Botón para refrescar datos
- 🔍 **Detalle**: Modal con información completa
- 📤 **Exportación**: Preparado para Excel/CSV (en desarrollo)
- 📈 **Estadísticas**: Contadores y métricas

### 6. **Seguridad y Rendimiento**

- 🛡️ **No eliminación**: Los registros de auditoría son permanentes
- ⚡ **Consultas optimizadas**: Con índices y filtros eficientes
- 🚫 **Solo lectura**: Los registros no pueden ser modificados
- 📊 **Limitación de resultados**: Paginación para evitar sobrecarga

---
