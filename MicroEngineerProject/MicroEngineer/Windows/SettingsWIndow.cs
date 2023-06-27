using Newtonsoft.Json;

namespace MicroMod
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SettingsWIndow : BaseWindow
    {
        [JsonProperty]
        public Theme ActiveTheme { get; set; }
        [JsonProperty]
        private bool snapWindows = true;
        public bool SnapWindows { get => snapWindows; set => snapWindows = value; }
        [JsonProperty]
        private float snapDistance = 20f;
        public float SnapDistance { get => snapDistance; set => snapDistance = value; }

        public void LoadSettings()
        {
            Styles.SetActiveTheme(ActiveTheme);
        }
    }
}