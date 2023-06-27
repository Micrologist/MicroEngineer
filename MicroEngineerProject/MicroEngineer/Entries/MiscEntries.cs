using KSP.Sim;

namespace MicroMod
{
    public class MiscEntry : BaseEntry
    { }

    public class Separator : MiscEntry
    {
        public Separator()
        {
            Name = "Separator";
            Description = "It's a separator!";
            EntryType = EntryType.Separator;
            Category = MicroEntryCategory.Misc;
            IsDefault = false;
            BaseUnit = null;
            Formatting = null;
            EntryValue = null;
        }
    }

    public class GravityForPos : MiscEntry
    {
        public GravityForPos()
        {
            Name = "Local gravity";
            Description = "Local gravity vessel is experiencing.";
            Category = MicroEntryCategory.Misc;
            IsDefault = false;
            BaseUnit = "ms2";
            NumberOfDecimalDigits = 3;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.gravityForPos.magnitude;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class EndUT : MiscEntry
    {
        public EndUT()
        {
            Name = "UT";
            Description = "Universal Time.";
            EntryType = EntryType.Time;
            Category = MicroEntryCategory.Misc;
            IsDefault = false;
            BaseUnit = "s";
            NumberOfDecimalDigits = 0;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.EndUT;
        }
    }

    public class StartUT : MiscEntry
    {
        public StartUT()
        {
            Name = "Start UT";
            Description = "Time passed since vessel was launched.";
            EntryType = EntryType.Time;
            Category = MicroEntryCategory.Misc;
            IsDefault = false;
            BaseUnit = "s";
            NumberOfDecimalDigits = 0;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.StartUT;
        }
    }

    public class LaunchTime : MiscEntry
    {
        public LaunchTime()
        {
            Name = "Launch time";
            Description = "Universal Time when vessel was launched.";
            EntryType = EntryType.Time;
            Category = MicroEntryCategory.Misc;
            IsDefault = false;
            BaseUnit = "s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.launchTime;
        }
    }

    public class TimeSinceLaunch : MiscEntry
    {
        public TimeSinceLaunch()
        {
            Name = "Time since launch";
            Description = "Time since the vessel launched.";
            EntryType = EntryType.Time;
            Category = MicroEntryCategory.Misc;
            IsDefault = false;
            BaseUnit = "s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TimeSinceLaunch;
        }
    }

    public class AutopilotStatus_IsEnabled : MiscEntry
    {
        public AutopilotStatus_IsEnabled()
        {
            Name = "Autopilot";
            Description = "Is autopilot enabled or disabled.";
            Category = MicroEntryCategory.Misc;
            IsDefault = false;
            BaseUnit = null;
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
            Name = "Autopilot mode";
            Description = "Mode vessel's autopilot is using: stability assist, prograde, retrograde, normal, etc.";
            Category = MicroEntryCategory.Misc;
            IsDefault = false;
            BaseUnit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.AutopilotStatus.Mode;
        }

        public override string ValueDisplay => EntryValue != null ? (AutopilotMode)EntryValue == AutopilotMode.StabilityAssist ? "Stability" : EntryValue.ToString() : null;
    }

    public class AltimeterMode : MiscEntry
    {
        public AltimeterMode()
        {
            Name = "Altimeter mode";
            Description = "Mode vessel's altimeter is using: Sea Level or Ground Level.";
            Category = MicroEntryCategory.Misc;
            IsDefault = false;
            BaseUnit = null;
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
            Name = "Speed mode";
            Description = "Mode vessel's velocity meter is using: Orbit, Surface or Target.";
            Category = MicroEntryCategory.Misc;
            IsDefault = false;
            BaseUnit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.speedMode;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class UniversalTimeAtClosestApproach : MiscEntry
    {
        public UniversalTimeAtClosestApproach()
        {
            Name = "UT close.app.";
            Description = "Universal Time at closest approach.";
            EntryType = EntryType.Time;
            Category = MicroEntryCategory.Misc;
            IsDefault = false;
            BaseUnit = "s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.UniversalTimeAtClosestApproach;
        }
    }

    public class UniversalTimeAtSoiEncounter : MiscEntry
    {
        public UniversalTimeAtSoiEncounter()
        {
            Name = "UT SOI enc.";
            Description = "Universal Time at the point of transfer to another sphere of influence.";
            EntryType = EntryType.Time;
            Category = MicroEntryCategory.Misc;
            IsDefault = false;
            BaseUnit = "s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.UniversalTimeAtSoiEncounter;
        }
    }
}