using Newtonsoft.Json;

namespace MicroMod
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class SettingsWIndow : BaseWindow
    {
        [JsonProperty]
        internal Theme ActiveTheme { get; set; }
        [JsonProperty]
        private bool snapWindows = true;
        internal bool SnapWindows { get => snapWindows; set => snapWindows = value; }
        [JsonProperty]
        private float snapDistance = 20f;
        internal float SnapDistance { get => snapDistance; set => snapDistance = value; }

        internal void LoadSettings()
        {
            Styles.SetActiveTheme(ActiveTheme);
        }
    }
}