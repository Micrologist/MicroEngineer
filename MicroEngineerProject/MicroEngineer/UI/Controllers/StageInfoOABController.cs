using BepInEx.Logging;
using MicroMod;
using System;
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
        public VisualElement Footer { get; set; }
        public VisualElement TotalAslDeltaVContainer { get; set; }
        public VisualElement TotalVacDeltaVContainer { get; set; }
        public VisualElement TotalBurnTimeContainer { get; set; }

        public StageInfoOABController()
        { }

        public void OnEnable()
        {
            _logger.LogDebug("Entering OnEnable().");
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
            // Nothing in Header at the moment
        }

        private void BuildBody()
        {
            Body = Root.Q<VisualElement>("body");
            // TODO
        }

        private void BuildFooter()
        {
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
            StageInfoOABWindow.IsEditorActive = false;
            Utility.SaveLayout(Manager.Instance.Windows);
            OABSceneController.Instance.ShowGui = false;
        }
    }
}
