global using Logger = LabApi.Features.Console.Logger;
#if EXILED
using Exiled.API.Enums;
using Exiled.API.Features;
#else
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
#endif

using HarmonyLib;
using JITDebugTool.API.Features;

using System;
using LabApi.Loader.Features.Plugins.Enums;

namespace JITDebugTool
{
    internal class Plugin : Plugin<Config>
    {
        public override string Name => "JITDebugTool";

        public override string Author => "FoxWorn3365 & UCS Collective";

        public override Version Version => new(1, 0, 0);
#if EXILED
        public override PluginPriority Priority => PluginPriority.First;
        public override string Prefix => "jit_debug_tool";
#else
        public override Version RequiredApiVersion { get; } = LabApiProperties.CurrentVersion;
        public override string Description => ":3";
        public override LoadPriority Priority => LoadPriority.Lowest;

#endif

        internal static Plugin Instance { get; private set; }

        internal Writer writer;

        internal Patcher patcher;

        private Harmony _harmony;

#if EXILED
        public override void OnEnabled()
#else
        public override void Enable()
#endif
        {
            Instance = this;
            writer = new();
            writer.Start();
            new SocketServer();

            _harmony = new($"adb-{Guid.NewGuid()}");

            patcher = new(_harmony);
            patcher.PatchMethods();

            Logger.Info("Welcome on JITDebugTool!");

#if EXILED
            base.OnEnabled();
#endif
        }

#if EXILED
        public override void OnDisabled()
#else
        public override void Disable()
#endif
        {
            _harmony.UnpatchAll();
            _harmony = null;

            Instance = null;

#if EXILED
            base.OnDisabled();
#endif
        }
    }
}
