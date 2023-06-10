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

        public UIDocument MainGui { get; set; }
        public VisualElement Root { get; set; }
        public VisualElement Body { get; set; }
        public bool ShowWindow;

        public MainGuiController()
        { }

        public void Start()
        {
            _logger.LogDebug("Entering Start() of MainGuiController");
            MainGui = GetComponent<UIDocument>();
            Root = MainGui.rootVisualElement;
            Body = Root.Q<VisualElement>("body");            
            BuildDockedWindows();
        }

        public void Update()
        {
            return;
        }

        public void BuildDockedWindows()
        {
            foreach (EntryWindow entryWindow in Manager.Instance.Windows.Where(w => w is EntryWindow))
            {
                //TODO only build docked windows

                EntryWindowController ewc = new EntryWindowController(entryWindow);

                Body.Add(ewc.Root);
                _logger.LogDebug($"Window {entryWindow.Name} added to root.");
            }

            Root[0].CenterByDefault();        
        }

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
