using System;
using System.Web;

namespace WebServer
{
    public class StatusCode : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string statusCodeString = context.Request.QueryString["statuscode"];
            try
            {
                int statusCode = int.Parse(statusCodeString);
                context.Response.StatusCode = statusCode;
            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;
                context.Response.StatusDescription = "Error parsing statuscode: " + statusCodeString;
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
