<%@ Page Language="C#" AutoEventWireup="true" %>

<!doctype html>
<html>
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <meta http-equiv="X-UA-Compatible" content="IE=10" />
    <title>Web Socket Test</title>
    <script type="text/javascript">

    var socket = null;

    function createBlobFromUrl(url)
    {
        try
        {
            var xhr = new XMLHttpRequest();
            xhr.open("GET", url, false);
            xhr.responseType = "blob";
            xhr.send(null);
            var blob = xhr.response;
            if (blob == null || blob instanceof Blob)
            {
                return blob;
            }
            else
            {
                return null;
            }
        }
        catch (e)
        {
            alert(e.message);
        }
    }

    function createStringFromUrl(url)
    {
        try
        {
            var xhr = new XMLHttpRequest();
            xhr.open("GET", url, false);
            //xhr.responseType = "text";
            xhr.send(null);
            var theString = xhr.responseText;
            alert(theString);
            return theString;
        }
        catch (e)
        {
            alert(e.message);
        }
    }

    var defaultScheme = "<%: Request.Url.Scheme %>";
    var defaultHost = "<%: Request.Url.Host %>";
    var defaultPort = "<%: Request.Url.Port %>";
    var defaultPath = '<%: Response.ApplyAppPathModifier("~/WebSocket/EchoWebSocket.ashx") %>';
    
    function connect()
    {
        var host = document.getElementById("uri").value;

        try 
        {
            if (!window.WebSocket && window.MozWebSocket)
            {
                WebSocket = MozWebSocket;
            }

            socket = new WebSocket(host);

            socket.onopen = function ()
            {
                document.getElementById("websocketOpen").innerHTML = 'Called';
                updateUI();
            };

            socket.onerror = function ()
            {
                document.getElementById("websocketError").innerHTML = 'Called';
            };
            
            socket.onclose = function (evt)
            {
                var s = 'wasClean=' + evt.wasClean + ', code=' + evt.code + ', reason=' + evt.reason;
                document.getElementById("websocketClose").innerHTML = s;
                updateUI();
            };

            socket.onmessage = function (evt)
            {
                var s = '';
                if (evt.data instanceof Blob)
                {
                    s = 'type=blob, size=' + evt.data.size;
                }
                else
                {
                    s = 'type=string, length=' + evt.data.length + ', value=' + evt.data;
                }
                document.getElementById("websocketMessage").innerHTML = s;
            };
        }
        catch (e)
        {
            alert(e.message);
        }
    }

    function send()
    {
        var e = document.getElementById("msgText");
        socket.send(e.value);
    }

    function sendArrayBuffer(size)
    {
        socket.send(new ArrayBuffer(size));
    }

    function updateUI()
    {
        var readyState;
        if (socket == null)
        {
            document.getElementById("websocketUrl").innerHTML = ' ';
            document.getElementById("websocketReadyState").innerHTML = ' ';
            readyState = WebSocket.CLOSED;
        }
        else
        {
            document.getElementById("websocketUrl").innerHTML = socket.url;
            document.getElementById("websocketReadyState").innerHTML = socket.readyState;
            readyState = socket.readyState;
        }
        
        if (readyState == WebSocket.CLOSED)
        {
            document.getElementById("btnConnect").disabled = false;
            document.getElementById("btnResetStatus").disabled = false;
            
            document.getElementById("btnSend").disabled = true;
            document.getElementById("btnSendArrayBufferSmall").disabled = true;
            document.getElementById("btnSendArrayBufferLarge").disabled = true;
            document.getElementById("btnCloseEmpty").disabled = true;
            document.getElementById("btnClose").disabled = true;
        }
        else if (readyState == WebSocket.OPEN)
        {
            document.getElementById("btnConnect").disabled = true;
            document.getElementById("btnResetStatus").disabled = true;
            
            document.getElementById("btnSend").disabled = false;
            document.getElementById("btnSendArrayBufferSmall").disabled = false;
            document.getElementById("btnSendArrayBufferLarge").disabled = false;
            document.getElementById("btnCloseEmpty").disabled = false;
            document.getElementById("btnClose").disabled = false;
        }
    }

    function resetStatus()
    {
        document.getElementById("websocketUrl").innerHTML = ' ';
        document.getElementById("websocketReadyState").innerHTML = ' ';
        
        document.getElementById("websocketOpen").innerHTML = 'Not called';
        document.getElementById("websocketError").innerHTML = 'Not called';
        document.getElementById("websocketClose").innerHTML = 'Not called';
        document.getElementById("websocketMessage").innerHTML = 'Not called';
    }
    
    function initialize()
    {
        var uri = document.getElementById("uri");
        var scheme;
        if (defaultScheme == "http")
        {
            scheme = "ws";
        }
        else
        {
            scheme = "wss";
        }
        uri.value = scheme + "://" + defaultHost + ":" + defaultPort + defaultPath;
        
        updateUI();
        resetStatus();
    }
    
    </script>
</head>

<body onload='initialize()'>
    <h1>Web Socket Echo Demo</h1>
    <p>
        WebSocket.url:&nbsp;&nbsp;&nbsp;<span id="websocketUrl">&nbsp;</span><br />
        WebSocket.readyState:&nbsp;&nbsp;&nbsp;<span id="websocketReadyState">&nbsp;</span><br /><br />
        
        WebSocket OnOpen:&nbsp;&nbsp;&nbsp;<span id="websocketOpen">Not called</span><br />
        WebSocket OnError:&nbsp;&nbsp;<span id="websocketError">Not called</span><br />
        WebSocket OnClose:&nbsp;<span id="websocketClose">Not called</span><br />
        WebSocket OnMessage:<span id="websocketMessage">Not called</span><br />
    </p>
    
    <span>Uri:</span><input id="uri" type="text" size="75">
    <input id="btnConnect" type="button" value="Connect" onclick="connect()" />
    <input id="btnResetStatus" type="button" value="Reset Status" onclick="resetStatus()" />
    <br /><br />
    <span>This text will be sent on the socket:</span><input id="msgText" type="text" size="30">

    <input id="btnSend" type="button" value="Send" onclick="send()" />
    <br />
    <input id="btnSendArrayBufferSmall" type="button" value="Send ArrayBuffer (size=0)" onclick="sendArrayBuffer(0)" />
    <input id="btnSendArrayBufferLarge" type="button" value="Send ArrayBuffer (size=70000)" onclick="sendArrayBuffer(70000)" />
    <input id="btnCloseEmpty" type="button" value="Close - empty" onclick="socket.close();" />
    <input id="btnClose" type="button" value="Close" onclick="socket.close(3000, 'my close reason');" />
</body>

</html>
