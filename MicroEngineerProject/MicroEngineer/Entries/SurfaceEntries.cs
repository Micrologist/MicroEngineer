using KSP.Sim.impl;
using static KSP.Rendering.Planets.PQSData;

namespace MicroMod
{
    public class SurfaceEntry : BaseEntry
    { }

    public class AltitudeAgl : SurfaceEntry
    {
        public AltitudeAgl()
        {
            Name = "Altitude (Ground)";
            Description = "Shows the vessel's altitude above ground Level.";
            Category = MicroEntryCategory.Surface;
            IsDefault = true;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.AltitudeFromTerrain;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class AltitudeAsl : SurfaceEntry
    {
        public AltitudeAsl()
        {
            Name = "Altitude (Sea Lvl)";
            Description = "Shows the vessel's altitude above sea level.";
            Category = MicroEntryCategory.Surface;
            IsDefault = true;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.AltitudeFromSeaLevel;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class AltitudeFromScenery : SurfaceEntry
    {
        public AltitudeFromScenery()
        {
            Name = "Altitude (Scenery)";
            Description = "Shows the vessel's altitude above scenery.";
            Category = MicroEntryCategory.Surface;
            IsDefault = false;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.AltitudeFromScenery;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class VerticalVelocity : SurfaceEntry
    {
        public VerticalVelocity()
        {
            Name = "Vertical Vel.";
            Description = "Shows the vessel's vertical velocity (up/down).";
            Category = MicroEntryCategory.Surface;
            IsDefault = true;
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VerticalSrfSpeed;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class HorizontalVelocity : SurfaceEntry
    {
        public HorizontalVelocity()
        {
            Name = "Horizontal Vel.";
            Description = "Shows the vessel's horizontal velocity across a celestial body's surface.";
            Category = MicroEntryCategory.Surface;
            IsDefault = true;
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.HorizontalSrfSpeed;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }
    
    public class Situation : SurfaceEntry
    {
        public Situation()
        {
            Name = "Situation";
            Description = "Shows the vessel's current situation: Landed, Flying, Orbiting, etc.";
            Category = MicroEntryCategory.Surface;
            IsDefault = true;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Situation;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return Utility.SituationToString((VesselSituations)EntryValue);
            }
        }
    }

    public class Biome : SurfaceEntry
    {
        public Biome()
        {
            Name = "Biome";
            Description = "Shows the biome currently below the vessel.";
            Category = MicroEntryCategory.Surface;
            IsDefault = true;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.SimulationObject.Telemetry.SurfaceBiome;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return Utility.BiomeToString((BiomeSurfaceData)EntryValue);
            }
        }
    }

    public class Latitude : SurfaceEntry
    {
        public Latitude()
        {
            Name = "Latitude";
            Description = "Shows the vessel's latitude position around the celestial body. Latitude is the angle from the equator towards the poles.";
            Category = MicroEntryCategory.Surface;
            IsDefault = true;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Latitude;
            Unit = Utility.ActiveVessel.Latitude < 0 ? "S" : "N";
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return Utility.DegreesToDMS((double)EntryValue);
            }
        }
    }

    public class Longitude : SurfaceEntry
    {
        public Longitude()
        {
            Name = "Longitude";
            Description = "Shows the vessel's longitude position around the celestial body. Longitude is the angle from the body's prime meridian to the current meridian.";
            Category = MicroEntryCategory.Surface;
            IsDefault = true;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Longitude;
            Unit = Utility.ActiveVessel.Longitude < 0 ? "W" : "E";
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return Utility.DegreesToDMS((double)EntryValue);
            }
        }
    }

    public class DynamicPressure_kPa : SurfaceEntry
    {
        public DynamicPressure_kPa()
        {
            Name = "Dynamic Pressure";
            Description = "Dynamic Pressure (q) is a defined property of a moving flow of gas. It describes how much pressure the airflow is having on the vessel.";
            Category = MicroEntryCategory.Surface;
            IsDefault = false;
            Unit = "kPa";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.DynamicPressure_kPa;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class StaticPressure_kPa : SurfaceEntry
    {
        public StaticPressure_kPa()
        {
            Name = "Static Pressure";
            Description = "Static pressure is a term used to define the amount of pressure exerted by a fluid that is not moving - ambient pressure.";
            Category = MicroEntryCategory.Surface;
            IsDefault = false;
            Unit = "kPa";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.StaticPressure_kPa;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class AtmosphericTemperature : SurfaceEntry
    {
        public AtmosphericTemperature()
        {
            Name = "Static ambient temp.";
            Description = "Temperature measured outside the vessel. The sensor which detects SAT must be carefully sited to ensure that airflow over it does not affect the indicated temperature.";
            Category = MicroEntryCategory.Surface;
            IsDefault = false;
            Unit = "K";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.AtmosphericTemperature;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ExternalTemperature : SurfaceEntry
    {
        public ExternalTemperature()
        {
            Name = "Total air temp.";
            Description = "Measured by means of a sensor positioned in the airflow, kinetic heating will result, raising the temperature measured above the Static ambient temperature.";
            Category = MicroEntryCategory.Surface;
            IsDefault = false;
            Unit = "K";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.ExternalTemperature;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }    
}
