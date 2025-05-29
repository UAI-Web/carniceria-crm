# Diagrama de Secuencia - Gestión de Usuarios

## 👥 Sistema de Administración de Usuarios

### Crear Nuevo Usuario

```mermaid
sequenceDiagram
    participant W as WebMaster
    participant GU as GestionUsuarios.aspx
    participant US as UsuarioService
    participant UD as UsuarioDAL
    participant BD as Base de Datos
    participant BIT as BitacoraDAL
    participant ENC as Encriptador

    W->>GU: Click "Nuevo Usuario"
    GU->>GU: VerificarPermisos() - Solo WebMaster
    GU->>GU: Mostrar formulario vacío

    W->>GU: Completa datos del usuario
    W->>GU: Email, Nombre, Apellido, Contraseña inicial
    W->>GU: Selecciona roles a asignar
    W->>GU: Click "Guardar"

    GU->>GU: Validar formulario
    GU->>GU: ValidateField(email, nombre, apellido)

    alt Datos inválidos
        GU-->>W: Mostrar errores de validación
    end

    GU->>US: CrearUsuario(usuario, rolesSeleccionados)

    Note over US: Verificaciones de negocio
    US->>UD: VerificarEmailUnico(email)
    UD->>BD: SELECT COUNT(*) FROM Usuarios WHERE Mail = @Email
    BD-->>UD: Resultado conteo
    UD-->>US: emailExiste = true/false

    alt Email ya existe
        US-->>GU: Exception("Email ya registrado")
        GU-->>W: "El email ya está en uso"
    end

    US->>ENC: Encriptar(contraseña)
    ENC-->>US: Contraseña encriptada

    US->>UD: Insertar(usuario)
    UD->>BD: INSERT INTO Usuarios (Nombre, Apellido, Mail, Clave, etc.)
    BD-->>UD: Usuario creado con ID

    loop Para cada rol seleccionado
        US->>UD: AsignarRol(usuarioId, rolId)
        UD->>BD: INSERT INTO UsuarioFamilia (UsuarioId, FamiliaId, Activo)
    end

    US->>BIT: RegistrarCreacionUsuario(usuarioCreado)
    BIT->>BD: INSERT INTO Bitacora (Accion: "Usuario creado")

    US-->>GU: Usuario creado exitosamente
    GU->>GU: LimpiarFormulario()
    GU->>GU: RefrescarGrilla()
    GU-->>W: "Usuario creado y roles asignados correctamente"
```

### Modificar Usuario Existente

```mermaid
sequenceDiagram
    participant W as WebMaster
    participant GU as GestionUsuarios.aspx
    participant US as UsuarioService
    participant UD as UsuarioDAL
    participant BD as Base de Datos
    participant BIT as BitacoraDAL

    W->>GU: Selecciona usuario de la grilla
    W->>GU: Click "Editar"

    GU->>UD: ObtenerPorId(usuarioId)
    UD->>BD: SELECT * FROM Usuarios WHERE Id = @Id
    BD-->>UD: Datos del usuario
    UD->>UD: CargarFamiliasYPermisos(usuario)
    UD-->>GU: Usuario completo con roles

    GU->>GU: CargarDatosEnFormulario(usuario)
    GU->>GU: PreseleccionarRoles(usuario.Familias)
    GU-->>W: Formulario con datos actuales

    W->>GU: Modifica datos (nombre, apellido, etc.)
    W->>GU: Cambia asignación de roles
    W->>GU: Click "Actualizar"

    GU->>GU: ValidarCambios()
    GU->>US: ActualizarUsuario(usuarioModificado, nuevosRoles)

    US->>UD: Modificar(usuario)
    UD->>BD: UPDATE Usuarios SET Nombre=@Nombre, Apellido=@Apellido, etc.

    Note over US: Actualizar roles
    US->>UD: EliminarTodosLosRoles(usuarioId)
    UD->>BD: DELETE FROM UsuarioFamilia WHERE UsuarioId = @Id

    loop Para cada nuevo rol
        US->>UD: AsignarRol(usuarioId, rolId)
        UD->>BD: INSERT INTO UsuarioFamilia
    end

    US->>BIT: RegistrarModificacionUsuario(usuario, cambios)
    BIT->>BD: INSERT INTO Bitacora (Accion: "Usuario modificado")

    US-->>GU: Usuario actualizado
    GU->>GU: RefrescarGrilla()
    GU-->>W: "Usuario actualizado correctamente"
```

