# Diagrama de Secuencia - Autenticación

## 🔐 Ciclo de Vida de Autenticación (Versión Resumida)

```mermaid
sequenceDiagram
    actor U as Usuario
    participant L as Login.aspx
    participant US as UsuarioService
    participant SS as SesionSingleton
    participant BD as Base de Datos
    participant D as Dashboard

    U->>L: Accede al sistema
    L->>L: Verificar sesión existente
    alt Sesión activa
        L->>D: Redireccionar a Dashboard
    else Sin sesión
        L->>L: Mostrar formulario login
        U->>L: Ingresa credenciales
        L->>US: Validar credenciales
        US->>BD: Consultar usuario y roles
        alt Error de login o usuario bloqueado
            US-->>L: Mostrar error/bloqueo
            L->>U: Mensaje de error
        else Login exitoso
            US->>SS: Crear sesión y cargar permisos
            SS->>BD: Cargar datos usuario/roles/perm.
            US->>L: Login exitoso
            L->>D: Redireccionar a Dashboard
        end
    end

    D->>SS: Verificar sesión
    SS-->>D: Datos sesión válidos
    D->>D: Cargar módulos según rol

    U->>D: Cerrar sesión
    D->>SS: Logout y limpiar sesión
    SS->>L: Redireccionar a Login
```

## 🏗️ Diagrama de Autenticación por Capas Arquitectónicas

```mermaid
sequenceDiagram
    actor U as Usuario
    participant GUI as GUI Layer
    participant BE as BE Layer
    participant BLL as BLL Layer
    participant DAL as DAL Layer

    Note over GUI: Login.aspx / Login.aspx.cs
    Note over BE: SesionSingleton.cs, Usuario.cs, ResultadoLogin.cs
    Note over BLL: UsuarioService.cs, BitacoraService.cs, Encriptador.cs
    Note over DAL: UsuarioDAL.cs, BitacoraDAL.cs

    U->>GUI: Accede al sistema
    GUI->>BE: SesionSingleton.Instancia
    alt Sesión activa
        BE-->>GUI: Usuario != null
        GUI->>GUI: Response.Redirect("Dashboard.aspx")
    else Sin sesión
        BE-->>GUI: Usuario == null
        GUI->>GUI: Mostrar formulario login
        U->>GUI: Ingresa credenciales (btnEntrar_Click)
        GUI->>BLL: UsuarioService.Login(mail, pass)
        BLL->>DAL: UsuarioDAL.ObtenerPorMail(mail)

        alt Usuario no existe
            DAL-->>BLL: return null
            BLL->>DAL: BitacoraDAL.RegistrarLoginFallido()
            BLL-->>GUI: ExcepcionLogin(MailInvalido)
            GUI->>U: Mostrar error mail inválido
        else Usuario bloqueado
            DAL-->>BLL: Usuario.Bloqueado == true
            BLL->>DAL: BitacoraDAL.RegistrarLoginFallido()
            BLL-->>GUI: ExcepcionLogin(UsuarioBloqueado)
            GUI->>U: Mostrar error usuario bloqueado
        else Contraseña incorrecta
            DAL-->>BLL: return Usuario
            BLL->>BLL: Encriptador.Encriptar(password)
            BLL->>BLL: Comparar con Usuario.Clave
            alt Máximo intentos alcanzado
                BLL->>DAL: UsuarioDAL.Modificar(Usuario.Bloqueado = true)
                BLL->>DAL: BitacoraDAL.RegistrarBloqueoUsuario()
            else Incrementar intentos
                BLL->>DAL: UsuarioDAL.IncrementarIntento()
            end
            BLL->>DAL: BitacoraDAL.RegistrarLoginFallido()
            BLL-->>GUI: ExcepcionLogin(ContraseñaInvalida)
            GUI->>U: Mostrar error contraseña inválida
        else Login exitoso
            DAL-->>BLL: return Usuario válido
            BLL->>BLL: Encriptador.Encriptar(password) == Usuario.Clave
            BLL->>DAL: UsuarioDAL.ResetearIntentos()
            BLL->>DAL: UsuarioDAL.CargarFamiliasYPermisos()
            BLL->>BE: SesionSingleton.Instancia.Login(Usuario)
            BLL->>DAL: BitacoraDAL.RegistrarLogin()
            BLL-->>GUI: ResultadoLogin.UsuarioValido
            GUI->>GUI: Response.Redirect("Dashboard.aspx")
        end
    end

    Note over GUI, DAL: Logout Process
    U->>GUI: Cerrar sesión
    GUI->>BLL: UsuarioService.Logout()
    BLL->>BE: SesionSingleton.Instancia.Usuario
    BLL->>DAL: BitacoraDAL.RegistrarLogout()
    BLL->>BE: SesionSingleton.Instancia.Logout()
    BLL-->>GUI: Logout exitoso
    GUI->>GUI: Response.Redirect("Login.aspx")
```

### 📋 Archivos por Capa

#### GUI Layer (Interfaz de Usuario)

- **Login.aspx**: Formulario web de autenticación
- **Login.aspx.cs**: Código behind del formulario
- **Dashboard.aspx**: Interfaz principal del sistema
- **Dashboard.aspx.cs**: Código behind del dashboard

#### BE Layer (Business Entities)

- **Usuario.cs**: Entidad que representa un usuario del sistema
- **SesionSingleton.cs**: Patrón Singleton para gestión de sesión
- **Sesion.cs**: Clase base para manejo de sesión
- **ResultadoLogin.cs**: Enumeración de resultados de autenticación
- **ExcepcionLogin.cs**: Excepción personalizada para errores de login

#### BLL Layer (Business Logic Layer)

