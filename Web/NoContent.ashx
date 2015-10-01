<%@ WebHandler Language="C#" Class="NoContent" %>

using System;
using System.IO;
using System.Text;
using System.Web;

public class NoContent : IHttpHandler 
{
    public void ProcessRequest (HttpContext context)
    {
        // By default, this empty method sends back a 200 status code with 'Content-Length: 0' response header.
        // There are no other entity-body related (i.e. 'Content-Type') headers returned.
    }

    public bool IsReusable
    {
        get
        {
            return true;
        }
    }
}
