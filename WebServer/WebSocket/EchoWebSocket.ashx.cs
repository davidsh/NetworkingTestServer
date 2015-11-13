using System;
using System.Net.WebSockets;
using System.Web;
using System.Threading;

namespace WebServer
{
    public class EchoWebSocket : IHttpHandler
    {
        private const int MaxBufferSize = 64 * 1024;

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
                    var receiveBuffer = new byte[MaxBufferSize];
                    var throwAwayBuffer = new byte[MaxBufferSize];
                    var socket = wsContext.WebSocket;

                    try
                    {
                        // Stay in loop while websocket is open
                        while (socket.State == WebSocketState.Open)
                        {
                            var receiveResult = await socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

                            if (receiveResult.MessageType == WebSocketMessageType.Close)
                            {
                                await socket.CloseAsync(
                                    receiveResult.CloseStatus.GetValueOrDefault(),
                                    receiveResult.CloseStatusDescription,
                                    CancellationToken.None);
                                return;
                            }

                            // Keep reading until we get an entire message.
                            int offset = receiveResult.Count;
                            while (receiveResult.EndOfMessage == false)
                            {
                                if (offset < MaxBufferSize)
                                {
                                    receiveResult = await socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer, offset, MaxBufferSize - offset), CancellationToken.None);
                                }
                                else
                                {
                                    receiveResult = await socket.ReceiveAsync(new ArraySegment<byte>(throwAwayBuffer), CancellationToken.None);
                                }

                                offset += receiveResult.Count;
                            }

                            // Close socket if the message was too big.
                            if (offset > MaxBufferSize)
                            {
                                await socket.CloseAsync(
                                    WebSocketCloseStatus.MessageTooBig,
                                    String.Format("{0}: {1} > {2}", WebSocketCloseStatus.MessageTooBig.ToString(), offset, MaxBufferSize),
                                    CancellationToken.None);
                            }
                            else
                            {
                                // Echo back message.
                                await socket.SendAsync(
                                    new ArraySegment<byte>(receiveBuffer, 0, offset),
                                    receiveResult.MessageType,
                                    true,
                                    CancellationToken.None);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (socket.State == WebSocketState.Open)
                        {
                            socket.CloseAsync(
                                WebSocketCloseStatus.InternalServerError,
                                e.Message,
                                CancellationToken.None).Wait(100);
                        }
                    }
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
                return false;
            }
        }
    }
}
