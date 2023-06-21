using BepInEx.Logging;
using MicroMod;
using UitkForKsp2.API;
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
        public UIDocument EditWindows { get; set; }

        public MainGuiController()
        { }

        public void OnEnable()
        {
            _logger.LogDebug("Entering OnEnable() of MainGuiController");
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
            _logger.LogDebug("OnPointerUpEvent triggered.");

            if (MainGuiWindow == null)
                return;

            MainGuiWindow.FlightRect.position = Root[0].transform.position;
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
            EditWindowsButton.RegisterCallback<ClickEvent>(OnOpenEditWindows);

            // TEMP
            /*
            Button saveBtn = new Button() { text = "Save" };
            saveBtn.RegisterCallback<ClickEvent>(_ => Utility.SaveLayout(Manager.Instance.Windows));
            mainGuiHeader.Add(saveBtn);
            */
            // END TEMP
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

        public void SetWindowVisibility(bool isVisible)
        {
            if (isVisible)
                Root[0].style.display = DisplayStyle.Flex;
            else
                Root[0].style.display = DisplayStyle.None;
        }

        private void HandleSettingsButton()
        {
            throw new NotImplementedException();
        }

        private void HandleCloseButton()
        {
            throw new NotImplementedException();
        }

        private void OnCloseButton(ClickEvent evt)
        {
            MainGuiWindow.IsFlightActive = false;
            Utility.SaveLayout(Manager.Instance.Windows);
            FlightSceneController.Instance.ShowGui = false;
        }

        private void OnOpenEditWindows(ClickEvent evt)
        {
            if (EditWindows == null)
            {
                EditWindows = Window.CreateFromUxml(Uxmls.Instance.EditWindows, "EditWindows", null, true);

                EditWindows.rootVisualElement[0].RegisterCallback<GeometryChangedEvent>((evt) => Utility.CenterWindow(evt, EditWindows.rootVisualElement[0]));

                EditWindowsController editWindowsController = EditWindows.gameObject.AddComponent<EditWindowsController>();
            }
            else
            {
                var controller = EditWindows.GetComponent<EditWindowsController>();
                controller.CloseWindow();
                EditWindows = null;
            }
        }
    }
}
