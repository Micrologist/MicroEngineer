using BepInEx.Logging;
using MicroMod;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class MainGuiController : MonoBehaviour
    {
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.MainGuiController");

        public MainGuiWindow MainGuiWindow { get; set; }
        public UIDocument MainGui { get; set; }
        public VisualElement Root { get; set; }
        public VisualElement Header { get; set; }
        public Button EditWindowsButton { get; set; }
        public Button CloseButton { get; set; }
        public VisualElement Body { get; set; }

        public MainGuiController()
        { }

        public void OnEnable()
        {
            _logger.LogDebug("Entering OnEnable.");
            MainGui = GetComponent<UIDocument>();
            Root = MainGui.rootVisualElement;
            Header = Root.Q<VisualElement>("header");
            BuildMainGuiHeader();
            Body = Root.Q<VisualElement>("body");
            BuildDockedWindows();

            Root[0].RegisterCallback<PointerUpEvent>(UpdateWindowPosition);

            MainGuiWindow = (MainGuiWindow)Manager.Instance.Windows.Find(w => w is MainGuiWindow);
            Root[0].transform.position = MainGuiWindow.FlightRect.position;
        }

        private void UpdateWindowPosition(PointerUpEvent evt)
        {
            if (MainGuiWindow == null)
                return;

            MainGuiWindow.FlightRect.position = Root[0].transform.position;
            _logger.LogDebug($"Initiating Save from UpdateWindowPosition.");
            Utility.SaveLayout();
        }

        public void Update()
        {
            return;
        }

        private void BuildMainGuiHeader()
        {
            var mainGuiHeader = Uxmls.Instance.MainGuiHeader.CloneTree();
            CloseButton = mainGuiHeader.Q<Button>("close-button");
            CloseButton.RegisterCallback<ClickEvent>(OnCloseButton);
            EditWindowsButton = mainGuiHeader.Q<Button>("editwindows-button");
            EditWindowsButton.RegisterCallback<ClickEvent>(evt => FlightSceneController.Instance.ToggleEditWindows());
            Header.Add(mainGuiHeader);
        }

        public void BuildDockedWindows()
        {
            foreach (EntryWindow entryWindow in Manager.Instance.Windows.Where(w => w is EntryWindow && !((EntryWindow)w).IsFlightPoppedOut))
            {
                EntryWindowController ewc = new EntryWindowController(entryWindow, Root);

                Body.Add(ewc.Root);
                _logger.LogDebug($"Window {entryWindow.Name} added to root.");
            }
        }

        private void OnCloseButton(ClickEvent evt)
        {            
            MainGuiWindow.IsFlightActive = false;

            _logger.LogDebug($"Initiating Save from OnCloseButton.");
            Utility.SaveLayout();
            FlightSceneController.Instance.ShowGui = false;
        }
    }
}