### Bloquear/Desbloquear Usuario

```mermaid
sequenceDiagram
    participant W as WebMaster
    participant GU as GestionUsuarios.aspx
    participant US as UsuarioService
    participant UD as UsuarioDAL
    participant BD as Base de Datos
    participant BIT as BitacoraDAL

    W->>GU: Selecciona usuario bloqueado/desbloqueado
    W->>GU: Click "Cambiar Estado"

    GU->>GU: ConfirmarAccion("¿Está seguro?")
    W->>GU: Confirma acción

    GU->>US: CambiarEstadoBloqueo(usuarioId, nuevoEstado)
    US->>UD: ObtenerPorId(usuarioId)
    UD-->>US: Usuario actual

    US->>UD: CambiarBloqueo(usuario, bloqueado)
    UD->>BD: UPDATE Usuarios SET Bloqueado = @Bloqueado, IntentosFallidos = 0

    alt Desbloqueando usuario
        US->>UD: ResetearIntentos(usuario)
        UD->>BD: UPDATE Usuarios SET IntentosFallidos = 0
        US->>BIT: RegistrarDesbloqueoUsuario(usuario)
        BIT->>BD: INSERT INTO Bitacora (Accion: "Usuario desbloqueado")
    else Bloqueando usuario
        US->>BIT: RegistrarBloqueoManualUsuario(usuario)
        BIT->>BD: INSERT INTO Bitacora (Accion: "Usuario bloqueado manualmente")
    end

    US-->>GU: Estado cambiado
    GU->>GU: RefrescarGrilla()
    GU-->>W: "Estado del usuario actualizado"
```

### Resetear Contraseña

```mermaid
sequenceDiagram
    participant W as WebMaster
    participant GU as GestionUsuarios.aspx
    participant US as UsuarioService
    participant UD as UsuarioDAL
    participant BD as Base de Datos
    participant BIT as BitacoraDAL
    participant ENC as Encriptador

    W->>GU: Selecciona usuario
    W->>GU: Click "Resetear Contraseña"

    GU->>GU: IngresarNuevaContraseña()
    W->>GU: Ingresa nueva contraseña
    W->>GU: Confirma nueva contraseña

    GU->>GU: ValidarContraseña(política de seguridad)

    alt Contraseña no cumple política
        GU-->>W: "La contraseña debe cumplir los requisitos mínimos"
    end

    GU->>US: ResetearContraseña(usuarioId, nuevaContraseña)
    US->>ENC: Encriptar(nuevaContraseña)
    ENC-->>US: Contraseña encriptada

    US->>UD: CambiarContraseña(usuarioId, contraseñaEncriptada)
    UD->>BD: UPDATE Usuarios SET Clave = @NuevaClaveEncriptada

    US->>UD: ResetearIntentos(usuario) - Por seguridad
    UD->>BD: UPDATE Usuarios SET IntentosFallidos = 0, Bloqueado = 0

    US->>BIT: RegistrarReseteoContraseña(usuario, webmaster)
    BIT->>BD: INSERT INTO Bitacora (Accion: "Contraseña reseteada por admin")

    US-->>GU: Contraseña actualizada
    GU-->>W: "Contraseña reseteada exitosamente"
```

### Eliminar Usuario (Soft Delete)

