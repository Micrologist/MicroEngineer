using BepInEx.Logging;
using MicroEngineer.MicroEngineer.UI.Controls;
using MicroMod;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class EntryWindowController : MonoBehaviour
    {
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.EntryWindowController");

        public EntryWindow EntryWindow { get; set; }
        public VisualElement WindowRoot { get; set; }
        public VisualElement Root { get; set; }
        public VisualElement Title { get; set; }
        public VisualElement TitleArrowDown { get; set; }
        public VisualElement TitleArrowRight { get; set; }
        public Label NameLabel { get; set; }
        public Button SettingsButton { get; set; }
        public Button PopOutButton { get; set; }
        public Button CloseButton { get; set; }
        public VisualElement Header { get; set; }
        public VisualElement Body { get; set; }
        public VisualElement Footer { get; set; }

        public EntryWindowController()
        { }

        public EntryWindowController(EntryWindow w, VisualElement windowRoot)
        {
            EntryWindow = w;
            WindowRoot = windowRoot;
            Initialize();
        }

        public void Initialize()
        {
            _logger.LogDebug($"Creating window: {EntryWindow.Name}.");

            Root = Uxmls.Instance.EntryWindow.CloneTree();

            BuildTitle();
            BuildHeader();
            BuildBody();
            BuildFooter();

            if (EntryWindow.IsFlightActive)
                Expand();
            else
                Collapse();

            WindowRoot[0].RegisterCallback<PointerUpEvent>(UpdateWindowPosition);
            WindowRoot[0].transform.position = EntryWindow.FlightRect.position;
        }

        public void UpdateWindowPosition(PointerUpEvent evt)
        {
            if (EntryWindow == null || !EntryWindow.IsFlightPoppedOut)
                return;

            EntryWindow.FlightRect.position = WindowRoot[0].transform.position;
        }

        private void BuildTitle()
        {
            Title = Root.Q<VisualElement>("title");
            Title.AddManipulator(new Clickable(OnTitleClick));
            TitleArrowDown = Root.Q<VisualElement>("title-arrow-down");
            TitleArrowRight = Root.Q<VisualElement>("title-arrow-right");
            NameLabel = Root.Q<Label>("window-name");
            NameLabel.text = EntryWindow.Name;
            SettingsButton = Root.Q<Button>("settings-button");
            PopOutButton = Root.Q<Button>("popout-button");
            PopOutButton.RegisterCallback<ClickEvent>(OnPopOutOrCloseButton);
            CloseButton = Root.Q<Button>("close-button");
            CloseButton.RegisterCallback<ClickEvent>(OnPopOutOrCloseButton);

            if (EntryWindow.IsFlightPoppedOut)
            {
                PopOutButton.style.display = DisplayStyle.None;

                if (EntryWindow.IsLocked)
                    CloseButton.style.display = DisplayStyle.None;
            }
            else
                CloseButton.style.display = DisplayStyle.None;
        }        

        private void BuildHeader()
        {
            Header = Root.Q<VisualElement>("header");

            if (EntryWindow is StageWindow)
            {
                Header.Add(Uxmls.Instance.StageInfoHeader.CloneTree());
                return;
            }                
        }

        private void BuildBody()
        {
            Body = Root.Q<VisualElement>("body");

            foreach (var entry in EntryWindow.Entries)
            {
                VisualElement control;

                switch (entry.EntryType)
                {
                    case EntryType.BasicText:
                        control = new BaseEntryControl(entry);
                        break;
                    case EntryType.Time:
                        control = new TimeEntryControl(entry);
                        break;
                    case EntryType.LatitudeLongitude:
                        control = new LatLonEntryControl(entry);
                        break;
                    case EntryType.StageInfo:
                        control = new StageInfoEntriesBuilder(entry);
                        break;
                    case EntryType.Separator:
                        control = new SeparatorEntryControl();
                        break;
                    default:
                        control = new VisualElement();
                        break;
                }

                Body.Add(control);
            }
        }

        private void BuildFooter()
        {
            Footer = Root.Q<VisualElement>("footer");
        }

        public void Expand()
        {
            Title.AddToClassList("window-title__active");
            TitleArrowDown.style.display = DisplayStyle.Flex;
            TitleArrowRight.style.display = DisplayStyle.None;
            Header.style.display = DisplayStyle.Flex;
            Body.style.display = DisplayStyle.Flex;
            Footer.style.display = DisplayStyle.Flex;

        }

        public void Collapse()
        {
            Title.RemoveFromClassList("window-title__active");
            TitleArrowDown.style.display = DisplayStyle.None;
            TitleArrowRight.style.display = DisplayStyle.Flex;
            Header.style.display = DisplayStyle.None;
            Body.style.display = DisplayStyle.None;
            Footer.style.display = DisplayStyle.None;
        }

        private void OnTitleClick(EventBase evt)
        {
            EntryWindow.IsFlightActive = !EntryWindow.IsFlightActive;

            if (EntryWindow.IsFlightActive)
                Expand();
            else
                Collapse();
        }

        private void OnPopOutOrCloseButton(ClickEvent evt)
        {            
            EntryWindow.IsFlightPoppedOut = !EntryWindow.IsFlightPoppedOut;
            Utility.SaveLayout(Manager.Instance.Windows);
            FlightSceneController.Instance.RebuildUI();
        }

        public void SetWindowVisibility(bool isVisible)
        {
            if (isVisible)
                Root.style.display = DisplayStyle.Flex;
            else
                Root.style.display = DisplayStyle.None;
        }
    }
}
