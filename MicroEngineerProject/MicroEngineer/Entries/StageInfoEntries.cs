using KSP.Sim.DeltaV;
using UnityEngine;

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

            var nonEmptyStages = deltaVStages.FindAll(s => s.DeltaVinVac > 0.0001 || s.DeltaVatASL > 0.0001);

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