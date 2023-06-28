using BepInEx.Logging;
using MicroMod;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class StageInfoOABController : MonoBehaviour
    {
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.StageInfoOABController");

        public StageInfoOabWindow StageInfoOABWindow { get; set; }
        public UIDocument StageInfoOAB { get; set; }
        public VisualElement Root { get; set; }
        public VisualElement TitleBar { get; set; }
        public VisualElement TorqueContainer { get; set; }
        public Button CloseButton { get; set; }
        public VisualElement Header { get; set; }
        public VisualElement Body { get; set; }
        public StageInfo_OAB StageEntry { get; set; }
        public VisualElement Footer { get; set; }
        public VisualElement TotalAslDeltaVContainer { get; set; }
        public VisualElement TotalVacDeltaVContainer { get; set; }
        public VisualElement TotalBurnTimeContainer { get; set; }

        public StageInfoOABController()
        { }

        public void OnEnable()
        {
            _logger.LogDebug("Entering OnEnable.");
            StageInfoOABWindow = (StageInfoOabWindow)Manager.Instance.Windows.Find(w => w is StageInfoOabWindow);

            StageInfoOAB = GetComponent<UIDocument>();
            Root = StageInfoOAB.rootVisualElement;

            BuildTitleBar();
            BuildHeader();
            BuildBody();
            BuildFooter();

            Root[0].RegisterCallback<PointerUpEvent>(UpdateWindowPosition);
            Root[0].transform.position = StageInfoOABWindow.EditorRect.position;
        }

        private void UpdateWindowPosition(PointerUpEvent evt)
        {
            if (StageInfoOABWindow == null)
                return;

            StageInfoOABWindow.EditorRect.position = Root[0].transform.position;
            _logger.LogDebug($"Initiating Save from UpdateWindowPosition.");
            Utility.SaveLayout(Manager.Instance.Windows);
        }

        public void Update()
        {
            return;
        }

        private void BuildTitleBar()
        {
            _logger.LogDebug("Entering BuildTitleBar.");
            TitleBar = Root.Q<VisualElement>("titlebar");

            TorqueContainer = TitleBar.Q<VisualElement>("torque-container");
            var torqueEntry = StageInfoOABWindow.Entries.Find(e => e is Torque);
            VisualElement torqueControl = new BaseEntryControl(torqueEntry);
            TorqueContainer.Add(torqueControl);

            CloseButton = TitleBar.Q<Button>("close-button");
            CloseButton.RegisterCallback<ClickEvent>(OnCloseButton);
        }

        private void BuildHeader()
        {
            _logger.LogDebug("Entering BuildHeader.");
            Header = Root.Q<VisualElement>("header");
        }

        private void BuildBody()
        {
            _logger.LogDebug("Entering BuildBody.");
            Body = Root.Q<VisualElement>("body");
            StageEntry = StageInfoOABWindow.Entries.Find(e => e is StageInfo_OAB) as StageInfo_OAB;
            StageEntry.OnStageInfoOABChanged += HandleStageInfoChanged;

            // TODO maybe purge celestial bodies in stage entry here
        }

        private void BuildStages(List<DeltaVStageInfo_OAB> stages)
        {
            _logger.LogDebug("Entering BuildStages.");
            for (int i = stages.Count - 1; i >= 0; i--)
            {
                var stage = stages[i];
                var control = new StageInfoOABEntryControl(
                    stage.Stage, stage.TWRVac, stage.TWRASL, stage.DeltaVASL,
                    stage.DeltaVVac, stage.StageBurnTime.Days, stage.StageBurnTime.Hours,
                    stage.StageBurnTime.Minutes, stage.StageBurnTime.Seconds,
                    MicroCelestialBodies.Instance.Bodies.Select(b => b.DisplayName).ToList(),
                    stage.CelestialBody.Name
                    );
                var celestialBodyDropdown = control.Q<DropdownField>("body-dropdown");
                // Update entry's CelestialBody at the same index as the stage
                celestialBodyDropdown.RegisterValueChangedCallback(evt => StageEntry.UpdateCelestialBodyAtIndex(stage.Index, celestialBodyDropdown.value));
                Body.Add(control);
            }

            // If there are no stages, insert an empty stage
            if (stages == null || stages.Count == 0)
            {
                var control = new StageInfoOABEntryControl();
                Body.Add(control);
            }
        }

        private void HandleStageInfoChanged(List<DeltaVStageInfo_OAB> stages)
        {
            _logger.LogDebug("Entering HandleStageInfoChanged.");
            Body.Clear();
            BuildStages(stages);
        }

        private void BuildFooter()
        {
            _logger.LogDebug("Entering BuildFooter.");
            BaseEntry entry;
            VisualElement control;

            Footer = Root.Q<VisualElement>("footer");

            TotalAslDeltaVContainer = Root.Q<VisualElement>("asl-dv-container");
            entry = StageInfoOABWindow.Entries.Find(e => e is TotalDeltaVASL_OAB);
            control = new BaseEntryControl(entry);
            TotalAslDeltaVContainer.Add(control);
            
            TotalVacDeltaVContainer = Root.Q<VisualElement>("vac-dv-container");
            entry = StageInfoOABWindow.Entries.Find(e => e is TotalDeltaVVac_OAB);
            control = new BaseEntryControl(entry);
            TotalVacDeltaVContainer.Add(control);

            TotalBurnTimeContainer = Root.Q<VisualElement>("burn-time-container");
            entry = StageInfoOABWindow.Entries.Find(e => e is TotalBurnTime_OAB);
            control = new TimeEntryControl(entry);
            TotalBurnTimeContainer.Add(control);
        }

        private void OnCloseButton(ClickEvent evt)
        {
            _logger.LogDebug("Entering OnCloseButton.");
            StageInfoOABWindow.IsEditorActive = false;
            _logger.LogDebug($"Initiating Save from OnCloseButton.");
            Utility.SaveLayout(Manager.Instance.Windows);
            OABSceneController.Instance.ShowGui = false;
        }
    }
}
