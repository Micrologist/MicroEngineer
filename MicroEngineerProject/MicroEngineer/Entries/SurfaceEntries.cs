﻿using KSP.Game.Science;
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
            Name = "Altitude (ground)";
            Description = "Shows the vessel's altitude above ground Level.";
            Category = MicroEntryCategory.Surface;
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
            // AltitudeFromScenery seems to be the correct value to use for altitude above ground underneath the vessel
            EntryValue = Utility.ActiveVessel.AltitudeFromScenery;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class AltitudeAsl : SurfaceEntry
    {
        public AltitudeAsl()
        {
            Name = "Altitude (sea)";
            Description = "Shows the vessel's altitude above sea level.";
            Category = MicroEntryCategory.Surface;
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
            EntryValue = Utility.ActiveVessel.AltitudeFromSeaLevel;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class AltitudeFromScenery : SurfaceEntry
    {
        public AltitudeFromScenery()
        {
            Name = "Altitude (scenery)";
            Description = "Shows the vessel's altitude above scenery.";
            Category = MicroEntryCategory.Surface;
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
            // We'll use AltitudeFromTerrain for this entry for now. It indicates some height above the ground level
            // This may be a bug where they've switched AltitudeFromTerrain and AltitudeFromScenery values
            EntryValue = Utility.ActiveVessel.AltitudeFromTerrain;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class VerticalVelocity : SurfaceEntry
    {
        public VerticalVelocity()
        {
            Name = "Vertical vel.";
            Description = "Shows the vessel's vertical velocity (up/down).";
            Category = MicroEntryCategory.Surface;
            IsDefault = true;
            MiliUnit = "mm/s";
            BaseUnit = "m/s";
            KiloUnit = "km/s";
            MegaUnit = "Mm/s";
            GigaUnit = "Gm/s";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
            AltUnit = new AltUnit()
            {
                IsActive = false,
                Unit = "km/h",
                Factor = (60f * 60f) / 1000f
            };
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
            Name = "Horizontal vel.";
            Description = "Shows the vessel's horizontal velocity across a celestial body's surface.";
            Category = MicroEntryCategory.Surface;
            IsDefault = true;
            MiliUnit = "mm/s";
            BaseUnit = "m/s";
            KiloUnit = "km/s";
            MegaUnit = "Mm/s";
            GigaUnit = "Gm/s";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
            AltUnit = new AltUnit()
            {
                IsActive = false,
                Unit = "km/h",
                Factor = (60f * 60f) / 1000f
            };
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
            Name = "State";
            Description = "Shows the vessel's current state: Landed, Flying, Orbiting, etc.";
            Category = MicroEntryCategory.Surface;
            IsDefault = false;
            BaseUnit = null;
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
            IsDefault = false;
            BaseUnit = null;
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

    public class ScienceSituationEntry : SurfaceEntry
    {
        public ScienceSituationEntry()
        {
            Name = "Situation";
            Description = "Shows the current science situation - high/low orbit, atmosphere, etc.";
            Category = MicroEntryCategory.Surface;
            IsDefault = true;
            BaseUnit = null;
            Formatting = null;
        }
        
        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselScienceRegionSituation.ResearchLocation?.ScienceSituation;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return Utility.ScienceSituationToString((ScienceSitutation)EntryValue);
            }
        }
    }

    public class ScienceRegion : SurfaceEntry
    {
        public ScienceRegion()
        {
            Name = "Region";
            Description = "Shows the current science region.";
            Category = MicroEntryCategory.Surface;
            IsDefault = true;
            BaseUnit = null;
            Formatting = null;
        }
        
        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.VesselScienceRegionSituation.ResearchLocation?.ScienceRegion;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return ScienceRegionsHelper.GetRegionDisplayName(EntryValue.ToString());
            }
        }
    }

    public class Latitude : SurfaceEntry
    {
        public Latitude()
        {
            Name = "Latitude";
            Description = "Shows the vessel's latitude position around the celestial body. Latitude is the angle from the equator towards the poles.";
            EntryType = EntryType.LatitudeLongitude;
            Category = MicroEntryCategory.Surface;
            IsDefault = true;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Latitude;
            BaseUnit = Utility.ActiveVessel.Latitude < 0 ? "S" : "N";
        }
    }
    
    public class LatitudeDecimal : SurfaceEntry
    {
        public LatitudeDecimal()
        {
            Name = "Latitude (dec)";
            Description = "Shows the vessel's latitude position around the celestial body. Latitude is the angle from the equator towards the poles. Decimal format.";
            Category = MicroEntryCategory.Surface;
            IsDefault = false;
            BaseUnit = null;
            NumberOfDecimalDigits = 3;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Latitude;
        }
    }

    public class Longitude : SurfaceEntry
    {
        public Longitude()
        {
            Name = "Longitude";
            Description = "Shows the vessel's longitude position around the celestial body. Longitude is the angle from the body's prime meridian to the current meridian.";
            EntryType = EntryType.LatitudeLongitude;
            Category = MicroEntryCategory.Surface;
            IsDefault = true;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Longitude;
            BaseUnit = Utility.ActiveVessel.Longitude < 0 ? "W" : "E";
        }
    }
    
    public class LongitudeDecimal : SurfaceEntry
    {
        public LongitudeDecimal()
        {
            Name = "Longitude (dec)";
            Description = "Shows the vessel's longitude position around the celestial body. Longitude is the angle from the body's prime meridian to the current meridian. Decimal format.";
            Category = MicroEntryCategory.Surface;
            IsDefault = false;
            BaseUnit = null;
            NumberOfDecimalDigits = 3;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Longitude;
        }
    }

    public class DynamicPressure_kPa : SurfaceEntry
    {
        public DynamicPressure_kPa()
        {
            Name = "Dynamic pressure";
            Description = "Dynamic Pressure (q) is a defined property of a moving flow of gas. It describes how much pressure the airflow is having on the vessel.";
            Category = MicroEntryCategory.Surface;
            IsDefault = false;
            MiliUnit = "Pa";
            BaseUnit = "kPa";
            KiloUnit = "MPa";
            MegaUnit = "GPa";
            GigaUnit = null;
            NumberOfDecimalDigits = 2;
            Formatting = "N";
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
            Name = "Static pressure";
            Description = "Static pressure is a term used to define the amount of pressure exerted by a fluid that is not moving - ambient pressure.";
            Category = MicroEntryCategory.Surface;
            IsDefault = false;
            MiliUnit = "Pa";
            BaseUnit = "kPa";
            KiloUnit = "MPa";
            MegaUnit = "GPa";
            GigaUnit = null;
            NumberOfDecimalDigits = 2;
            Formatting = "N";
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
            BaseUnit = "K";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
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
            BaseUnit = "K";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.ExternalTemperature;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }
}