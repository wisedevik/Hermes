using Newtonsoft.Json;

namespace Hermes.Core.Config;

internal static class ConfigManager
{
    public static Config Config = LoadConfig();

    public static Config GenerateConfig(string filePath = "config.cfg")
    {
        if (File.Exists(filePath))
        {
            Console.WriteLine("Config file already exists.");
            return null;
        }

        Config config = new Config()
        {
            Version = "0.0.7",
            ModulePath = "modules"
        };
        string serConfig = JsonConvert.SerializeObject(config);

        File.WriteAllText(filePath, serConfig.Trim());
        Console.WriteLine($"[d] genereated config: {serConfig}");

        return config;
    }

    public static Config? LoadConfig(string filePath = "config.cfg")
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Config file not found!");
            GenerateConfig();
        }

        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
                continue;
        }

        return JsonConvert.DeserializeObject<Config>(File.ReadAllText(filePath));
    }
}
