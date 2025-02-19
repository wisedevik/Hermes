using Newtonsoft.Json;

namespace Hermes.Core.Config;

internal class Config
{
    [JsonProperty("version")]
    public string Version { get; set; }
    [JsonProperty("module_path")]
    public string ModulePath { get; set; }
}
