# Diagrama de Clases - Carnicería CRM

## 🏗️ Arquitectura de Clases del Sistema

### Diagrama Completo de Clases

```mermaid
classDiagram
    %% ===============================
    %% BUSINESS ENTITIES (BE)
    %% ===============================

    class Usuario {
        -string nombre
        +string Id
        +string Nombre
        +string Apellido
        +string Clave
        +string Mail
        +int IntentosFallidos
        +bool Bloqueado
        +bool Activo
        +List~Familia~ Familias
        +DateTime FechaCreacion
        +DateTime FechaUltimaModificacion
        +string NombreCompleto
        --
        +bool TienePermiso(string permiso)
        +List~Patente~ ObtenerTodosLosPermisos()
        +bool TieneRol(string nombreRol)
        +List~string~ ObtenerRoles()
    }

    class Familia {
        +int Id
        +string Nombre
        +string Descripcion
        +bool Activo
        +DateTime FechaCreacion
        +List~Patente~ Patentes
        +List~Usuario~ Usuarios
        --
        +Familia()
    }

    class Patente {
        +int Id
        +string Nombre
        +string Descripcion
        +string Permiso
        +bool Activo
        +DateTime FechaCreacion
        --
        +Patente()
    }

    class Bitacora {
        +int Id
        +string Descripcion
        +string Accion
        +int IdUsuario
        +DateTime FechaHora
        +string DireccionIP
        +string UserAgent
        +Usuario Usuario
    }

    class Sesion {
        -Usuario _usuario
        +Usuario Usuario
        --
        +void Login(Usuario usu)
        +void Logout()
        +bool EstaLogueado()
        +void CerrarSesion()
    }

    class SesionSingleton {
        -static SesionSingleton _instancia
        -static Object _lock
        +static SesionSingleton Instancia
    }

    class ExcepcionLogin {
        +ResultadoLogin resultado
        --
        +ExcepcionLogin(ResultadoLogin rex)
    }

    class ResultadoLogin {
        <<enumeration>>
        MailInvalido
        ContraseñaInvalida
        UsuarioValido
        SesionYaIniciada
        NoHaySesion
        UsuarioBloqueado
    }

    %% ===============================
    %% BUSINESS LOGIC LAYER (BLL)
    %% ===============================

    class UsuarioService {
        -UsuarioDAL _usuarioDAL
        -BitacoraDAL _bitacoraDAL
        --
        +UsuarioService()
        +ResultadoLogin Login(string mail, string password)
        +void Logout()
        +List~Usuario~ Listar()
        +void Eliminar(Usuario usu)
        +void Insertar(Usuario usu)
        +void Modificar(Usuario usu)
        +void IncrementarIntentos(Usuario usu)
        +void ResetearIntentos(Usuario usu)
        +Usuario ObtenerPorId(int id)
        +Usuario ObtenerPorMail(string mail)
        +void CambiarPassword(int usuarioId, string passwordActual, string passwordNuevo)
        +void DesbloquearUsuario(int usuarioId)
        +List~Usuario~ ObtenerTodos()
        -string ObtenerDireccionIP()
        -string ObtenerUserAgent()
        -void RegistrarBitacoraSafe(Action accionBitacora)
    }

    class BitacoraService {
        -BitacoraDAL _bitacoraDAL
        --
        +BitacoraService()
        +List~Bitacora~ ObtenerTodas()
        +List~Bitacora~ ObtenerConFiltros(int? idUsuario, string accion, DateTime? fechaDesde, DateTime? fechaHasta)
        +Bitacora ObtenerPorId(int id)
        +void RegistrarActividad(string descripcion, string accion, int idUsuario)
    }

    class Encriptador {
        --
        +static string Encriptar(string texto)
    }

    %% ===============================
    %% DATA ACCESS LAYER (References)
    %% ===============================

    class UsuarioDAL {
        <<external>>
        +Usuario ObtenerPorMail(string mail)
        +Usuario ObtenerPorId(int id)
        +List~Usuario~ Listar()
        +void Insertar(Usuario usuario)
        +void Modificar(Usuario usuario)
        +void Borrar(Usuario usuario)
        +void IncrementarIntento(Usuario usuario)
        +void ResetearIntentos(Usuario usuario)
    }

    class BitacoraDAL {
        <<external>>
        +List~Bitacora~ ObtenerTodas()
        +Bitacora ObtenerPorId(int id)
        +void Registrar(Bitacora bitacora)
        +void RegistrarLogin(Usuario usuario, string ip, string userAgent)
        +void RegistrarLogout(Usuario usuario, string ip, string userAgent)
        +void RegistrarLoginFallido(string mail, string motivo, string ip, string userAgent)
        +void RegistrarBloqueoUsuario(Usuario usuario, string ip, string userAgent)
        +void RegistrarEvento(int usuarioId, string accion, string descripcion, string ip, string userAgent)
    }

    %% ===============================
    %% RELATIONSHIPS
    %% ===============================

    %% Herencia
    SesionSingleton --|> Sesion : hereda
    ExcepcionLogin --|> Exception : hereda

    %% Agregación y Composición
    Usuario ||--o{ Familia : "tiene roles"
    Familia ||--o{ Patente : "contiene permisos"
    Bitacora }o--|| Usuario : "registra acciones de"

    %% Uso/Dependencia - Services
    UsuarioService ..> Usuario : usa
    UsuarioService ..> ExcepcionLogin : lanza
    UsuarioService ..> ResultadoLogin : retorna
    UsuarioService ..> SesionSingleton : gestiona
    UsuarioService ..> Encriptador : usa
    UsuarioService --> UsuarioDAL : depende
    UsuarioService --> BitacoraDAL : depende

    BitacoraService ..> Bitacora : usa
    BitacoraService --> BitacoraDAL : depende

    %% Singleton Pattern
    SesionSingleton --> Usuario : "almacena usuario actual"

    %% Exception Pattern
    ExcepcionLogin --> ResultadoLogin : "contiene resultado"

    %% Estilos
    classDef entityClass fill:#e1f5fe,stroke:#01579b,stroke-width:2px
    classDef serviceClass fill:#f3e5f5,stroke:#4a148c,stroke-width:2px
    classDef utilityClass fill:#fff3e0,stroke:#e65100,stroke-width:2px
    classDef dalClass fill:#e8f5e8,stroke:#2e7d32,stroke-width:2px,stroke-dasharray: 5 5
    classDef enumClass fill:#fce4ec,stroke:#c2185b,stroke-width:2px

    class Usuario,Familia,Patente,Bitacora,Sesion,SesionSingleton entityClass
    class UsuarioService,BitacoraService serviceClass
    class Encriptador,ExcepcionLogin utilityClass
    class UsuarioDAL,BitacoraDAL dalClass
    class ResultadoLogin enumClass
```

