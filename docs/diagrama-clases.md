# Diagrama de Clases - Carnicería CRM

## 🏗️ Arquitectura de Clases del Sistema

### Diagrama de Clases del Dominio

```mermaid
---
config:
  theme: base
  themeVariables:
    fontFamily: ''
    fontSize: 16px
    diagramPadding: 20
---
classDiagram
direction TB
    class Usuario {
	    - string nombre
	    + string Id
	    + string Nombre
	    + string Apellido
	    + string Clave
	    + string Mail
	    + int IntentosFallidos
	    + bool Bloqueado
	    + bool Activo
	    + List~Familia~ Familias
	    + DateTime FechaCreacion
	    + DateTime FechaUltimaModificacion
	    + string NombreCompleto()
	    + bool TienePermiso(string permiso)
	    + List~Patente~ ObtenerTodosLosPermisos()
	    + bool TieneRol(string nombreRol)
	    + List~string~ ObtenerRoles()
	    + ResultadoLogin Login(string mail, string password)
	    + void Logout()
	    + List~Usuario~ Listar()
	    + void Eliminar()
	    + void Insertar()
	    + void Modificar()
	    + void IncrementarIntentos()
	    + void ResetearIntentos()
	    + Usuario ObtenerPorId(int id)
	    + Usuario ObtenerPorMail(string mail)
	    + void CambiarPassword(string passwordActual, string passwordNuevo)
	    + void DesbloquearUsuario()
	    + List~Usuario~ ObtenerTodos()
    }
    class Familia {
	    + int Id
	    + string Nombre
	    + string Descripcion
	    + bool Activo
	    + DateTime FechaCreacion
	    + List~Patente~ Patentes
	    + List~Usuario~ Usuarios
	    + Familia()
	    + void AgregarPatente(Patente patente)
	    + void RemoverPatente(Patente patente)
	    + void AgregarUsuario(Usuario usuario)
	    + void RemoverUsuario(Usuario usuario)
	    + bool TienePatente(string permiso)
    }
    class Patente {
	    + int Id
	    + string Nombre
	    + string Descripcion
	    + string Permiso
	    + bool Activo
	    + DateTime FechaCreacion
	    + Patente()
	    + void Activar()
	    + void Desactivar()
	    + bool EsValida()
    }
    class Bitacora {
	    + int Id
	    + string Descripcion
	    + string Accion
	    + int IdUsuario
	    + DateTime FechaHora
	    + string DireccionIP
	    + string UserAgent
	    + Usuario Usuario
	    + List~Bitacora~ ObtenerTodas()
	    + List~Bitacora~ ObtenerConFiltros(int? idUsuario, string accion, DateTime? desde, DateTime? hasta)
	    + Bitacora ObtenerPorId(int id)
	    + void RegistrarActividad(string descripcion, string accion, int idUsuario)
	    + void RegistrarLogin(Usuario usuario, string ip, string userAgent)
	    + void RegistrarLogout(Usuario usuario, string ip, string userAgent)
	    + void RegistrarLoginFallido(string mail, string motivo, string ip, string userAgent)
    }
    class Sesion {
	    - Usuario _usuario
	    + Usuario Usuario
	    + void Login(Usuario usu)
	    + void Logout()
	    + bool EstaLogueado()
	    + void CerrarSesion()
    }
    class SesionSingleton {
	    - static SesionSingleton _instancia
	    - static Object _lock
	    + static SesionSingleton Instancia
	    + static SesionSingleton ObtenerInstancia()
    }
    class ExcepcionLogin {
	    + ResultadoLogin Resultado
	    + ExcepcionLogin(ResultadoLogin resultado)
	    + string ObtenerMensaje()
    }
    class ResultadoLogin {
	    MailInvalido
	    ContrasenaInvalida
	    UsuarioValido
	    SesionYaIniciada
	    NoHaySesion
	    UsuarioBloqueado
    }
    class Encriptador {
	    + static string Encriptar(string texto)
	    + static bool ValidarPassword(string password, string hash)
	    + static string GenerarSalt()
    }

	<<enumeration>> ResultadoLogin

    SesionSingleton --|> Sesion
    Usuario "1" o-- "*" Familia : tiene
    Familia "1" o-- "*" Patente : contiene
    Bitacora "1" o-- "1" Usuario : registra
    Usuario ..> ExcepcionLogin : lanza
    Usuario ..> ResultadoLogin : retorna
    Usuario ..> Encriptador : usa
    Usuario ..> SesionSingleton : gestiona
    Bitacora ..> Usuario : referencia
    SesionSingleton --> Usuario : almacena
    ExcepcionLogin --> ResultadoLogin : contiene
```

## 📋 Descripción de las Clases

