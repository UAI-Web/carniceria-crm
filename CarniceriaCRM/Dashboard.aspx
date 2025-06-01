<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs"
Inherits="CarniceriaCRM.Dashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Dashboard - Carnicería CRM</title>
    <link href="Content/dashboard.css" rel="stylesheet" />
    <link
      href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css"
      rel="stylesheet"
    />
  </head>
  <body>
    <form id="form1" runat="server">
      <!-- Header con información del usuario -->
      <header class="dashboard-header">
        <div class="header-content">
          <div class="logo-section">
            <i class="fas fa-store"></i>
            <h1>Carnicería CRM</h1>
          </div>
          <div class="user-section">
            <div class="user-info">
              <span class="user-name">
                <asp:Label
                  ID="lblNombreUsuario"
                  runat="server"
                  Text="Usuario"
                ></asp:Label>
              </span>
              <span class="user-role">
                <asp:Label
                  ID="lblRolUsuario"
                  runat="server"
                  Text="Rol"
                ></asp:Label>
              </span>
            </div>
            <div class="user-actions">
              <asp:Button
                ID="btnCerrarSesion"
                runat="server"
                Text="Cerrar Sesión"
                CssClass="btn-logout"
                OnClick="btnCerrarSesion_Click"
              />
            </div>
          </div>
        </div>
      </header>

      <!-- Contenido principal -->
      <main class="dashboard-main">
        <!-- Bienvenida -->
        <section class="welcome-section">
          <div class="welcome-card">
            <h2>
              <i class="fas fa-hand-wave"></i>
              ¡Bienvenido,
              <asp:Label ID="lblBienvenida" runat="server"></asp:Label>!
            </h2>
            <p>
              Accediste como
              <strong
                ><asp:Label ID="lblRolBienvenida" runat="server"></asp:Label
              ></strong>
            </p>
            <div class="last-login">
              <i class="fas fa-clock"></i>
              Última conexión:
              <asp:Label
                ID="lblUltimaConexion"
                runat="server"
                Text="Ahora"
              ></asp:Label>
            </div>
          </div>
        </section>

        <!-- Navegación por módulos -->
        <section class="modules-section">
          <div class="modules-grid">
            <!-- Módulo Productos (Todos los roles pueden ver) -->
            <asp:Panel
              ID="pnlProductos"
              runat="server"
              CssClass="module-card"
              Visible="false"
            >
              <i class="fas fa-box module-icon"></i>
              <h3>Productos</h3>
              <p>Gestión del catálogo de productos</p>
              <asp:Button
                ID="btnProductos"
                runat="server"
                Text="Ver Productos"
                CssClass="btn-module"
                OnClick="btnProductos_Click"
              />
            </asp:Panel>

            <!-- Módulo Clientes (WebMaster y Carnicero) -->
            <asp:Panel
              ID="pnlClientes"
              runat="server"
              CssClass="module-card"
              Visible="false"
            >
              <i class="fas fa-users module-icon"></i>
              <h3>Clientes</h3>
              <p>Administración de clientes</p>
              <asp:Button
                ID="btnClientes"
                runat="server"
                Text="Gestionar Clientes"
                CssClass="btn-module"
                OnClick="btnClientes_Click"
              />
            </asp:Panel>

            <!-- Módulo Pedidos (Todos los roles) -->
            <asp:Panel
              ID="pnlPedidos"
              runat="server"
              CssClass="module-card"
              Visible="false"
            >
              <i class="fas fa-shopping-cart module-icon"></i>
              <h3>Pedidos</h3>
              <p>Gestión de pedidos y ventas</p>
              <asp:Button
                ID="btnPedidos"
                runat="server"
                Text="Ver Pedidos"
                CssClass="btn-module"
                OnClick="btnPedidos_Click"
              />
            </asp:Panel>

            <!-- Módulo Proveedores (WebMaster y Carnicero) -->
            <asp:Panel
              ID="pnlProveedores"
              runat="server"
              CssClass="module-card"
              Visible="false"
            >
              <i class="fas fa-truck module-icon"></i>
              <h3>Proveedores</h3>
              <p>Gestión de proveedores</p>
              <asp:Button
                ID="btnProveedores"
                runat="server"
                Text="Gestionar Proveedores"
                CssClass="btn-module"
                OnClick="btnProveedores_Click"
              />
            </asp:Panel>

            <!-- Módulo Reportes (WebMaster y Carnicero) -->
            <asp:Panel
              ID="pnlReportes"
              runat="server"
              CssClass="module-card"
              Visible="false"
            >
              <i class="fas fa-chart-bar module-icon"></i>
              <h3>Reportes</h3>
              <p>Estadísticas y reportes</p>
              <asp:Button
                ID="btnReportes"
                runat="server"
                Text="Ver Reportes"
                CssClass="btn-module"
                OnClick="btnReportes_Click"
              />
            </asp:Panel>

            <!-- Módulo Usuarios (Solo WebMaster) -->
            <asp:Panel
              ID="pnlUsuarios"
              runat="server"
              CssClass="module-card"
              Visible="false"
            >
              <i class="fas fa-user-cog module-icon"></i>
              <h3>Usuarios</h3>
              <p>Administración de usuarios</p>
              <asp:Button
                ID="btnUsuarios"
                runat="server"
                Text="Gestionar Usuarios"
                CssClass="btn-module"
                OnClick="btnUsuarios_Click"
              />
            </asp:Panel>

            <!-- Módulo Bitácora (Solo WebMaster) -->
            <asp:Panel
              ID="pnlBitacora"
              runat="server"
              CssClass="module-card"
              Visible="false"
            >
              <i class="fas fa-history module-icon"></i>
              <h3>Bitácora</h3>
              <p>Auditoría del sistema</p>
              <asp:Button
                ID="btnBitacora"
                runat="server"
                Text="Ver Bitácora"
                CssClass="btn-module"
                OnClick="btnBitacora_Click"
              />
            </asp:Panel>

            <!-- Módulo Configuración (Solo WebMaster) -->
            <asp:Panel
              ID="pnlConfiguracion"
              runat="server"
              CssClass="module-card"
              Visible="false"
            >
              <i class="fas fa-cogs module-icon"></i>
              <h3>Configuración</h3>
              <p>Configuración del sistema</p>
              <asp:Button
                ID="btnConfiguracion"
                runat="server"
                Text="Configurar"
                CssClass="btn-module"
                OnClick="btnConfiguracion_Click"
              />
            </asp:Panel>

            <!-- Módulo Backup (Solo WebMaster) -->
            <asp:Panel
              ID="panBackup"
              runat="server"
              CssClass="module-card"
              Visible="false"
            >
              <i class="fas fa-database module-icon"></i>
              <h3>Copia de respaldo</h3>
              <p>Realizar un backup de Base de Datos</p>
              <asp:Button
                ID="btnBackup"
                runat="server"
                Text="Ejecutar"
                CssClass="btn-module"
                OnClick="btnBackupDB_Click"
              />
            </asp:Panel>

            <!-- Módulo Dígito verificador (Solo WebMaster) -->
            <asp:Panel
              ID="panCalculateVD"
              runat="server"
              CssClass="module-card"
              Visible="false"
            >
              <i class="fas fa-hashtag module-icon"></i>
              <h3>Dígito verificador</h3>
              <p>Revalidar el dígito verificador</p>
              <asp:Button
                ID="btnCalculateVD"
                runat="server"
                Text="Ejecutar"
                CssClass="btn-module"
                OnClick="btnCalculateVD_Click"
              />
            </asp:Panel>

            <!-- Módulo Restore (Solo WebMaster) -->
            <asp:Panel
              ID="panRestore"
              runat="server"
              CssClass="module-card"
              Visible="false"
            >
              <i class="fas fa-database module-icon"></i>
              <h3>Restaurar respaldo</h3>
              <p>Restauración de Base de Datos</p>
              <asp:Button
                ID="btnRestore"
                runat="server"
                Text="Ejecutar"
                CssClass="btn-module"
                OnClick="btnRestoreDB_Click"
              />
            </asp:Panel>

          </div>
        </section>

        <!-- Estadísticas rápidas (solo para WebMaster y Carnicero) -->
        <asp:Panel
          ID="pnlEstadisticas"
          runat="server"
          CssClass="stats-section"
          Visible="false"
        >
          <h3><i class="fas fa-tachometer-alt"></i> Resumen Rápido</h3>
          <div class="stats-grid">
            <div class="stat-card">
              <i class="fas fa-users stat-icon"></i>
              <div class="stat-info">
                <span class="stat-number">
                  <asp:Label
                    ID="lblTotalClientes"
                    runat="server"
                    Text="0"
                  ></asp:Label>
                </span>
                <span class="stat-label">Clientes</span>
              </div>
            </div>
            <div class="stat-card">
              <i class="fas fa-box stat-icon"></i>
              <div class="stat-info">
                <span class="stat-number">
                  <asp:Label
                    ID="lblTotalProductos"
                    runat="server"
                    Text="0"
                  ></asp:Label>
                </span>
                <span class="stat-label">Productos</span>
              </div>
            </div>
            <div class="stat-card">
              <i class="fas fa-shopping-cart stat-icon"></i>
              <div class="stat-info">
                <span class="stat-number">
                  <asp:Label
                    ID="lblPedidosHoy"
                    runat="server"
                    Text="0"
                  ></asp:Label>
                </span>
                <span class="stat-label">Pedidos Hoy</span>
              </div>
            </div>
            <div class="stat-card">
              <i class="fas fa-dollar-sign stat-icon"></i>
              <div class="stat-info">
                <span class="stat-number">
                  <asp:Label
                    ID="lblVentasHoy"
                    runat="server"
                    Text="$0"
                  ></asp:Label>
                </span>
                <span class="stat-label">Ventas Hoy</span>
              </div>
            </div>
          </div>
        </asp:Panel>

        <!-- Mensaje de información para clientes -->
        <asp:Panel
          ID="pnlMensajeCliente"
          runat="server"
          CssClass="client-message"
          Visible="false"
        >
          <div class="message-card">
            <i class="fas fa-info-circle"></i>
            <h3>Acceso de Cliente</h3>
            <p>
              Como cliente, puedes consultar nuestro catálogo de productos y el
              estado de tus pedidos.
            </p>
            <p>
              Para realizar pedidos o consultas, contacta con nuestro personal.
            </p>
          </div>
        </asp:Panel>

        <asp:PlaceHolder runat="server" ID="phDVErrors" Visible="false">
            <!-- Modal para detalles -->
            <div id="modalDVErrors" class="modal" style="display: block;">
              <div class="modal-content">
                <div class="modal-header">
                  <h3><i class="fas fa-circle-exclamation errorColor"></i> Se encontraron inconsistencias en los datos</h3>
                  <span class="modal-close" onclick="cerrarModal('modalDVErrors')">&times;</span>
                </div>
                <div class="modal-body">
                    <asp:PlaceHolder runat="server" ID="phDVHErrors" Visible="false">
                        <label>Errores de fila:</label><br />
                        <asp:BulletedList runat="server" ID="blDVHError"></asp:BulletedList><br />
                        <br />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="phDVVErrors" Visible="false">
                        <label>Errores de tablas:</label><br />
                        <asp:BulletedList runat="server" ID="blDVVError"></asp:BulletedList><br />
                        <br />
                    </asp:PlaceHolder>
                </div>
                <div class="modal-footer">
                  <button
                    type="button"
                    class="btn-modal-close"
                    onclick="cerrarModal('modalDVErrors')"
                  >
                    Cerrar
                  </button>
                </div>
              </div>
            </div>

        </asp:PlaceHolder>

      </main>
    </form>

    <script>
      // Efecto de carga suave
      document.addEventListener("DOMContentLoaded", function () {
        const cards = document.querySelectorAll(
          ".module-card, .stat-card, .welcome-card"
        );
        cards.forEach((card, index) => {
          card.style.opacity = "0";
          card.style.transform = "translateY(20px)";
          setTimeout(() => {
            card.style.transition = "all 0.5s ease";
            card.style.opacity = "1";
            card.style.transform = "translateY(0)";
          }, index * 100);
        });
      });

        function cerrarModal(nombreModal) {
            var modal = document.getElementById(nombreModal);
            modal.style.display = 'none';
        }
    </script>
  </body>
</html>
