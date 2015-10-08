using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer
{
    /// <summary>
    /// Summary description for EmptyContent
    /// </summary>
    public class EmptyContent : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
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
}
