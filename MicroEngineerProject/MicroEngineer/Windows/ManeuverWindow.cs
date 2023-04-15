using KSP.Game;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;
using UnityEngine;

namespace MicroMod
{
    internal class ManeuverWindow : BaseWindow
    {
        internal int SelectedNodeIndex = 0;
        internal List<ManeuverNodeData> Nodes = new();

        override internal void DrawWindowHeader()
        {
            if (Nodes == null || Nodes.Count <= 1)
                return;

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("<", Styles.NormalBtnStyle) && SelectedNodeIndex > 0)
                SelectedNodeIndex--;

            GUILayout.Label($"Node #{SelectedNodeIndex + 1}", Styles.TableHeaderCenteredLabelStyle);

            if (GUILayout.Button(">", Styles.NormalBtnStyle) && SelectedNodeIndex + 1 < Nodes.Count)
                SelectedNodeIndex++;

            GUILayout.EndHorizontal();
        }

        override internal void DrawWindowFooter()
        {
            if (Nodes == null || Nodes.Count == 0)
                return;

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete", Styles.NormalBtnStyle))
                DeleteNodes();

            GUILayout.EndHorizontal();
        }

        private void DeleteNodes()
        {
            var activeVesselPlan = MicroUtility.ActiveVessel.SimulationObject.FindComponent<ManeuverPlanComponent>();
            List<ManeuverNodeData> nodeData = new List<ManeuverNodeData>();

            var nodeToDelete = activeVesselPlan.GetNodes()[SelectedNodeIndex];
            nodeData.Add(nodeToDelete);

            foreach (ManeuverNodeData node in activeVesselPlan.GetNodes())
            {
                if (!nodeData.Contains(node) && (!nodeToDelete.IsOnManeuverTrajectory || nodeToDelete.Time < node.Time))
                    nodeData.Add(node);
            }
            GameManager.Instance.Game.SpaceSimulation.Maneuvers.RemoveNodesFromVessel(MicroUtility.ActiveVessel.GlobalId, nodeData);
            SelectedNodeIndex = 0;
        }

        internal override void RefreshData()
        {
            base.RefreshData();
            RefreshManeuverNodes();

            // Add SelectedNodeIndex to base entries as well. They will show the correct node's info.
            (Entries.Find(e => e.GetType() == typeof(ProjectedAp)) as ProjectedAp).SelectedNodeIndex = SelectedNodeIndex;
            (Entries.Find(e => e.GetType() == typeof(ProjectedPe)) as ProjectedPe).SelectedNodeIndex = SelectedNodeIndex;
            (Entries.Find(e => e.GetType() == typeof(DeltaVRequired)) as DeltaVRequired).SelectedNodeIndex = SelectedNodeIndex;
            (Entries.Find(e => e.GetType() == typeof(TimeToNode)) as TimeToNode).SelectedNodeIndex = SelectedNodeIndex;
            (Entries.Find(e => e.GetType() == typeof(BurnTime)) as BurnTime).SelectedNodeIndex = SelectedNodeIndex;
        }

        internal void RefreshManeuverNodes()
        {
            //MicroUtility.RefreshActiveVesselAndCurrentManeuver(); -> check if we need this here

            ManeuverPlanComponent activeVesselPlan = MicroUtility.ActiveVessel.SimulationObject.FindComponent<ManeuverPlanComponent>();
            if (activeVesselPlan != null)
            {
                Nodes = activeVesselPlan.GetNodes();
            }
        }
    }
}
