using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;

namespace CarniceriaCRM
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery",
            new ScriptResourceDefinition
            {
                Path = "~/js/jquery-3.7.1.slim.min.js", // Path to your local jQuery file
                DebugPath = "~/js/jquery-3.7.1.slim.min.js", // Same for debug mode
                CdnPath = "https://code.jquery.com/jquery-3.7.1.slim.min.js", // Optional: CDN path for production
                CdnDebugPath = "https://code.jquery.com/jquery-3.7.1.slim.min.js" // Optional: CDN path for debug
            });

        }
    }
}