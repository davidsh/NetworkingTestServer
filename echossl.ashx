<%@ WebHandler Language="C#" Class="EchoSsl" %>

using System;
using System.Text;
using System.Web;

public class EchoSsl: IHttpHandler 
{
    public void ProcessRequest (HttpContext context)
    {
        var builder = new StringBuilder();

        try
        {
            if (context.Request.IsSecureConnection)
            {
                builder.AppendLine("HTTPS connection");
            }
            else
            {
                builder.AppendLine("HTTP connection");
            }
        
            var cs = context.Request.ClientCertificate;
            if (cs.IsPresent)
            {
                builder.AppendLine("ClientCertificate:");
                builder.AppendLine("Subject: " + cs.Subject);
                builder.AppendLine("Issuer: " + cs.Issuer);
                builder.AppendLine("SerialNumber: " + cs.SerialNumber);
                builder.AppendLine("KeySize: " + cs.KeySize);
                builder.AppendLine("ValidFrom: " + cs.ValidFrom);
                builder.AppendLine("ValidUntil: " + cs.ValidUntil);
            }
            else
            {
                builder.AppendLine("ClientCertificate: none");
            }
        }
        catch (Exception ex)
        {
            builder.AppendLine(ex.Message);
        }
        
        context.Response.ContentType = "text/plain";
        context.Response.Write(builder.ToString());
    }
 
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}
