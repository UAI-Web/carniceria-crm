using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CarniceriaCRM
{
    public partial class Login : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Unnamed_Click(object sender, EventArgs e)
        {
            Session["Mail"] = tbMail.Text;
            Session["Password"] = tbPass.Text;
            //Response.Redirect("Inicio.aspx");
        }

        protected void cvMail_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (args.Value.Length>=3 && args.Value.Length<= 20)
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }

            
        }
    }
}