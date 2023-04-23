using KSP.Game;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;

namespace MicroMod
{
    public class ManeuverEntry : BaseEntry
    {
        internal int SelectedNodeIndex = 0;
    }

    public class DeltaVRequired : ManeuverEntry
    {
        public DeltaVRequired()
        {
            Name = "∆v required";
            Description = "Delta velocity needed to complete the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = true;
            Unit = "m/s";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            List<ManeuverNodeData> nodes = Utility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();

            EntryValue = (nodes == null || nodes.Count == 0) ? null :
                         (nodes.Count == 1) ? (Utility.ActiveVessel.Orbiter.ManeuverPlanSolver.GetVelocityAfterFirstManeuver(out double ut).vector - Utility.ActiveVessel.Orbit.GetOrbitalVelocityAtUTZup(ut)).magnitude :
                         (nodes.Count >= base.SelectedNodeIndex + 1) ? nodes[base.SelectedNodeIndex].BurnRequiredDV :
                         null;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ManeuverPrograde : ManeuverEntry
    {
        public ManeuverPrograde()
        {
            Name = "∆v Prograde";
            Description = "Prograde/Retrograde component of the total change in velocity.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "m/s";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            List<ManeuverNodeData> nodes = Utility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            EntryValue = nodes?.ElementAtOrDefault(base.SelectedNodeIndex)?.BurnVector.z;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ManeuverNormal : ManeuverEntry
    {
        public ManeuverNormal()
        {
            Name = "∆v Normal";
            Description = "Normal component of the total change in velocity.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "m/s";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            List<ManeuverNodeData> nodes = Utility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            EntryValue = nodes?.ElementAtOrDefault(base.SelectedNodeIndex)?.BurnVector.y;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ManeuverRadial : ManeuverEntry
    {
        public ManeuverRadial()
        {
            Name = "∆v Radial";
            Description = "Radial component of the total change in velocity.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "m/s";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            List<ManeuverNodeData> nodes = Utility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            EntryValue = nodes?.ElementAtOrDefault(base.SelectedNodeIndex)?.BurnVector.x;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TimeToNode : ManeuverEntry
    {
        public TimeToNode()
        {
            Name = "Time to Node";
            Description = "Time until vessel reaches the maneuver node.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = true;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            List<ManeuverNodeData> nodes = Utility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            EntryValue = nodes?.ElementAtOrDefault(base.SelectedNodeIndex)?.Time - GameManager.Instance.Game.UniverseModel.UniversalTime;
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

    public class BurnTime : ManeuverEntry
    {
        public BurnTime()
        {
            Name = "Burn Time";
            Description = "Length of time needed to complete the maneuver node.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = true;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            List<ManeuverNodeData> nodes = Utility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            EntryValue = nodes?.ElementAtOrDefault(base.SelectedNodeIndex)?.BurnDuration;
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

    public class ProjectedAp : ManeuverEntry
    {
        public ProjectedAp()
        {
            Name = "Projected Ap.";
            Description = "Projected Apoapsis vessel will have after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = true;
            Unit = "m";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .ApoapsisArl;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ProjectedPe : ManeuverEntry
    {
        public ProjectedPe()
        {
            Name = "Projected Pe.";
            Description = "Projected Periapsis vessel will have after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = true;
            Unit = "m";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .PeriapsisArl;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_TimeToAp : ManeuverEntry
    {
        public Maneuver_TimeToAp()
        {
            Name = "Time to Ap.";
            Description = "Shows the Time to Apoapsis vessel will have after reaching the maneuver node.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = true;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .TimeToAp;
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

    public class Maneuver_TimeToPe : ManeuverEntry
    {
        public Maneuver_TimeToPe()
        {
            Name = "Time to Pe.";
            Description = "Shows the Time to Periapsis vessel will have after reaching the maneuver node.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = true;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .TimeToPe;
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

    public class Maneuver_Inclination : ManeuverEntry
    {
        public Maneuver_Inclination()
        {
            Name = "Inclination";
            Description = "The inclination of the vessel's orbit after the burn.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = true;
            Unit = "°";
            NumberOfDecimalDigits = 3;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .OrbitalElements.Inclination;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_Eccentricity : ManeuverEntry
    {
        public Maneuver_Eccentricity()
        {
            Name = "Eccentricity";
            Description = "The eccentricity of the vessel's orbit after the burn.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = true;
            Unit = null;
            NumberOfDecimalDigits = 3;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .OrbitalElements.Eccentricity;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_Period : ManeuverEntry
    {
        public Maneuver_Period()
        {
            Name = "Period";
            Description = "The period of the vessel's orbit after the burn.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = true;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .period;
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

    public class Maneuver_TrueAnomaly : ManeuverEntry
    {
        public Maneuver_TrueAnomaly()
        {
            Name = "True Anomaly";
            Description = "True Anomaly vessel will have after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "°";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .TrueAnomaly;
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

    public class Maneuver_MeanAnomaly : ManeuverEntry
    {
        public Maneuver_MeanAnomaly()
        {
            Name = "Mean Anomaly";
            Description = "Mean Anomaly vessel will have after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .MeanAnomaly;
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

    public class Maneuver_EccentricAnomaly : ManeuverEntry
    {
        public Maneuver_EccentricAnomaly()
        {
            Name = "Eccentric Anomaly";
            Description = "Eccentric Anomaly vessel will have after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .EccentricAnomaly;
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

    public class Maneuver_LongitudeOfAscendingNode : ManeuverEntry
    {
        public Maneuver_LongitudeOfAscendingNode()
        {
            Name = "LAN Ω";
            Description = "Longitude of Ascending Node vessel will have after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .OrbitalElements.LongitudeOfAscendingNode;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_ArgumentOfPeriapsis : ManeuverEntry
    {
        public Maneuver_ArgumentOfPeriapsis()
        {
            Name = "Argument of Pe.";
            Description = "Argument of Periapsis vessel will have after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .OrbitalElements.ArgumentOfPeriapsis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_SemiLatusRectum : ManeuverEntry
    {
        public Maneuver_SemiLatusRectum()
        {
            Name = "Semi Latus Rectum";
            Description = "Semi Latus Rectum vessel will have after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "m";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .SemiLatusRectum;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_SemiMajorAxis : ManeuverEntry
    {
        public Maneuver_SemiMajorAxis()
        {
            Name = "Semi Major Axis";
            Description = "Semi Major Axis vessel will have after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "m";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .OrbitalElements.SemiMajorAxis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_SemiMinorAxis : ManeuverEntry
    {
        public Maneuver_SemiMinorAxis()
        {
            Name = "Semi Minor Axis";
            Description = "Semi Minor Axis vessel will have after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "m";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .SemiMinorAxis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_OrbitalEnergy : ManeuverEntry
    {
        public Maneuver_OrbitalEnergy()
        {
            Name = "Orbital Energy";
            Description = "Orbital Energy vessel will have after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "kJ";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .OrbitalEnergy;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_ObT : ManeuverEntry
    {
        public Maneuver_ObT()
        {
            Name = "Orbit Time";
            Description = "Shows orbit time in seconds from the Periapsis when vessel reaches the maneuver node.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "s";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .ObT;
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

    public class Maneuver_OrbitPercent : ManeuverEntry
    {
        public Maneuver_OrbitPercent()
        {
            Name = "Orbit percent";
            Description = "Orbit percent vessel will have passed after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "%";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .orbitPercent * 100;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_EndUT : ManeuverEntry
    {
        public Maneuver_EndUT()
        {
            Name = "UT";
            Description = "Universal Time when vessel reaches the maneuver node.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = "s";
            NumberOfDecimalDigits = 3;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .EndUT;
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

    public class Maneuver_UniversalTimeAtClosestApproach : ManeuverEntry
    {
        public Maneuver_UniversalTimeAtClosestApproach()
        {
            Name = "UT Close.App.";
            Description = "Universal Time at the point of closest approach.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .UniversalTimeAtClosestApproach;
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

    public class Maneuver_UniversalTimeAtSoiEncounter : ManeuverEntry
    {
        public Maneuver_UniversalTimeAtSoiEncounter()
        {
            Name = "UT SOI Enc.";
            Description = "Universal Time at the point of transition to another Sphere Of Influence.";
            Category = MicroEntryCategory.Maneuver;
            IsDefault = false;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .UniversalTimeAtSoiEncounter;
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
}
