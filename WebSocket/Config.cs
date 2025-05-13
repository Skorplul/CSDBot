using System.ComponentModel;
using PRMainBot.API;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public sealed class Config
{
    private static Config? _instance;
    private static readonly object _lock = new object();
    private static readonly string ConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config.yaml");


    [Description("If debugging messages should be shown.")]
    public bool IsDebug { get; set; } = false;

    [Description("The token for the bot used.")]
    public string BotToken { get; set; } = "";

    [Description("The Northwood API key. Ask Skorp")]
    public string SL_API_Key { get; set; } = "";

    [Description("The server verify acc ID. Ask Skorp")]
    public string SL_Acc_ID { get; set; } = "";

    [Description("The Port of tracking server.")]
    public int Server_Port { get; set; } = 7777;

    public Config() 
    { 

    }

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
                        _instance = LoadConfig(ConfigPath); // Load YAML when first accessed
                        Task.Delay(500);
                    }
                }
            }
            return _instance;
        }
    }


    private static Config LoadConfig(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Log.Warn($"Config file not found: {filePath}, creating new one!");

                var firstConf = new Config();

                // Serialisiere in YAML
                var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
                string yamlContent = serializer.Serialize(firstConf);
                
                // YAML-Content in die Datei schreiben
                // Log.Debug($"write before : {yamlContent}");
                File.WriteAllText(filePath, yamlContent, new System.Text.UTF8Encoding(false));
                Log.Info($"Konfigurationsdatei wurde erstellt: {Path.GetFullPath(filePath)}"); 
            }

        
            var yaml = File.ReadAllText(filePath, new System.Text.UTF8Encoding(false));
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

            // Log.Debug($"after read : {yaml}");

            return deserializer.Deserialize<Config>(yaml);
        }
        catch(Exception ex)
        {
            // I know this try catch is useless lol
            throw new Exception("ERROR:", ex);
        }
    }
}