### 🎯 **Entidades Principales**

#### **Usuario**

- **Propósito**: Entidad central que representa un usuario del sistema
- **Responsabilidades**:
  - Gestionar información personal y credenciales
  - Autenticación y autorización (métodos de login/logout)
  - Administrar roles y permisos
  - Control de intentos fallidos y bloqueos
  - Operaciones CRUD sobre usuarios

#### **Familia**

- **Propósito**: Representa un rol o familia de permisos
- **Responsabilidades**:
  - Agrupar permisos relacionados
  - Definir niveles de acceso (WebMaster, Carnicero, Cliente)
  - Gestionar la relación N:M con usuarios y patentes
  - Validar permisos dentro del rol

#### **Patente**

- **Propósito**: Representa un permiso específico del sistema
- **Responsabilidades**:
  - Definir acciones granulares del sistema
  - Control de activación/desactivación
  - Validación de permisos

#### **Bitacora**

- **Propósito**: Registro de auditoría de todas las acciones del sistema
- **Responsabilidades**:
  - Registrar trazabilidad completa de operaciones
  - Filtrado y consulta de registros históricos
  - Capturar información de contexto (IP, UserAgent)
  - Generar reportes de auditoría

### 🔧 **Gestión de Sesiones**

#### **Sesion**

- **Propósito**: Clase base para manejo de sesiones de usuario
- **Responsabilidades**:
  - Gestionar estado de autenticación
  - Controlar login/logout
  - Validar sesión activa

#### **SesionSingleton**

- **Propósito**: Implementación Singleton para sesión única global
- **Responsabilidades**:
  - Garantizar una sola sesión por aplicación
  - Thread-safe con patrón de bloqueo
  - Acceso global a la sesión actual

### ⚠️ **Manejo de Excepciones**

#### **ExcepcionLogin**

- **Propósito**: Excepción específica para errores de autenticación
- **Responsabilidades**:
  - Manejo tipado de errores de login
  - Clasificación específica de tipos de fallo
  - Mensajes de error descriptivos

#### **ResultadoLogin**

- **Propósito**: Enumeración de posibles resultados de autenticación
- **Valores**: MailInvalido, ContraseñaInvalida, UsuarioValido, SesionYaIniciada, NoHaySesion, UsuarioBloqueado

### 🛠️ **Utilidades**

#### **Encriptador**

- **Propósito**: Utilidad para encriptación y validación de contraseñas
- **Responsabilidades**:
  - Hasheo seguro con SHA256
  - Validación de contraseñas
  - Generación de sales criptográficas

## 🔗 Relaciones y Patrones

### **1. Relaciones de Herencia**

- `SesionSingleton` hereda de `Sesion` - Especialización del manejo de sesiones
- `ExcepcionLogin` hereda de `Exception` - Excepción especializada

### **2. Relaciones de Agregación**

- `Usuario` ↔ `Familia` (N:M) - Un usuario puede tener múltiples roles
- `Familia` ↔ `Patente` (N:M) - Un rol contiene múltiples permisos
- `Bitacora` → `Usuario` (N:1) - Cada registro pertenece a un usuario

### **3. Dependencias de Uso**

- `Usuario` usa `Encriptador` para manejar contraseñas
- `Usuario` usa `SesionSingleton` para gestionar sesiones
- `Usuario` lanza `ExcepcionLogin` en errores de autenticación
- `Bitacora` referencia `Usuario` para los registros

### **4. Patrones Implementados**

#### **🔄 Singleton Pattern**

```csharp
SesionSingleton.Instancia.Login(usuario);
```

#### **⚠️ Exception Pattern**

```csharp
throw new ExcepcionLogin(ResultadoLogin.ContraseñaInvalida);
```

#### **🎯 Domain Model Pattern**

```csharp
if (usuario.TienePermiso("GestionarUsuarios")) {
    // Permitir acceso
}
```

## 📊 Métricas del Dominio

### **Entidades del Dominio**

- **Principales**: 4 entidades de negocio (Usuario, Familia, Patente, Bitacora)
- **Soporte**: 2 clases de sesión
- **Utilidades**: 1 clase de encriptación
- **Excepciones**: 1 excepción personalizada + 1 enum

### **Complejidad por Clase**

- **Usuario**: 16 métodos (más compleja - entidad central)
- **Bitacora**: 7 métodos (gestión de auditoría)
- **Familia**: 5 métodos (gestión de roles)
- **Patente**: 3 métodos (gestión de permisos)

### **Relaciones**

- **Herencia**: 2 relaciones
- **Agregación**: 3 relaciones principales
- **Dependencias**: 5 relaciones de uso

---

_Diagrama de Clases del Dominio - Carnicería CRM v1.0_
