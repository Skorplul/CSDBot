using System;
using System.ComponentModel;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public sealed class Config
{
    private static Config? _instance;
    private static readonly object _lock = new object();

    [Description("If debugging messages should be shown.")]
        public bool IsDebug { get; set; } = false;
    [Description("The token for the bot used.")]
        public string BotToken { get; } = "";


    // Private constructor to prevent direct instantiation
    private Config() { }

    public static Config Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock) // Ensures thread safety
                {
                    if (_instance == null)
                    {
                        _instance = LoadConfig("config.yaml"); // Load YAML when first accessed
                    }
                }
            }
            return _instance;
        }
    }

    private static Config LoadConfig(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Config file not found: {filePath}");

        var yaml = File.ReadAllText(filePath);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<Config>(yaml);
    }
}
