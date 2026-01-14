#if EXILED
using Exiled.API.Interfaces;
#endif
using System.Collections.Generic;
using System.ComponentModel;

namespace JITDebugTool
{
#if EXILED
    internal class Config : IConfig
#else
    internal class Config
#endif
    {
        [Description("Whether the plugin is enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether the debug mode is enabled")]
        public bool Debug { get; set; } = false;

        [Description("The targeted plugin's name")]
        public string Plugin { get; set; } = "UncomplicatedCustomRoles";

        [Description("These types will be ignored. Please put the FULL NAME")]
        public List<string> IgnoreTypes { get; set; } = [];

        [Description("These methods will be ignored. Please put only the METHOD NAME")]
        public List<string> IgnoreMethods { get; set; } = [];

        [Description("The port of the socket debug server")]
        public int SocketPort { get; set; } = 1234;

        [Description("The secret key needed to connect to the socket debug server")]
        public string SocketKey { get; set; } = "banana";
    }
}
