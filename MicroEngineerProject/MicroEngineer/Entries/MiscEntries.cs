using KSP.Sim;

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
            Description = "Is autopilot enabled or disabled.";
            Category = MicroEntryCategory.Misc;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.AutopilotStatus.IsEnabled;
        }

        public override string ValueDisplay => EntryValue != null ? (bool)EntryValue ? "Enabled" : "Disabled" : "-";
    }

    public class AutopilotStatus_Mode : MiscEntry
    {
        public AutopilotStatus_Mode()
        {
            Name = "Autopilot Mode";
            Description = "Mode vessel's autopilot is using: stability assist, prograde, retrograde, normal, etc.";
            Category = MicroEntryCategory.Misc;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.AutopilotStatus.Mode;
        }

        public override string ValueDisplay => EntryValue != null ? (AutopilotMode)EntryValue == AutopilotMode.StabilityAssist ? "Stability" : EntryValue.ToString() : null;
    }

    public class TimeSinceLaunch : MiscEntry
    {
        public TimeSinceLaunch()
        {
            Name = "Time since launch";
            Description = "Time since the vessel launched.";
            Category = MicroEntryCategory.Misc;
            Unit = "s";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TimeSinceLaunch;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? Utility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, Utility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class GravityForPos : MiscEntry
    {
        public GravityForPos()
        {
            Name = "Local Gravity";
            Description = "Local gravity vessel is experiencing.";
            Category = MicroEntryCategory.Misc;
            Unit = "ms2";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.gravityForPos.magnitude;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class LaunchTime : MiscEntry
    {
        public LaunchTime()
        {
            Name = "Launch Time";
            Description = "Universal Time when vessel was launched.";
            Category = MicroEntryCategory.Misc;
            Unit = "s";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.launchTime;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? Utility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, Utility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class AltimeterMode : MiscEntry
    {
        public AltimeterMode()
        {
            Name = "Altimeter Mode";
            Description = "Mode vessel's altimeter is using: Sea Level or Ground Level.";
            Category = MicroEntryCategory.Misc;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.altimeterMode;
        }

        public override string ValueDisplay => EntryValue != null ? (AltimeterDisplayMode)EntryValue == AltimeterDisplayMode.SeaLevel ? "Sea Level" : (AltimeterDisplayMode)EntryValue == AltimeterDisplayMode.GroundLevel ? "Ground Level" : "-" : "-";
    }

    public class SpeedMode : MiscEntry
    {
        public SpeedMode()
        {
            Name = "Speed Mode";
            Description = "Mode vessel's velocity meter is using: Orbit, Surface or Target.";
            Category = MicroEntryCategory.Misc;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.speedMode;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class StartUT : MiscEntry
    {
        public StartUT()
        {
            Name = "Start UT";
            Description = "Time passed since vessel was launched.";
            Category = MicroEntryCategory.Misc;
            Unit = "s";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.StartUT;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? Utility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, Utility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class EndUT : MiscEntry
    {
        public EndUT()
        {
            Name = "UT";
            Description = "Universal Time.";
            Category = MicroEntryCategory.Misc;
            Unit = "s";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.EndUT;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? Utility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, Utility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class UniversalTimeAtClosestApproach : MiscEntry
    {
        public UniversalTimeAtClosestApproach()
        {
            Name = "UT Close.App.";
            Description = "Universal Time at closest approach.";
            Category = MicroEntryCategory.Misc;
            Unit = "s";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.UniversalTimeAtClosestApproach;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? Utility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, Utility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class UniversalTimeAtSoiEncounter : MiscEntry
    {
        public UniversalTimeAtSoiEncounter()
        {
            Name = "UT SOI Enc.";
            Description = "Universal Time at the point of transfer to another sphere of influence.";
            Category = MicroEntryCategory.Misc;
            Unit = "s";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.UniversalTimeAtSoiEncounter;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? Utility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, Utility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }
}