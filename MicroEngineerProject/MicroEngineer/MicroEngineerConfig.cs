using SpaceWarp.API.Configuration;
using Newtonsoft.Json;

namespace MicroEngineer
{
    [JsonObject(MemberSerialization.OptOut)]
    [ModConfig]
    public class MicroEngineerConfig
    {
    }
}