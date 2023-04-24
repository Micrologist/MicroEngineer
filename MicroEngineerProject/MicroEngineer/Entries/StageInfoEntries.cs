
namespace MicroMod
{
    public class StageInfoEntry: BaseEntry
    { }

    public class StageInfo : StageInfoEntry
    {
        public StageInfo()
        {
            Name = "Stage Info";
            Description = "Stage Info object, not implemented yet."; // TODO Stage Info display and description
            Category = MicroEntryCategory.Stage;
            IsDefault = true;
            BaseUnit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.StageInfo;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }
}
