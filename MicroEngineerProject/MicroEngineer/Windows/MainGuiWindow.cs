using Newtonsoft.Json;

namespace MicroMod
{
    internal class MainGuiWindow : BaseWindow
    {
        [JsonProperty]
        internal int LayoutVersion;
    }
}