using Newtonsoft.Json;
using UnityEngine;

namespace MicroMod
{
    public class EntryWindow: BaseWindow
    {
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public string Abbreviation;
        [JsonProperty]
        public string Description; // not used

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
        public bool IsEditable { get => MainWindow != MainWindow.StageInfoOAB; }

        public bool IsBeingDragged { get => FlightRect.Contains(Event.current.mousePosition); }

        [JsonProperty]
        public MainWindow MainWindow;

        [JsonProperty]
        public List<BaseEntry> Entries;

        /// <summary>
        /// Moves entry upwards in the window. Does nothing if it's already first.
        /// </summary>
        /// <param name="entryIndex">Entry's current index</param>
        public void MoveEntryUp(int entryIndex)
        {
            // check if entry exists and it's not first
            if (entryIndex < Entries.Count && entryIndex > 0)
            {
                var temp = Entries[entryIndex - 1];
                Entries[entryIndex - 1] = Entries[entryIndex];
                Entries[entryIndex] = temp;
            }
        }

        public void MoveEntryUp(BaseEntry entry) => MoveEntryUp(Entries.IndexOf(entry));

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

        public void MoveEntryDown(BaseEntry entry) => MoveEntryDown(Entries.IndexOf(entry));

        /// <summary>
        /// Adds an entry to the window to the last position
        /// </summary>
        /// <param name="entry"></param>
        public void AddEntry(BaseEntry entry) => Entries.Add(entry);

        public void RemoveEntry(BaseEntry entry) => Entries.Remove(entry);

        /// <summary>
        /// Removes entry from the window 
        /// </summary>
        /// <param name="entryIndex">Entry's index</param>
        public void RemoveEntry(int entryIndex)
        {
            if (entryIndex < Entries.Count)
                Entries.RemoveAt(entryIndex);
        }

        public virtual void RefreshData()
        {
            if (Entries == null || Entries.Count == 0)
                return;

            foreach (BaseEntry entry in Entries)
                entry.RefreshData();
        }
    }
}