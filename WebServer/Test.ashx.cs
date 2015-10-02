using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Web;

using Newtonsoft.Json;

namespace WebServer
{
    public class Test : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            RequestInformation info = RequestInformation.Create(context.Request);

            string echoJson = info.SerializeToJson();

            RequestInformation newEcho = RequestInformation.DeSerializeFromJson(echoJson);
            context.Response.ContentType = "text/plain"; //"application/json";
            context.Response.Write(echoJson);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
