using System;
using System.ComponentModel;
using YamlDotNet.Serialization;
namespace CSDBot
{
    public static class Config
    {
        [Description("If debugging messages should be shown.")]
        public static bool IsDebug { get; set; } = false;
    }
}