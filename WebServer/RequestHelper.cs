using System;
using System.Web;

namespace WebServer
{
    public static class RequestHelper
    {
        public static void AddResponseCookies(HttpContext context)
        {
            // Iterate thru the request headers.
            // Turn all 'X-SetCookie' request headers into response cookies.
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
            }
        }
    }
}
