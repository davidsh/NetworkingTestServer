using System;
using System.Net.WebSockets;
using System.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

                context.AcceptWebSocketRequest(ProcessWebSocketRequest);
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

        private async Task ProcessWebSocketRequest(WebSocketContext wsContext)
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
                    WebSocketMessageType messageType;

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        if (receiveResult.CloseStatus == WebSocketCloseStatus.Empty)
                        {
                            await socket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
                        }
                        else
                        {
                            await socket.CloseAsync(
                                receiveResult.CloseStatus.GetValueOrDefault(),
                                receiveResult.CloseStatusDescription,
                                CancellationToken.None);
                        }
                        return;
                    }

                    // Keep reading until we get an entire message.
                    messageType = receiveResult.MessageType;
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
                        bool sendMessage = false;

                        if (messageType == WebSocketMessageType.Text)
                        {
                            string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, offset);
                            if (receivedMessage == ".close")
                            {
                                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, receivedMessage, CancellationToken.None);
                            }
                            if (receivedMessage == ".shutdown")
                            {
                                await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, receivedMessage, CancellationToken.None);
                            }
                            else if (receivedMessage == ".abort")
                            {
                                socket.Abort();
                            }
                            else if (receivedMessage == ".delay5sec")
                            {
                                await Task.Delay(5000);
                            }
                            else
                            {
                                sendMessage = true;
                            }
                        }

                        if (sendMessage && (socket.State == WebSocketState.Open))
                        {
                            await socket.SendAsync(
                                    new ArraySegment<byte>(receiveBuffer, 0, offset),
                                    messageType,
                                    true,
                                    CancellationToken.None);
                        }
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
        }
    }
}
