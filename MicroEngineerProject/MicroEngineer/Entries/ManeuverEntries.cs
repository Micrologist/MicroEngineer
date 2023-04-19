using KSP.Game;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;

namespace MicroMod
{
    public class ManeuverEntry : MicroEntry
    {
        internal int SelectedNodeIndex = 0;
    }

    public class ProjectedAp : ManeuverEntry
    {
        public ProjectedAp()
        {
            Name = "Projected Ap.";
            Description = "Shows the projected apoapsis vessel will have after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
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
            Description = "Shows the projected periapsis vessel will have after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .PeriapsisArl;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class DeltaVRequired : ManeuverEntry
    {
        public DeltaVRequired()
        {
            Name = "∆v required";
            Description = "Shows the delta velocity needed to complete the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            List<ManeuverNodeData> nodes = MicroUtility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();

            EntryValue = (nodes == null || nodes.Count == 0) ? null :
                         (nodes.Count == 1) ? (MicroUtility.ActiveVessel.Orbiter.ManeuverPlanSolver.GetVelocityAfterFirstManeuver(out double ut).vector - MicroUtility.ActiveVessel.Orbit.GetOrbitalVelocityAtUTZup(ut)).magnitude :
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
            Description = "";
            Category = MicroEntryCategory.Maneuver;
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            List<ManeuverNodeData> nodes = MicroUtility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            EntryValue = nodes?.ElementAtOrDefault(base.SelectedNodeIndex)?.BurnVector.z;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ManeuverNormal : ManeuverEntry
    {
        public ManeuverNormal()
        {
            Name = "∆v Normal";
            Description = "";
            Category = MicroEntryCategory.Maneuver;
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            List<ManeuverNodeData> nodes = MicroUtility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            EntryValue = nodes?.ElementAtOrDefault(base.SelectedNodeIndex)?.BurnVector.y;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ManeuverRadial : ManeuverEntry
    {
        public ManeuverRadial()
        {
            Name = "∆v Radial";
            Description = "";
            Category = MicroEntryCategory.Maneuver;
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            List<ManeuverNodeData> nodes = MicroUtility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            EntryValue = nodes?.ElementAtOrDefault(base.SelectedNodeIndex)?.BurnVector.x;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TimeToNode : ManeuverEntry
    {
        public TimeToNode()
        {
            Name = "Time to Node";
            Description = "Shows the time until vessel reaches the maneuver node.";
            Category = MicroEntryCategory.Maneuver;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            List<ManeuverNodeData> nodes = MicroUtility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            EntryValue = nodes?.ElementAtOrDefault(base.SelectedNodeIndex)?.Time - GameManager.Instance.Game.UniverseModel.UniversalTime;
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

    public class BurnTime : ManeuverEntry
    {
        public BurnTime()
        {
            Name = "Burn Time";
            Description = "Shows the length of time needed to complete the maneuver node.";
            Category = MicroEntryCategory.Maneuver;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            List<ManeuverNodeData> nodes = MicroUtility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            EntryValue = nodes?.ElementAtOrDefault(base.SelectedNodeIndex)?.BurnDuration;
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


    // NEW ENTRIES

    public class Maneuver_EccentricAnomaly : ManeuverEntry
    {
        public Maneuver_EccentricAnomaly()
        {
            Name = "Eccentric Anomaly";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
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

    public class Maneuver_EndUT : ManeuverEntry
    {
        public Maneuver_EndUT()
        {
            Name = "EndUT";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = "s";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
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

                return String.IsNullOrEmpty(base.Formatting) ? MicroUtility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class Maneuver_MeanAnomaly : ManeuverEntry
    {
        public Maneuver_MeanAnomaly()
        {
            Name = "Mean Anomaly";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
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

    public class Maneuver_ObT : ManeuverEntry
    {
        public Maneuver_ObT()
        {
            Name = "Orbit Time";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
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

                return String.IsNullOrEmpty(base.Formatting) ? MicroUtility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class Maneuver_ArgumentOfPeriapsis : ManeuverEntry
    {
        public Maneuver_ArgumentOfPeriapsis()
        {
            Name = "Argument of Periapsis";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .OrbitalElements.ArgumentOfPeriapsis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_Eccentricity : ManeuverEntry
    {
        public Maneuver_Eccentricity()
        {
            Name = "Eccentricity";
            Description = "Shows the vessel's orbital eccentricity which is a measure of how much an elliptical orbit is 'squashed'.";
            Category = MicroEntryCategory.Accepted2;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .OrbitalElements.Eccentricity;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_Inclination : ManeuverEntry
    {
        public Maneuver_Inclination()
        {
            Name = "Inclination";
            Description = "Shows the vessel's orbital inclination relative to the equator.";
            Category = MicroEntryCategory.Accepted2;
            Unit = "°";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .OrbitalElements.Inclination;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_LongitudeOfAscendingNode : ManeuverEntry
    {
        public Maneuver_LongitudeOfAscendingNode()
        {
            Name = "LAN";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .OrbitalElements.LongitudeOfAscendingNode;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_SemiMajorAxis : ManeuverEntry
    {
        public Maneuver_SemiMajorAxis()
        {
            Name = "Semi Major Axis";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
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
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
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
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .OrbitalEnergy;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_SemiLatusRectum : ManeuverEntry
    {
        public Maneuver_SemiLatusRectum()
        {
            Name = "Semi Latus Rectum ℓ";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .SemiLatusRectum;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_TimeToAp : ManeuverEntry
    {
        public Maneuver_TimeToAp()
        {
            Name = "Time to Ap.";
            Description = "Shows the time until the vessel reaches apoapsis, the highest point of the orbit.";
            Category = MicroEntryCategory.Accepted2;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
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

                return String.IsNullOrEmpty(base.Formatting) ? MicroUtility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class Maneuver_TimeToPe : ManeuverEntry
    {
        public Maneuver_TimeToPe()
        {
            Name = "Time to Pe.";
            Description = "Shows the time until the vessel reaches periapsis, the lowest point of the orbit.";
            Category = MicroEntryCategory.Accepted2;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
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

                return String.IsNullOrEmpty(base.Formatting) ? MicroUtility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class Maneuver_TrueAnomaly : ManeuverEntry
    {
        public Maneuver_TrueAnomaly()
        {
            Name = "True Anomaly";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
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

    public class Maneuver_UniversalTimeAtClosestApproach : ManeuverEntry
    {
        public Maneuver_UniversalTimeAtClosestApproach()
        {
            Name = "UT Closest Appr.";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
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

                return String.IsNullOrEmpty(base.Formatting) ? MicroUtility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class Maneuver_UniversalTimeAtSoiEncounter : ManeuverEntry
    {
        public Maneuver_UniversalTimeAtSoiEncounter()
        {
            Name = "UT SOI Enc";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
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

                return String.IsNullOrEmpty(base.Formatting) ? MicroUtility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class Maneuver_orbitPercent : ManeuverEntry
    {
        public Maneuver_orbitPercent()
        {
            Name = "Orbit percent";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = "%";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .orbitPercent * 100;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Maneuver_period : ManeuverEntry
    {
        public Maneuver_period()
        {
            Name = "Period";
            Description = "Shows the amount of time it will take to complete a full orbit.";
            Category = MicroEntryCategory.Accepted2;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
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

                return String.IsNullOrEmpty(base.Formatting) ? MicroUtility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }


}
