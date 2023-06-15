using Newtonsoft.Json;

namespace MicroMod
{
    public class MainGuiWindow : BaseWindow
    {
        [JsonProperty]
        public int LayoutVersion;
    }
}