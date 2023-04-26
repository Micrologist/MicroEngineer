using UnityEngine;
using Newtonsoft.Json;

namespace MicroMod
{
    /// <summary>
    /// Window that can hold a list of Entries
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class BaseWindow
    {
        [JsonProperty]
        internal bool IsEditorActive;
        [JsonProperty]
        internal bool IsFlightActive;
        [JsonProperty]
        internal bool IsMapActive; // TODO: implement
        
        [JsonProperty]
        internal Rect EditorRect;
        [JsonProperty]
        internal Rect FlightRect;

        internal virtual void DrawWindowHeader() { }

        internal virtual void DrawWindowFooter() { }
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
        Body,
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