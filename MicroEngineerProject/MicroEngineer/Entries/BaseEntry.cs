using Newtonsoft.Json;
using System.Xml;
using UnityEngine;

namespace MicroMod
{
    /// <summary>
    /// Base class for all Entries (values that can be attached to windows)
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BaseEntry
    {
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public string Description;
        [JsonProperty]
        public MicroEntryCategory Category;
        public bool IsDefault;
        [JsonProperty]
        public bool HideWhenNoData;
        [JsonProperty]
        public string Unit;
        [JsonProperty]
        public byte NumberOfDecimalDigits;
        [JsonProperty("Formatting")]
        private string _formatting;
        public string Formatting
        {
            get => String.IsNullOrEmpty(_formatting) ? null : $"{{0:{_formatting}{this.NumberOfDecimalDigits}}}";
            set => _formatting = value;
        }        

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

        

        public virtual void RefreshData() { }
    }    
}
