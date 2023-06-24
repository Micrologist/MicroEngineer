using Newtonsoft.Json;

namespace MicroMod
{
    public class StageInfoOabWindow : BaseWindow
    {
        [JsonProperty]
        internal bool IsEditorPoppedOut;
        [JsonProperty]
        internal List<BaseEntry> Entries;

        internal virtual void RefreshData()
        {
            if (Entries == null || Entries.Count == 0)
                return;

            foreach (BaseEntry entry in Entries)
                entry.RefreshData();
        }
    }
}
