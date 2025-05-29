using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CarniceriaCRM.BLL;
using CarniceriaCRM.BE;

namespace CarniceriaCRM
{
    public partial class Bitacora : System.Web.UI.Page
    {
        private SesionSingleton sesion;
        private Usuario usuarioActual;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar autenticación y permisos
            if (!VerificarPermisos())
            {
                Response.Redirect("~/Login.aspx", false);
                return;
            }

            if (!Page.IsPostBack)
            {
                CargarInformacionUsuario();
                CargarUsuariosFiltro();
                ConfigurarFechasPorDefecto();
                CargarBitacora();
            }
        }

        /// <summary>
        /// Verifica que el usuario esté autenticado y tenga permisos de WebMaster
        /// </summary>
        /// <returns>True si tiene permisos, False en caso contrario</returns>
        private bool VerificarPermisos()
        {
            try
            {
                sesion = SesionSingleton.Instancia;
                usuarioActual = sesion.Usuario;
                
                if (usuarioActual == null)
                    return false;

                // Verificar que sea WebMaster
                return usuarioActual.Familias?.Any(f => f.Nombre == "WebMaster") == true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Carga la información del usuario en la interfaz
        /// </summary>
        private void CargarInformacionUsuario()
        {
            if (usuarioActual != null)
            {
                lblNombreUsuario.Text = $"{usuarioActual.Nombre} {usuarioActual.Apellido}";
                lblRolUsuario.Text = "WebMaster";
            }
        }

        /// <summary>
        /// Carga los usuarios en el filtro
        /// </summary>
        private void CargarUsuariosFiltro()
        {
            try
            {
                var usuarioService = new UsuarioService();
                var usuarios = usuarioService.ObtenerTodos(); // Necesitaremos implementar este método

                ddlUsuarios.Items.Clear();
                ddlUsuarios.Items.Add(new ListItem("Todos los usuarios", ""));

                foreach (var usuario in usuarios)
                {
                    ddlUsuarios.Items.Add(new ListItem(
                        $"{usuario.Nombre} {usuario.Apellido} ({usuario.Mail})", 
                        usuario.Id.ToString()));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando usuarios: {ex.Message}");
                // En caso de error, mantener solo "Todos los usuarios"
            }
        }

        /// <summary>
        /// Configura las fechas por defecto en los filtros
        /// </summary>
        private void ConfigurarFechasPorDefecto()
        {
            // Últimos 7 días por defecto
            txtFechaHasta.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtFechaDesde.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Carga los datos de la bitácora
        /// </summary>
        private void CargarBitacora()
        {
            try
            {
                var bitacoraService = new BitacoraService();
                
                // Obtener filtros
                int? usuarioId = string.IsNullOrEmpty(ddlUsuarios.SelectedValue) ? 
                    (int?)null : int.Parse(ddlUsuarios.SelectedValue);
                string accion = string.IsNullOrEmpty(ddlAcciones.SelectedValue) ? 
                    null : ddlAcciones.SelectedValue;
                
                DateTime? fechaDesde = null;
                DateTime? fechaHasta = null;
                
                if (DateTime.TryParse(txtFechaDesde.Text, out DateTime fd))
                    fechaDesde = fd;
                if (DateTime.TryParse(txtFechaHasta.Text, out DateTime fh))
                    fechaHasta = fh.AddDays(1); // Incluir todo el día

                // Obtener registros filtrados
                var registros = bitacoraService.ObtenerConFiltros(usuarioId, accion, fechaDesde, fechaHasta);
                
                // Crear DataTable para el GridView
                var dt = CrearDataTable(registros);
                
                // Configurar GridView
                gvBitacora.DataSource = dt;
                gvBitacora.DataBind();

                // Actualizar estadísticas
                lblTotalRegistros.Text = registros.Count().ToString();
                lblUltimaActualizacion.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando bitácora: {ex.Message}");
                
                // Mostrar mensaje de error al usuario
                ClientScript.RegisterStartupScript(this.GetType(), "error", 
                    "alert('Error cargando los registros de bitácora. Intente nuevamente.');", true);
                
                // Limpiar grid
                gvBitacora.DataSource = new DataTable();
                gvBitacora.DataBind();
                
                lblTotalRegistros.Text = "0";
                lblUltimaActualizacion.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        /// <summary>
        /// Crea un DataTable para mostrar en el GridView
        /// </summary>
        /// <param name="registros">Lista de registros de bitácora</param>
        /// <returns>DataTable configurado</returns>
        private DataTable CrearDataTable(IEnumerable<Bitacora> registros)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("FechaHora", typeof(DateTime));
            dt.Columns.Add("NombreUsuario", typeof(string));
            dt.Columns.Add("Mail", typeof(string));
            dt.Columns.Add("Accion", typeof(string));
            dt.Columns.Add("Descripcion", typeof(string));
            dt.Columns.Add("DireccionIP", typeof(string));
            dt.Columns.Add("UserAgent", typeof(string));

            foreach (var registro in registros.OrderByDescending(r => r.FechaHora))
            {
                var row = dt.NewRow();
                row["Id"] = registro.Id;
                row["FechaHora"] = registro.FechaHora;
                row["NombreUsuario"] = registro.Usuario != null ? 
                    $"{registro.Usuario.Nombre} {registro.Usuario.Apellido}" : "Sistema";
                row["Mail"] = registro.Usuario?.Mail ?? "sistema@carniceria.com";
                row["Accion"] = registro.Accion ?? "";
                row["Descripcion"] = registro.Descripcion ?? "";
                row["DireccionIP"] = registro.DireccionIP ?? "";
                row["UserAgent"] = registro.UserAgent ?? "";
                dt.Rows.Add(row);
            }

            return dt;
        }

        #region Event Handlers

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Dashboard.aspx", false);
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            try
            {
                if (sesion != null)
                {
                    sesion.CerrarSesion();
                }
                Session.Clear();
                Session.Abandon();
                Response.Redirect("~/Login.aspx", false);
            }
            catch
            {
                Response.Redirect("~/Login.aspx", false);
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            CargarBitacora();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            ddlUsuarios.SelectedIndex = 0;
            ddlAcciones.SelectedIndex = 0;
            ConfigurarFechasPorDefecto();
            CargarBitacora();
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarBitacora();
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implementar exportación a Excel/CSV
                ClientScript.RegisterStartupScript(this.GetType(), "info", 
                    "alert('Función de exportación en desarrollo.');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exportando: {ex.Message}");
                ClientScript.RegisterStartupScript(this.GetType(), "error", 
                    "alert('Error al exportar los datos.');", true);
            }
        }

        protected void gvBitacora_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBitacora.PageIndex = e.NewPageIndex;
            CargarBitacora();
        }

        protected void gvBitacora_Sorting(object sender, GridViewSortEventArgs e)
        {
            // TODO: Implementar ordenamiento
            // Por ahora solo recargamos
            CargarBitacora();
        }

        protected void btnVerDetalle_Click(object sender, EventArgs e)
        {
            try
            {
                var btn = (Button)sender;
                int registroId = int.Parse(btn.CommandArgument);
                
                var bitacoraService = new BitacoraService();
                var registro = bitacoraService.ObtenerPorId(registroId);
                
                if (registro != null)
                {
                    // Cargar datos en el modal
                    lblDetalleId.Text = registro.Id.ToString();
                    lblDetalleFecha.Text = registro.FechaHora.ToString("dd/MM/yyyy HH:mm:ss");
                    lblDetalleUsuario.Text = registro.Usuario != null ? 
                        $"{registro.Usuario.Nombre} {registro.Usuario.Apellido}" : "Sistema";
                    lblDetalleMail.Text = registro.Usuario?.Mail ?? "sistema@carniceria.com";
                    lblDetalleAccion.Text = registro.Accion ?? "";
                    lblDetalleDescripcion.Text = registro.Descripcion ?? "";
                    lblDetalleIP.Text = registro.DireccionIP ?? "";
                    lblDetalleUserAgent.Text = registro.UserAgent ?? "";
                    
                    // Mostrar modal
                    ClientScript.RegisterStartupScript(this.GetType(), "showModal", 
                        "mostrarModal();", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error mostrando detalle: {ex.Message}");
                ClientScript.RegisterStartupScript(this.GetType(), "error", 
                    "alert('Error cargando los detalles del registro.');", true);
            }
        }

        #endregion
    }
} 