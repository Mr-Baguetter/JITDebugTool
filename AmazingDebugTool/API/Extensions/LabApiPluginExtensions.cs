using LabApi.Loader;
using System.Reflection;

namespace JITDebugTool.API.Extensions
{
    public static class LabApiPluginExtensions
    {
        public static Assembly GetAssembly(this LabApi.Loader.Features.Plugins.Plugin plugin)
        {
            PluginLoader.Plugins.TryGetValue(plugin, out var asm);
            return asm;
        }
    }
}
