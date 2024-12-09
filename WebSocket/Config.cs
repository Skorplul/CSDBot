using System;
using YamlDotNet.Serialization;
namespace CSDBot;
{
    public static class Config
    {
        public static bool IsDebug { get; set; } = false;
    }
}