using KSP.Game;
using KSP.Sim.DeltaV;
using KSP.Sim.impl;
using Newtonsoft.Json;
using static KSP.Rendering.Planets.PQSData;

namespace MicroMod
{
    /// <summary>
    /// Base class for all Entries (values that can be attached to windows)
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class MicroEntry
    {
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public string Description;
        [JsonProperty]
        public MicroEntryCategory Category;
        [JsonProperty]
        public string Unit;
        [JsonProperty]
        public string Formatting;

        public virtual object EntryValue { get; set; }

        /// <summary>
        /// Controls how the value should be displayed. Should be overriden in a inheritet class for a concrete implementation.
        /// </summary>
        public virtual string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(this.Formatting) ? EntryValue.ToString() : String.Format(Formatting, EntryValue);
            }
        }

        public virtual void RefreshData() { }
    }
    

    #region Flight scene entries    

    public class Apoapsis : MicroEntry
    {
        public Apoapsis()
        {
            Name = "Apoapsis";
            Description = "Vessel's apoapsis height relative to the sea level. Apoapsis is the highest point of an orbit.";
            Category = MicroEntryCategory.Orbital;
            Unit = "m";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.ApoapsisArl;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return MicroUtility.MetersToDistanceString((double)EntryValue);
            }
        }
    }

    public class TimeToApoapsis : MicroEntry
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

    public class Periapsis : MicroEntry
    {
        public Periapsis()
        {
            Name = "Periapsis";
            Description = "Vessel's periapsis height relative to the sea level. Periapsis is the lowest point of an orbit.";
            Category = MicroEntryCategory.Orbital;
            Unit = "m";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.PeriapsisArl;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return MicroUtility.MetersToDistanceString((double)EntryValue);
            }
        }
    }

    public class TimeToPeriapsis : MicroEntry
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
            EntryValue = EntryValue = MicroUtility.ActiveVessel.Orbit.TimeToPe;
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

    public class Inclination : MicroEntry
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

    public class Eccentricity : MicroEntry
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

    public class Period : MicroEntry
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

    public class SoiTransition : MicroEntry
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

    public class Body : MicroEntry
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

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(this.Formatting) ? EntryValue.ToString() : String.Format(Formatting, EntryValue);
            }
        }
    }

    public class Situation : MicroEntry
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

    public class Latitude : MicroEntry
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

    public class Longitude : MicroEntry
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

    public class Biome : MicroEntry
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

    public class AltitudeAsl : MicroEntry
    {
        public AltitudeAsl()
        {
            Name = "Altitude (Sea Level)";
            Description = "Shows the vessel's altitude Above Sea Level";
            Category = MicroEntryCategory.Surface;
            Unit = "m";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.AltitudeFromSeaLevel;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return MicroUtility.MetersToDistanceString((double)EntryValue);
            }
        }
    }

    public class AltitudeAgl : MicroEntry
    {
        public AltitudeAgl()
        {
            Name = "Altitude (Ground)";
            Description = "Shows the vessel's altitude Above Ground Level";
            Category = MicroEntryCategory.Surface;
            Unit = "m";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.AltitudeFromTerrain;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return MicroUtility.MetersToDistanceString((double)EntryValue);
            }
        }
    }

    public class HorizontalVelocity : MicroEntry
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

    public class VerticalVelocity : MicroEntry
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

    public class Speed : MicroEntry
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

    public class MachNumber : MicroEntry
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

    public class AtmosphericDensity : MicroEntry
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

    public class TotalLift : MicroEntry
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

    public class TotalDrag : MicroEntry
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

    public class LiftDivDrag : MicroEntry
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
    
    

    public class StageInfo : MicroEntry
    {
        public StageInfo()
        {
            Name = "Stage Info";
            Description = "Stage Info object, not implemented yet."; // TODO Stage Info display and description
            Category = MicroEntryCategory.Stage;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.VesselDeltaV?.StageInfo;
        }

        public override string ValueDisplay
        {
            get
            {
                //TODO: stageinfo display
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(base.Formatting, EntryValue);
            }
        }
    }

    public class Separator : MicroEntry
    {
        public Separator()
        {
            Name = "--------------";
            Description = "It's a separator!";
            Category = MicroEntryCategory.Misc;
            Unit = "---";
            Formatting = null;
            EntryValue = "---------------";
        }
    }
    #endregion

    #region OAB scene entries
    public class TotalBurnTime_OAB : MicroEntry
    {
        public bool UseDHMSFormatting; // TODO: implement

        public TotalBurnTime_OAB()
        {
            Name = "Total Burn Time (OAB)";
            Description = "Shows the total length of burn the vessel can mantain.";
            Category = MicroEntryCategory.OAB;
            Unit = "s";
            Formatting = "{0:N1}";
            UseDHMSFormatting = true;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.VesselDeltaVComponentOAB?.TotalBurnTime;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                if (UseDHMSFormatting)
                    return MicroUtility.SecondsToTimeString((double)EntryValue);
                else
                    return String.IsNullOrEmpty(this.Formatting) ? EntryValue.ToString() : String.Format(Formatting, EntryValue);
            }
        }
    }

    public class TotalDeltaVASL_OAB : MicroEntry
    {
        public TotalDeltaVASL_OAB()
        {
            Name = "Total ∆v ASL (OAB)";
            Description = "Shows the vessel's total delta velocity At Sea Level.";
            Category = MicroEntryCategory.OAB;
            Unit = "m/s";
            Formatting = "{0:N0}";
        }
        public override void RefreshData()
        {
            EntryValue = MicroUtility.VesselDeltaVComponentOAB?.TotalDeltaVASL;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";
                
                return String.IsNullOrEmpty(this.Formatting) ? EntryValue.ToString() : String.Format(Formatting, EntryValue);
            }
        }
    }

    public class TotalDeltaVActual_OAB : MicroEntry
    {
        public TotalDeltaVActual_OAB()
        {
            Name = "Total ∆v Actual (OAB)";
            Description = "Shows the vessel's actual total delta velocity (not used in OAB).";
            Category = MicroEntryCategory.OAB;
            Unit = "m/s";
            Formatting = "{0:N0}";
        }
        public override void RefreshData()
        {
            EntryValue = MicroUtility.VesselDeltaVComponentOAB?.TotalDeltaVActual;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(this.Formatting) ? EntryValue.ToString() : String.Format(Formatting, EntryValue);
            }
        }
    }

    public class TotalDeltaVVac_OAB : MicroEntry
    {
        public TotalDeltaVVac_OAB()
        {
            Name = "Total ∆v Vac (OAB)";
            Description = "Shows the vessel's total delta velocity in Vacuum.";
            Category = MicroEntryCategory.OAB;
            Unit = "m/s";
            Formatting = "{0:N0}";
        }
        public override void RefreshData()
        {
            EntryValue = MicroUtility.VesselDeltaVComponentOAB?.TotalDeltaVVac;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(this.Formatting) ? EntryValue.ToString() : String.Format(Formatting, EntryValue);
            }
        }
    }

    /// <summary>
    /// Calculates torque from the Center of Thrust and Center of Mass
    /// </summary>    
    public class Torque : MicroEntry
    {
        [JsonProperty]
        internal bool IsActive = false;

        public Torque()
        {
            Name = "Torque";
            Description = "Thrust torque that is generated by not having Thrust Vector and Center of Mass aligned. Turn on the Center of Thrust and Center of Mass VAB indicators to get an accurate value.";
            Category = MicroEntryCategory.OAB;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            if (!this.IsActive)
                return;

            Vector3d com = GameManager.Instance?.Game?.OAB?.Current?.Stats?.MainAssembly?.CenterOfProperties?.CoM ?? Vector3d.zero;
            Vector3d cot = GameManager.Instance?.Game?.OAB?.Current?.Stats?.MainAssembly?.CenterOfProperties?.CoT ?? Vector3d.zero;

            if (com == Vector3d.zero || cot == Vector3d.zero)
                return;

            List<DeltaVEngineInfo> engines = GameManager.Instance?.Game?.OAB?.Current?.Stats?.MainAssembly?.VesselDeltaV?.EngineInfo;
            if (engines == null || engines.Count == 0)
                return;

            Vector3d force = new Vector3d();

            foreach (var engine in engines)
            {
                force += engine.ThrustVectorVac;
            }

            var leverArm = cot - com;

            Vector3d torque = Vector3d.Cross(force, (Vector3d)leverArm);

            this.EntryValue = torque.magnitude;
            this.Unit = (double)EntryValue >= 1.0 ? "kNm" : "Nm";
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null || !this.IsActive)
                    return "-";

                if ((double)EntryValue >= 1.0)
                    return $"{String.Format("{0:F2}", EntryValue)}";

                return Math.Abs((double)EntryValue) > double.Epsilon ? $"{String.Format("{0:F2}", (double)EntryValue * 1000.0)}" : $"{String.Format("{0:F0}", (double)EntryValue)}";
            }
        }
    }

    /// <summary>
    /// Holds stage info parameters for each stage. Also keeps information about the celestial body user selected in the window.
    /// </summary>
    public class StageInfo_OAB : MicroEntry
    {
        public List<string> CelestialBodyForStage = new();

        public StageInfo_OAB()
        {
            Name = "Stage Info (OAB)";
            Description = "Holds a list of stage info parameters.";
            Category = MicroEntryCategory.OAB;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue ??= new List<DeltaVStageInfo_OAB>();

            ((List<DeltaVStageInfo_OAB>)EntryValue).Clear();

            if (MicroUtility.VesselDeltaVComponentOAB?.StageInfo == null) return;

            foreach (var stage in MicroUtility.VesselDeltaVComponentOAB.StageInfo)
            {
                ((List<DeltaVStageInfo_OAB>)EntryValue).Add(new DeltaVStageInfo_OAB
                {
                    DeltaVActual = stage.DeltaVActual,
                    DeltaVASL = stage.DeltaVatASL,
                    DeltaVVac = stage.DeltaVinVac,
                    DryMass = stage.DryMass,
                    EndMass = stage.EndMass,
                    FuelMass = stage.FuelMass,
                    IspASL = stage.IspASL,
                    IspActual = stage.IspActual,
                    IspVac = stage.IspVac,
                    SeparationIndex = stage.SeparationIndex,
                    Stage = stage.Stage,
                    StageBurnTime = stage.StageBurnTime,
                    StageMass = stage.StageMass,
                    StartMass = stage.StartMass,
                    TWRASL = stage.TWRASL,
                    TWRActual = stage.TWRActual,
                    TWRVac = stage.TWRVac,
                    ThrustASL = stage.ThrustASL,
                    ThrustActual = stage.ThrustActual,
                    ThrustVac = stage.ThrustVac,
                    TotalExhaustVelocityASL = stage.TotalExhaustVelocityASL,
                    TotalExhaustVelocityActual = stage.TotalExhaustVelocityActual,
                    TotalExhaustVelocityVAC = stage.TotalExhaustVelocityVAC,
                    DeltaVStageInfo = stage
                });
            }
        }

        public override string ValueDisplay
        {
            get
            {
                return "-";
            }
        }

        /// <summary>
        /// Adds a new string to the CelestialBodyForStage list that corresponds to the HomeWorld, i.e. Kerbin
        /// </summary>
        /// <param name="celestialBodies"></param>
        internal void AddNewCelestialBody(MicroCelestialBodies celestialBodies)
        {
            this.CelestialBodyForStage.Add(celestialBodies.Bodies.Find(b => b.IsHomeWorld).Name);            
        }
    }

    /// <summary>
    /// Parameters for one stage
    /// </summary>
    internal class DeltaVStageInfo_OAB
    {
        internal double DeltaVActual;
        internal double DeltaVASL;
        internal double DeltaVVac;
        internal double DryMass;
        internal double EndMass;
        internal double FuelMass;
        internal double IspASL;
        internal double IspActual;
        internal double IspVac;
        internal int SeparationIndex;
        internal int Stage;
        internal double StageBurnTime;
        internal double StageMass;
        internal double StartMass;
        internal float TWRASL;
        internal float TWRActual;
        internal float TWRVac;
        internal float ThrustASL;
        internal float ThrustActual;
        internal float ThrustVac;
        internal float TotalExhaustVelocityASL;
        internal float TotalExhaustVelocityActual;
        internal float TotalExhaustVelocityVAC;
        internal DeltaVStageInfo DeltaVStageInfo;

        private float GetThrustAtAltitude(double altitude, CelestialBodyComponent cel) => this.DeltaVStageInfo.EnginesActiveInStage?.Select(e => e.Engine.MaxThrustOutputAtm(atmPressure: cel.GetPressure(altitude) / 101.325))?.Sum() ?? 0;
        private double GetISPAtAltitude(double altitude, CelestialBodyComponent cel)
        {
            float sum = 0;
            foreach (DeltaVEngineInfo engInfo in this.DeltaVStageInfo.EnginesActiveInStage)
                sum += engInfo.Engine.MaxThrustOutputAtm(atmPressure: cel.GetPressure(altitude) / 101.325) /
                 engInfo.Engine.currentEngineModeData.atmosphereCurve.Evaluate((float)cel.GetPressure(altitude) / 101.325f);
            return GetThrustAtAltitude(altitude, cel) / sum;
        }
        private double GetDeltaVelAlt(double altitude, CelestialBodyComponent cel) => GetISPAtAltitude(altitude, cel) * 9.80665 * Math.Log(this.DeltaVStageInfo.StartMass / this.DeltaVStageInfo.EndMass);
        private double GetTWRAtAltitude(double altitude, CelestialBodyComponent cel) => this.DeltaVStageInfo.TWRVac * (GetThrustAtAltitude(altitude, cel) / this.DeltaVStageInfo.ThrustVac);
        internal double GetTWRAtSeaLevel(CelestialBodyComponent cel) => this.GetTWRAtAltitude(0, cel);
        internal double GetDeltaVelAtSeaLevel(CelestialBodyComponent cel) => GetDeltaVelAlt(0, cel);
    }
    #endregion


    #region New entries

    public class AltitudeFromScenery : MicroEntry
    {
        public AltitudeFromScenery()
        {
            Name = "Altitude (Scenery)";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.AltitudeFromScenery;
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

    public class AtmosphericTemperature : MicroEntry
    {
        public AtmosphericTemperature()
        {
            Name = "AtmosphericTemperature";
            Description = "";
            Category = MicroEntryCategory.Misc;
            Unit = "K";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.AtmosphericTemperature;
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

    public class ExternalTemperature : MicroEntry
    {
        public ExternalTemperature()
        {
            Name = "ExternalTemperature";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "K";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.ExternalTemperature;
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

    public class DynamicPressure_kPa : MicroEntry
    {
        public DynamicPressure_kPa()
        {
            Name = "DynamicPressure_kPa";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "kPa";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.DynamicPressure_kPa;
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

    public class ExposedArea : MicroEntry
    {
        public ExposedArea()
        {
            Name = "ExposedArea";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.ExposedArea;
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

    

    public class FuelPercentage : MicroEntry
    {
        public FuelPercentage()
        {
            Name = "FuelPercentage";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "%";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.FuelPercentage;
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

    public class Heading : MicroEntry
    {
        public Heading()
        {
            Name = "Heading";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Heading;
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

    public class Pitch_HorizonRelative : MicroEntry
    {
        public Pitch_HorizonRelative()
        {
            Name = "Pitch_HorizonRelative";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Pitch_HorizonRelative;
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

    public class Roll_HorizonRelative : MicroEntry
    {
        public Roll_HorizonRelative()
        {
            Name = "Roll_HorizonRelative";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Roll_HorizonRelative;
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

    public class Yaw_HorizonRelative : MicroEntry
    {
        public Yaw_HorizonRelative()
        {
            Name = "Yaw_HorizonRelative";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Yaw_HorizonRelative;
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

    public class DragCoefficient : MicroEntry
    {
        public DragCoefficient()
        {
            Name = "DragCoefficient";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N5}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.DragCoefficient;
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

    public class OrbitalSpeed : MicroEntry
    {
        public OrbitalSpeed()
        {
            Name = "Orbital Velocity";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.OrbitalSpeed;
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

    public class SoundSpeed : MicroEntry
    {
        public SoundSpeed()
        {
            Name = "SoundSpeed";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.SoundSpeed;
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

    public class StageFuelPercentage : MicroEntry
    {
        public StageFuelPercentage()
        {
            Name = "StageFuelPercentage";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.StageFuelPercentage;
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

    public class StaticPressure_kPa : MicroEntry
    {
        public StaticPressure_kPa()
        {
            Name = "StaticPressure_kPa";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.StaticPressure_kPa;
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

    public class TimeSinceLaunch : MicroEntry
    {
        public TimeSinceLaunch()
        {
            Name = "TimeSinceLaunch";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TimeSinceLaunch;
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

    public class TotalCommandCrewCapacity : MicroEntry
    {
        public TotalCommandCrewCapacity()
        {
            Name = "TotalCommandCrewCapacity";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TotalCommandCrewCapacity;
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

    public class Zenith : MicroEntry
    {
        public Zenith()
        {
            Name = "Zenith";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Zenith;
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

    public class altimeterMode : MicroEntry
    {
        public altimeterMode()
        {
            Name = "altimeterMode";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.altimeterMode;
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

    public class geeForce : MicroEntry
    {
        public geeForce()
        {
            Name = "geeForce";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.geeForce;
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

    public class gravityForPos : MicroEntry
    {
        public gravityForPos()
        {
            Name = "gravityForPos";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.gravityForPos.magnitude;
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


    public class launchTime : MicroEntry
    {
        public launchTime()
        {
            Name = "launchTime";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.launchTime;
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

    public class speedMode : MicroEntry
    {
        public speedMode()
        {
            Name = "speedMode";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.speedMode;
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

    public class AutopilotStatus_IsEnabled : MicroEntry
    {
        public AutopilotStatus_IsEnabled()
        {
            Name = "AutopilotStatus_IsEnabled";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.AutopilotStatus.IsEnabled;
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

    public class AutopilotStatus_Mode : MicroEntry
    {
        public AutopilotStatus_Mode()
        {
            Name = "AutopilotStatus_Mode";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.AutopilotStatus.Mode;
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




    public class EccentricAnomaly : MicroEntry
    {
        public EccentricAnomaly()
        {
            Name = "EccentricAnomaly";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
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

    public class EndUT : MicroEntry
    {
        public EndUT()
        {
            Name = "EndUT";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.EndUT;
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

    public class MeanAnomaly : MicroEntry
    {
        public MeanAnomaly()
        {
            Name = "MeanAnomaly";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
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

    public class ObT : MicroEntry
    {
        public ObT()
        {
            Name = "Orbit Time";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
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

    public class ArgumentOfPeriapsis : MicroEntry
    {
        public ArgumentOfPeriapsis()
        {
            Name = "ArgumentOfPeriapsis";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.OrbitalElements.ArgumentOfPeriapsis;
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

    public class LongitudeOfAscendingNode : MicroEntry
    {
        public LongitudeOfAscendingNode()
        {
            Name = "LongitudeOfAscendingNode";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.OrbitalElements.LongitudeOfAscendingNode;
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

    public class SemiMajorAxis : MicroEntry
    {
        public SemiMajorAxis()
        {
            Name = "SemiMajorAxis";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.OrbitalElements.SemiMajorAxis;
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

    public class SemiMinorAxis : MicroEntry
    {
        public SemiMinorAxis()
        {
            Name = "SemiMinorAxis";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.SemiMinorAxis;
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

    public class OrbitalEnergy : MicroEntry
    {
        public OrbitalEnergy()
        {
            Name = "OrbitalEnergy";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.OrbitalEnergy;
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

    public class ReferenceBodyConstants_Radius : MicroEntry
    {
        public ReferenceBodyConstants_Radius()
        {
            Name = "Ref.Bod.Con.Radius";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.ReferenceBodyConstants.Radius;
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

    public class ReferenceBodyConstants_StandardGravitationParameter : MicroEntry
    {
        public ReferenceBodyConstants_StandardGravitationParameter()
        {
            Name = "Ref.Bod.Con.GPar";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.ReferenceBodyConstants.StandardGravitationParameter;
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

    public class SemiLatusRectum : MicroEntry
    {
        public SemiLatusRectum()
        {
            Name = "SemiLatusRectum";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.SemiLatusRectum;
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

    public class StartUT : MicroEntry
    {
        public StartUT()
        {
            Name = "StartUT";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.StartUT;
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

    public class TrueAnomaly : MicroEntry
    {
        public TrueAnomaly()
        {
            Name = "T.Anomaly";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N2}";
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

    public class UniversalTimeAtClosestApproach : MicroEntry
    {
        public UniversalTimeAtClosestApproach()
        {
            Name = "UniversalTime Closest Appr.";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.UniversalTimeAtClosestApproach;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? MicroUtility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class UniversalTimeAtSoiEncounter : MicroEntry
    {
        public UniversalTimeAtSoiEncounter()
        {
            Name = "UniversalTime SOI Enc";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.UniversalTimeAtSoiEncounter;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? MicroUtility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class orbitPercent : MicroEntry
    {
        public orbitPercent()
        {
            Name = "orbitPercent";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "%";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.orbitPercent * 100;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class radius : MicroEntry
    {
        public radius()
        {
            Name = "Orbit Radius";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.radius;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    








    #endregion

}
