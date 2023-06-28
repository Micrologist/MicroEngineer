using MicroEngineer.UI;

namespace MicroMod
{
    public class TargetWindow : EntryWindow
    {
        public override void RefreshData()
        {
            base.RefreshData();

            // Toggle showing/hiding UI window depending on whether a target exists
            FlightSceneController.Instance.TargetWindowShown = Utility.TargetExists();
        }
    }
}
