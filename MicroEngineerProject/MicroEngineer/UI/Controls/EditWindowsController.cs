using BepInEx.Logging;
using MicroMod;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class EditWindowsController : MonoBehaviour
    {
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.EditWindowsController");

        public UIDocument EditWindows { get; set; }        
        public VisualElement Root { get; set; }
        public VisualElement Header { get; set; }
        //public Button CloseButton { get; set; }
        public VisualElement Body { get; set; }

        public EditWindowsController()
        { }

        public void OnEnable()
        {
            _logger.LogDebug("Entering OnEnable() of EditWindowsController");
            EditWindows = GetComponent<UIDocument>();
            Root = EditWindows.rootVisualElement;
            Header = Root.Q<VisualElement>("header");
            //BuildMainGuiHeader();
            Body = Root.Q<VisualElement>("body");
            //BuildDockedWindows();

            //Root[0].RegisterCallback<PointerUpEvent>(UpdateWindowPosition);

            //MainGuiWindow = (MainGuiWindow)Manager.Instance.Windows.Find(w => w is MainGuiWindow);
            //Root[0].transform.position = MainGuiWindow.FlightRect.position;
        }
    }
}
