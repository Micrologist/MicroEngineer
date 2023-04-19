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

    public class OrbitalSpeed : OrbitalEntry
    {
        public OrbitalSpeed()
        {
            Name = "Orbital Velocity";
            Description = "";
            Category = MicroEntryCategory.Orbital;
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.OrbitalSpeed;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class EccentricAnomaly : OrbitalEntry
    {
        public EccentricAnomaly()
        {
            Name = "Eccentric Anomaly";
            Description = "";
            Category = MicroEntryCategory.Orbital;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.EccentricAnomaly;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, (double)EntryValue * PatchedConicsOrbit.Rad2Deg);
            }
        }
    }

    public class MeanAnomaly : OrbitalEntry
    {
        public MeanAnomaly()
        {
            Name = "Mean Anomaly";
            Description = "";
            Category = MicroEntryCategory.Orbital;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.MeanAnomaly;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, (double)EntryValue * PatchedConicsOrbit.Rad2Deg);
            }
        }
    }

    public class TrueAnomaly : OrbitalEntry
    {
        public TrueAnomaly()
        {
            Name = "True Anomaly";
            Description = "";
            Category = MicroEntryCategory.Orbital;
            Unit = "°";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.TrueAnomaly;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, MicroUtility.RadiansToDegrees((double)EntryValue));
            }
        }
    }

    public class ObT : OrbitalEntry
    {
        public ObT()
        {
            Name = "Orbit Time";
            Description = "";
            Category = MicroEntryCategory.Orbital;
            Unit = "s";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.ObT;
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

    public class ArgumentOfPeriapsis : OrbitalEntry
    {
        public ArgumentOfPeriapsis()
        {
            Name = "Argument of Pe. ω";
            Description = "";
            Category = MicroEntryCategory.Orbital;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.OrbitalElements.ArgumentOfPeriapsis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class LongitudeOfAscendingNode : OrbitalEntry
    {
        public LongitudeOfAscendingNode()
        {
            Name = "LAN Ω";
            Description = "";
            Category = MicroEntryCategory.Orbital;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.OrbitalElements.LongitudeOfAscendingNode;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class SemiMajorAxis : OrbitalEntry
    {
        public SemiMajorAxis()
        {
            Name = "Semi Major Axis <i>a</i>";
            Description = "";
            Category = MicroEntryCategory.Orbital;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.OrbitalElements.SemiMajorAxis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class SemiMinorAxis : OrbitalEntry
    {
        public SemiMinorAxis()
        {
            Name = "Semi Minor Axis <i>b</i>";
            Description = "";
            Category = MicroEntryCategory.Orbital;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.SemiMinorAxis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class OrbitalEnergy : OrbitalEntry
    {
        public OrbitalEnergy()
        {
            Name = "Orbital Energy";
            Description = "";
            Category = MicroEntryCategory.Orbital;
            Unit = "kJ";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.OrbitalEnergy / 1000.0;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class SemiLatusRectum : OrbitalEntry
    {
        public SemiLatusRectum()
        {
            Name = "Semi Latus Rectum ℓ";
            Description = "";
            Category = MicroEntryCategory.Orbital;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.SemiLatusRectum;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class OrbitPercent : OrbitalEntry
    {
        public OrbitPercent()
        {
            Name = "Orbit Percent";
            Description = "";
            Category = MicroEntryCategory.Orbital;
            Unit = "%";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.orbitPercent * 100;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class OrbitRadius : OrbitalEntry
    {
        public OrbitRadius()
        {
            Name = "Orbit Radius";
            Description = "";
            Category = MicroEntryCategory.Orbital;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.radius;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }
}
