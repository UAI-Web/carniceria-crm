<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Bitacora.aspx.cs"
Inherits="CarniceriaCRM.Bitacora" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Bitácora del Sistema - Carnicería CRM</title>
    <link href="Content/dashboard.css" rel="stylesheet" />
    <link href="Content/bitacora.css" rel="stylesheet" />
    <link
      href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css"
      rel="stylesheet"
    />
  </head>
  <body>
    <form id="form1" runat="server">
      <!-- Header -->
      <header class="dashboard-header">
        <div class="header-content">
          <div class="logo-section">
            <i class="fas fa-history"></i>
            <h1>Bitácora del Sistema</h1>
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
                  Text="WebMaster"
                ></asp:Label>
              </span>
            </div>
            <div class="user-actions">
              <asp:Button
                ID="btnVolver"
                runat="server"
                Text="Volver al Dashboard"
                CssClass="btn-back"
                OnClick="btnVolver_Click"
              />
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
        <!-- Panel de información -->
        <section class="info-section">
          <div class="info-card">
            <i class="fas fa-shield-alt"></i>
            <h2>Auditoría del Sistema</h2>
            <p>
              Registro completo de todas las actividades de los usuarios en el
              sistema.
            </p>
            <div class="stats-info">
              <span
                ><strong>Total de registros:</strong>
                <asp:Label
                  ID="lblTotalRegistros"
                  runat="server"
                  Text="0"
                ></asp:Label
              ></span>
              <span
                ><strong>Última actualización:</strong>
                <asp:Label
                  ID="lblUltimaActualizacion"
                  runat="server"
                ></asp:Label
              ></span>
            </div>
          </div>
        </section>

        <!-- Filtros -->
        <section class="filters-section">
          <div class="filters-card">
            <h3><i class="fas fa-filter"></i> Filtros de Búsqueda</h3>
            <div class="filters-grid">
              <div class="filter-group">
                <label for="ddlUsuarios">Usuario:</label>
                <asp:DropDownList
                  ID="ddlUsuarios"
                  runat="server"
                  CssClass="form-control"
                >
                  <asp:ListItem
                    Value=""
                    Text="Todos los usuarios"
                  ></asp:ListItem>
                </asp:DropDownList>
              </div>
              <div class="filter-group">
                <label for="ddlAcciones">Acción:</label>
                <asp:DropDownList
                  ID="ddlAcciones"
                  runat="server"
                  CssClass="form-control"
                >
                  <asp:ListItem
                    Value=""
                    Text="Todas las acciones"
                  ></asp:ListItem>
                  <asp:ListItem
                    Value="Login"
                    Text="Inicio de Sesión"
                  ></asp:ListItem>
                  <asp:ListItem
                    Value="Logout"
                    Text="Cierre de Sesión"
                  ></asp:ListItem>
                  <asp:ListItem Value="Create" Text="Creación"></asp:ListItem>
                  <asp:ListItem
                    Value="Update"
                    Text="Modificación"
                  ></asp:ListItem>
                  <asp:ListItem
                    Value="Delete"
                    Text="Eliminación"
                  ></asp:ListItem>
                </asp:DropDownList>
              </div>
              <div class="filter-group">
                <label for="txtFechaDesde">Fecha Desde:</label>
                <asp:TextBox
                  ID="txtFechaDesde"
                  runat="server"
                  TextMode="Date"
                  CssClass="form-control"
                ></asp:TextBox>
              </div>
              <div class="filter-group">
                <label for="txtFechaHasta">Fecha Hasta:</label>
                <asp:TextBox
                  ID="txtFechaHasta"
                  runat="server"
                  TextMode="Date"
                  CssClass="form-control"
                ></asp:TextBox>
              </div>
              <div class="filter-actions">
                <asp:Button
                  ID="btnFiltrar"
                  runat="server"
                  Text="Filtrar"
                  CssClass="btn-filter"
                  OnClick="btnFiltrar_Click"
                />
                <asp:Button
                  ID="btnLimpiar"
                  runat="server"
                  Text="Limpiar"
                  CssClass="btn-clear"
                  OnClick="btnLimpiar_Click"
                />
              </div>
            </div>
          </div>
        </section>

        <!-- Tabla de bitácora -->
        <section class="bitacora-section">
          <div class="bitacora-card">
            <div class="table-header">
              <h3><i class="fas fa-list"></i> Registros de Auditoría</h3>
              <div class="table-actions">
                <asp:Button
                  ID="btnExportar"
                  runat="server"
                  Text="Exportar"
                  CssClass="btn-export"
                  OnClick="btnExportar_Click"
                />
                <asp:Button
                  ID="btnActualizar"
                  runat="server"
                  Text="Actualizar"
                  CssClass="btn-refresh"
                  OnClick="btnActualizar_Click"
                />
              </div>
            </div>

            <!-- GridView de bitácora -->
            <div class="table-container">
              <asp:GridView
                ID="gvBitacora"
                runat="server"
                CssClass="bitacora-grid"
                AutoGenerateColumns="false"
                AllowPaging="true"
                PageSize="20"
                AllowSorting="true"
                OnPageIndexChanging="gvBitacora_PageIndexChanging"
                OnSorting="gvBitacora_Sorting"
                EmptyDataText="No hay registros para mostrar"
              >
                <HeaderStyle CssClass="grid-header" />
                <RowStyle CssClass="grid-row" />
                <AlternatingRowStyle CssClass="grid-row-alt" />
                <PagerStyle CssClass="grid-pager" />

                <Columns>
                  <asp:BoundField
                    DataField="FechaHora"
                    HeaderText="Fecha/Hora"
                    SortExpression="FechaHora"
                    DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"
                  />
                  <asp:BoundField
                    DataField="NombreUsuario"
                    HeaderText="Usuario"
                    SortExpression="NombreUsuario"
                  />
                  <asp:BoundField
                    DataField="Mail"
                    HeaderText="Email"
                    SortExpression="Mail"
                  />
                  <asp:BoundField
                    DataField="Accion"
                    HeaderText="Acción"
                    SortExpression="Accion"
                  />
                  <asp:BoundField
                    DataField="Descripcion"
                    HeaderText="Descripción"
                    SortExpression="Descripcion"
                  />
                  <asp:BoundField
                    DataField="DireccionIP"
                    HeaderText="IP"
                    SortExpression="DireccionIP"
                  />

                  <asp:TemplateField HeaderText="Detalles">
                    <ItemTemplate>
                      <asp:Button
                        ID="btnVerDetalle"
                        runat="server"
                        Text="Ver"
                        CssClass="btn-detail"
                        CommandArgument='<%# Eval("Id") %>'
                        OnClick="btnVerDetalle_Click"
                      />
                    </ItemTemplate>
                  </asp:TemplateField>
                </Columns>
              </asp:GridView>
            </div>
          </div>
        </section>

        <!-- Modal para detalles -->
        <div id="modalDetalle" class="modal" style="display: none">
          <div class="modal-content">
            <div class="modal-header">
              <h3><i class="fas fa-info-circle"></i> Detalles del Registro</h3>
              <span class="modal-close" onclick="cerrarModal()">&times;</span>
            </div>
            <div class="modal-body">
              <asp:Panel
                ID="pnlDetalles"
                runat="server"
                CssClass="details-panel"
              >
                <div class="detail-row">
                  <strong>ID:</strong>
                  <asp:Label ID="lblDetalleId" runat="server"></asp:Label>
                </div>
                <div class="detail-row">
                  <strong>Fecha/Hora:</strong>
                  <asp:Label ID="lblDetalleFecha" runat="server"></asp:Label>
                </div>
                <div class="detail-row">
                  <strong>Usuario:</strong>
                  <asp:Label ID="lblDetalleUsuario" runat="server"></asp:Label>
                </div>
                <div class="detail-row">
                  <strong>Email:</strong>
                  <asp:Label ID="lblDetalleMail" runat="server"></asp:Label>
                </div>
                <div class="detail-row">
                  <strong>Acción:</strong>
                  <asp:Label ID="lblDetalleAccion" runat="server"></asp:Label>
                </div>
                <div class="detail-row">
                  <strong>Descripción:</strong>
                  <asp:Label
                    ID="lblDetalleDescripcion"
                    runat="server"
                  ></asp:Label>
                </div>
                <div class="detail-row">
                  <strong>Dirección IP:</strong>
                  <asp:Label ID="lblDetalleIP" runat="server"></asp:Label>
                </div>
                <div class="detail-row">
                  <strong>User Agent:</strong>
                  <asp:Label
                    ID="lblDetalleUserAgent"
                    runat="server"
                  ></asp:Label>
                </div>
              </asp:Panel>
            </div>
            <div class="modal-footer">
              <button
                type="button"
                class="btn-modal-close"
                onclick="cerrarModal()"
              >
                Cerrar
              </button>
            </div>
          </div>
        </div>
      </main>
    </form>

    <script>
      function mostrarModal() {
        document.getElementById("modalDetalle").style.display = "flex";
      }

      function cerrarModal() {
        document.getElementById("modalDetalle").style.display = "none";
      }

      // Cerrar modal al hacer clic fuera
      window.onclick = function (event) {
        var modal = document.getElementById("modalDetalle");
        if (event.target == modal) {
          modal.style.display = "none";
        }
      };

      // Efecto de carga
      document.addEventListener("DOMContentLoaded", function () {
        const sections = document.querySelectorAll(
          ".info-card, .filters-card, .bitacora-card"
        );
        sections.forEach((section, index) => {
          section.style.opacity = "0";
          section.style.transform = "translateY(20px)";
          setTimeout(() => {
            section.style.transition = "all 0.5s ease";
            section.style.opacity = "1";
            section.style.transform = "translateY(0)";
          }, index * 150);
        });
      });
    </script>
  </body>
</html>
