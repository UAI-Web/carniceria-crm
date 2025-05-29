using System;
using CarniceriaCRM.BLL;
using CarniceriaCRM.BE;
using System.Web.UI.WebControls;

namespace CarniceriaCRM
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Si ya hay una sesión activa, redirigir al dashboard
            if (!Page.IsPostBack)
            {
                try
                {
                    var singleton = SesionSingleton.Instancia;
                    if (singleton.Usuario != null)
                    {
                        Response.Redirect("~/Dashboard.aspx", false);
                        return;
                    }
                }
                catch
                {
                    // Si no hay sesión, continuar con el login normal
                }

                // Limpiar mensaje de error al cargar la página
                lblError.Text = "";
                lblError.Visible = false;
            }
        }

        protected void btnEntrar_Click(object sender, EventArgs e)
        {
            // Limpiar mensajes previos
            lblError.Text = "";
            lblError.Visible = false;

            if (!Page.IsValid)
                return;

            var mail = tbMail.Text.Trim();
            var pass = tbPass.Text.Trim();

            // Validaciones adicionales
            if (string.IsNullOrEmpty(mail))
            {
                MostrarError("Por favor ingrese su email.");
                return;
            }

            if (string.IsNullOrEmpty(pass))
            {
                MostrarError("Por favor ingrese su contraseña.");
                return;
            }

            try
            {
                var svc = new UsuarioService();
                var resultado = svc.Login(mail, pass);
                
                // Login exitoso - el UsuarioService ya configuró el Singleton
                // Registrar en bitácora se hace automáticamente en el service
                
                // Limpiar campos por seguridad
                tbPass.Text = "";
                
                // Redirigir al dashboard
                Response.Redirect("~/Dashboard.aspx", false);
            }
            catch (ExcepcionLogin ex)
            {
                // Mapeo detallado de errores a mensajes amigables
                switch (ex.resultado)
                {
                    case ResultadoLogin.MailInvalido:
                        MostrarError("❌ El email ingresado no está registrado en el sistema.");
                        break;
                    case ResultadoLogin.ContraseñaInvalida:
                        MostrarError("❌ La contraseña ingresada es incorrecta.");
                        break;
                    case ResultadoLogin.UsuarioBloqueado:
                        MostrarError("🔒 Su cuenta ha sido bloqueada debido a múltiples intentos fallidos de login. Contacte al administrador.");
                        break;
                    case ResultadoLogin.SesionYaIniciada:
                        MostrarError("⚠️ Ya existe una sesión activa. Será redirigido al dashboard.");
                        // Redirigir después de mostrar el mensaje
                        ClientScript.RegisterStartupScript(this.GetType(), "redirect", 
                            "setTimeout(function(){ window.location.href='Dashboard.aspx'; }, 2000);", true);
                        break;
                    default:
                        MostrarError("❌ Error inesperado al iniciar sesión. Intente nuevamente.");
                        break;
                }
                
                // Limpiar contraseña por seguridad
                tbPass.Text = "";
            }
            catch (Exception ex)
            {
                // Error no controlado
                MostrarError("❌ Error del sistema. Por favor contacte al administrador.");
                
                // Log del error (opcional, para debugging)
                System.Diagnostics.Debug.WriteLine($"Error en Login: {ex.Message}");
                
                // Limpiar contraseña por seguridad
                tbPass.Text = "";
            }
        }

        /// <summary>
        /// Método auxiliar para mostrar mensajes de error de forma consistente
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        private void MostrarError(string mensaje)
        {
            lblError.Text = mensaje;
            lblError.Visible = true;
            
            // Agregar efecto visual con JavaScript
            ClientScript.RegisterStartupScript(this.GetType(), "errorShow", 
                "document.getElementById('" + lblError.ClientID + "').style.animation = 'fadeIn 0.5s ease-in';", true);
        }

        protected void cvMail_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = args.Value.Length >= 3 && args.Value.Length <= 50;
        }
    }
}
