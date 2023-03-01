using SpaceWarp.API.Configuration;
using Newtonsoft.Json;

namespace MicroEngineer
{
    [JsonObject(MemberSerialization.OptOut)]
    [ModConfig]
    public class MicroEngineerConfig
    {
         [ConfigField("pi")] [ConfigDefaultValue(3.14159)] public double pi;
    }
}