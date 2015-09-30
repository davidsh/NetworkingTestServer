<%@ WebHandler Language="C#" Class="Redirect" %>

using System;
using System.IO;
using System.Web;

public class Redirect : IHttpHandler 
{
    public void ProcessRequest (HttpContext context)
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
        context.Response.End();
    }
 
    public bool IsReusable
    {
        get
        {
            return true;
        }
    }
}
