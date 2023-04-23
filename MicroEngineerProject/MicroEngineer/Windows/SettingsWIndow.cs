using Newtonsoft.Json;

namespace MicroMod
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class SettingsWIndow : BaseWindow
    {
        [JsonProperty]
        internal Theme ActiveTheme { get; set; }

        internal void LoadSettings()
        {
            Styles.SetActiveTheme(ActiveTheme);
        }
    }
}