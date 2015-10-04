using System;

namespace uPlayAgain
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            // Give the user some information, but
            // stay on the default page
            Exception exc = Server.GetLastError();
            Response.Write("<h2>Global Page Error</h2>\n");
            Response.Write("<p>" + exc.Message + "</p>\n");
            
            // Clear the error from the server
            Server.ClearError();
        }
    }
}
