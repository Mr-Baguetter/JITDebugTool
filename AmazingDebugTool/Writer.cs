#if EXILED
using Exiled.API.Features;
#else
using LabApi.Features.Wrappers;
#endif

using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Concurrent;
using JITDebugTool.API.Features;
using System.Collections.Generic;
using System.Threading.Tasks;
using JITDebugTool.API.SerializedElements;

namespace JITDebugTool
{
#pragma warning disable CS4014
    internal class Writer()
    {
        internal readonly ConcurrentQueue<CallEntry> fullLogs = [];

        internal readonly ConcurrentDictionary<string, SerializedMethod> fullMethods = [];

        internal readonly ConcurrentQueue<SerializedMethod> _methods = [];

        private readonly ConcurrentQueue<CallEntry> _builder = [];

        public void Start()
        {
            Task.Run(Action);
        }

        public CallEntry PreWrite()
        {
            CallEntry entry = new();
            _builder.Enqueue(entry);
            fullLogs.Enqueue(entry);
            return entry;
        }

        public static void Write(CallEntry entry, object obj, Stopwatch stopwatch, MethodInfo method, StackTrace stackTrace, int threadId)
        {
            try
            {
                entry.Populate(method, stopwatch, stackTrace, obj, threadId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private async Task Action()
        {
#if EXILED
            while (!Round.IsEnded)
#else
            while (!Round.IsRoundEnded)
#endif
            {
                if (_builder.Count == 0 && _methods.Count == 0)
                    goto rtx;

                try
                {
                    List<SerializedCallEntry> calls = [];
                    List<SerializedMethod> methods = [];

                    while (_builder.TryPeek(out CallEntry entry) && entry.IsReady && _builder.TryDequeue(out CallEntry _) && calls.Count < 11)
                        calls.Add(new(entry));

                    while (_methods.TryDequeue(out SerializedMethod method))
                        methods.Add(method);

                    if (calls.Count == 0 && methods.Count == 0)
                        goto rtx;

                    Task.Run(() =>
                    {
                        LogService.Broadcast(SerializedCallEntry.Serialize(new SerializedMessage(calls, methods)));
                    });
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                rtx:
                await Task.Delay(10);
            }
        }
    }
}