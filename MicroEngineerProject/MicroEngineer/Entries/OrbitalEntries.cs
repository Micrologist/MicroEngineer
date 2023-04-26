using KSP.Game;
using KSP.Sim.impl;

namespace MicroMod
{
    public class OrbitalEntry : BaseEntry
    { }

    public class Apoapsis : OrbitalEntry
    {
        public Apoapsis()
        {
            Name = "Apoapsis";
            Description = "Vessel's apoapsis height relative to the sea level. Apoapsis is the highest point of an orbit.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = true;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.ApoapsisArl;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Periapsis : OrbitalEntry
    {
        public Periapsis()
        {
            Name = "Periapsis";
            Description = "Vessel's periapsis height relative to the sea level. Periapsis is the lowest point of an orbit.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = true;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.PeriapsisArl;
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
            IsDefault = true;
            BaseUnit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = (Utility.ActiveVessel.Situation == VesselSituations.Landed || Utility.ActiveVessel.Situation == VesselSituations.PreLaunch) ? 0f : Utility.ActiveVessel.Orbit.TimeToAp;
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

    public class TimeToPeriapsis : OrbitalEntry
    {
        public TimeToPeriapsis()
        {
            Name = "Time to Pe.";
            Description = "Shows the time until the vessel reaches periapsis, the lowest point of the orbit.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = true;
            BaseUnit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.TimeToPe;
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

    public class Inclination : OrbitalEntry
    {
        public Inclination()
        {
            Name = "Inclination";
            Description = "Shows the vessel's orbital inclination relative to the equator.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = true;
            BaseUnit = "°";
            NumberOfDecimalDigits = 3;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.inclination;
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
            IsDefault = true;
            BaseUnit = null;
            NumberOfDecimalDigits = 3;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.eccentricity;
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
            IsDefault = true;
            BaseUnit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.period;
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

    public class OrbitalSpeed : OrbitalEntry
    {
        public OrbitalSpeed()
        {
            Name = "Orbital speed";
            Description = "Shows the vessel's orbital speed";
            Category = MicroEntryCategory.Orbital;
            IsDefault = true;
            MiliUnit = "mm/s";
            BaseUnit = "m/s";
            KiloUnit = "km/s";
            MegaUnit = "Mm/s";
            GigaUnit = "Gm/s";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.OrbitalSpeed;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TrueAnomaly : OrbitalEntry
    {
        public TrueAnomaly()
        {
            Name = "True anomaly";
            Description = "Angle between the direction of periapsis and the current position of the object, as seen from the main focus of the ellipse.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = false;
            BaseUnit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.TrueAnomaly;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, Utility.RadiansToDegrees((double)EntryValue));
            }
        }
    }

    public class MeanAnomaly : OrbitalEntry
    {
        public MeanAnomaly()
        {
            Name = "Mean anomaly";
            Description = "Parameter used to describe the position of an object in its orbit around the celestial body.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = false;
            BaseUnit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.MeanAnomaly;
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

    public class EccentricAnomaly : OrbitalEntry
    {
        public EccentricAnomaly()
        {
            Name = "Eccentric anomaly";
            Description = "Angle at the center of the orbital ellipse from the semi major axis to the line that passes through the center of the ellipse and the point on the auxiliary circle that is the intersection of the line perpendicular to the semi major axis and passes through the point in the orbit where the vessel is.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = false;
            BaseUnit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.EccentricAnomaly;
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

    public class LongitudeOfAscendingNode : OrbitalEntry
    {
        public LongitudeOfAscendingNode()
        {
            Name = "LAN Ω";
            Description = "Longitude of Ascending Node is an angle from a specified reference direction, called the origin of longitude, to the direction of the ascending node, as measured in a specified reference plane.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = false;
            BaseUnit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.OrbitalElements.LongitudeOfAscendingNode;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ArgumentOfPeriapsis : OrbitalEntry
    {
        public ArgumentOfPeriapsis()
        {
            Name = "Argument of Pe.";
            Description = "Angle from the line of the ascending node on the equatorial plane to the point of periapsis passage.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = false;
            BaseUnit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.OrbitalElements.ArgumentOfPeriapsis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class SemiLatusRectum : OrbitalEntry
    {
        public SemiLatusRectum()
        {
            Name = "Semi latus rect";
            Description = "Half the length of the chord through one focus, perpendicular to the major axis.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = false;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.SemiLatusRectum;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class SemiMajorAxis : OrbitalEntry
    {
        public SemiMajorAxis()
        {
            Name = "Semi major axis";
            Description = "Shows the distance from the center of an orbit to the farthest edge.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = false;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.OrbitalElements.SemiMajorAxis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class SemiMinorAxis : OrbitalEntry
    {
        public SemiMinorAxis()
        {
            Name = "Semi minor axis";
            Description = "Shows the distance from the center of an orbit to the nearest edge.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = false;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.SemiMinorAxis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class OrbitalEnergy : OrbitalEntry
    {
        public OrbitalEnergy()
        {
            Name = "Orbital energy";
            Description = "Constant sum of two orbiting bodies' mutual potential energy and their total kinetic energy divided by the reduced mass.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = false;
            MiliUnit = "mJ";
            BaseUnit = "J";
            KiloUnit = "kJ";
            MegaUnit = "MJ";
            GigaUnit = "GJ";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.OrbitalEnergy / 1000.0;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ObT : OrbitalEntry
    {
        public ObT()
        {
            Name = "Orbit time";
            Description = "Shows orbit time in seconds from the Periapsis.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = false;
            BaseUnit = "s";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.ObT;
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

    public class OrbitPercent : OrbitalEntry
    {
        public OrbitPercent()
        {
            Name = "Orbit percent";
            Description = "Percent of the orbit completed.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = false;
            BaseUnit = "%";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.orbitPercent * 100;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class OrbitRadius : OrbitalEntry
    {
        public OrbitRadius()
        {
            Name = "Orbit radius";
            Description = "Length from the center of the ellipse to the object.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = false;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.radius;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class SoiTransition : OrbitalEntry
    {
        public SoiTransition()
        {
            Name = "SOI trans.";
            Description = "Shows the amount of time it will take to transition to another Sphere Of Influence.";
            Category = MicroEntryCategory.Orbital;
            IsDefault = false;
            BaseUnit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.UniversalTimeAtSoiEncounter - GameManager.Instance.Game.UniverseModel.UniversalTime;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return (double)EntryValue >= 0 ? Utility.SecondsToTimeString((double)EntryValue) : "-";
            }
        }
    }  
}