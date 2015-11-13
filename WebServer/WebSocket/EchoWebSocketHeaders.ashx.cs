using System;
using System.Web;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace WebServer
{
    public class EchoWebSocketHeaders : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                if (!context.IsWebSocketRequest)
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("Not a websocket request");

                    return;
                }

                context.AcceptWebSocketRequest(async wsContext =>
                {
                    WebSocket socket = wsContext.WebSocket;

                    // Reflect all headers and cookies
                    var sb = new StringBuilder();
                    sb.AppendLine("Headers:");

                    foreach (string header in wsContext.Headers.AllKeys)
                    {
                        sb.Append(header);
                        sb.Append(":");
                        sb.AppendLine(wsContext.Headers[header]);
                    }

                    byte[] sendBuffer = Encoding.UTF8.GetBytes(sb.ToString());
                    await socket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, new CancellationToken());

                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(2000);

                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Headers were sent", cts.Token);
                });
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.StatusDescription = ex.Message;
            }
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
