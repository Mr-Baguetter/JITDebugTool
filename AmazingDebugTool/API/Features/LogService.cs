using JITDebugTool.API.SerializedElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
#if LABAPI
using static JITDebugTool.API.Extensions.QueueExtensions;
#endif

namespace JITDebugTool.API.Features
{
#pragma warning disable IDE0306
#pragma warning disable IDE0305

    internal class LogService : WebSocketBehavior
    {
        internal static bool CanTransmit => _authed.Count > 0;

        private static readonly List<LogService> _authed = [];

        private bool _isAuthed = false;

        protected override void OnMessage(MessageEventArgs e)
        {
            if (!_isAuthed)
                if (e.Data == Plugin.Instance.Config.SocketKey)
                {
                    _isAuthed = true;
                    SendAsync($"WELCOME! {Plugin.Instance.Name} v{Plugin.Instance.Version}", delegate { });
                    SendAsync(SerializedCallEntry.Serialize(new SerializedWelcome()), delegate { });
                    SendAsync(SerializedCallEntry.Serialize(Plugin.Instance.patcher.pluginData), delegate { });
                } 
                else
                {
                    SendAsync("WRONG KEY!", delegate { });
                }
            else
            {
                if (e.Data == "GET_EVENT_CHRONO")
                {
                    Logger.Info("GETTING CHRONO v2!");
                    try
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                Queue<CallEntry> queue = new(Plugin.Instance.writer.fullLogs.ToArray());
                                Queue<SerializedMethod> methods = new(Plugin.Instance.writer.fullMethods.Values.ToArray());

                                Logger.Info($"PREPPING LOGS AS {queue.Count}! ({Plugin.Instance.writer.fullLogs.Count})");

                                while (queue.Count > 0 || methods.Count > 0)
                                {
                                    List<SerializedCallEntry> elements = [];
                                    List<SerializedMethod> _methods = [];

                                    while (queue.TryDequeue(out CallEntry entry) && elements.Count < 30 && entry is not null)
                                        elements.Add(new(entry));

                                    while (methods.TryDequeue(out SerializedMethod method) && _methods.Count < 150)
                                        _methods.Add(method);

                                    SendAsync(SerializedCallEntry.Serialize(new SerializedMessage(elements, _methods)), delegate { });
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
                else if (e.Data == "SUBSCRIBE")
                    _authed.Add(this);
                else if (e.Data == "PING")
                    SendAsync("PONG", delegate { });
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            _authed.Remove(this);
        }

        public static void Broadcast(string message)
        {
            foreach (LogService log in _authed)
                if (log.Context.WebSocket.IsAlive)
                    log.SendAsync(message, delegate { });
        }
    }
}
