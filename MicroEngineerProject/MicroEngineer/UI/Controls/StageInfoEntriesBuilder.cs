using MicroMod;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class StageInfoEntriesBuilder : VisualElement
    {
        public StageInfoEntriesBuilder(BaseEntry entry)
        {
            BuildStages((List<Stage>)entry.EntryValue);

            entry.OnStageInfoChanged += HandleStageInfoChanged;
        }

        private void BuildStages(List<Stage> stages)
        {
            foreach (Stage stage in stages ?? Enumerable.Empty<Stage>())
            {
                this.Add(new StageInfoEntryControl(stage.StageNumber, stage.DeltaVActual, stage.TwrActual, stage.BurnDays, stage.BurnHours, stage.BurnMinutes, stage.BurnSeconds));
            }
        }

        private void HandleStageInfoChanged(List<Stage> stages)
        {
            this.Clear();
            BuildStages(stages);
        }
    }
}
