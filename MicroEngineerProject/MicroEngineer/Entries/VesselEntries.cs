
namespace MicroMod
{
    public class VesselEntry : BaseEntry
    { }

    public class VesselName : VesselEntry
    {
        public VesselName()
        {
            Name = "Name";
            Description = "Name of the current vessel.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = true;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.DisplayName;
        }

        public override string ValueDisplay => EntryValue?.ToString();
    }

    public class Mass : VesselEntry
    {
        public Mass()
        {
            Name = "Mass";
            Description = "Total mass of the vessel.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = true;
            Unit = "kg";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.totalMass * 1000;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TotalDeltaVActual : VesselEntry
    {
        public TotalDeltaVActual()
        {
            Name = "Total ∆v";
            Description = "Vessel's total delta velocity.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = true;
            Unit = "m/s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.TotalDeltaVActual;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TotalDeltaVASL : VesselEntry
    {
        public TotalDeltaVASL()
        {
            Name = "Total ∆v ASL";
            Description = "Total delta velocity of the vessel At Sea Level.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = "m/s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.TotalDeltaVASL;
        }

        public override string ValueDisplay => base.ValueDisplay;

    }

    public class TotalDeltaVVac : VesselEntry
    {
        public TotalDeltaVVac()
        {
            Name = "Total ∆v Vac";
            Description = "Total delta velocity of the vessel in vacuum.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = "m/s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.TotalDeltaVVac;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TotalBurnTime : VesselEntry
    {
        public TotalBurnTime()
        {
            Name = "Total Burn Time";
            Description = "Burn Time vessel can sustain with 100% thrust.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = "s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.TotalBurnTime;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, Utility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class StageThrustActual : VesselEntry
    {
        public StageThrustActual()
        {
            Name = "Thrust";
            Description = "Vessel's actual thrust.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = true;
            Unit = "N";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.StageInfo.FirstOrDefault()?.ThrustActual * 1000;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class StageThrustASL : VesselEntry
    {
        public StageThrustASL()
        {
            Name = "Thrust (ASL)";
            Description = "Vessel's thrust At Sea Level.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = "N";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.StageInfo?.FirstOrDefault()?.ThrustASL * 1000;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class StageThrustVac : VesselEntry
    {
        public StageThrustVac()
        {
            Name = "Thrust (Vacuum)";
            Description = "Vessel's thrust in vacuum.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = "N";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.StageInfo?.FirstOrDefault()?.ThrustVac * 1000;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class StageTWRActual : VesselEntry
    {
        public StageTWRActual()
        {
            Name = "TWR";
            Description = "Vessel's Thrust to Weight Ratio.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = null;
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.StageInfo.FirstOrDefault()?.TWRActual;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class StageTWRASL : VesselEntry
    {
        public StageTWRASL()
        {
            Name = "TWR (ASL)";
            Description = "Vessel's Thrust to Weight Ratio At Sea Level.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = null;
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.StageInfo?.FirstOrDefault()?.TWRASL;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class StageTWRVac : VesselEntry
    {
        public StageTWRVac()
        {
            Name = "TWR (Vacuum)";
            Description = "Vessel's Thrust to Weight Ratio in vacuum.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = null;
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.StageInfo?.FirstOrDefault()?.TWRVac;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class StageISPActual : VesselEntry
    {
        public StageISPActual()
        {
            Name = "ISP";
            Description = "Specific impulse (ISP) is a measure of how efficiently a reaction mass engine creates thrust.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = "s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.StageInfo?.FirstOrDefault()?.IspActual;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class StageISPAsl : VesselEntry
    {
        public StageISPAsl()
        {
            Name = "ISP (ASL)";
            Description = "Specific impulse At Sea Level.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = "s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.StageInfo?.FirstOrDefault()?.IspASL;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }    

    public class StageISPVac : VesselEntry
    {
        public StageISPVac()
        {
            Name = "ISP (Vacuum)";
            Description = "Specific impulse in vacuum.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = "s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.StageInfo?.FirstOrDefault()?.IspVac;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class FuelPercentage : VesselEntry
    {
        public FuelPercentage()
        {
            Name = "Vessel Fuel";
            Description = "Vessel's fuel percentage left.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = "%";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.FuelPercentage;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class StageFuelPercentage : VesselEntry
    {
        public StageFuelPercentage()
        {
            Name = "Stage Fuel";
            Description = "Stage fuel percentage left.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = "%";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.StageFuelPercentage;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class PartsCount : VesselEntry
    {
        public PartsCount()
        {
            Name = "Parts";
            Description = "Number of parts vessel is constructed of.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = true;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselDeltaV?.PartInfo?.Count;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Throttle : VesselEntry
    {
        public Throttle()
        {
            Name = "Throttle";
            Description = "Vessel's current throttle in %.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = "%";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.flightCtrlState.mainThrottle * 100;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }
    
    public class TotalCommandCrewCapacity : VesselEntry
    {
        public TotalCommandCrewCapacity()
        {
            Name = "Command Crew Capacity";
            Description = "Crew capacity of all parts.";
            Category = MicroEntryCategory.Vessel;
            IsDefault = false;
            Unit = null;
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TotalCommandCrewCapacity;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }
}
