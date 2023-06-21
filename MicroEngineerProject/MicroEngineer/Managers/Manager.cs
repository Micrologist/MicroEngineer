using BepInEx.Logging;
using KSP.Game;
using MicroEngineer.UI;
using System.Reflection;
using UitkForKsp2.API;
using UnityEngine;

namespace MicroMod
{
    internal class Manager
    {
        private static Manager _instance;

        internal List<BaseWindow> Windows;
        internal List<BaseEntry> Entries;

        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.Manager");

        public List<string> TextFieldNames = new List<string>();

        internal Manager()
        {
            Entries = InitializeEntries();
            Windows = InitializeWindows();
        }

        public static Manager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Manager();

                return _instance;
            }
        }

        public void Update()
        {
            Utility.RefreshGameManager();

            bool isFlightActive = Windows.OfType<MainGuiWindow>().FirstOrDefault().IsFlightActive;

            // Perform flight UI updates only if we're in Flight or Map view
            if (Utility.GameState != null && (Utility.GameState.GameState == GameState.FlightView || Utility.GameState.GameState == GameState.Map3DView) && isFlightActive)
            {
                Utility.RefreshActiveVesselAndCurrentManeuver();

                if (Utility.ActiveVessel == null)
                    return;

                // Refresh all active windows' entries
                foreach (EntryWindow window in Windows.Where(w => w.IsFlightActive && w is EntryWindow))
                    window.RefreshData();
            }
        }

        /// <summary>
        /// Builds the list of all Entries
        /// </summary>
        internal List<BaseEntry> InitializeEntries()
        {
            Entries = new List<BaseEntry>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();

            // Exclude base classes
            Type[] excludedTypes = new[] { typeof(BaseEntry), typeof(BodyEntry), typeof(FlightEntry),
                typeof(ManeuverEntry), typeof(MiscEntry), typeof(OabStageInfoEntry), typeof(OrbitalEntry),
                typeof(StageInfoEntry), typeof(SurfaceEntry), typeof(TargetEntry), typeof(VesselEntry) };

            Type[] entryTypes = types.Where(t => typeof(BaseEntry).IsAssignableFrom(t) && !excludedTypes.Contains(t)).ToArray();

            foreach (Type entryType in entryTypes)
            {
                BaseEntry entry = Activator.CreateInstance(entryType) as BaseEntry;
                if (entry != null)
                    Entries.Add(entry);
            }

            return Entries;
        }

        /// <summary>
        /// Builds the default Windows and fills them with default Entries
        /// </summary>
        internal List<BaseWindow> InitializeWindows()
        {
            Windows = new List<BaseWindow>();

            try
            {
                Windows.Add(new MainGuiWindow
                {
                    LayoutVersion = Utility.CurrentLayoutVersion,
                    IsEditorActive = false,
                    IsFlightActive = false,
                    IsMapActive = false,
                    //EditorRect = null,
                    FlightRect = new Rect(1350, 160, 0, 0) // About 3/4 of the screen
                });

                Windows.Add(new SettingsWIndow
                {
                    ActiveTheme = Styles.ActiveTheme,
                    IsEditorActive = false,
                    IsFlightActive = false,
                    IsMapActive = false,
                    //EditorRect = null,
                    FlightRect = new Rect(ReferenceResolution.Width/2, ReferenceResolution.Height/2, 0, 0)
                });

                Windows.Add(new EntryWindow
                {
                    Name = "Vessel",
                    Abbreviation = "VES",
                    Description = "Vessel entries",
                    IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Vessel,
                    //EditorRect = null,
                    FlightRect = new Rect(ReferenceResolution.Width / 2, ReferenceResolution.Height / 2, 0, 0),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.Vessel && entry.IsDefault).ToList()
                });

                Windows.Add(new EntryWindow
                {
                    Name = "Orbital",
                    Abbreviation = "ORB",
                    Description = "Orbital entries",
                    IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Orbital,
                    //EditorRect = null,
                    FlightRect = new Rect(ReferenceResolution.Width / 2, ReferenceResolution.Height / 2, 0, 0),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.Orbital && entry.IsDefault).ToList()
                });

                Windows.Add(new EntryWindow
                {
                    Name = "Surface",
                    Abbreviation = "SUR",
                    Description = "Surface entries",
                    IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Surface,
                    //EditorRect = null,
                    FlightRect = new Rect(ReferenceResolution.Width / 2, ReferenceResolution.Height / 2, 0, 0),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.Surface && entry.IsDefault).ToList()
                });

                Windows.Add(new EntryWindow
                {
                    Name = "Flight",
                    Abbreviation = "FLT",
                    Description = "Flight entries",
                    IsEditorActive = false,
                    IsFlightActive = false,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Flight,
                    //EditorRect = null,
                    FlightRect = new Rect(ReferenceResolution.Width / 2, ReferenceResolution.Height / 2, 0, 0),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.Flight && entry.IsDefault).ToList()
                });

                Windows.Add(new EntryWindow
                {
                    Name = "Target",
                    Abbreviation = "TGT",
                    Description = "Flight entries",
                    IsEditorActive = false,
                    IsFlightActive = false,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Target,
                    //EditorRect = null,
                    FlightRect = new Rect(ReferenceResolution.Width / 2, ReferenceResolution.Height / 2, 0, 0),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.Target && entry.IsDefault).ToList()
                });

                Windows.Add(new ManeuverWindow
                {
                    Name = "Maneuver",
                    Abbreviation = "MAN",
                    Description = "Maneuver entries",
                    IsEditorActive = false,
                    IsFlightActive = false,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Maneuver,
                    //EditorRect = null,
                    FlightRect = new Rect(ReferenceResolution.Width / 2, ReferenceResolution.Height / 2, 0, 0),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.Maneuver && entry.IsDefault).ToList()
                });

                Windows.Add(new StageWindow
                {
                    Name = "Stage",
                    Abbreviation = "STG",
                    Description = "Stage entries",
                    IsEditorActive = false,
                    IsFlightActive = false,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Stage,
                    //EditorRect = null,
                    FlightRect = new Rect(ReferenceResolution.Width / 2, ReferenceResolution.Height / 2, 0, 0),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.Stage && entry.IsDefault).ToList()
                });

                Windows.Add(new EntryWindow
                {
                    Name = "Stage (OAB)",
                    Abbreviation = "SOAB",
                    Description = "Stage Info window for OAB",
                    IsEditorActive = false,
                    IsFlightActive = false, // Not used
                    IsMapActive = false, // Not used
                    IsEditorPoppedOut = true, // Not used
                    IsFlightPoppedOut = false, // Not used
                    IsMapPoppedOut = false, // Not used
                    IsLocked = false, // Not used
                    MainWindow = MainWindow.StageInfoOAB,
                    EditorRect = new Rect(ReferenceResolution.Width / 2, ReferenceResolution.Height / 2, 0, 0),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.OAB && entry.IsDefault).ToList()
                });

                return Windows;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating a BaseWindow. Full exception: " + ex);
                return null;
            }
        }

        /// <summary>
        /// Creates a new custom window user can fill with any entry
        /// </summary>
        /// <param name="editableWindows"></param>
        internal int CreateCustomWindow(List<EntryWindow> editableWindows)
        {
            // Default window's name will be CustomX where X represents the first not used integer
            int nameID = 1;
            foreach (EntryWindow window in editableWindows)
            {
                if (window.Name == "Custom" + nameID)
                    nameID++;
            }

            EntryWindow newWindow = new()
            {
                Name = "Custom" + nameID,
                Abbreviation = nameID.ToString().Length == 1 ? "Cu" + nameID : nameID.ToString().Length == 2 ? "C" + nameID : nameID.ToString(),
                Description = "",
                IsEditorActive = false,
                IsFlightActive = true,
                IsMapActive = false,
                IsEditorPoppedOut = false,
                IsFlightPoppedOut = false,
                IsMapPoppedOut = false,
                IsLocked = false,
                MainWindow = MainWindow.None,
                //EditorRect = null,
                FlightRect = new Rect(ReferenceResolution.Width / 2, ReferenceResolution.Height / 2, 0, 0),
                Entries = new List<BaseEntry>()
            };

            Windows.Add(newWindow);
            editableWindows.Add(newWindow);

            return editableWindows.Count - 1;
        }

        internal void ResetLayout()
        {
            Windows.Clear();
            Entries.Clear();
            Entries = InitializeEntries();
            Windows = InitializeWindows();
        }

        internal void LoadLayout()
        {
            Utility.LoadLayout(Windows);
        }

        internal void SaveLayout() => Utility.SaveLayout(Windows);

        public void PupulateTextFieldNames(List<BaseEntry> entries)
        {
            TextFieldNames.Clear();
            TextFieldNames.Add(Utility.InputDisableWindowAbbreviation);
            TextFieldNames.Add(Utility.InputDisableWindowName);

            foreach (var entry in entries)
            {
                entry.Id = Guid.NewGuid();
                TextFieldNames.Add(entry.Id.ToString());
            }
        }

        public void AddTextFieldName(string name) => TextFieldNames.Add(name);
    }
}