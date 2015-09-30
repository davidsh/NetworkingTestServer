<%@ WebHandler Language="C#" Class="Echo" %>

using System;
using System.IO;
using System.Text;
using System.Web;

public class Echo : IHttpHandler 
{
    public void ProcessRequest (HttpContext context)
    {
        string authType = context.Request.QueryString["auth"];
        string user = context.Request.QueryString["user"];
        string password = context.Request.QueryString["password"];
        string domain = context.Request.QueryString["domain"];

        if (string.Equals("basic", authType, StringComparison.OrdinalIgnoreCase))
        {
            if (!HandleBasicAuth(context, user, password, domain))
            {
                context.Response.End();
                return;
            }
        }
        else if (authType != null)
        {
            context.Response.StatusCode = 500;
            context.Response.StatusDescription = "Unsupported auth type: " + authType;
            context.Response.End();
            return;
        }

        // Iterate thru the request headers.
        // Turn all 'X-SetCookie' request headers into response cookies.
        // Build up buffer of all request headers to echo back.
        var buffer = new StringBuilder();
        string headerName;
        string headerValue;
        for (int i = 0; i < context.Request.Headers.Count; i++)
        {
            headerName = context.Request.Headers.Keys[i];
            headerValue = context.Request.Headers[i];
            
            if (string.Compare(headerName, "X-SetCookie", StringComparison.OrdinalIgnoreCase) == 0)
            {
                context.Response.Headers.Add("Set-Cookie", headerValue);
            }
            
            buffer.Append(string.Format("{0}: {1}\r\n", context.Request.Headers.Keys[i], context.Request.Headers[i]));
        }
        
        context.Response.ContentType = "text/plain";
        context.Response.Write(String.Format("Request headers count: {0}\r\n", context.Request.Headers.Count));
        context.Response.Write(buffer.ToString());
        
        var stream = context.Request.GetBufferedInputStream();
        using (var reader = new StreamReader(stream))
        {
            var requestBody = reader.ReadToEnd();
            context.Response.Write(String.Format("\r\n\r\nContentLength: {0}\r\nContentBody:\r\n{1}", requestBody.Length, requestBody));
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

    private bool HandleBasicAuth(HttpContext context, string user, string password, string domain)
    {
        string authHeader = context.Request.Headers["Authorization"];
        if (authHeader == null)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.Add("WWW-Authenticate", "Basic realm=\"NCLWEB\"");
            return false;
        }

        string[] split = authHeader.Split(new Char[] { ' ' });
        if (split.Length < 2)
        {
            context.Response.StatusCode = 500;
            context.Response.StatusDescription = "Invalid Authorization header: " + authHeader;
            return false;
        }

        if (!string.Equals("basic", split[0], StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = 500;
            context.Response.StatusDescription = "Unsupported auth type: " + split[0];
            return false;
        }

        // Decode base64 username:password.
        byte[] bytes = Convert.FromBase64String(split[1]);
        string credential = Encoding.ASCII.GetString(bytes);
        string[] pair = credential.Split(new Char[] { ':' });

        // Prefix "domain\" to username if domain is specified.
        if (domain != null)
        {
            user = domain + "\\" + user;
        }

        if (pair.Length != 2 || pair[0] != user || pair[1] != password)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.Add("WWW-Authenticate", "Basic realm=\"NCLWEB\"");
            return false;
        }

        // Success.
        return true;
    }
}
