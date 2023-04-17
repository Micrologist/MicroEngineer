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
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .ApoapsisArl;
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

    public class ProjectedPe : ManeuverEntry
    {
        public ProjectedPe()
        {
            Name = "Projected Pe.";
            Description = "Shows the projected periapsis vessel will have after completing the maneuver.";
            Category = MicroEntryCategory.Maneuver;
            Unit = "m";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?
                .Where(p => p.ActivePatch == true)
                .ElementAtOrDefault(base.SelectedNodeIndex)?
                .PeriapsisArl;
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

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(base.Formatting, EntryValue);
            }
        }
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

                return MicroUtility.SecondsToTimeString((double)EntryValue);
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

                return MicroUtility.SecondsToTimeString((double)EntryValue);
            }
        }
    }
}
