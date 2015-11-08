using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebServer
{
    public class Echo : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (!AuthenticationHelper.HandleAuthentication(context))
            {
                context.Response.End();
                return;
            }

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

            // Add original request method verb as a custom response header.
            context.Response.Headers.Add("X-HttpRequest-Method", context.Request.HttpMethod);

            // Echo back JSON encoded payload.
            RequestInformation info = RequestInformation.Create(context.Request);
            string echoJson = info.SerializeToJson();

            // Compute MD5 hash to clients can verify the received data.
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(echoJson);
                byte[] hash = md5.ComputeHash(bytes);
                string encodedHash = Convert.ToBase64String(hash);

                context.Response.Headers.Add("Content-MD5", encodedHash);
                context.Response.ContentType = "application/json";
                context.Response.Write(echoJson);
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
}
