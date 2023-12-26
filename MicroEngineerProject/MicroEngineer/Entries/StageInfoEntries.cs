using KSP.Sim.DeltaV;

namespace MicroMod
{
    public class StageInfoEntry: BaseEntry
    { }

    public class StageInfo : StageInfoEntry
    {
        public StageInfo()
        {
            Name = "Stage Info";
            Description = "";
            EntryType = EntryType.StageInfo;
            Category = MicroEntryCategory.Stage;
            IsDefault = true;
            BaseUnit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = ParseStages(Utility.ActiveVessel.VesselDeltaV?.StageInfo);
        }

        public override string ValueDisplay => base.ValueDisplay;

        public List<Stage> ParseStages(List<DeltaVStageInfo> deltaVStages)
        {
            var stages = new List<Stage>();

            if (deltaVStages == null)
                return stages;

            var nonEmptyStages = new List<DeltaVStageInfo>();
            foreach (var stage in deltaVStages)
            {
                if (stage.DeltaVinVac > 0.0001 || stage.DeltaVatASL > 0.0001)
                {
                    nonEmptyStages.Add(stage);
                }
            }
            
            for (int i = nonEmptyStages.Count - 1; i >= 0; i--)
            {
                var time = Utility.ParseSecondsToTimeFormat(nonEmptyStages[i].StageBurnTime);
                var stage = new Stage
                {
                    StageNumber = deltaVStages.Count - nonEmptyStages[i].Stage,
                    DeltaVActual = nonEmptyStages[i].DeltaVActual,
                    TwrActual = nonEmptyStages[i].TWRActual,
                    BurnDays = time.Days,
                    BurnHours = time.Hours,
                    BurnMinutes = time.Minutes,
                    BurnSeconds = time.Seconds
                };
                stages.Add(stage);
            }

            return stages;
        }
    }
}