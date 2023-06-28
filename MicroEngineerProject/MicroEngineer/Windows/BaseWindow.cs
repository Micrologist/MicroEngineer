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
        public bool IsEditorActive;
        [JsonProperty]
        public bool IsFlightActive;
        [JsonProperty]
        public bool IsMapActive; // TODO: implement
        
        [JsonProperty]
        public Rect EditorRect;
        [JsonProperty]
        public Rect FlightRect;

        public virtual void DrawWindowHeader() { }

        public virtual void DrawWindowFooter() { }
    }    
}