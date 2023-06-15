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
        public VisualElement Body { get; set; }

        public MainGuiController()
        { }

        public void OnEnable()
        {
            _logger.LogDebug("Entering Start() of MainGuiController");
            MainGui = GetComponent<UIDocument>();
            Root = MainGui.rootVisualElement;
            Header = Root.Q<VisualElement>("header");
            BuildMainGuiHeader();
            Body = Root.Q<VisualElement>("body");            
            BuildDockedWindows();

            Root[0].RegisterCallback<PointerUpEvent>(UpdateWindowPosition);

            MainGuiWindow = (MainGuiWindow)Manager.Instance.Windows.Find(w => w is MainGuiWindow);
            Root[0].transform.position = MainGuiWindow.FlightRect.position;

            // Set visibility
            SetWindowVisibility(Manager.Instance.Windows.OfType<MainGuiWindow>().FirstOrDefault().IsFlightActive);
            //SetWindowVisibility(false);
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
            
            // TEMP
            Button saveBtn = new Button() { text = "Save" };
            saveBtn.RegisterCallback<ClickEvent>(_ => Utility.SaveLayout(Manager.Instance.Windows));
            mainGuiHeader.Add(saveBtn);
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

        /*
        private void CenterWindow(GeometryChangedEvent evt)
        {
            if (evt.newRect.width == 0 || evt.newRect.height == 0)
                return;

            Root[0].transform.position = new Vector2((ReferenceResolution.Width - evt.newRect.width) / 2, (ReferenceResolution.Height - evt.newRect.height) / 2);
            Root[0].UnregisterCallback<GeometryChangedEvent>(CenterWindow);
        }
        */

        private void HandleSettingsButton()
        {
            throw new NotImplementedException();
        }

        private void HandleCloseButton()
        {
            throw new NotImplementedException();
        }
    }
}
