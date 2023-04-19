
namespace MicroMod
{
    public class FlightEntry : MicroEntry
    { }

    public class Speed : FlightEntry
    {
        public Speed()
        {
            Name = "Speed";
            Description = "Shows the vessel's total velocity.";
            Category = MicroEntryCategory.Flight;
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.SurfaceVelocity.magnitude;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class MachNumber : FlightEntry
    {
        public MachNumber()
        {
            Name = "Mach Number";
            Description = "Shows the ratio of vessel's speed and local speed of sound.";
            Category = MicroEntryCategory.Flight;
            Unit = null;
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.SimulationObject.Telemetry.MachNumber;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class AtmosphericDensity : FlightEntry
    {
        public AtmosphericDensity()
        {
            Name = "Atm. Density";
            Description = "Shows the atmospheric density.";
            Category = MicroEntryCategory.Flight;
            Unit = "g/L";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.SimulationObject.Telemetry.AtmosphericDensity;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TotalLift : FlightEntry
    {
        public TotalLift()
        {
            Name = "Total Lift";
            Description = "Shows the total lift force produced by the vessel.";
            Category = MicroEntryCategory.Flight;
            Unit = "N";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = AeroForces.TotalLift;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                double toReturn = (double)EntryValue * 1000;
                return String.IsNullOrEmpty(base.Formatting) ? toReturn.ToString() : String.Format(base.Formatting, toReturn);
            }
        }
    }

    public class TotalDrag : FlightEntry
    {
        public TotalDrag()
        {
            Name = "Total Drag";
            Description = "Shows the total drag force exerted on the vessel.";
            Category = MicroEntryCategory.Flight;
            Unit = "N";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = AeroForces.TotalDrag;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                double toReturn = (double)EntryValue * 1000;
                return String.IsNullOrEmpty(base.Formatting) ? toReturn.ToString() : String.Format(base.Formatting, toReturn);
            }
        }
    }

    public class LiftDivDrag : FlightEntry
    {
        public LiftDivDrag()
        {
            Name = "Lift / Drag";
            Description = "Shows the ratio of total lift and drag forces.";
            Category = MicroEntryCategory.Flight;
            Unit = null;
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = AeroForces.TotalLift / AeroForces.TotalDrag;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                double toReturn = (double)EntryValue * 1000;
                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(base.Formatting, EntryValue);
            }
        }
    }

    public class AngleOfAttack : FlightEntry
    {
        public AngleOfAttack()
        {
            Name = "Angle of Attack";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = "°";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = AeroForces.AngleOfAttack;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class SideSlip : FlightEntry
    {
        public SideSlip()
        {
            Name = "Sideslip";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = "°";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = AeroForces.SideSlip;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class DragCoefficient : FlightEntry
    {
        public DragCoefficient()
        {
            Name = "DragCoefficient";
            Description = "";
            Category = MicroEntryCategory.Flight;
            Unit = null;
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.DragCoefficient;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ExposedArea : FlightEntry
    {
        public ExposedArea()
        {
            Name = "ExposedArea";
            Description = "";
            Category = MicroEntryCategory.Flight;
            Unit = null; // TODO
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.ExposedArea;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Heading : FlightEntry
    {
        public Heading()
        {
            Name = "Heading";
            Description = "";
            Category = MicroEntryCategory.Flight;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Heading;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Pitch_HorizonRelative : FlightEntry
    {
        public Pitch_HorizonRelative()
        {
            Name = "Pitch";
            Description = "";
            Category = MicroEntryCategory.Flight;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Pitch_HorizonRelative;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Roll_HorizonRelative : FlightEntry
    {
        public Roll_HorizonRelative()
        {
            Name = "Roll";
            Description = "";
            Category = MicroEntryCategory.Flight;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Roll_HorizonRelative;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Yaw_HorizonRelative : FlightEntry
    {
        public Yaw_HorizonRelative()
        {
            Name = "Yaw";
            Description = "";
            Category = MicroEntryCategory.Flight;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Yaw_HorizonRelative;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Zenith : FlightEntry
    {
        public Zenith()
        {
            Name = "Zenith";
            Description = "";
            Category = MicroEntryCategory.Flight;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Zenith;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class SoundSpeed : FlightEntry
    {
        public SoundSpeed()
        {
            Name = "Speed of sound";
            Description = "";
            Category = MicroEntryCategory.Flight;
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.SoundSpeed;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class GeeForce : FlightEntry
    {
        public GeeForce()
        {
            Name = "G-Force";
            Description = "";
            Category = MicroEntryCategory.Flight;
            Unit = "g";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.geeForce;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }


}
