using KSP.Sim.impl;
using static KSP.Rendering.Planets.PQSData;

namespace MicroMod
{
    public class SurfaceEntry : MicroEntry
    { }

    public class Body : SurfaceEntry
    {
        public Body()
        {
            Name = "Body";
            Description = "Shows the body that vessel is currently at.";
            Category = MicroEntryCategory.Surface;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.mainBody.bodyName;
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
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Situation;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return MicroUtility.SituationToString((VesselSituations)EntryValue);
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
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Latitude;
            Unit = MicroUtility.ActiveVessel.Latitude < 0 ? "S" : "N";
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return MicroUtility.DegreesToDMS((double)EntryValue);
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
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Longitude;
            Unit = MicroUtility.ActiveVessel.Longitude < 0 ? "W" : "E";
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return MicroUtility.DegreesToDMS((double)EntryValue);
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
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.SimulationObject.Telemetry.SurfaceBiome;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return MicroUtility.BiomeToString((BiomeSurfaceData)EntryValue);
            }
        }
    }

    public class AltitudeAsl : SurfaceEntry
    {
        public AltitudeAsl()
        {
            Name = "Altitude (Sea Lvl)";
            Description = "Shows the vessel's altitude Above Sea Level";
            Category = MicroEntryCategory.Surface;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.AltitudeFromSeaLevel;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class AltitudeAgl : SurfaceEntry
    {
        public AltitudeAgl()
        {
            Name = "Altitude (Ground)";
            Description = "Shows the vessel's altitude Above Ground Level";
            Category = MicroEntryCategory.Surface;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.AltitudeFromTerrain;
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
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.HorizontalSrfSpeed;
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
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.VerticalSrfSpeed;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }
}
