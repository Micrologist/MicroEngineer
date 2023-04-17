using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace MicroMod
{
    /// <summary>
    /// Window that can hold a list of Entries
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class BaseWindow
    {
        [JsonProperty]
        internal string Name;
        [JsonProperty]
        internal string Abbreviation;
        [JsonProperty]
        internal string Description; // not used
        [JsonProperty]
        internal Type WindowType = typeof(BaseWindow);

        [JsonProperty]
        internal bool IsEditorActive;
        [JsonProperty]
        internal bool IsFlightActive;
        [JsonProperty]
        internal bool IsMapActive; // TODO: implement

        [JsonProperty]
        internal bool IsEditorPoppedOut;
        [JsonProperty]
        internal bool IsFlightPoppedOut;
        [JsonProperty]
        internal bool IsMapPoppedOut;

        /// <summary>
        /// Can the window be dragged or closed
        /// </summary>
        [JsonProperty]
        internal bool IsLocked;

        /// <summary>
        /// Window can be deleted if it's not one of main windows
        /// </summary>
        [JsonProperty]
        internal bool IsDeletable { get => MainWindow == MainWindow.None; }

        /// <summary>
        /// Can the window be edited (add, remove & arrange entries)
        /// </summary>
        [JsonProperty]
        internal bool IsEditable { get => MainWindow != MainWindow.MainGui && MainWindow != MainWindow.Settings && MainWindow != MainWindow.Stage && MainWindow != MainWindow.StageInfoOAB; }

        [JsonProperty]
        internal MainWindow MainWindow;
        [JsonProperty]
        internal Rect EditorRect;
        [JsonProperty]
        internal Rect FlightRect;
        [JsonProperty]
        internal List<MicroEntry> Entries;

        /// <summary>
        /// Moves entry upwards in the window. Does nothing if it's already first.
        /// </summary>
        /// <param name="entryIndex">Entry's current index</param>
        internal void MoveEntryUp(int entryIndex)
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
        internal void MoveEntryDown(int entryIndex)
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
        internal void RemoveEntry(int entryIndex)
        {
            if (entryIndex < Entries.Count)
                Entries.RemoveAt(entryIndex);
        }

        /// <summary>
        /// Adds an entry to the window to the last position
        /// </summary>
        /// <param name="entry"></param>
        internal void AddEntry(MicroEntry entry) => Entries.Add(entry);

        /// <summary>
        /// Grabs new data for each entry in the window
        /// </summary>
        internal void RefreshEntryData()
        {
            foreach (MicroEntry entry in Entries)
                entry.RefreshData();
        }

        internal virtual void DrawWindowHeader() { }

        internal virtual void DrawWindowFooter() { }

        internal virtual void RefreshData()
        {
            if (Entries == null || Entries.Count == 0)
                return;

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
        OAB,
        New,
        Accepted,
        Accepted2
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
