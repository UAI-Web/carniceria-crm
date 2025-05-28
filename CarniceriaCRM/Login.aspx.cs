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

        }

        protected void btnEntrar_Click(object sender, EventArgs e)
        {

            if (!Page.IsValid)
                return;

            var mail = tbMail.Text.Trim();
            var pass = tbPass.Text;

            try
            {
                var svc = new UsuarioService();
                var resultado = svc.Login(mail, pass);
                Session["Mail"] = mail;
                Response.Redirect("~/Inicio.aspx", false);
            }
            catch (ExcepcionLogin ex)
            {
                // 4) Mapeo de errores a mensajes
                switch (ex.resultado)
                {
                    case ResultadoLogin.MailInvalido:
                        lblError.Text = "El mail no está registrado.";
                        break;
                    case ResultadoLogin.ContraseñaInvalida:
                        lblError.Text = "Contraseña incorrecta.";
                        break;
                    case ResultadoLogin.UsuarioBloqueado:
                        lblError.Text = "Usuario bloqueado por múltiples intentos.";
                        break;
                    case ResultadoLogin.SesionYaIniciada:
                        lblError.Text = "Ya existe una sesión iniciada.";
                        break;
                    default:
                        lblError.Text = "Error al iniciar sesión.";
                        break;
                }
            }
        }

        protected void cvMail_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = args.Value.Length >= 3 && args.Value.Length <= 20;
        }
    }
}
