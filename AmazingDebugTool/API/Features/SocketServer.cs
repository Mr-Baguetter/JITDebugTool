#if EXILED
using Exiled.API.Features;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace JITDebugTool.API.Features
{
    internal class SocketServer
    {
        public bool IsEnabled { get; private set; } = false;

        private WebSocketServer Server { get; set; }

        public SocketServer()
        {
            IsEnabled = true;
            Server = new WebSocketServer(IPAddress.Parse("0.0.0.0"), Plugin.Instance.Config.SocketPort);
            Server.AddWebSocketService<LogService>("/logs");
            Server.Start();
            Logger.Info($"Socket server is ready and is listening on 0.0.0.0:{Plugin.Instance.Config.SocketPort}");
        }

        public void Stop()
        {
            Server.Stop();
            IsEnabled = false;
        }
    }
}
