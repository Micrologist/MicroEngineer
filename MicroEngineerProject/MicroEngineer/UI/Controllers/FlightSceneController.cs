using BepInEx.Logging;
using KSP.UI.Binding;
using MicroMod;
using UitkForKsp2.API;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class FlightSceneController
    {
        private static FlightSceneController _instance;
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.FlightSceneController");
        private bool _showGui = false;
        private EditWindowsController _editWindowsController;
        private bool _maneuverWindowOpened = true;
        private bool _targetWindowOpened = true;

        public UIDocument MainGui { get; set; }
        public List<UIDocument> Windows = new ();
        public UIDocument EditWindows { get; set; }        

        public bool ShowGui
        {
            get => _showGui;
            set
            {
                _logger.LogDebug($"Inside ShowGui SET. Old value: {_showGui}. New value: {value}");
                _showGui = value;
                
                GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(value);

                RebuildUI();
                
                // If UI is closing, close EditWindows as well
                if (!value && EditWindows != null)
                    ToggleEditWindows();
            }
        }

        public bool ManeuverWindowOpened
        {
            get => _maneuverWindowOpened;
            set
            {
                if (_maneuverWindowOpened != value)
                {
                    _maneuverWindowOpened = value;
                    RebuildUI();
                }
            }
        }

        public bool TargetWindowOpened
        {
            get => _targetWindowOpened;
            set
            {
                if (_targetWindowOpened != value)
                {
                    _targetWindowOpened = value;
                    RebuildUI();
                }
            }
        }

        public static FlightSceneController Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FlightSceneController();

                return _instance;
            }
        }

        public void InitializeUI()
        {
            _logger.LogDebug("InitializeUI triggered.");
            //Build MainGui
            MainGui = Window.CreateFromUxml(Uxmls.Instance.BaseWindow, "MainGui", null, true);
            MainGuiController mainGuiController = MainGui.gameObject.AddComponent<MainGuiController>();
            MainGui.rootVisualElement[0].RegisterCallback<PointerMoveEvent>(evt => Utility.ClampToScreenUitk(MainGui.rootVisualElement[0]));

            //Build poppedout windows
            foreach (EntryWindow poppedOutWindow in Manager.Instance.Windows.Where(w => w is EntryWindow && ((EntryWindow)w).IsFlightPoppedOut))
            {
                // Skip creating Maneuver and/or Target windows if maneuver/target do not exist
                if ((poppedOutWindow is ManeuverWindow && !ManeuverWindowOpened) || (poppedOutWindow is TargetWindow && !TargetWindowOpened))
                    continue;

                var window = Window.CreateFromUxml(Uxmls.Instance.BaseWindow, poppedOutWindow.Name, null, !poppedOutWindow.IsLocked);
                var header = window.rootVisualElement.Q<VisualElement>("header");
                var body = window.rootVisualElement.Q<VisualElement>("body");
                var footer = window.rootVisualElement.Q<VisualElement>("footer");
                EntryWindowController ewc = new EntryWindowController(poppedOutWindow, window.rootVisualElement);
                body.Add(ewc.Root);

                if (poppedOutWindow.IsLocked)
                {
                    header.AddToClassList("no-border");
                    body.AddToClassList("no-border");
                    footer.AddToClassList("no-border");
                    var entryRoot = body.Q<VisualElement>("window-root");
                    entryRoot.AddToClassList("no-border");
                }

                //Keep window inside screen bounds
                window.rootVisualElement[0].RegisterCallback<PointerMoveEvent>(evt => Utility.ClampToScreenUitk(window.rootVisualElement[0]));

                Windows.Add(window);
            }
        }

        public void RebuildUI()
        {
            _logger.LogDebug("RebuildUI triggered.");
            DestroyUI();
            if (ShowGui)
                InitializeUI();
        }

        public void DestroyUI()
        {
            _logger.LogDebug("DestroyUI triggered.");
            if (MainGui != null && MainGui.gameObject != null)
                MainGui.gameObject.DestroyGameObject();
            GameObject.Destroy(MainGui);

            if (Windows != null)
            {
                foreach (var w in Windows)
                {
                    if (w != null && w.gameObject != null)
                        w.gameObject?.DestroyGameObject();
                    GameObject.Destroy(w);
                }
                Windows.Clear();
            }
        }

        public void ToggleEditWindows() => ToggleEditWindows(false);
        public void ToggleEditWindows(bool needToOpenWithSpecificWindowSelected, int editableWindowId = 0)
        {
            if (EditWindows == null)
            {
                EditWindows = Window.CreateFromUxml(Uxmls.Instance.EditWindows, "EditWindows", null, true);

                EditWindows.rootVisualElement[0].RegisterCallback<GeometryChangedEvent>((evt) => Utility.CenterWindow(evt, EditWindows.rootVisualElement[0]));

                _editWindowsController = EditWindows.gameObject.AddComponent<EditWindowsController>();
                _editWindowsController.SelectedWindowId = editableWindowId;
            }
            else if (needToOpenWithSpecificWindowSelected)
            {
                _editWindowsController.SelectedWindowId = editableWindowId;
                _editWindowsController.ResetSelectedWindow();
            }
            else
            {
                var controller = EditWindows.GetComponent<EditWindowsController>();
                controller.CloseWindow();
                EditWindows = null;
            }
        }

        public List<EntryWindow> GetEditableWindows()
        {
            return Manager.Instance.Windows.FindAll(w => w is EntryWindow).Cast<EntryWindow>().ToList().FindAll(w => w.IsEditable);
        }
    }
}
