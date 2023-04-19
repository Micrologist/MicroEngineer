
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

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, EntryValue);
            }
        }
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

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, EntryValue);
            }
        }
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

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, EntryValue);
            }
        }
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
            Formatting = "{0:N3}";
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
}
