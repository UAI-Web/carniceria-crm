using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CarniceriaCRM.BLL;
using CarniceriaCRM.BE;
using CarniceriaCRM.BLL.DigitVerifier;

namespace CarniceriaCRM
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private SesionSingleton sesion;
        private Usuario usuarioActual;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar autenticación
            if (!VerificarAutenticacion())
            {
                Response.Redirect("~/Login.aspx", false);
                return;
            }

            if (!Page.IsPostBack)
            {
                CargarInformacionUsuario();

                //revisar si hay algun problema en la bd y el digito verificador

                if (ChequearIntegridad())
                {
                    ConfigurarVisibilidadPorRol();
                    CargarEstadisticas();
                }
            }
        }

        /// <summary>
        /// Verifica que el usuario esté autenticado
        /// </summary>
        /// <returns>True si está autenticado, False en caso contrario</returns>
        private bool VerificarAutenticacion()
        {
            try
            {
                sesion = SesionSingleton.Instancia;
                usuarioActual = sesion.Usuario;
                return usuarioActual != null;
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
                // Información básica del usuario
                lblNombreUsuario.Text = $"{usuarioActual.Nombre} {usuarioActual.Apellido}";
                lblBienvenida.Text = usuarioActual.Nombre;
                
                // Obtener y mostrar el rol principal
                var rolPrincipal = ObtenerRolPrincipal();
                lblRolUsuario.Text = rolPrincipal;
                lblRolBienvenida.Text = rolPrincipal;
                
                // Información adicional
                lblUltimaConexion.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            }
        }

        /// <summary>
        /// Obtiene el rol principal del usuario (para mostrar en UI)
        /// </summary>
        /// <returns>Nombre del rol principal</returns>
        private string ObtenerRolPrincipal()
        {
            if (usuarioActual == null || usuarioActual.Familias == null || !usuarioActual.Familias.Any())
                return "Sin Rol";

            // Prioridad: WebMaster > Carnicero > Cliente
            if (usuarioActual.Familias.Any(f => f.Nombre == "WebMaster"))
                return "WebMaster";
            else if (usuarioActual.Familias.Any(f => f.Nombre == "Carnicero"))
                return "Carnicero";
            else if (usuarioActual.Familias.Any(f => f.Nombre == "Cliente"))
                return "Cliente";
            else
                return usuarioActual.Familias.First().Nombre;
        }

        /// <summary>
        /// Configura la visibilidad de los módulos según el rol del usuario
        /// </summary>
        private void ConfigurarVisibilidadPorRol()
        {
            if (usuarioActual == null || usuarioActual.Familias == null)
                return;

            // Determinar permisos basados en roles
            bool esWebMaster = usuarioActual.Familias.Any(f => f.Nombre == "WebMaster");
            bool esCarnicero = usuarioActual.Familias.Any(f => f.Nombre == "Carnicero");
            bool esCliente = usuarioActual.Familias.Any(f => f.Nombre == "Cliente");

            // Productos - Todos pueden ver (pero con diferentes niveles de acceso)
            pnlProductos.Visible = true;
            if (esCliente && !esWebMaster && !esCarnicero)
            {
                btnProductos.Text = "Ver Catálogo";
            }

            // Clientes - Solo WebMaster y Carnicero
            pnlClientes.Visible = esWebMaster || esCarnicero;

            // Pedidos - Todos pueden ver (pero con diferentes filtros)
            pnlPedidos.Visible = true;
            if (esCliente && !esWebMaster && !esCarnicero)
            {
                btnPedidos.Text = "Mis Pedidos";
            }

            // Proveedores - Solo WebMaster y Carnicero
            pnlProveedores.Visible = esWebMaster || esCarnicero;

            // Reportes - Solo WebMaster y Carnicero
            pnlReportes.Visible = esWebMaster || esCarnicero;

            // Usuarios - Solo WebMaster
            pnlUsuarios.Visible = esWebMaster;

            // Bitácora - Solo WebMaster
            pnlBitacora.Visible = esWebMaster;

            // Configuración - Solo WebMaster
            pnlConfiguracion.Visible = esWebMaster;

            // Estadísticas - Solo WebMaster y Carnicero
            pnlEstadisticas.Visible = esWebMaster || esCarnicero;

            // Mensaje especial para clientes
            pnlMensajeCliente.Visible = esCliente && !esWebMaster && !esCarnicero;
        }

        /// <summary>
        /// Carga las estadísticas del dashboard (solo para WebMaster y Carnicero)
        /// </summary>
        private void CargarEstadisticas()
        {
            if (!pnlEstadisticas.Visible)
                return;

            try
            {
                // Aquí se cargarían las estadísticas reales desde la base de datos
                // Por ahora usamos valores de ejemplo
                lblTotalClientes.Text = "15";
                lblTotalProductos.Text = "42";
                lblPedidosHoy.Text = "8";
                lblVentasHoy.Text = "$12,500";

                // TODO: Implementar carga de estadísticas reales
                // var clienteService = new ClienteService();
                // lblTotalClientes.Text = clienteService.ContarClientesActivos().ToString();
            }
            catch (Exception ex)
            {
                // En caso de error, mostrar valores por defecto
                System.Diagnostics.Debug.WriteLine($"Error cargando estadísticas: {ex.Message}");
            }
        }

        private bool ChequearIntegridad()
        {
            // Determinar permisos basados en roles
            bool esWebMaster = usuarioActual.Familias.Any(f => f.Nombre == "WebMaster");

            var bdService = new BaseDatosService();

            panRestore.Visible = esWebMaster && bdService.RestoreAvailable();

            //si el estado de la BD es correcto entonces no mostrar el boton de calculo de inconsistencia

            var digitVerifierManager = new DigitVerifierManager();

            IntegrityResume integrityResume = digitVerifierManager.VerifyIntegrity();

            //ademas necesito saber el resultado de las tablas modificadas y las columnas modificadas

            if (!integrityResume.Result)
            {
                if (integrityResume.DVHErrors.Count != 0)
                {
                    phDVHErrors.Visible = true;

                    blDVHError.DataTextField = "Message";
                    blDVHError.DataSource = integrityResume.DVHErrors;
                    blDVHError.DataBind();
                }

                if (integrityResume.DVVErrors.Count != 0)
                {
                    phDVVErrors.Visible = true;

                    blDVVError.DataTextField = "Message";
                    blDVVError.DataSource = integrityResume.DVVErrors;
                    blDVVError.DataBind();
                }
            }

            phDVErrors.Visible = esWebMaster && !integrityResume.Result;

            panBackup.Visible = esWebMaster && integrityResume.Result;
            panCalculateVD.Visible = esWebMaster && !integrityResume.Result;

            return integrityResume.Result;
        }

        #region Event Handlers para navegación

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            try
            {
                // Usar el servicio para logout (registra en bitácora)
                var usuarioService = new UsuarioService();
                usuarioService.Logout();
                
                // Limpiar session del ASP.NET también
                Session.Clear();
                Session.Abandon();
                
                // Redirigir al login
                Response.Redirect("~/Login.aspx", false);
            }
            catch (ExcepcionLogin ex)
            {
                // Si no hay sesión activa, simplemente redirigir
                System.Diagnostics.Debug.WriteLine($"Excepción logout: {ex.Message}");
                Session.Clear();
                Session.Abandon();
                Response.Redirect("~/Login.aspx", false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cerrando sesión: {ex.Message}");
                // Forzar redirección al login
                Session.Clear();
                Session.Abandon();
                Response.Redirect("~/Login.aspx", false);
            }
        }

        protected void btnProductos_Click(object sender, EventArgs e)
        {
            // TODO: Redirigir a página de productos
            Response.Redirect("~/Productos.aspx", false);
        }

        protected void btnClientes_Click(object sender, EventArgs e)
        {
            // TODO: Redirigir a página de clientes
            Response.Redirect("~/Clientes.aspx", false);
        }

        protected void btnPedidos_Click(object sender, EventArgs e)
        {
            // TODO: Redirigir a página de pedidos
            Response.Redirect("~/Pedidos.aspx", false);
        }

        protected void btnProveedores_Click(object sender, EventArgs e)
        {
            // TODO: Redirigir a página de proveedores
            Response.Redirect("~/Proveedores.aspx", false);
        }

        protected void btnReportes_Click(object sender, EventArgs e)
        {
            // TODO: Redirigir a página de reportes
            Response.Redirect("~/Reportes.aspx", false);
        }

        protected void btnUsuarios_Click(object sender, EventArgs e)
        {
            // TODO: Redirigir a página de usuarios
            Response.Redirect("~/Usuarios.aspx", false);
        }

        protected void btnBitacora_Click(object sender, EventArgs e)
        {
            // Redirigir a página de bitácora
            Response.Redirect("~/Bitacora.aspx", false);
        }

        protected void btnConfiguracion_Click(object sender, EventArgs e)
        {
            // TODO: Redirigir a página de configuración
            Response.Redirect("~/Configuracion.aspx", false);
        }

        protected void btnBackupDB_Click(object sender, EventArgs e)
        {
            bool esWebMaster = usuarioActual.Familias.Any(f => f.Nombre == "WebMaster");

            if (!esWebMaster)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error",
                    "alert('Usted no posee los permisos necesarios para realizar esta acción.');", true);

                return;
            }

            try
            {
                var baseDatosService = new BaseDatosService();

                baseDatosService.Backup();

                Response.Redirect("~/Dashboard.aspx");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al realizar el backup de la base de datos: {ex.Message}");

                // Mostrar mensaje de error al usuario
                ClientScript.RegisterStartupScript(this.GetType(), "error",
                    "alert('Ocurrió un error al intentar realizar el backup de la base de datos. Intente nuevamente.');", true);
            }
        }

        protected void btnCalculateVD_Click(object sender, EventArgs e)
        {
            bool esWebMaster = usuarioActual.Familias.Any(f => f.Nombre == "WebMaster");

            if (!esWebMaster)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error",
                    "alert('Usted no posee los permisos necesarios para realizar esta acción.');", true);

                return;
            }

            try
            {
                var digitVerifierManager = new DigitVerifierManager();
                digitVerifierManager.RecalcularDV();

                Response.Redirect("~/Dashboard.aspx");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al ejecutar el cálculo del nuevo dígito verificador de la base de datos: {ex.Message}");

                // Mostrar mensaje de error al usuario
                ClientScript.RegisterStartupScript(this.GetType(), "error",
                    "alert('Ocurrió un error al intentar calcular el nuevo dígito verificador de la base de datos. Intente nuevamente.');", true);
            }
        }

        protected void btnRestoreDB_Click(object sender, EventArgs e)
        {
            bool esWebMaster = usuarioActual.Familias.Any(f => f.Nombre == "WebMaster");

            if (!esWebMaster)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error",
                    "alert('Usted no posee los permisos necesarios para realizar esta acción.');", true);

                return;
            }

            try
            {
                var baseDatosService = new BaseDatosService();

                baseDatosService.Restore();

                Response.Redirect("~/Dashboard.aspx");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al realizar el restore de la base de datos: {ex.Message}");

                // Mostrar mensaje de error al usuario
                ClientScript.RegisterStartupScript(this.GetType(), "error",
                    "alert('Ocurrió un error al intentar restaurar el backup de la base de datos. Intente nuevamente.');", true);
            }
        }

        #endregion
    }
} 