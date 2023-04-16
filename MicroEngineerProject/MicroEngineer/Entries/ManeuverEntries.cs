using KSP.Game;
using KSP.Sim.impl;

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
            List<PatchedConicsOrbit> patchedConicsList = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?.Where(p => p.ActivePatch == true).ToList();

            if (patchedConicsList == null || patchedConicsList.Count() == 0)
            {
                EntryValue = null;
                return;
            }

            if (patchedConicsList.Count >= base.SelectedNodeIndex + 1)
            {
                EntryValue = MicroUtility.ActiveVessel.Orbiter.ManeuverPlanSolver.PatchedConicsList[base.SelectedNodeIndex].ApoapsisArl;
            }
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
            List<PatchedConicsOrbit> patchedConicsList = MicroUtility.ActiveVessel.Orbiter?.ManeuverPlanSolver?.PatchedConicsList?.Where(p => p.ActivePatch == true).ToList();

            if (patchedConicsList == null || patchedConicsList.Count() == 0)
            {
                EntryValue = null;
                return;
            }

            if (patchedConicsList.Count >= base.SelectedNodeIndex + 1)
            {
                EntryValue = MicroUtility.ActiveVessel.Orbiter.ManeuverPlanSolver.PatchedConicsList[base.SelectedNodeIndex].PeriapsisArl;
            }
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
            ManeuverPlanComponent activeVesselPlan = MicroUtility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>();
            var nodes = activeVesselPlan?.GetNodes();

            if (nodes == null || nodes.Count == 0)
            {
                EntryValue = null;
                return;
            }

            if (nodes.Count == 1)
            {
                EntryValue = (MicroUtility.ActiveVessel.Orbiter.ManeuverPlanSolver.GetVelocityAfterFirstManeuver(out double ut).vector - MicroUtility.ActiveVessel.Orbit.GetOrbitalVelocityAtUTZup(ut)).magnitude;
            }
            else if (nodes.Count >= base.SelectedNodeIndex + 1)
            {
                EntryValue = nodes[base.SelectedNodeIndex].BurnRequiredDV;
            }
            else
            {
                EntryValue = null;
                return;
            }
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
            ManeuverPlanComponent activeVesselPlan = MicroUtility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>();
            var nodes = activeVesselPlan?.GetNodes();

            if (nodes == null || nodes.Count == 0)
            {
                EntryValue = null;
                return;
            }

            if (nodes.Count >= base.SelectedNodeIndex + 1)
            {
                EntryValue = nodes[base.SelectedNodeIndex].BurnVector.z;
            }
            else
            {
                EntryValue = null;
                return;
            }

            // DOESN'T WORK
            //EntryValue = MicroUtility.ActiveVessel.Orbiter.ManeuverPlanSolver.GetVelocityAfterFirstManeuver(out double ut).vector.z - MicroUtility.ActiveVessel.Orbit.GetOrbitalVelocityAtUTZup(ut).z;

            //EntryValue = MicroUtility.CurrentManeuver?.BurnVector.z;
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
            ManeuverPlanComponent activeVesselPlan = MicroUtility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>();
            var nodes = activeVesselPlan?.GetNodes();

            if (nodes == null || nodes.Count == 0)
            {
                EntryValue = null;
                return;
            }

            if (nodes.Count >= base.SelectedNodeIndex + 1)
            {
                EntryValue = nodes[base.SelectedNodeIndex].BurnVector.y;
            }
            else
            {
                EntryValue = null;
                return;
            }

            // DOESN'T WORK
            // EntryValue = MicroUtility.ActiveVessel.Orbiter.ManeuverPlanSolver.GetVelocityAfterFirstManeuver(out double ut).vector.y - MicroUtility.ActiveVessel.Orbit.GetOrbitalVelocityAtUTZup(ut).y;

            //EntryValue = MicroUtility.CurrentManeuver?.BurnVector.y;
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
            ManeuverPlanComponent activeVesselPlan = MicroUtility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>();
            var nodes = activeVesselPlan?.GetNodes();

            if (nodes == null || nodes.Count == 0)
            {
                EntryValue = null;
                return;
            }

            if (nodes.Count >= base.SelectedNodeIndex + 1)
            {
                EntryValue = nodes[base.SelectedNodeIndex].BurnVector.x;
            }
            else
            {
                EntryValue = null;
                return;
            }

            // DOESN'T WORK
            // EntryValue = MicroUtility.ActiveVessel.Orbiter.ManeuverPlanSolver.GetVelocityAfterFirstManeuver(out double ut).vector.x - MicroUtility.ActiveVessel.Orbit.GetOrbitalVelocityAtUTZup(ut).x;

            //EntryValue = MicroUtility.CurrentManeuver?.BurnVector.x;
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
            ManeuverPlanComponent activeVesselPlan = MicroUtility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>();
            var nodes = activeVesselPlan?.GetNodes();

            if (nodes == null || nodes.Count == 0)
            {
                EntryValue = null;
                return;
            }

            if (nodes.Count == 1)
            {
                EntryValue = MicroUtility.CurrentManeuver.Time - GameManager.Instance.Game.UniverseModel.UniversalTime;
            }
            else if (nodes.Count >= base.SelectedNodeIndex + 1)
            {
                EntryValue = nodes[base.SelectedNodeIndex].Time - GameManager.Instance.Game.UniverseModel.UniversalTime;
            }
            else
            {
                EntryValue = null;
                return;
            }
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
            ManeuverPlanComponent activeVesselPlan = MicroUtility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>();
            var nodes = activeVesselPlan?.GetNodes();

            if (nodes == null || nodes.Count == 0)
            {
                EntryValue = null;
                return;
            }

            if (nodes.Count == 1)
            {
                EntryValue = MicroUtility.CurrentManeuver?.BurnDuration;
            }
            else if (nodes.Count >= base.SelectedNodeIndex + 1)
            {
                EntryValue = nodes[base.SelectedNodeIndex].BurnDuration;
            }
            else
            {
                EntryValue = null;
                return;
            }
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
