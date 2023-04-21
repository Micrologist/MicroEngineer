using BepInEx.Logging;
using KSP.Game;
using MicroMod;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MicroMod
{
    internal class Manager
    {
        internal List<BaseWindow> Windows;
        internal List<BaseEntry> Entries;
        private MicroEngineerMod _plugin;

        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.Manager");

        internal Manager(MicroEngineerMod plugin)
        {
            _plugin = plugin;
            Entries = InitializeEntries();
            Windows = InitializeWindows();

            // Load window positions and states from disk, if file exists
            Utility.LoadLayout(Windows);
        }

        public void Update()
        {
            Utility.RefreshGameManager();

            // Perform flight UI updates only if we're in Flight or Map view
            if (Utility.GameState != null && (Utility.GameState.GameState == GameState.FlightView || Utility.GameState.GameState == GameState.Map3DView))
            {
                Utility.RefreshActiveVesselAndCurrentManeuver();

                if (Utility.ActiveVessel == null)
                    return;

                // Refresh all active windows' entries
                foreach (BaseWindow window in Windows.Where(w => w.IsFlightActive))
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
                Windows.Add(new BaseWindow
                {
                    Name = "MainGui",
                    LayoutVersion = Utility.CurrentLayoutVersion,
                    Abbreviation = null,
                    Description = "Main GUI",
                    IsEditorActive = false,
                    IsFlightActive = false,
                    IsMapActive = false,
                    IsEditorPoppedOut = false, // not relevant to Main GUI
                    IsFlightPoppedOut = false, // not relevant to Main GUI
                    IsMapPoppedOut = false, // not relevant to Main GUI
                    IsLocked = false,
                    MainWindow = MainWindow.MainGui,
                    //EditorRect = null,
                    FlightRect = new Rect(Styles.MainGuiX, Styles.MainGuiY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = null
                });

                Windows.Add(new BaseWindow
                {
                    Name = "Settings",
                    Abbreviation = "SET",
                    Description = "Settings",
                    IsEditorActive = false,
                    IsFlightActive = false,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Settings,
                    //EditorRect = null,
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = null
                });

                Windows.Add(new BaseWindow
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
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.Vessel && entry.IsDefault).ToList()
                });

                Windows.Add(new BaseWindow
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
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.Orbital && entry.IsDefault).ToList()
                });

                Windows.Add(new BaseWindow
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
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.Surface && entry.IsDefault).ToList()
                });

                Windows.Add(new BaseWindow
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
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.Flight && entry.IsDefault).ToList()
                });

                Windows.Add(new BaseWindow
                {
                    Name = "Target",
                    Abbreviation = "TGT",
                    Description = "Flight entries",
                    IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Target,
                    //EditorRect = null,
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.Target && entry.IsDefault).ToList()
                });

                Windows.Add(new ManeuverWindow
                {
                    Name = "Maneuver",
                    Abbreviation = "MAN",
                    Description = "Maneuver entries",
                    WindowType = typeof(ManeuverWindow),
                    IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Maneuver,
                    //EditorRect = null,
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.Maneuver && entry.IsDefault).ToList()
                });

                Windows.Add(new BaseWindow
                {
                    Name = "Stage",
                    Abbreviation = "STG",
                    Description = "Stage entries",
                    IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Stage,
                    //EditorRect = null,
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = Entries.Where(entry => entry.Category == MicroEntryCategory.Stage && entry.IsDefault).ToList()
                });

                Windows.Add(new BaseWindow
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
                    EditorRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, 0, 0),
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
        internal int CreateCustomWindow(List<BaseWindow> editableWindows)
        {
            // Default window's name will be CustomX where X represents the first not used integer
            int nameID = 1;
            foreach (BaseWindow window in editableWindows)
            {
                if (window.Name == "Custom" + nameID)
                    nameID++;
            }

            BaseWindow newWindow = new()
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
                FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                Entries = new List<BaseEntry>()
            };

            Windows.Add(newWindow);
            editableWindows.Add(newWindow);

            return editableWindows.Count - 1;
        }
    }
}
