using UnityEngine;
using Newtonsoft.Json;

namespace MicroMod
{
    /// <summary>
    /// Window that can hold a list of Entries
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class MicroWindow
    {
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public string Abbreviation;
        [JsonProperty]
        public string Description; // not used

        [JsonProperty]
        public bool IsEditorActive;
        [JsonProperty]
        public bool IsFlightActive;
        [JsonProperty]
        public bool IsMapActive; // TODO: implement

        [JsonProperty]
        public bool IsEditorPoppedOut;
        [JsonProperty]
        public bool IsFlightPoppedOut;
        [JsonProperty]
        public bool IsMapPoppedOut;

        /// <summary>
        /// Can the window be dragged or closed
        /// </summary>
        [JsonProperty]
        public bool IsLocked;

        /// <summary>
        /// Window can be deleted if it's not one of main windows
        /// </summary>
        [JsonProperty]
        public bool IsDeletable { get => MainWindow == MainWindow.None; }

        /// <summary>
        /// Can the window be edited (add, remove & arrange entries)
        /// </summary>
        [JsonProperty]
        public bool IsEditable { get => MainWindow != MainWindow.MainGui && MainWindow != MainWindow.Settings && MainWindow != MainWindow.Stage && MainWindow != MainWindow.StageInfoOAB; }

        [JsonProperty]
        public MainWindow MainWindow;
        [JsonProperty]
        public Rect EditorRect;
        [JsonProperty]
        public Rect FlightRect;
        [JsonProperty]
        public List<MicroEntry> Entries;

        /// <summary>
        /// Moves entry upwards in the window. Does nothing if it's already first.
        /// </summary>
        /// <param name="entryIndex">Entry's current index</param>
        public void MoveEntryUp(int entryIndex)
        {
            // check if entry exists and it's not first
            if (entryIndex < Entries.Count && entryIndex > 0)
            {
                var temp = Entries[entryIndex-1];
                Entries[entryIndex - 1] = Entries[entryIndex];
                Entries[entryIndex] = temp;
            }
        }

        /// <summary>
        /// Moves entry downwards in the window. Does nothing if it's already last.
        /// </summary>
        /// <param name="entryIndex">Entry's current index</param>
        public void MoveEntryDown(int entryIndex)
        {
            // check if entry is not last
            if (entryIndex < Entries.Count - 1)
            {
                var temp = Entries[entryIndex + 1];
                Entries[entryIndex + 1] = Entries[entryIndex];
                Entries[entryIndex] = temp;
            }            
        }

        /// <summary>
        /// Removes entry from the window 
        /// </summary>
        /// <param name="entryIndex">Entry's index</param>
        public void RemoveEntry(int entryIndex)
        {
            if (entryIndex < Entries.Count)
                Entries.RemoveAt(entryIndex);
        }

        /// <summary>
        /// Adds an entry to the window to the last position
        /// </summary>
        /// <param name="entry"></param>
        public void AddEntry(MicroEntry entry) => Entries.Add(entry);

        /// <summary>
        /// Grabs new data for each entry in the window
        /// </summary>
        public void RefreshEntryData()
        {
            foreach (MicroEntry entry in Entries)
                entry.RefreshData();
        }
    }

    /// <summary>
    /// Each entry has a category we assign to it depending on where it fits best
    /// </summary>
    public enum MicroEntryCategory
    {
        Vessel,
        Orbital,
        Surface,
        Flight,
        Target,
        Maneuver,
        Stage,
        Misc,
        OAB
    }

    /// <summary>
    /// Main windows cannot be deleted. Value None indicated that the window is not a main window
    /// </summary>
    public enum MainWindow
    {
        None = 0,
        MainGui,
        Vessel,
        Stage,
        Orbital,
        Surface,
        Flight,
        Target,
        Maneuver,
        Settings,
        StageInfoOAB
    }
}
