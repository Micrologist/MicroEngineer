using KSP.Sim;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroMod
{
    public class MiscEntry : MicroEntry
    { }

    public class Separator : MiscEntry
    {
        public Separator()
        {
            Name = "--------------";
            Description = "It's a separator!";
            Category = MicroEntryCategory.Misc;
            Unit = "---";
            Formatting = null;
            EntryValue = "---------------";
        }
    }

    public class AutopilotStatus_IsEnabled : MiscEntry
    {
        public AutopilotStatus_IsEnabled()
        {
            Name = "Autopilot";
            Description = "";
            Category = MicroEntryCategory.Misc;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.AutopilotStatus.IsEnabled;
        }

        public override string ValueDisplay => EntryValue != null ? (bool)EntryValue ? "Enabled" : "Disabled" : "-";
    }

    public class AutopilotStatus_Mode : MiscEntry
    {
        public AutopilotStatus_Mode()
        {
            Name = "Autopilot Mode";
            Description = "";
            Category = MicroEntryCategory.Misc;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.AutopilotStatus.Mode;
        }

        public override string ValueDisplay => EntryValue != null ? (AutopilotMode)EntryValue == AutopilotMode.StabilityAssist ? "Stability" : EntryValue.ToString() : null;
    }

    public class TimeSinceLaunch : MiscEntry
    {
        public TimeSinceLaunch()
        {
            Name = "Time since Launch";
            Description = "";
            Category = MicroEntryCategory.Misc;
            Unit = "s";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TimeSinceLaunch;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }
    
    public class GravityForPos : MiscEntry
    {
        public GravityForPos()
        {
            Name = "Local Gravity";
            Description = "";
            Category = MicroEntryCategory.Misc;
            Unit = "ms2";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.gravityForPos.magnitude;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class LaunchTime : MiscEntry
    {
        public LaunchTime()
        {
            Name = "Launch Time";
            Description = "";
            Category = MicroEntryCategory.Misc;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.launchTime;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class AltimeterMode : MiscEntry
    {
        public AltimeterMode()
        {
            Name = "Altimeter Mode";
            Description = "";
            Category = MicroEntryCategory.Misc;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.altimeterMode;
        }

        public override string ValueDisplay => EntryValue != null ? (AltimeterDisplayMode)EntryValue == AltimeterDisplayMode.SeaLevel ? "Sea Level" : (AltimeterDisplayMode)EntryValue == AltimeterDisplayMode.GroundLevel ? "Ground Level" : "-" : "-";
    }

    public class SpeedMode : MiscEntry
    {
        public SpeedMode()
        {
            Name = "Speed Mode";
            Description = "";
            Category = MicroEntryCategory.Misc;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.speedMode;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class StartUT : MiscEntry
    {
        public StartUT()
        {
            Name = "Start UT";
            Description = "";
            Category = MicroEntryCategory.Misc;
            Unit = "s";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.StartUT;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class EndUT : MiscEntry
    {
        public EndUT()
        {
            Name = "Universal Time";
            Description = "";
            Category = MicroEntryCategory.Misc;
            Unit = "s";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.EndUT;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class UniversalTimeAtClosestApproach : MiscEntry
    {
        public UniversalTimeAtClosestApproach()
        {
            Name = "UT Closest Appr.";
            Description = "";
            Category = MicroEntryCategory.Misc;
            Unit = "s";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.UniversalTimeAtClosestApproach;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? MicroUtility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class UniversalTimeAtSoiEncounter : MiscEntry
    {
        public UniversalTimeAtSoiEncounter()
        {
            Name = "UT SOI Enc.";
            Description = "";
            Category = MicroEntryCategory.Misc;
            Unit = "s";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.UniversalTimeAtSoiEncounter;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? MicroUtility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }
}
