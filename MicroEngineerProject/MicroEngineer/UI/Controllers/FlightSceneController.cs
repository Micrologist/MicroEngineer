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
            //Build MainGui
            MainGui = Window.CreateFromUxml(Uxmls.Instance.MainGui, "MainGui", null, true);
            MainGuiController mainGuiController = MainGui.gameObject.AddComponent<MainGuiController>();

            //Build poppedout windows
            foreach (EntryWindow poppedOutWindow in Manager.Instance.Windows.Where(w => w is EntryWindow && ((EntryWindow)w).IsFlightPoppedOut))
            {
                var window = Window.CreateFromUxml(Uxmls.Instance.BasicWindow, poppedOutWindow.Name, null, true);
                var body = window.rootVisualElement.Q<VisualElement>("body");
                EntryWindowController ewc = new EntryWindowController(poppedOutWindow);
                body.Add(ewc.Root);
            }
        }
    }
}
