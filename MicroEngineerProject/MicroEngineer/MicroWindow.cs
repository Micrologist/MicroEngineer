using SpaceWarp.API.Mods;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MicroEngineer.MicroEngineer
{

    // THINK ABOUT
    // unlock windows button - enables 'X', unables undocking, enables drag - maybe?
    // stage info window - editable? defined as a MicroWindow?


    public class MicroWindow
    {
        public int Index;
        public string Name;
        public string Description; // not used?
        public bool IsEditorActive;
        public bool IsFlightActive;
        public bool IsMapActive;

        public bool IsEditorPoppedOut;
        public bool IsFlightPoppedOut;
        public bool IsMapPoppedOut;

        // TODO enable dragging and closing of unlocked windows
        /// <summary>
        /// Can the window be dragged or closed
        /// </summary>
        public bool IsLocked;

        // TODO implement disabling deleting of "main" windows
        /// <summary>
        /// Window can be deleted if it's not one of main windows
        /// </summary>
        public bool IsDeletable { get => MainWindow == MainWindow.None; }

        /// <summary>
        /// Can the window be edited (add, remove & arrange entries)
        /// </summary>
        public bool IsEditable;

        public MainWindow MainWindow;

        public Vector2 EditorPosition;
        public Vector2 FlightPosition;
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

        // TODO
        // windowBackgroundColor
        // windowTransparency - float, min 0, max 1 -> if >max set to max, if <min set to min

        // TODO
        // EntryNameColor
        // EntryValueColor
        // EntryUnitColor
        // EntryBold
        // EntryItalic

    }

    public class MicroEntry
    {
        public MicroEntryCategory Category;
        public string Name;
        public string Description;
        public string Unit;
        public string Formatting;

        private object entryValue;
        public object EntryValue
        {
            get => String.IsNullOrEmpty(Formatting) ? entryValue : String.Format(Formatting, entryValue);
            set => this.entryValue = value;
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
        Maneuver
    }

    /// <summary>
    /// Main windows cannot be deleted
    /// </summary>
    public enum MainWindow
    {
        None = 0,
        Vessel,
        Stage,
        Orbital,
        Surface,
        Flight,
        Target,
        Maneuver,
        Settings
    }
}
