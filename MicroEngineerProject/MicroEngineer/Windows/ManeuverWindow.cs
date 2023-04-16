using KSP.Game;
using KSP.Messages;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;
using UnityEngine;

namespace MicroMod
{
    internal class ManeuverWindow : BaseWindow
    {
        private int _selectedNodeIndex = 0;
        private List<ManeuverNodeData> _nodes = new();

        override internal void DrawWindowHeader()
        {
            if (_nodes == null || _nodes.Count <= 1)
                return;

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("<", Styles.NormalBtnStyle) && _selectedNodeIndex > 0)
                _selectedNodeIndex--;

            GUILayout.Label($"Node #{_selectedNodeIndex + 1}", Styles.TableHeaderCenteredLabelStyle);

            if (GUILayout.Button(">", Styles.NormalBtnStyle) && _selectedNodeIndex + 1 < _nodes.Count)
                _selectedNodeIndex++;

            GUILayout.EndHorizontal();
        }

        override internal void DrawWindowFooter()
        {
            if (_nodes == null || _nodes.Count == 0)
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

            var nodeToDelete = activeVesselPlan.GetNodes()[_selectedNodeIndex];
            nodeData.Add(nodeToDelete);

            foreach (ManeuverNodeData node in activeVesselPlan.GetNodes())
            {
                if (!nodeData.Contains(node) && (!nodeToDelete.IsOnManeuverTrajectory || nodeToDelete.Time < node.Time))
                    nodeData.Add(node);
            }
            GameManager.Instance.Game.SpaceSimulation.Maneuvers.RemoveNodesFromVessel(MicroUtility.ActiveVessel.GlobalId, nodeData);
            _selectedNodeIndex = 0;
        }

        internal override void RefreshData()
        {
            base.RefreshData();
            RefreshManeuverNodes();

            // Add _selectedNodeIndex to base entries as well. They will show the correct node's info.
            (Entries.Find(e => e.GetType() == typeof(ProjectedAp)) as ProjectedAp).SelectedNodeIndex = _selectedNodeIndex;
            (Entries.Find(e => e.GetType() == typeof(ProjectedPe)) as ProjectedPe).SelectedNodeIndex = _selectedNodeIndex;
            (Entries.Find(e => e.GetType() == typeof(DeltaVRequired)) as DeltaVRequired).SelectedNodeIndex = _selectedNodeIndex;
            (Entries.Find(e => e.GetType() == typeof(TimeToNode)) as TimeToNode).SelectedNodeIndex = _selectedNodeIndex;
            (Entries.Find(e => e.GetType() == typeof(BurnTime)) as BurnTime).SelectedNodeIndex = _selectedNodeIndex;
        }

        private void RefreshManeuverNodes()
        {
            //MicroUtility.RefreshActiveVesselAndCurrentManeuver(); -> check if we need this here

            ManeuverPlanComponent activeVesselPlan = MicroUtility.ActiveVessel.SimulationObject.FindComponent<ManeuverPlanComponent>();
            if (activeVesselPlan != null)
            {
                _nodes = activeVesselPlan.GetNodes();
            }
        }

        internal void OnManeuverCreatedMessage(MessageCenterMessage message)
        {
            var nodeData = MicroUtility.ActiveVessel.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            _selectedNodeIndex = nodeData != null ? nodeData.Count > 0 ? nodeData.Count - 1 : 0 : 0;
        }
    }
}
