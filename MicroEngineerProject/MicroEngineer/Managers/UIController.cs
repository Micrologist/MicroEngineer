using System.ComponentModel;
using UitkForKsp2.API;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.UIElements;

namespace MicroMod
{
    public class UitkControllerTest
    {
        private static UitkControllerTest _instance;
        private UIDocument _window;

        public bool IsInitialized;
        public bool ShowWindow;

        public Label EntryValue;

        private UitkControllerTest()
        { }

        public static UitkControllerTest Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UitkControllerTest();

                return _instance;
            }
        }

        public void Initialize()
        {
            _window = Window.CreateFromUxml(Styles.FlightUi, "surface", MicroEngineerMod.Instance.transform, true);
            IsInitialized = true;
            ShowWindow = true;

            //var x = Styles.FlightUi.visualElementAssets.FirstOrDefault();
            var root = _window.rootVisualElement;

            root.transform.position = new Vector3(1000, 200, 0);

            //EntryValue = root.Q<Name>("sample-_name");

            EntryWindow surfaceWindow = Manager.Instance.Windows.Find(w => w is EntryWindow && ((EntryWindow)w).Name == "Surface") as EntryWindow;
            //((AltitudeAgl)surfaceWindow.Entries[0])._name = EntryValue;

            var windowTitle = root.Q<Label>("window-name");
            windowTitle.text = surfaceWindow.Name;

            var body = root.Q<VisualElement>("body");

            /* SUCCESSFUL TEST
            var entry = ((AltitudeAgl)surfaceWindow.Entries[0]);
            var entryControl = new BaseEntryControl();
            entryControl.Name = entry.Name;
            entry.OnEntryValueChanged += entryControl.HandleEntryValueChanged;
            entryControl.Unit = entry.UnitDisplay;
            body.Add(entryControl);
            */

            
            while (body.childCount > 0)
                body.RemoveAt(0);


            /*
            foreach (var child in body.Children())
            {
                child.RemoveFromHierarchy();
            }
            */

            int i = 0;
            foreach (var entry in surfaceWindow.Entries)
            {
                if (i == 2)
                    body.Add(new SeparatorEntryControl());

                // skip lat & lon entry for now
                if (i == 6 || i == 7)
                    continue;

                var control = new BaseEntryControl(entry.Name, entry.ValueDisplay, entry.UnitDisplay);
                entry.OnEntryValueChanged += control.HandleEntryValueChanged;
                body.Add(control);
                
                i++;
            }

            // Latitude, Longitude
            var latEntry = surfaceWindow.Entries[6];
            var longEntry = surfaceWindow.Entries[7];
            var latitude = new LatLonEntryControl(latEntry.Name, 1, 2, 3, "S");
            var longitude = new LatLonEntryControl(longEntry.Name, 33, 42, 7, "W");

            body.Add(latitude);
            body.Add(longitude);

            
            
            



            /*
            var entryName = root.Q<Name>("entry-name");
            EntryValue = root.Q<Name>("entry-_value");
            var entryUnit = root.Q<Name>("entry-_unit");

            entryName.text = entry.Name;
            EntryValue.text = entry.ValueDisplay;
            entryUnit.text = entry.UnitDisplay;
            */
            


            //EntryValue.text = surfaceWindow.Entries[0].ValueDisplay;

            var firstVisualElement = root[0];
            /*
            var newLabel = new Name() { name = "new _name", text = "text for new _name" };
            newLabel.visible = true;
            newLabel.enabledSelf = true;
            firstVisualElement.Add(newLabel);
            */
        }

        public void Toggle()
        {
            if (ShowWindow)
            {
                _window.rootVisualElement.style.display = DisplayStyle.None;
            }
            else
            {
                _window.rootVisualElement.style.display = DisplayStyle.Flex;
            }
            ShowWindow = !ShowWindow;

            //_window.enabled = !_window.enabled;
        }
    }
}