## 📋 Descripción de las Clases

### 🎯 **Business Entities (BE)**

#### **Usuario**

- **Propósito**: Entidad principal que representa un usuario del sistema
- **Responsabilidades**:
  - Almacenar información personal y credenciales
  - Gestionar roles y permisos
  - Validar permisos de acceso
  - Control de intentos fallidos y bloqueos

#### **Familia**

- **Propósito**: Representa un rol o familia de permisos
- **Responsabilidades**:
  - Agrupar permisos relacionados
  - Definir niveles de acceso (WebMaster, Carnicero, Cliente)
  - Relación N:M con usuarios y patentes

#### **Patente**

- **Propósito**: Representa un permiso específico del sistema
- **Responsabilidades**:
  - Definir acciones granulares
  - Control de activación/desactivación
  - Asignación a familias de roles

#### **Bitacora**

- **Propósito**: Registro de auditoría de todas las acciones del sistema
- **Responsabilidades**:
  - Trazabilidad completa de operaciones
  - Información de contexto (IP, UserAgent)
  - Asociación con usuarios y acciones

### 🔧 **Session Management**

#### **Sesion**

- **Propósito**: Clase base para manejo de sesiones
- **Responsabilidades**:
  - Gestionar estado de autenticación
  - Login/Logout de usuarios
  - Validación de sesión activa

#### **SesionSingleton**

- **Propósito**: Implementación Singleton para sesión única
- **Responsabilidades**:
  - Garantizar una sola sesión por aplicación
  - Thread-safe con patrón de bloqueo
  - Acceso global a sesión actual

### ⚠️ **Exception Handling**

#### **ExcepcionLogin**

- **Propósito**: Excepción específica para errores de autenticación
- **Responsabilidades**:
  - Manejo tipado de errores de login
  - Clasificación de tipos de fallo

#### **ResultadoLogin**

- **Propósito**: Enumeración de posibles resultados de login
- **Valores**: MailInvalido, ContraseñaInvalida, UsuarioValido, etc.

### 🏭 **Business Logic Layer (BLL)**

#### **UsuarioService**

- **Propósito**: Lógica de negocio para gestión de usuarios
- **Responsabilidades**:
  - Autenticación y autorización
  - CRUD de usuarios
  - Gestión de contraseñas y bloqueos
  - Registro en bitácora
  - Validaciones de negocio

#### **BitacoraService**

- **Propósito**: Lógica de negocio para consulta de auditoría
- **Responsabilidades**:
  - Filtrado avanzado de registros
  - Consultas optimizadas
  - Registro de actividades

#### **Encriptador**

- **Propósito**: Utilidad para encriptación de contraseñas
- **Responsabilidades**:
  - Hasheo seguro con SHA256
  - Métodos estáticos para uso global

## 🔗 Relaciones Principales

### **1. Herencia**

- `SesionSingleton` hereda de `Sesion`
- `ExcepcionLogin` hereda de `Exception`

### **2. Agregación**

- `Usuario` tiene múltiples `Familia` (roles)
- `Familia` contiene múltiples `Patente` (permisos)
- `Bitacora` está asociada a un `Usuario`

### **3. Dependencias**

- **Services → DAL**: Todas las clases de servicio dependen de sus respectivos DAL
- **Services → Entities**: Los servicios operan sobre las entidades
- **UsuarioService → Utilities**: Usa `Encriptador` y `SesionSingleton`

### **4. Patrones Implementados**

#### **🔄 Singleton Pattern**

```csharp
SesionSingleton.Instancia.Login(usuario);
```

#### **🎯 Service Layer Pattern**

```csharp
var usuarioService = new UsuarioService();
var resultado = usuarioService.Login(email, password);
```

#### **⚠️ Exception Pattern**

```csharp
throw new ExcepcionLogin(ResultadoLogin.ContraseñaInvalida);
```

## 📊 Métricas del Sistema

### **Complejidad de Clases**

- **Entidades**: 7 clases principales
- **Servicios**: 2 clases de lógica de negocio
- **Utilidades**: 2 clases de soporte
- **DAL**: 2 clases de acceso a datos

### **Relaciones**

- **Herencia**: 2 relaciones
- **Agregación**: 3 relaciones principales
- **Dependencias**: 8 relaciones de uso

### **Métodos por Capa**

- **BE**: 15 métodos de negocio
- **BLL**: 20+ métodos de servicio
- **Utilities**: 3 métodos estáticos

---

_Diagrama generado para Carnicería CRM - Arquitectura de Clases v1.0_
