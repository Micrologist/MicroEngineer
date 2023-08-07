using Newtonsoft.Json;

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
        public Guid Id;
        public string Description;
        [JsonProperty]
        public MicroEntryCategory Category;
        public EntryType EntryType;
        public bool IsDefault;
        [JsonProperty]
        public bool HideWhenNoData;
        [JsonProperty]
        public string MiliUnit;
        [JsonProperty]
        public string BaseUnit;
        [JsonProperty]
        public string KiloUnit;
        [JsonProperty]
        public string MegaUnit;
        [JsonProperty]
        public string GigaUnit;
        [JsonProperty]
        public AltUnit AltUnit;
        [JsonProperty]
        public byte NumberOfDecimalDigits;
        [JsonProperty("Formatting")]
        private string _formatting;
        public string Formatting
        {
            get => String.IsNullOrEmpty(_formatting) ? null : $"{{0:{_formatting}{this.NumberOfDecimalDigits}}}";
            set => _formatting = value;
        }        

        private object entryValue;
        public virtual object EntryValue
        {
            get => entryValue;

            set
            {
                if (value != entryValue)
                {
                    entryValue = value;

                    switch (EntryType)
                    {
                        case EntryType.BasicText:
                            OnEntryValueChanged?.Invoke(ValueDisplay, UnitDisplay);
                            break;
                        case EntryType.Time:
                            var time = Utility.ParseSecondsToTimeFormat((double?)value ?? 0);
                            OnEntryTimeValueChanged?.Invoke(time.Days, time.Hours, time.Minutes, time.Seconds);
                            break;
                        case EntryType.LatitudeLongitude:
                            var latLon = Utility.ParseDegreesToDMSFormat((double?)value ?? 0);
                            OnEntryLatLonChanged?.Invoke(latLon.Degrees, latLon.Minutes, latLon.Seconds, BaseUnit);
                            break;
                        case EntryType.StageInfo:
                            OnStageInfoChanged?.Invoke((List<Stage>)entryValue);
                            break;
                        case EntryType.Separator:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public delegate void EntryValueChanged(string value, string unit);
        public delegate void EntryTimeValueChanged(int days, int hours, int minutes, int seconds);
        public delegate void EntryLatLonChanged(int degrees, int minutes, int seconds, string direction);
        public delegate void StageInfoChanged(List<Stage> stages);
        public event EntryValueChanged OnEntryValueChanged;
        public event EntryTimeValueChanged OnEntryTimeValueChanged;
        public event EntryLatLonChanged OnEntryLatLonChanged;
        public event StageInfoChanged OnStageInfoChanged;

        /// <summary>
        /// Controls how the value should be displayed. Can be overriden in a inheritet class for a more specialized implementation.
        /// </summary>
        public virtual string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                if (String.IsNullOrEmpty(this.Formatting))
                    return EntryValue.ToString();

                if (!double.TryParse(EntryValue.ToString(), out double d))
                    return EntryValue.ToString(); // This case shouldn't exist, but just to be sure

                if (AltUnit != null && AltUnit.IsActive)
                {
                    return !string.IsNullOrEmpty(Formatting) ? String.Format(Formatting, d * AltUnit.Factor) :
                        (d * AltUnit.Factor).ToString();
                }

                if (Math.Abs(d) < 1) // mili
                {
                    return !String.IsNullOrEmpty(this.MiliUnit) ? String.Format(Formatting, d * 1000) :
                        String.Format(Formatting, d);
                }
                else if (Math.Abs(d) < 1000000) // base
                {
                    return String.Format(Formatting, d);
                }
                else if (Math.Abs(d) < 1000000000) // kilo
                {
                    return !String.IsNullOrEmpty(this.KiloUnit) ? String.Format(Formatting, d / 1000) :
                        String.Format(Formatting, d);

                }
                else if (Math.Abs(d) < 1000000000000) // mega
                {
                    return !String.IsNullOrEmpty(this.MegaUnit) ? String.Format(Formatting, d / 1000000) :
                        !String.IsNullOrEmpty(this.KiloUnit) ? String.Format(Formatting, d / 1000) :
                        String.Format(Formatting, d);

                }
                else // giga
                {
                    return !String.IsNullOrEmpty(this.GigaUnit) ? String.Format(Formatting, d / 1000000000) :
                        !String.IsNullOrEmpty(this.MegaUnit) ? String.Format(Formatting, d / 1000000) :
                        !String.IsNullOrEmpty(this.KiloUnit) ? String.Format(Formatting, d / 1000) :
                        String.Format(Formatting, d);
                }
            }
        }

        public virtual string UnitDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "";

                if (String.IsNullOrEmpty(this.Formatting))
                    return this.BaseUnit ?? "";

                if (!double.TryParse(EntryValue.ToString(), out double d))
                    return this.BaseUnit ?? ""; // This case shouldn't exist, but just to be sure

                if (AltUnit != null && AltUnit.IsActive)
                    return AltUnit.Unit;

                if (d > 0.001 && d < 1) // mili
                {
                    return this.MiliUnit ?? this.BaseUnit ?? "";
                }
                else if (Math.Abs(d) < 1000000) // base
                {
                    return this.BaseUnit ?? "";
                }
                else if (Math.Abs(d) < 1000000000) // kilo
                {
                    return this.KiloUnit ?? this.BaseUnit ?? "";

                }
                else if (Math.Abs(d) < 1000000000000) // mega
                {
                    return this.MegaUnit ?? this.KiloUnit ?? this.BaseUnit ?? "";

                }
                else // giga
                {
                    return this.GigaUnit ?? this.MegaUnit ?? this.KiloUnit ?? this.BaseUnit ?? "";
                }
            }
        }

        public virtual void RefreshData() { }
    }

    public class AltUnit
    {
        [JsonProperty]
        public bool IsActive;
        [JsonProperty]
        public string Unit;        
        [JsonProperty]
        public float Factor;
    }
}