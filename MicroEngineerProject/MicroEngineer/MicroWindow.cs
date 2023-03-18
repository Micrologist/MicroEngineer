using KSP.Messages.PropertyWatchers;
using SpaceWarp.API.Mods;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using KSP.Sim.impl;
using static KSP.Rendering.Planets.PQSData;
using KSP.Sim;
using static KSP.Modules.Data_LiftingSurface;
using KSP.UI.Flight;
using KSP.UI.Binding;

namespace MicroMod
{

    // THINK ABOUT
    // unlock windows button - enables 'X', unables undocking, enables drag - maybe?
    // stage info window - editable? defined as a MicroWindow?
    
    // TODO
    // Add separator



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
        public string Name;
        public string Description;
        public MicroEntryCategory Category;
        public string Unit;
        public string Formatting;

        public virtual object EntryValue { get; set; }

        /// <summary>
        /// Controls how the value should be displayed. Should be overriden in a inheritet class for a concrete implementation.
        /// </summary>
        public virtual string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(this.Formatting) ? EntryValue.ToString() : String.Format(Formatting, EntryValue);
            }
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
        Stage
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

    public class Latitude : MicroEntry
    {
        public override string ValueDisplay { get => MicroUtility.DegreesToDMS((double)EntryValue); }        
    }

    public class Longitude : MicroEntry
    {
        public override string ValueDisplay { get => MicroUtility.DegreesToDMS((double)EntryValue); }
    }

    public class Apoapsis : MicroEntry
    {
        public override string ValueDisplay { get => MicroUtility.MetersToDistanceString((double)EntryValue); }
    }

    public class TimeToApoapsis : MicroEntry
    {
        public override string ValueDisplay { get => MicroUtility.SecondsToTimeString((double)EntryValue); }
    }

    public class Periapsis : MicroEntry
    {
        public override string ValueDisplay { get => MicroUtility.MetersToDistanceString((double)EntryValue); }
    }

    public class TimeToPeriapsis : MicroEntry
    {
        public override string ValueDisplay { get => MicroUtility.SecondsToTimeString((double)EntryValue); }
    }

    public class Period : MicroEntry
    {
        public override string ValueDisplay { get => MicroUtility.SecondsToTimeString((double)EntryValue); }
    }

    public class SoiTransition : MicroEntry
    {
        public override string ValueDisplay { get => (double)EntryValue >= 0 ? MicroUtility.SecondsToTimeString((double)EntryValue) : "-"; }
    }

    public class Situation : MicroEntry
    {
        public override string ValueDisplay { get => MicroUtility.SituationToString((VesselSituations)EntryValue); }
    }

    public class Biome : MicroEntry
    {
        public override string ValueDisplay { get => MicroUtility.BiomeToString((BiomeSurfaceData)EntryValue); }
    }

    public class AltitudeAsl : MicroEntry
    {
        public override string ValueDisplay { get => MicroUtility.MetersToDistanceString((double)EntryValue); }
    }

    public class AltitudeAgl : MicroEntry
    {
        public override string ValueDisplay { get => MicroUtility.MetersToDistanceString((double)EntryValue); }
    }

    public class TotalLift : MicroEntry
    {
        public override object EntryValue { get => AeroForces.TotalLift; }

        public override string ValueDisplay
        {
            get
            {
                double toReturn = (double)EntryValue * 1000;
                return String.IsNullOrEmpty(base.Formatting) ? toReturn.ToString() : String.Format(base.Formatting, toReturn);
            }
        }
    }

    public class TotalDrag : MicroEntry
    {
        public override object EntryValue { get => AeroForces.TotalDrag; }

        public override string ValueDisplay
        {
            get
            {
                double toReturn = (double)EntryValue * 1000;
                return String.IsNullOrEmpty(base.Formatting) ? toReturn.ToString() : String.Format(base.Formatting, toReturn);
            }
        }
    }

    public class LiftDivDrag: MicroEntry
    {
        public override object EntryValue { get => AeroForces.TotalLift / AeroForces.TotalDrag; }
        public override string ValueDisplay
        {
            get
            {
                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(base.Formatting, EntryValue);
            }
        }
    }

    public class TargetApoapsis : MicroEntry
    {
        public override string ValueDisplay { get => EntryValue != null ? MicroUtility.MetersToDistanceString((double)EntryValue) : "-"; }
    }

    public class TargetPeriapsis : MicroEntry
    {
        public override string ValueDisplay { get => EntryValue != null ? MicroUtility.MetersToDistanceString((double)EntryValue): "-"; }
    }

    public class DistanceToTarget : MicroEntry
    {
        public override string ValueDisplay
        {
            // return value only if vessel and target are in the same SOI
            get => EntryValue != null && MicroUtility.ActiveVessel.Orbit.referenceBody == MicroUtility.ActiveVessel.TargetObject.Orbit.referenceBody ?
                MicroUtility.MetersToDistanceString((double)EntryValue) : "-";
        }
    }

    public class RelativeSpeed : MicroEntry
    {
        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                // return value only if vessel and target are in the same SOI
                if (MicroUtility.ActiveVessel.Orbit.referenceBody != MicroUtility.ActiveVessel.TargetObject.Orbit.referenceBody)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(base.Formatting, EntryValue);
            }
        }
    }

    public class RelativeInclination : MicroEntry
    {
        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                // return value only if vessel and target are in the same SOI
                if (MicroUtility.ActiveVessel.Orbit.referenceBody != MicroUtility.ActiveVessel.TargetObject?.Orbit.referenceBody)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(base.Formatting, EntryValue);
            }
        }
    }

    public class ProjectedAp : MicroEntry
    {
        public override string ValueDisplay { get => EntryValue != null ? MicroUtility.MetersToDistanceString((double)EntryValue) : "-"; }
    }

    public class ProjectedPe : MicroEntry
    {
        public override string ValueDisplay { get => EntryValue != null ? MicroUtility.MetersToDistanceString((double)EntryValue) : "-"; }
    }

    public class TimeToNode : MicroEntry
    {
        public override string ValueDisplay { get => EntryValue != null ? MicroUtility.SecondsToTimeString((double)EntryValue) : "-"; }
    }

    public class BurnTime : MicroEntry
    {
        public override string ValueDisplay { get => EntryValue != null ? MicroUtility.SecondsToTimeString((double)EntryValue) : "-"; }
    }

    public class StageInfo : MicroEntry
    {
        //TODO: stageinfo display
    }


}
