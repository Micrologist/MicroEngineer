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
            Root[0].RegisterCallback<GeometryChangedEvent>(CenterWindow);
        }

        public void Update()
        {
            return;
        }

        public void BuildDockedWindows()
        {
            foreach (EntryWindow entryWindow in Manager.Instance.Windows.Where(w => w is EntryWindow && !((EntryWindow)w).IsFlightPoppedOut))
            {
                EntryWindowController ewc = new EntryWindowController(entryWindow);

                Body.Add(ewc.Root);
                _logger.LogDebug($"Window {entryWindow.Name} added to root.");
            }            

            // DOESN'T WORK
            ////Root[0].CenterByDefault();
            //Root[0].SetDefaultPosition(windowSize =>
            //{
            //    return new Vector2((ReferenceResolution.Width - windowSize.x) / 2, (ReferenceResolution.Height - windowSize.y) / 2);
            //});
        }

        private void CenterWindow(GeometryChangedEvent evt)
        {
            if (evt.newRect.width == 0 || evt.newRect.height == 0)
                return;

            Root[0].transform.position = new Vector2((ReferenceResolution.Width - evt.newRect.width) / 2, (ReferenceResolution.Height - evt.newRect.height) / 2);
            Root[0].UnregisterCallback<GeometryChangedEvent>(CenterWindow);
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
