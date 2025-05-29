# Diagrama de Secuencia - Gestión de Roles y Permisos

## 🔐 Sistema de Control de Acceso Basado en Roles (RBAC)

### Verificación de Permisos al Acceder a Funcionalidades

```mermaid
sequenceDiagram
    participant U as Usuario
    participant P as Página.aspx
    participant SS as SesionSingleton
    participant USR as Usuario (Objeto)
    participant F as Familia (Rol)
    participant PAT as Patente (Permiso)

    U->>P: Intenta acceder a funcionalidad
    P->>P: Page_Load() o ActionMethod()
    P->>SS: Obtener usuario actual
    SS-->>P: Usuario con roles cargados

    alt Usuario no autenticado
        SS-->>P: null
        P->>P: Response.Redirect("Login.aspx")
        P-->>U: Redirección a login
    end

    P->>USR: Verificar permisos necesarios

    Note over P,USR: Verificación por rol
    P->>USR: TieneRol("WebMaster")
    USR->>F: Buscar en lista de familias
    F-->>USR: Boolean resultado
    USR-->>P: true/false

    Note over P,USR: Verificación por permiso específico
    P->>USR: TienePermiso("GestionarUsuarios")
    USR->>F: ObtenerTodosLosPermisos()
    F->>PAT: Iterar patentes
    PAT-->>F: Lista de permisos
    F-->>USR: Lista completa de permisos
    USR->>USR: Buscar permiso específico
    USR-->>P: true/false

    alt Sin permisos suficientes
        P->>P: Mostrar error o redireccionar
        P-->>U: "Acceso denegado"
    else Con permisos
        P->>P: Continuar con funcionalidad
        P-->>U: Contenido autorizado
    end
```

### Configuración de Visibilidad en Dashboard

```mermaid
sequenceDiagram
    participant U as Usuario
    participant D as Dashboard.aspx
    participant SS as SesionSingleton
    participant USR as Usuario (Objeto)

    U->>D: Accede al Dashboard
    D->>D: Page_Load()
    D->>SS: Obtener usuario actual
    SS-->>D: Usuario con familias cargadas

    D->>D: ConfigurarVisibilidadPorRol()

    Note over D,USR: Verificación de cada módulo
    D->>USR: usuarioActual.Familias.Any(f => f.Nombre == "WebMaster")
    USR-->>D: esWebMaster = true/false

    D->>USR: usuarioActual.Familias.Any(f => f.Nombre == "Carnicero")
    USR-->>D: esCarnicero = true/false

    D->>USR: usuarioActual.Familias.Any(f => f.Nombre == "Cliente")
    USR-->>D: esCliente = true/false

    Note over D: Configuración de paneles
    D->>D: pnlProductos.Visible = true (todos)
    D->>D: pnlClientes.Visible = esWebMaster || esCarnicero
    D->>D: pnlProveedores.Visible = esWebMaster || esCarnicero
    D->>D: pnlUsuarios.Visible = esWebMaster
    D->>D: pnlBitacora.Visible = esWebMaster
    D->>D: pnlConfiguracion.Visible = esWebMaster

    alt Cliente sin otros roles
        D->>D: btnProductos.Text = "Ver Catálogo"
        D->>D: btnPedidos.Text = "Mis Pedidos"
        D->>D: pnlMensajeCliente.Visible = true
    end

    D-->>U: Dashboard personalizado según rol
```

### Carga de Roles y Permisos desde Base de Datos

```mermaid
sequenceDiagram
    participant UD as UsuarioDAL
    participant DB as Base de Datos
    participant USR as Usuario (Objeto)
    participant F as Familia (Rol)
    participant PAT as Patente (Permiso)

    UD->>UD: CargarFamiliasYPermisos(usuario)
    UD->>DB: Query JOIN completa

    Note over DB: Consulta optimizada
    DB->>DB: SELECT DISTINCT f.*, p.*<br/>FROM Usuarios u<br/>INNER JOIN UsuarioFamilia uf ON u.Id = uf.UsuarioId<br/>INNER JOIN Familias f ON uf.FamiliaId = f.Id<br/>LEFT JOIN FamiliaPatente fp ON f.Id = fp.FamiliaId<br/>LEFT JOIN Patentes p ON fp.PatenteId = p.Id<br/>WHERE u.Id = @UsuarioId AND activos

    DB-->>UD: ResultSet con roles y permisos

    UD->>UD: Procesar resultados
    loop Para cada registro
        UD->>F: Crear/obtener familia
        F->>F: Configurar propiedades (Id, Nombre, Descripcion)

        alt Tiene patente asociada
            UD->>PAT: Crear patente
            PAT->>PAT: Configurar propiedades (Id, Nombre, Permiso)
            UD->>F: Agregar patente a familia.Patentes
            F->>F: Verificar no duplicados
        end
    end

    UD->>USR: Asignar lista de familias
    USR->>USR: usuario.Familias = familiasDict.Values.ToList()
    UD-->>USR: Usuario con roles completos
```

