using KSP.Sim.impl;
using Newtonsoft.Json;

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

        public override string ValueDisplay => base.ValueDisplay;
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

    #region New entries

    public class AltitudeFromScenery : MicroEntry
    {
        public AltitudeFromScenery()
        {
            Name = "Altitude (Scenery)";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "m";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.AltitudeFromScenery;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class AtmosphericTemperature : MicroEntry
    {
        public AtmosphericTemperature()
        {
            Name = "Static ambient temp.";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "K";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.AtmosphericTemperature;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ExternalTemperature : MicroEntry
    {
        public ExternalTemperature()
        {
            Name = "External temperature";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "K";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.ExternalTemperature;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class DynamicPressure_kPa : MicroEntry
    {
        public DynamicPressure_kPa()
        {
            Name = "Dynamic Pressure";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "kPa";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.DynamicPressure_kPa;
        }

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Heading : MicroEntry
    {
        public Heading()
        {
            Name = "Heading";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Heading;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Pitch_HorizonRelative : MicroEntry
    {
        public Pitch_HorizonRelative()
        {
            Name = "Pitch";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Pitch_HorizonRelative;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Roll_HorizonRelative : MicroEntry
    {
        public Roll_HorizonRelative()
        {
            Name = "Roll";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Roll_HorizonRelative;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Yaw_HorizonRelative : MicroEntry
    {
        public Yaw_HorizonRelative()
        {
            Name = "Yaw";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Yaw_HorizonRelative;
        }

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class SoundSpeed : MicroEntry
    {
        public SoundSpeed()
        {
            Name = "Speed of sound";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "m/s";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.SoundSpeed;
        }

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class StaticPressure_kPa : MicroEntry
    {
        public StaticPressure_kPa()
        {
            Name = "Static Pressure";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "kPa";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.StaticPressure_kPa;
        }

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Zenith : MicroEntry
    {
        public Zenith()
        {
            Name = "Zenith";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Zenith;
        }

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class geeForce : MicroEntry
    {
        public geeForce()
        {
            Name = "G-Force";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "g";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.geeForce;
        }

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class EccentricAnomaly : MicroEntry
    {
        public EccentricAnomaly()
        {
            Name = "Eccentric Anomaly";
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

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class LongitudeOfAscendingNode : MicroEntry
    {
        public LongitudeOfAscendingNode()
        {
            Name = "LAN";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.OrbitalElements.LongitudeOfAscendingNode;
        }

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
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

        public override string ValueDisplay => base.ValueDisplay;
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
            Name = "UT Closest Appr.";
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
            Name = "UT SOI Enc";
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
