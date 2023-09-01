using Newtonsoft.Json;

namespace MicroMod
{
    /// <summary>
    /// An alternative unit, activated by double-clicking the entry
    /// </summary>
    public class AltUnit
    {
        [JsonProperty]
        public bool IsActive;
        [JsonProperty]
        public string Unit;
        [JsonProperty]
        public float Factor;
    }
}
