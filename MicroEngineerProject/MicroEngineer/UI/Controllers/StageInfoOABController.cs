using BepInEx.Logging;
using MicroMod;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class StageInfoOABController : MonoBehaviour
    {
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

        private static ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.StageInfoOABController");
        private bool _lockUiRefresh;

        private double _totalDeltaVASL;
        private double _totalDeltaVVac;

        public StageInfoOABController()
        { }

        public void OnEnable()
        {
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
            Utility.SaveLayout();
        }

        public void Update()
        {
            return;
        }

        private void BuildTitleBar()
        {
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
            Header = Root.Q<VisualElement>("header");
        }

        private void BuildBody()
        {
            Body = Root.Q<VisualElement>("body");
            StageEntry = StageInfoOABWindow.Entries.Find(e => e is StageInfo_OAB) as StageInfo_OAB;
            StageEntry.OnStageInfoOABChanged += HandleStageInfoChanged;

            // TODO maybe purge celestial bodies in stage entry here
        }

        private void BuildStages(List<DeltaVStageInfo_OAB> stages)
        {
            for (int i = stages.Count - 1; i >= 0; i--)
            {
                var stage = stages[i];
                var control = new StageInfoOABEntryControl(
                    stage.Stage, stage.TWRVac, stage.TWRASL, stage.DeltaVASL,
                    stage.DeltaVVac, stage.StageBurnTime.Days, stage.StageBurnTime.Hours,
                    stage.StageBurnTime.Minutes, stage.StageBurnTime.Seconds,
                    // if stock body selector is set to a body different than the home body (Kerbin)
                    // then pass only this body to the control
                    bodies: string.IsNullOrEmpty(stage.SituationCelestialBody)
                        ? MicroCelestialBodies.Instance.Bodies.Select(b => b.DisplayName).ToList()
                        : new List<string> { stage.SituationCelestialBody},
                    selectedBody: string.IsNullOrEmpty(stage.SituationCelestialBody) 
                        ? stage.CelestialBody.Name
                        : stage.SituationCelestialBody
                    );

                _totalDeltaVASL += stage.DeltaVASL;
                _totalDeltaVVac += stage.DeltaVVac;

                // check if SituationCelestialBody contains data
                // if it does, it means that stock body selector is set to a body different than the home body (Kerbin)
                // in that case we will not allow body changing here since the game already does recalculations for TWR and DeltaV 
                if (string.IsNullOrEmpty(stage.SituationCelestialBody))
                {
                    // SituationCelestialBody does not contain data, perform body dropdown logic normally
                    
                    var celestialBodyDropdown = control.Q<DropdownField>("body-dropdown");
                    celestialBodyDropdown.RegisterCallback<MouseDownEvent>(evt =>
                    {
                        // When user clicks on the celestialBodyDropdown control we lock stage info refresh from happening
                        // and unlock it again when user selects something from the dropdown.
                        // We do this because if VesselDeltaVCalculationMessage is triggered between the user first clicking
                        // on the control and selection something from it, the event will cause the UI to refresh (destroy
                        // and rebuild the controls) and thus the original dropdown control will no longer exist and
                        // celestialBody will not be selectable.
                        _lockUiRefresh = true;
                    });
                    // Update entry's CelestialBody at the same index as the stage
                    celestialBodyDropdown.RegisterValueChangedCallback(evt =>
                    {
                        StageEntry.UpdateCelestialBodyAtIndex(stage.Index, celestialBodyDropdown.value);
                        _lockUiRefresh = false;
                        StageEntry.RefreshData();
                    });
                }
                
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
            if (_lockUiRefresh) return;

            Body.Clear();
            TotalAslDeltaVContainer.Clear();
            _totalDeltaVASL = 0;
            TotalVacDeltaVContainer.Clear();
            _totalDeltaVVac = 0;
            TotalBurnTimeContainer.Clear();
            BuildStages(stages);
            SetTotalValues();
        }

        private void BuildFooter()
        {
            Footer = Root.Q<VisualElement>("footer");
            TotalAslDeltaVContainer = Root.Q<VisualElement>("asl-dv-container");
            TotalVacDeltaVContainer = Root.Q<VisualElement>("vac-dv-container");
            TotalBurnTimeContainer = Root.Q<VisualElement>("burn-time-container");

            SetTotalValues();
        }

        private void SetTotalValues()
        {
            BaseEntry entry;
            VisualElement control;

            entry = StageInfoOABWindow.Entries.Find(e => e is TotalDeltaVASL_OAB);
            // total deltav from stock stage info is wrong because it doesn't take into account different bodies.
            // so here we'll sum up values for all calculated stages and won't subscribe to automatic value changes
            entry.EntryValue = _totalDeltaVASL;
            control = new BaseEntryControl(entry, false);
            TotalAslDeltaVContainer.Add(control);

            entry = StageInfoOABWindow.Entries.Find(e => e is TotalDeltaVVac_OAB);
            // total deltav from stock stage info is wrong because it doesn't take into account different bodies.
            // so here we'll sum up values for all calculated stages and won't subscribe to automatic value changes
            entry.EntryValue = _totalDeltaVVac;
            control = new BaseEntryControl(entry, false);
            TotalVacDeltaVContainer.Add(control);

            entry = StageInfoOABWindow.Entries.Find(e => e is TotalBurnTime_OAB);
            control = new TimeEntryControl(entry);
            TotalBurnTimeContainer.Add(control);
        }

        private void OnCloseButton(ClickEvent evt)
        {
            StageInfoOABWindow.IsEditorActive = false;
            Utility.SaveLayout();
            OABSceneController.Instance.ShowGui = false;
        }
    }
}
