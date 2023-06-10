﻿using BepInEx.Logging;
using MicroMod;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class EntryWindowController : MonoBehaviour
    {
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.EntryWindowController");

        public EntryWindow EntryWindow { get; set; }
        public VisualElement Root { get; set; }   
        public VisualElement Title { get; set; }
        public VisualElement TitleArrowDown { get; set; }
        public VisualElement TitleArrowRight { get; set; }
        public Label NameLabel { get; set; }
        public Button SettingsButton { get; set; }
        public Button PopOutButton { get; set; }
        public Button CloseButton { get; set; }
        public VisualElement Body { get; set; }

        public EntryWindowController(EntryWindow w)
        {
            EntryWindow = w;
            Initialize();
        }

        public void Start()
        { }

        public void Initialize()
        {
            _logger.LogDebug($"Creating window: {EntryWindow.Name}.");

            Root = Uxmls.Instance.EntryWindow.CloneTree();
            Title = Root.Q<VisualElement>("title");
            Title.RegisterCallback<MouseDownEvent>(OnTitleClick);
            TitleArrowDown = Root.Q<VisualElement>("title-arrow-down");
            TitleArrowRight = Root.Q<VisualElement>("title-arrow-right");
            NameLabel = Root.Q<Label>("window-name");
            NameLabel.text = EntryWindow.Name;
            SettingsButton = Root.Q<Button>("settings-button");
            PopOutButton = Root.Q<Button>("popout-button");
            CloseButton = Root.Q<Button>("close-button");
            Body = Root.Q<VisualElement>("body");

            foreach (var entry in EntryWindow.Entries)
            {
                VisualElement control;

                _logger.LogDebug($"Creating entry: {entry.Name}.");

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
                        // TODO
                        control = new VisualElement();
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

            if (EntryWindow.IsFlightPoppedOut)
                PopOutButton.style.display = DisplayStyle.None;
            else
                CloseButton.style.display = DisplayStyle.None;

            if (EntryWindow.IsFlightActive)
                Expand();
            else
                Collapse();
        }

        public void Expand()
        {
            Title.AddToClassList("window-title__active");
            TitleArrowDown.style.display = DisplayStyle.Flex;
            TitleArrowRight.style.display = DisplayStyle.None;
            Body.style.display = DisplayStyle.Flex;
        }

        public void Collapse()
        {
            // Collapse
            Title.RemoveFromClassList("window-title__active");
            TitleArrowDown.style.display = DisplayStyle.None;
            TitleArrowRight.style.display = DisplayStyle.Flex;
            Body.style.display = DisplayStyle.None;
        }

        private void OnTitleClick(MouseDownEvent evt)
        {
            EntryWindow.IsFlightActive = !EntryWindow.IsFlightActive;

            if (EntryWindow.IsFlightActive)
                Expand();
            else
                Collapse();
        }
    }
}