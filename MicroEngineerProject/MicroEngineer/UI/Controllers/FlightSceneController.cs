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
        //private UnityEvent RebuildUIEvent;

        public UIDocument MainGui;
        public List<UIDocument> Windows = new ();
        
        public bool ShowGui
        {
            get => _showGui;
            set
            {
                _logger.LogDebug($"Inside ShowGui SET. Old value: {_showGui}. New value: {value}");
                _showGui = value;
                //ToggleWindowVisibility(value);                
                
                GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(value);

                RebuildUI();
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

            //Build poppedout windows
            foreach (EntryWindow poppedOutWindow in Manager.Instance.Windows.Where(w => w is EntryWindow && ((EntryWindow)w).IsFlightPoppedOut))
            {
                var window = Window.CreateFromUxml(Uxmls.Instance.BaseWindow, poppedOutWindow.Name, null, true);
                var body = window.rootVisualElement.Q<VisualElement>("body");
                EntryWindowController ewc = new EntryWindowController(poppedOutWindow, window.rootVisualElement);
                body.Add(ewc.Root);
                Windows.Add(window);

                //EntryWindowController controller = window.gameObject.AddComponent<EntryWindowController>();
                //window.gameObject.AddComponent<EntryWindowController>();
                //var controller = window.gameObject.GetComponent<EntryWindowController>();
                //controller.AttachEntryWindow(poppedOutWindow);
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
            _logger.LogDebug("Destroy triggered.");
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

        public void ToggleWindowVisibility(bool isVisible)
        {
            MainGui.GetComponent<MainGuiController>().SetWindowVisibility(isVisible);

            foreach (var w in Windows)
            {
                var doc = w.GetComponent<EntryWindowController>();
                doc.SetWindowVisibility(isVisible);
            }
        }
    }
}
