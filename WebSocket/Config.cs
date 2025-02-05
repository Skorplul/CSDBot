using System;
using System.ComponentModel;
using System.IO;
using CSDBot.API;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public sealed class Config
{
    private static Config? _instance;
    private static readonly object _lock = new object();


    [Description("If debugging messages should be shown.")]
    public bool IsDebug { get; set; } = false;

    [Description("The token for the bot used.")]
    public string BotToken { get; set; } = "";

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
        {
            Log.Warn($"Config file not found: {filePath}, creating new one!");

            var firstConf = new Config();

            // Serialisiere in YAML
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            string yamlContent = serializer.Serialize(firstConf);
            
            // YAML-Content in die Datei schreiben
            Log.Warn($"before : {yamlContent}");
            File.WriteAllText(filePath, yamlContent, new System.Text.UTF8Encoding(false));
            Log.Debug($"Konfigurationsdatei wurde erstellt: {Path.GetFullPath(filePath)}");
        }

        try{
            var yaml = File.ReadAllText(filePath, new System.Text.UTF8Encoding(false));
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).IgnoreUnmatchedProperties().Build();

            Log.Warn($"after : {yaml}");

            return deserializer.Deserialize<Config>(yaml);
        }
        catch(Exception ex)
        {
            throw new Exception("Deserialisation error:", ex);
        }
    }
}
