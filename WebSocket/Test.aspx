<%@ Page Language="C#" AutoEventWireup="true" %>

<!doctype html>
<html>
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <meta http-equiv="X-UA-Compatible" content="IE=10" />
    <title>Web Socket Test</title>
    <script type="text/javascript">

    var socket = null;
    var operations = 0;
    
    var defaultScheme = "<%: Request.Url.Scheme %>";
    var defaultHost = "<%: Request.Url.Host %>";
    var defaultPort = "<%: Request.Url.Port %>";
    var defaultPath = '<%: Response.ApplyAppPathModifier("~/WebSocket/EchoWebSocket.ashx") %>';
    
    function writeLog(message)
    {
        var log = document.getElementById("txtLog");
        txtLog.innerHTML += message;
    }
    
    function test()
    {
        var host = document.getElementById("uri").value;

        try 
        {
            if (!window.WebSocket && window.MozWebSocket)
            {
                WebSocket = MozWebSocket;
            }

            socket = new WebSocket(host);
            writeLog("ctor ");

            socket.onopen = function ()
            {
                writeLog("OnOpen ");
                socket.close();
            };

            socket.onerror = function ()
            {
                writeLog("OnError <br />");
            };
            
            socket.onclose = function (evt)
            {
                operations = operations + 1;
                writeLog("OnClose - connections completed: " + operations + " <br />");
            };
        }
        catch (e)
        {
            alert(e.message);
        }
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
    }
    
    </script>
</head>

<body onload='initialize()'>
    <h1>Web Socket Test</h1>
    <p>
        WebSocket.url:&nbsp;&nbsp;&nbsp;<span id="websocketUrl">&nbsp;</span><br />
    </p>
    
    <span>Uri:</span><input id="uri" type="text" size="75">
    <input id="btnTest" type="button" value="Test" onclick="test()" />

    <br />
    
    <div id="txtLog">
    </div>
</body>

</html>
