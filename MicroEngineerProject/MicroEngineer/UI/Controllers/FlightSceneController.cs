using BepInEx.Logging;
using MicroMod;
using UitkForKsp2.API;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class FlightSceneController
    {
        private static FlightSceneController _instance;
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.FlightSceneController");

        public UIDocument MainGui;

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
            MainGui = Window.CreateFromUxml(Uxmls.Instance.MainGui, "MainGui", null, true);//MicroEngineerMod.Instance.transform, true);
            MainGuiController mainGuiController = MainGui.gameObject.AddComponent<MainGuiController>();

            foreach (EntryWindow poppedOutWindow in Manager.Instance.Windows.Where(w => w is EntryWindow && ((EntryWindow)w).IsFlightPoppedOut))
            {
                EntryWindowController ewc = new EntryWindowController(poppedOutWindow);
                var w = Window.CreateFromElement(ewc.Root, poppedOutWindow.Name, null, true);
                w.rootVisualElement.style.width = 220;

                //var w = Window.CreateFromUxml(Uxmls.Instance.EntryWindow, poppedOutWindow.Name, null, true);
                //EntryWindowController ctrl = w.gameObject.AddComponent<EntryWindowController>();
                _logger.LogDebug($"Poppedout window {poppedOutWindow.Name} created.");
            }

            //foreach (EntryWindow poppedOutWindow in Manager.Instance.Windows.Where(w => w is EntryWindow && ((EntryWindow)w).IsFlightPoppedOut))
            //{
            //    var window = Window.CreateFromUxml(Uxmls.Instance.MainGui, poppedOutWindow.Name, MicroEngineerMod.Instance.transform, true);
            //    var body = window.rootVisualElement.Q<VisualElement>("body");
            //    EntryWindowController ewc = new EntryWindowController(poppedOutWindow);
            //    body.Add(ewc.Root);
            //}
        }
    }
}