### Administración de Usuarios (Solo WebMaster)

```mermaid
sequenceDiagram
    participant W as WebMaster
    participant AU as AdminUsuarios.aspx
    participant US as UsuarioService
    participant UD as UsuarioDAL
    participant DB as Base de Datos
    participant BIT as BitacoraDAL

    W->>AU: Accede a gestión de usuarios
    AU->>AU: VerificarPermisos()

    alt No es WebMaster
        AU->>AU: Response.Redirect("Dashboard.aspx")
        AU-->>W: "Acceso denegado"
    end

    Note over W,AU: Crear nuevo usuario
    W->>AU: Completa formulario de usuario
    W->>AU: Selecciona roles a asignar
    W->>AU: Click "Guardar"

    AU->>US: CrearUsuario(usuario, roleIds)
    US->>UD: Insertar(usuario)
    UD->>DB: INSERT INTO Usuarios
    DB-->>UD: Usuario creado con ID

    loop Para cada rol seleccionado
        US->>UD: AsignarRol(usuarioId, rolId)
        UD->>DB: INSERT INTO UsuarioFamilia
    end

    US->>BIT: RegistrarCreacionUsuario(usuario)
    BIT->>DB: INSERT INTO Bitacora

    US-->>AU: Usuario creado exitosamente
    AU-->>W: "Usuario creado y roles asignados"

    Note over W,AU: Modificar roles de usuario
    W->>AU: Selecciona usuario existente
    W->>AU: Modifica asignación de roles
    W->>AU: Click "Actualizar"

    AU->>US: ActualizarRoles(usuarioId, nuevosRoles)
    US->>UD: EliminarTodosLosRoles(usuarioId)
    UD->>DB: DELETE FROM UsuarioFamilia WHERE UsuarioId = @Id

    loop Para cada nuevo rol
        US->>UD: AsignarRol(usuarioId, rolId)
        UD->>DB: INSERT INTO UsuarioFamilia
    end

    US->>BIT: RegistrarCambioRoles(usuario)
    BIT->>DB: INSERT INTO Bitacora

    US-->>AU: Roles actualizados
    AU-->>W: "Roles del usuario actualizados"
```

## 🔐 Arquitectura del Sistema de Permisos

### 1. **Estructura Jerárquica**

```
WebMaster (Administrador)
├── Todas las funciones del Carnicero
├── Gestión de usuarios
├── Configuración de roles
├── Acceso a bitácora
└── Configuración del sistema

Carnicero (Operativo)
├── Todas las funciones del Cliente
├── Gestión de productos
├── Gestión de clientes
├── Procesamiento de pedidos
├── Gestión de proveedores
└── Reportes y estadísticas

Cliente (Usuario final)
├── Ver catálogo de productos
├── Realizar pedidos propios
└── Ver historial personal
```

### 2. **Niveles de Validación**

#### 🔍 **Nivel 1: Autenticación**

- Verificación de sesión activa
- Redirección automática si no autenticado

#### 🎯 **Nivel 2: Autorización por Rol**

- Verificación de rol específico (WebMaster, Carnicero, Cliente)
- Control de visibilidad de módulos completos

#### 🔒 **Nivel 3: Autorización por Permiso**

- Verificación de permisos granulares específicos
- Control de acciones individuales dentro de módulos

### 3. **Patrones de Implementación**

#### 🏗️ **Patrón de Verificación**

```csharp
// En cada página que requiere permisos específicos
private bool VerificarPermisos()
{
    var usuario = SesionSingleton.Instancia.Usuario;
    if (usuario == null) return false;

    // Verificación por rol
    return usuario.TieneRol("WebMaster") ||
           usuario.TienePermiso("PermisoEspecifico");
}
```

#### 🔄 **Patrón de Carga Diferida**

- Roles y permisos se cargan una sola vez en el login
- Se mantienen en memoria durante toda la sesión
- No requiere consultas adicionales para verificaciones

### 4. **Beneficios del Sistema**

#### ✅ **Seguridad**

- Control granular de acceso
- Segregación clara de responsabilidades
- Auditoría completa de acciones

#### ⚡ **Rendimiento**

- Carga única en login
- Verificaciones en memoria
- Sin consultas repetitivas a BD

#### 🔧 **Mantenibilidad**

- Roles configurables desde base de datos
- Permisos granulares y extensibles
- Lógica centralizada y reutilizable

---
