using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Web;

using Newtonsoft.Json;

namespace WebServer
{
    public class EchoInformation
    {
        public NameValueCollection RequestHeaders { get; set; }
        public string RequestBody { get; set; }
    }

    public class Test : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var echo = new EchoInformation();
            echo.RequestHeaders = context.Request.Headers;
            echo.RequestBody = "foo";

            string echoJson = JsonConvert.SerializeObject(echo, new NameValueCollectionConverter());

            EchoInformation newEcho = (EchoInformation)JsonConvert.DeserializeObject(echoJson, typeof(EchoInformation), new NameValueCollectionConverter());
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
