using KSP.Game;
using KSP.Sim.impl;

namespace MicroMod
{
    public class OrbitalEntry : MicroEntry
    { }

    public class Apoapsis : OrbitalEntry
    {
        public Apoapsis()
        {
            Name = "Apoapsis";
            Description = "Vessel's apoapsis height relative to the sea level. Apoapsis is the highest point of an orbit.";
            Category = MicroEntryCategory.Orbital;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.ApoapsisArl;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TimeToApoapsis : OrbitalEntry
    {
        public TimeToApoapsis()
        {
            Name = "Time to Ap.";
            Description = "Shows the time until the vessel reaches apoapsis, the highest point of the orbit.";
            Category = MicroEntryCategory.Orbital;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = (MicroUtility.ActiveVessel.Situation == VesselSituations.Landed || MicroUtility.ActiveVessel.Situation == VesselSituations.PreLaunch) ? 0f : MicroUtility.ActiveVessel.Orbit.TimeToAp;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return MicroUtility.SecondsToTimeString((double)EntryValue);
            }
        }
    }

    public class Periapsis : OrbitalEntry
    {
        public Periapsis()
        {
            Name = "Periapsis";
            Description = "Vessel's periapsis height relative to the sea level. Periapsis is the lowest point of an orbit.";
            Category = MicroEntryCategory.Orbital;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.PeriapsisArl;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TimeToPeriapsis : OrbitalEntry
    {
        public TimeToPeriapsis()
        {
            Name = "Time to Pe.";
            Description = "Shows the time until the vessel reaches periapsis, the lowest point of the orbit.";
            Category = MicroEntryCategory.Orbital;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.TimeToPe;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return MicroUtility.SecondsToTimeString((double)EntryValue);
            }
        }
    }

    public class Inclination : OrbitalEntry
    {
        public Inclination()
        {
            Name = "Inclination";
            Description = "Shows the vessel's orbital inclination relative to the equator.";
            Category = MicroEntryCategory.Orbital;
            Unit = "°";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.inclination;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Eccentricity : OrbitalEntry
    {
        public Eccentricity()
        {
            Name = "Eccentricity";
            Description = "Shows the vessel's orbital eccentricity which is a measure of how much an elliptical orbit is 'squashed'.";
            Category = MicroEntryCategory.Orbital;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.eccentricity;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Period : OrbitalEntry
    {
        public Period()
        {
            Name = "Period";
            Description = "Shows the amount of time it will take to complete a full orbit.";
            Category = MicroEntryCategory.Orbital;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.period;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return MicroUtility.SecondsToTimeString((double)EntryValue);
            }
        }
    }

    public class SoiTransition : OrbitalEntry
    {
        public SoiTransition()
        {
            Name = "SOI Trans.";
            Description = "Shows the amount of time it will take to transition to another Sphere of Influence.";
            Category = MicroEntryCategory.Orbital;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.UniversalTimeAtSoiEncounter - GameManager.Instance.Game.UniverseModel.UniversalTime;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return (double)EntryValue >= 0 ? MicroUtility.SecondsToTimeString((double)EntryValue) : "-";
            }
        }
    }
}
