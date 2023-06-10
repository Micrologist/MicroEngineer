using MicroMod;
using UitkForKsp2.API;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class FlightSceneController
    {
        private static FlightSceneController _instance;

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
            MainGui = Window.CreateFromUxml(Uxmls.Instance.MainGui, "MainGui", MicroEngineerMod.Instance.transform, true);
            MainGuiController mainGuiController = MainGui.gameObject.AddComponent<MainGuiController>();


        }
    }
}
