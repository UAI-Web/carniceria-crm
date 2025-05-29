# Diagrama de Secuencia - Login y Autenticación

## 🔐 Proceso de Autenticación Completo

### Login Exitoso con Carga de Roles

```mermaid
sequenceDiagram
    participant U as Usuario
    participant LP as Login.aspx
    participant US as UsuarioService
    participant UD as UsuarioDAL
    participant BD as Base de Datos
    participant BIT as BitacoraDAL
    participant SS as SesionSingleton
    participant D as Dashboard.aspx

    U->>LP: Ingresa credenciales (email, password)
    LP->>LP: Validar formulario
    LP->>US: Login(email, password)

    Note over US: Verificaciones de seguridad
    US->>US: Verificar sesión no existente
    US->>UD: ObtenerPorMail(email)
    UD->>BD: SELECT Usuario WHERE Mail = @Mail
    BD-->>UD: Datos del usuario

    alt Usuario no existe
        UD-->>US: null
        US->>BIT: RegistrarLoginFallido("Mail inválido")
        BIT->>BD: INSERT Bitacora
        US-->>LP: ExcepcionLogin.MailInvalido
        LP-->>U: "Usuario no encontrado"
    end

    alt Usuario bloqueado
        UD-->>US: Usuario con Bloqueado = true
        US->>BIT: RegistrarLoginFallido("Usuario bloqueado")
        BIT->>BD: INSERT Bitacora
        US-->>LP: ExcepcionLogin.UsuarioBloqueado
        LP-->>U: "Usuario bloqueado"
    end

    alt Contraseña incorrecta
        US->>US: Encriptar(password) != usuario.Clave
        alt Tercer intento fallido
            US->>UD: Bloquear usuario + Reset intentos
            UD->>BD: UPDATE Usuario SET Bloqueado = 1
            US->>BIT: RegistrarBloqueoUsuario()
        else
            US->>UD: IncrementarIntento()
            UD->>BD: UPDATE Usuario SET IntentosFallidos++
        end
        US->>BIT: RegistrarLoginFallido("Contraseña inválida")
        BIT->>BD: INSERT Bitacora
        US-->>LP: ExcepcionLogin.ContraseñaInvalida
        LP-->>U: "Contraseña incorrecta"
    end

    Note over UD: Login exitoso - Cargar roles y permisos
    UD->>UD: CargarFamiliasYPermisos(usuario)
    UD->>BD: Query JOIN (Usuarios→UsuarioFamilia→Familias→FamiliaPatente→Patentes)
    BD-->>UD: Familias y Patentes del usuario
    UD->>UD: Mapear Familia.Patentes
    UD-->>US: Usuario con Familias cargadas

    US->>UD: ResetearIntentos() si > 0
    UD->>BD: UPDATE Usuario SET IntentosFallidos = 0

    US->>SS: Login(usuario)
    SS->>SS: Almacenar usuario en sesión

    US->>BIT: RegistrarLogin(usuario, IP, UserAgent)
    BIT->>BD: INSERT Bitacora (Accion: "Login exitoso")

    US-->>LP: ResultadoLogin.UsuarioValido
    LP->>D: Response.Redirect("Dashboard.aspx")

    D->>SS: Obtener usuario de sesión
    SS-->>D: Usuario con roles cargados
    D->>D: CargarInformacionUsuario()
    D->>D: ObtenerRolPrincipal()
    D->>D: ConfigurarVisibilidadPorRol()
    D-->>U: Dashboard personalizado según rol
```

### Logout con Registro en Bitácora

```mermaid
sequenceDiagram
    participant U as Usuario
    participant D as Dashboard/Bitacora
    participant US as UsuarioService
    participant SS as SesionSingleton
    participant BIT as BitacoraDAL
    participant BD as Base de Datos
    participant LP as Login.aspx

    U->>D: Click "Cerrar Sesión"
    D->>US: Logout()

    alt No hay sesión activa
        US->>US: !SesionSingleton.EstaLogueado()
        US-->>D: ExcepcionLogin.NoHaySesion
        D->>D: Redirect a Login.aspx
    end

    US->>SS: Obtener usuario actual
    SS-->>US: Usuario logueado

    US->>BIT: RegistrarLogout(usuario, IP, UserAgent)
    BIT->>BD: INSERT Bitacora (Accion: "Logout")

    US->>SS: Logout()
    SS->>SS: Limpiar sesión (_usuario = null)

    D->>D: Session.Clear() + Session.Abandon()
    D->>LP: Response.Redirect("Login.aspx")
    LP-->>U: Página de login
```

### Cambio de Contraseña

```mermaid
sequenceDiagram
    participant U as Usuario
    participant CP as CambiarPassword.aspx
    participant US as UsuarioService
    participant UD as UsuarioDAL
    participant BD as Base de Datos
    participant BIT as BitacoraDAL

    U->>CP: Ingresa contraseña actual y nueva
    CP->>CP: Validar formulario
    CP->>US: CambiarContraseña(actual, nueva)

    US->>US: Verificar usuario logueado
    US->>US: Verificar contraseña actual
    US->>US: Encriptar nueva contraseña

    US->>UD: ModificarContraseña(usuario)
    UD->>BD: UPDATE Usuario SET Clave = @NuevaClave

    US->>BIT: RegistrarCambioContraseña(usuario)
    BIT->>BD: INSERT Bitacora (Accion: "Cambio de contraseña")

    US-->>CP: Éxito
    CP-->>U: "Contraseña actualizada exitosamente"
```

## 🔑 Puntos Clave del Sistema

### 1. **Seguridad Implementada**

- ✅ Encriptación de contraseñas
- ✅ Control de intentos fallidos (máx. 3)
- ✅ Bloqueo automático de usuarios
- ✅ Validación de sesiones únicas
- ✅ Registro de auditoría completo

### 2. **Carga de Roles y Permisos**

- ✅ Automática en login exitoso
- ✅ JOIN optimizado con todas las tablas relacionadas
- ✅ Mapeo completo Familias → Patentes
- ✅ Control de activación (Activo = 1)

### 3. **Auditoría y Trazabilidad**

- ✅ Login exitoso
- ✅ Login fallido (con motivo)
- ✅ Logout
- ✅ Bloqueo de usuario
- ✅ Cambio de contraseña
- ✅ IP y UserAgent capturados

### 4. **Manejo de Errores**

- ✅ Excepciones específicas por tipo de error
- ✅ Mensajes amigables al usuario
- ✅ Logging de errores para diagnóstico
- ✅ Fallback seguro en caso de fallas

### 5. **Experiencia de Usuario**

- ✅ Dashboard personalizado por rol
- ✅ Visibilidad de módulos según permisos
- ✅ Mensajes informativos claros
- ✅ Redirección automática post-login

---