```mermaid
sequenceDiagram
    participant W as WebMaster
    participant GU as GestionUsuarios.aspx
    participant US as UsuarioService
    participant UD as UsuarioDAL
    participant BD as Base de Datos
    participant BIT as BitacoraDAL

    W->>GU: Selecciona usuario
    W->>GU: Click "Eliminar"

    GU->>GU: VerificarDependencias(usuarioId)
    GU->>BD: Verificar registros relacionados (pedidos, etc.)
    BD-->>GU: Información de dependencias

    alt Usuario tiene dependencias críticas
        GU-->>W: "No se puede eliminar: usuario tiene registros asociados"
    end

    GU->>GU: ConfirmarEliminacion("¿Está seguro? Esta acción es irreversible")
    W->>GU: Confirma eliminación

    GU->>US: EliminarUsuario(usuarioId)

    Note over US: Soft delete - mantener integridad
    US->>UD: DesactivarUsuario(usuarioId)
    UD->>BD: UPDATE Usuarios SET Activo = 0, FechaUltimaModificacion = GETDATE()

    US->>UD: DesactivarRoles(usuarioId)
    UD->>BD: UPDATE UsuarioFamilia SET Activo = 0 WHERE UsuarioId = @Id

    US->>BIT: RegistrarEliminacionUsuario(usuario, webmaster)
    BIT->>BD: INSERT INTO Bitacora (Accion: "Usuario eliminado")

    US-->>GU: Usuario eliminado (desactivado)
    GU->>GU: RefrescarGrilla()
    GU-->>W: "Usuario eliminado del sistema"
```

### Búsqueda y Filtrado de Usuarios

```mermaid
sequenceDiagram
    participant W as WebMaster
    participant GU as GestionUsuarios.aspx
    participant US as UsuarioService
    participant UD as UsuarioDAL
    participant BD as Base de Datos

    W->>GU: Ingresa criterios de búsqueda
    W->>GU: Nombre, email, rol, estado
    W->>GU: Click "Buscar"

    GU->>GU: ValidarCriteriosBusqueda()
    GU->>US: BuscarUsuarios(criterios)

    US->>UD: FiltrarUsuarios(nombre, email, rol, activo, bloqueado)
    UD->>BD: Query SQL con múltiples WHERE condicionales

    Note over BD: Consulta optimizada con índices
    BD->>BD: SELECT u.*, f.Nombre as Rol<br/>FROM Usuarios u<br/>LEFT JOIN UsuarioFamilia uf ON u.Id = uf.UsuarioId<br/>LEFT JOIN Familias f ON uf.FamiliaId = f.Id<br/>WHERE [criterios aplicados]<br/>ORDER BY u.FechaCreacion DESC

    BD-->>UD: Resultados filtrados
    UD-->>US: Lista de usuarios encontrados
    US-->>GU: Usuarios que coinciden

    GU->>GU: CargarGrilla(usuariosFiltrados)
    GU->>GU: ActualizarContadores(total, activos, bloqueados)
    GU-->>W: Resultados de búsqueda mostrados
```

## 👥 Características del Sistema de Gestión

### 1. **Operaciones CRUD Completas**

- ✅ **Create**: Creación con validaciones y encriptación
- ✅ **Read**: Consulta con roles y permisos cargados
- ✅ **Update**: Modificación con auditoría completa
- ✅ **Delete**: Eliminación lógica (soft delete)

### 2. **Seguridad Implementada**

- 🔐 **Encriptación**: Contraseñas hasheadas
- 🛡️ **Validación**: Email único y políticas de contraseña
- 🔒 **Autorización**: Solo WebMaster puede gestionar usuarios
- 📝 **Auditoría**: Todas las operaciones registradas en bitácora

### 3. **Gestión de Estados**

- ✅ **Activo/Inactivo**: Control de acceso al sistema
- 🚫 **Bloqueado/Desbloqueado**: Gestión de seguridad
- 🔄 **Reset de Intentos**: Limpieza automática en operaciones admin

### 4. **Asignación de Roles**

- 👤 **Múltiples Roles**: Un usuario puede tener varios roles
- 🔄 **Actualización Dinámica**: Cambio de roles sin recrear usuario
- 📊 **Herencia de Permisos**: Roles se reflejan inmediatamente en permisos

### 5. **Búsqueda y Filtrado**

- 🔍 **Criterios Múltiples**: Nombre, email, rol, estado
- ⚡ **Consultas Optimizadas**: Índices en campos de búsqueda
- 📄 **Paginación**: Manejo eficiente de grandes listas

### 6. **Integridad de Datos**

- 🔗 **Verificación de Dependencias**: Antes de eliminar
- 📊 **Soft Delete**: Preservación de datos históricos
- 🔄 **Transacciones**: Operaciones atómicas garantizadas

---

_Diagrama generado para Carnicería CRM - Gestión de Usuarios_
