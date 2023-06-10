using UnityEngine;
using Newtonsoft.Json;

namespace MicroMod
{
    /// <summary>
    /// Window that can hold a list of Entries
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BaseWindow
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
}