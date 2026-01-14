#if EXILED
using Exiled.API.Interfaces;
#endif

using JITDebugTool.API.Extensions;

namespace JITDebugTool.API.SerializedElements
{
#if EXILED
    internal class SerializedTargetPlugin(IPlugin<IConfig> plugin)
#else
    internal class SerializedTargetPlugin(LabApi.Loader.Features.Plugins.Plugin plugin)
#endif
    {
        public string Name { get; } = plugin.Name;
#if EXILED
        public string Prefix { get; } = plugin.Prefix;
#else
        public string Description { get; } = plugin.Description;
#endif

        public string Author { get; } = plugin.Author;

        public string Version { get; } = plugin.Version.ToString();

        public string AssemblyName { get; } = plugin.GetAssembly().FullName;

        public string AssemblySimpleName { get; } = plugin.GetAssembly().GetName().Name;
    }
}
