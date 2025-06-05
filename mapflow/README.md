# 📂 MapFlow - Documentación de Navegación

Esta carpeta contiene la documentación visual y técnica del flujo de navegación del sistema **Carnicería CRM**.

## 📋 Contenido

### 📄 `diagrama-navegacion-completo.md` ⭐ **NUEVO**

**Diagrama de secuencia completo estilo docs/diagrama-autenticacion.md**

- Flujo completo de navegación con capas GUI, BLL, DAL únicamente
- Nombres de archivos específicos para cada función
- Casos de uso por perfil con diagramas de flujo
- Mapeo detallado archivo-función implementado

### 📄 `diagrama-navegacion-arquitectura.md`

**Diagrama de secuencia por capas de arquitectura**

- Flujos de navegación siguiendo las capas GUI, BLL, DAL
- Nombres específicos de archivos para cada función
- Diagramas de secuencia estilo Mermaid
- Mapeo detallado de funciones por capa

### 📄 `esquema-navegacion-simple.md`

**Esquema visual simple por perfiles**

- Diagrama limpio similar al formato solicitado
- Un diagrama separado por cada perfil de usuario
- Estados claros: Implementado vs En desarrollo
- Resumen estadístico por perfil

### 📄 `navegacion-carniceria-crm.md`

**Mapa completo de navegación del sistema**

- Diagrama Mermaid detallado con todos los flujos por rol
- Especificaciones técnicas de cada módulo
- Estado actual de implementación
- Roadmap de desarrollo futuro

### 📄 `diagrama-navegacion-visual.md`

**Diagrama visual simplificado**

- Esquema de navegación similar al formato solicitado
- Matriz de permisos por perfil de usuario
- Flujos típicos de cada tipo de usuario
- Estructura de archivos del proyecto

## 🎯 Propósito

Estos documentos proporcionan:

1. **🗺️ Guía de Navegación** - Para entender cómo se mueve cada tipo de usuario
2. **📊 Matriz de Permisos** - Qué puede hacer cada rol en el sistema
3. **🔄 Flujos de Trabajo** - Casos de uso típicos por perfil
4. **📈 Estado del Proyecto** - Qué está implementado y qué falta
5. **🏗️ Arquitectura Técnica** - Cómo fluyen las llamadas entre capas
6. **📁 Mapeo de Archivos** - Funciones específicas por archivo y capa

## 🎭 Perfiles de Usuario

### 🌐 **Acceso Público** (Sin autenticación)

- Página de inicio informativa
- Formulario de login
- Enlaces a registro y contacto (pendientes)

### 🟢 **Cliente** (Perfil limitado)

- Dashboard personalizado
- Catálogo de productos (solo lectura)
- Historial de pedidos propios
- Gestión de perfil personal

### 🟡 **Carnicero** (Personal operativo)

- Dashboard con estadísticas operativas
- Gestión completa de productos e inventario
- Administración de clientes
- Procesamiento de pedidos y ventas
- Reportes operativos

### 🔴 **WebMaster** (Administrador completo)

- Acceso total al sistema
- Gestión de usuarios y permisos
- Bitácora completa de auditoría
- Backup e integridad de datos
- Configuración del sistema
- Reportes avanzados

## 🚀 Estado Actual

**✅ Completamente Implementado:**

- Sistema de autenticación y autorización
- Dashboard dinámico por roles
- Bitácora de auditoría
- Backup e integridad de datos

**🔄 Base de Datos Lista - UI Pendiente:**

- Gestión de productos
- Administración de clientes
- Sistema de pedidos
- Gestión de proveedores
- Reportes y estadísticas

---

_Última actualización: Análisis completo post git-pull + Esquema simple + Diagrama arquitectura + Diagrama completo_
