using Newtonsoft.Json;
using UnityEngine;

namespace MicroMod
{
    internal class EntryWindow: BaseWindow
    {
        [JsonProperty]
        internal string Name;
        [JsonProperty]
        internal string Abbreviation;
        [JsonProperty]
        internal string Description; // not used

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
        internal bool IsEditable { get => MainWindow != MainWindow.Stage && MainWindow != MainWindow.StageInfoOAB; }

        internal bool IsBeingDragged { get => FlightRect.Contains(Event.current.mousePosition); }

        [JsonProperty]
        internal MainWindow MainWindow;

        [JsonProperty]
        internal List<BaseEntry> Entries;

        /// <summary>
        /// Moves entry upwards in the window. Does nothing if it's already first.
        /// </summary>
        /// <param name="entryIndex">Entry's current index</param>
        internal void MoveEntryUp(int entryIndex)
        {
            // check if entry exists and it's not first
            if (entryIndex < Entries.Count && entryIndex > 0)
            {
                var temp = Entries[entryIndex - 1];
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
        internal void AddEntry(BaseEntry entry) => Entries.Add(entry);

        /// <summary>
        /// Grabs new data for each entry in the window
        /// </summary>
        internal void RefreshEntryData()
        {
            foreach (BaseEntry entry in Entries)
                entry.RefreshData();
        }

        internal virtual void RefreshData()
        {
            if (Entries == null || Entries.Count == 0)
                return;

            foreach (BaseEntry entry in Entries)
                entry.RefreshData();
        }
    }
}