- **UsuarioService.cs**: Servicio que contiene toda la lógica de negocio de usuarios
- **BitacoraService.cs**: Servicio para registro de auditoría
- **Encriptador.cs**: Utilidad para encriptación de contraseñas

#### DAL Layer (Data Access Layer)

- **UsuarioDAL.cs**: Acceso a datos de usuarios en base de datos
- **BitacoraDAL.cs**: Acceso a datos de bitácora/auditoría

### 🔄 Flujo de Métodos Principales

1. **Verificación de Sesión**: `SesionSingleton.Instancia.EstaLogueado()`
2. **Validación de Credenciales**: `UsuarioService.Login(mail, password)`
3. **Consulta de Usuario**: `UsuarioDAL.ObtenerPorMail(mail)`
4. **Encriptación**: `Encriptador.Encriptar(password)`
5. **Gestión de Intentos**: `UsuarioDAL.IncrementarIntento()` / `UsuarioDAL.ResetearIntentos()`
6. **Creación de Sesión**: `SesionSingleton.Instancia.Login(Usuario)`
7. **Registro de Auditoría**: `BitacoraDAL.RegistrarLogin()` / `BitacoraDAL.RegistrarLoginFallido()`
8. **Cierre de Sesión**: `UsuarioService.Logout()` -> `SesionSingleton.Instancia.Logout()`

## 🔍 Descripción Resumida

- **Validación de Login**: Agrupa la verificación de credenciales, control de intentos y bloqueo.
- **Carga de datos**: Incluye la creación de sesión y la carga de roles y permisos.
- **Logout**: Se resume en un solo paso.

Este diagrama muestra el flujo principal de autenticación de manera compacta y fácil de leer, ideal para documentación técnica y presentaciones.

## 🔍 Descripción de Componentes

### 1. Participantes

- **Usuario**: Interactúa con el sistema
- **Login.aspx**: Interfaz de autenticación
- **UsuarioService**: Lógica de negocio de usuarios
- **SesionSingleton**: Gestión de sesión única
- **Base de Datos**: Almacenamiento persistente
- **BitacoraService**: Registro de auditoría
- **Dashboard**: Interfaz principal del sistema

### 2. Flujos Principales

#### Autenticación Exitosa

1. Usuario ingresa credenciales
2. Sistema valida credenciales
3. Se crea sesión Singleton
4. Se cargan roles y permisos
5. Se registra en bitácora
6. Redirección a Dashboard

#### Autenticación Fallida

1. Usuario ingresa credenciales
2. Sistema valida credenciales
3. Se incrementan intentos fallidos
4. Se muestra mensaje de error
5. Se registra en bitácora

#### Cierre de Sesión

1. Usuario solicita logout
2. Sistema registra en bitácora
3. Se limpia sesión Singleton
4. Redirección a Login

## 🔒 Características de Seguridad

1. **Control de Intentos**

   - Máximo 3 intentos fallidos
   - Bloqueo automático de cuenta
   - Registro en bitácora

2. **Gestión de Sesión**

   - Patrón Singleton
   - Timeout automático
   - Limpieza de recursos

3. **Auditoría**

   - Registro de login/logout
   - Captura de IP y UserAgent
   - Trazabilidad completa

4. **Roles y Permisos**
   - Verificación de roles
   - Carga de permisos
   - Control de acceso

## 📋 Caso de Uso - Login

```mermaid
graph TB
    subgraph "Caso de Uso: Login"
        A[Inicio] --> B{¿Sesión Activa?}
        B -->|Sí| C[Redireccionar a Dashboard]
        B -->|No| D[Mostrar Formulario Login]

        D --> E[Ingresar Credenciales]
        E --> F{Validar Credenciales}

        F -->|Inválidas| G[Incrementar Intentos]
        G --> H{¿Máximo Intentos?}
        H -->|Sí| I[Bloquear Usuario]
        H -->|No| J[Mostrar Error]
        J --> E

        F -->|Válidas| K[Reiniciar Intentos]
        K --> L[Cargar Roles y Permisos]
        L --> M[Crear Sesión]
        M --> N[Registrar en Bitácora]
        N --> O[Redireccionar a Dashboard]

        I --> P[Mostrar Mensaje Bloqueo]
    end
```

### Descripción del Caso de Uso

#### Actor Principal

- Usuario del sistema

#### Precondiciones

1. El usuario tiene una cuenta válida en el sistema
2. El usuario no está bloqueado
3. El usuario no tiene una sesión activa

#### Flujo Principal

1. El usuario accede al sistema
2. El sistema verifica si existe una sesión activa
3. Si no hay sesión, se muestra el formulario de login
4. El usuario ingresa sus credenciales
5. El sistema valida las credenciales
6. Si son válidas:
   - Se reinician los intentos fallidos
   - Se cargan roles y permisos
   - Se crea la sesión
   - Se registra en bitácora
   - Se redirecciona al Dashboard

#### Flujos Alternativos

1. **Credenciales Inválidas**

   - Se incrementa contador de intentos
   - Se muestra mensaje de error
   - Se permite reintentar

2. **Usuario Bloqueado**

   - Se muestra mensaje de bloqueo
   - Se requiere intervención administrativa

3. **Sesión Activa**
   - Se redirecciona directamente al Dashboard

#### Postcondiciones

1. Sesión creada exitosamente
2. Usuario autenticado en el sistema
3. Roles y permisos cargados
4. Registro en bitácora generado

#### Reglas de Negocio

1. Máximo 3 intentos fallidos antes del bloqueo
2. Bloqueo requiere desbloqueo administrativo
3. Sesión expira después de 30 minutos de inactividad
4. Registro obligatorio en bitácora de todos los intentos
