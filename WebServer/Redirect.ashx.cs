using System;
using System.Web;

namespace WebServer
{
    /// <summary>
    /// Summary description for Redirect
    /// </summary>
    public class Redirect : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string redirectUri = context.Request.QueryString["uri"];
            string hopsString = context.Request.QueryString["hops"];
            int hops = 1;

            try
            {
                if (string.IsNullOrEmpty(redirectUri))
                {
                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "Missing redirection uri";
                    return;
                }

                if (!string.IsNullOrEmpty(hopsString))
                {
                    hops = int.Parse(hopsString);
                }


                if (hops <= 1)
                {
                    context.Response.Headers.Add("Location", redirectUri);
                }
                else
                {
                    context.Response.Headers.Add(
                        "Location",
                        string.Format("/Redirect.ashx?uri={0}&hops={1}",
                        redirectUri,
                        hops - 1));
                }

                context.Response.StatusCode = 302;
            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;
                context.Response.StatusDescription = "Error parsing hops: " + hopsString;
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
