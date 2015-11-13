  var secureCb;
  var secureCbLabel;
  var wsUri;
  var consoleLog;
  var connectBut;
  var disconnectBut1;
  var disconnectBut2;
  var sendMessage;
  var sendBut1;
  var sendBut2;
  var sendBut3;
  var clearLogBut;

  function echoHandlePageLoad()
  {
    if (window.WebSocket)
    {
      document.getElementById("webSocketSupp").style.display = "block";
    }
    else
    {
      document.getElementById("noWebSocketSupp").style.display = "block";
    }

    secureCb = document.getElementById("secureCb");
    secureCb.checked = false;
    secureCb.onclick = toggleTls;
    
    secureCbLabel = document.getElementById("secureCbLabel")
    
    wsUri = document.getElementById("wsUri");
    toggleTls();
    
    connectBut = document.getElementById("connect");
    connectBut.onclick = doConnect;
    
    disconnectBut1 = document.getElementById("disconnect");
    disconnectBut1.onclick = doDisconnect;
    
    disconnectBut2 = document.getElementById("disconnectMsg");
    disconnectBut2.onclick = doDisconnectMsg;

    sendMessage = document.getElementById("sendMessage");

    sendBut1 = document.getElementById("send");
    sendBut1.onclick = doSend;

    sendBut2 = document.getElementById("sendArrayBufferSmall");
    sendBut2.onclick = doSendArrayBufferSmall;

    sendBut3 = document.getElementById("sendArrayBufferBig");
    sendBut3.onclick = doSendArrayBufferBig;

    consoleLog = document.getElementById("consoleLog");

    clearLogBut = document.getElementById("clearLogBut");
    clearLogBut.onclick = clearLog;

    setGuiConnected(false);

    document.getElementById("disconnect").onclick = doDisconnect;
    document.getElementById("disconnectMsg").onclick = doDisconnectMsg;
    document.getElementById("send").onclick = doSend;

  }

  function toggleTls()
  {
    var wsPort = (window.location.port.toString() === "" ? "" : ":" + window.location.port)
    if (wsUri.value === "")
    {
        wsUri.value = "ws://" + window.location.hostname + wsPort + "/WebSocket/EchoWebSocket.ashx";
    }
    
    if (secureCb.checked)
    {
      wsUri.value = wsUri.value.replace("ws:", "wss:");
    }
    else
    {
      wsUri.value = wsUri.value.replace ("wss:", "ws:");
    }
  }
  
  function doConnect()
  {
    if (window.MozWebSocket)
    {
        logToConsole('<span style="color: red;"><strong>Info:</strong> This browser supports WebSocket using the MozWebSocket constructor</span>');
        window.WebSocket = window.MozWebSocket;
    }
    else if (!window.WebSocket)
    {
        logToConsole('<span style="color: red;"><strong>Error:</strong> This browser does not have support for WebSocket</span>');
        return;
    }

    // prefer text messages
    var uri = wsUri.value;
    if (uri.indexOf("?") == -1) {
        uri += "?encoding=text";
    } else {
        uri += "&encoding=text";
    }
    websocket = new WebSocket(uri);
    websocket.onopen = function(evt) { onOpen(evt) };
    websocket.onclose = function(evt) { onClose(evt) };
    websocket.onmessage = function(evt) { onMessage(evt) };
    websocket.onerror = function(evt) { onError(evt) };
  }
  
  function doDisconnect()
  {
      websocket.close();
  }
  
  function doDisconnectMsg()
  {
      websocket.close(3000, "My close reason");
  }

  function doSend()
  {
      websocket.send(sendMessage.value);
      logToConsole("SENT: " + sendMessage.value);
  }

  function doSendArrayBufferSmall()
  {
      doSendArrayBuffer(0);
  }

  function doSendArrayBufferBig()
  {
      doSendArrayBuffer(70000);
  }

  function doSendArrayBuffer(size)
  {
      websocket.send(new ArrayBuffer(size));
      logToConsole("SENT: ArrayBuffer, size=" + size);
  }

  function logToConsole(message)
  {
    var pre = document.createElement("p");
    pre.style.wordWrap = "break-word";
    pre.innerHTML = getSecureTag()+message;
    consoleLog.appendChild(pre);

    while (consoleLog.childNodes.length > 50)
    {
      consoleLog.removeChild(consoleLog.firstChild);
    }
    
    consoleLog.scrollTop = consoleLog.scrollHeight;
  }
  
  function onOpen(evt)
  {
    logToConsole("CONNECTED");
    setGuiConnected(true);
  }
  
  function onClose(evt)
  {
    var s = 'wasClean=' + evt.wasClean + ', code=' + evt.code + ', reason=' + evt.reason;
    logToConsole("DISCONNECTED: " + s);
    setGuiConnected(false);
  }
  
  function onMessage(evt)
  {
    var s = '';
    if (evt.data instanceof Blob)
    {
        s = 'type=blob, size=' + evt.data.size;
    }
    else
    {
        s = evt.data;
    }

    logToConsole('<span style="color: blue;">RESPONSE: ' + s +'</span>');
  }

  function onError(evt)
  {
    logToConsole('<span style="color: red;">ERROR:</span> ' + evt.data);
  }
  
  function setGuiConnected(isConnected)
  {
    wsUri.disabled = isConnected;
    connectBut.disabled = isConnected;
    disconnectBut1.disabled = !isConnected;
    disconnectBut2.disabled = !isConnected;
    sendMessage.disabled = !isConnected;
    sendBut1.disabled = !isConnected;
    sendBut2.disabled = !isConnected;
    sendBut3.disabled = !isConnected;
    secureCb.disabled = isConnected;
    var labelColor = "black";
    if (isConnected)
    {
      labelColor = "#999999";
    }
     secureCbLabel.style.color = labelColor;
    
  }
	
	function clearLog()
	{
		while (consoleLog.childNodes.length > 0)
		{
			consoleLog.removeChild(consoleLog.lastChild);
		}
	}
	
	function getSecureTag()
	{
		if (secureCb.checked)
		{
			return '<img src="Includes/tls-lock.png" width="6px" height="9px"> ';
		}
		else
		{
			return '';
		}
	}
  
  window.addEventListener("load", echoHandlePageLoad, false);
