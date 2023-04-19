using KSP.Sim.impl;
using Newtonsoft.Json;

namespace MicroMod
{
    /// <summary>
    /// Base class for all Entries (values that can be attached to windows)
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class MicroEntry
    {
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public string Description;
        [JsonProperty]
        public MicroEntryCategory Category;
        [JsonProperty]
        public string Unit;
        [JsonProperty]
        public string Formatting;

        public virtual object EntryValue { get; set; }

        /// <summary>
        /// Controls how the value should be displayed. Should be overriden in a inheritet class for a concrete implementation.
        /// </summary>
        public virtual string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(this.Formatting) ? EntryValue.ToString() : String.Format(Formatting, EntryValue);
            }
        }

        public virtual void RefreshData() { }
    }


    public class StageInfo : MicroEntry
    {
        public StageInfo()
        {
            Name = "Stage Info";
            Description = "Stage Info object, not implemented yet."; // TODO Stage Info display and description
            Category = MicroEntryCategory.Stage;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.VesselDeltaV?.StageInfo;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }
}
