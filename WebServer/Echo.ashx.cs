using System;
using System.IO;
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

            // Echo back JSON encoded payload.
            RequestInformation info = RequestInformation.Create(context.Request);
            string echoJson = info.SerializeToJson();

            // Compute MD5 hash to clients can verify the received data.
            MD5 md5 = MD5.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(echoJson);
            byte[] hash = md5.ComputeHash(bytes);
            string encodedHash = Convert.ToBase64String(hash);

            context.Response.Headers.Add("Content-MD5", encodedHash);
            context.Response.ContentType = "text/plain"; //"application/json";
            context.Response.Write(echoJson);

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
