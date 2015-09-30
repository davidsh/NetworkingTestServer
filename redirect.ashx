<%@ WebHandler Language="C#" Class="Redirect" %>

using System;
using System.IO;
using System.Web;

public class Redirect : IHttpHandler 
{
    public void ProcessRequest (HttpContext context)
    {
        string redirectUri = context.Request.QueryString["uri"];
        
        if (string.IsNullOrEmpty(redirectUri))
        {
            context.Response.StatusCode = 500;
            context.Response.StatusDescription = "Missing redirection uri";
        }
        else
        {
            context.Response.StatusCode = 301;
            context.Response.Headers.Add("Location", redirectUri);
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