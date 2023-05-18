using KSP.Game;
using KSP.Messages;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;
using UnityEngine;

namespace MicroMod
{
    internal class ManeuverWindow : EntryWindow
    {
        private int _selectedNodeIndex = 0;
        private List<ManeuverNodeData> _nodes = new();

        override internal void DrawWindowHeader()
        {
            if (_nodes == null || _nodes.Count <= 1)
                return;

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("<", Styles.NormalBtnStyle) && _selectedNodeIndex > 0)
                _selectedNodeIndex--;

            GUILayout.Label($"Node #{_selectedNodeIndex + 1}", Styles.TableHeaderCenteredLabelStyle);

            if (GUILayout.Button(">", Styles.NormalBtnStyle) && _selectedNodeIndex + 1 < _nodes.Count)
                _selectedNodeIndex++;

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
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
            GUILayout.Space(-10);
        }

        private void DeleteNodes()
        {
            var nodes = Utility.ActiveVessel.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            List<ManeuverNodeData> nodesToDelete = new List<ManeuverNodeData>();

            // This should never happen, but better be safe
            if (_selectedNodeIndex + 1 > nodes.Count)
                _selectedNodeIndex = Math.Max(0, nodes.Count - 1);

            var nodeToDelete = nodes[_selectedNodeIndex];
            nodesToDelete.Add(nodeToDelete);

            foreach (ManeuverNodeData node in nodes)
            {
                if (!nodesToDelete.Contains(node) && (!nodeToDelete.IsOnManeuverTrajectory || nodeToDelete.Time < node.Time))
                    nodesToDelete.Add(node);
            }
            GameManager.Instance.Game.SpaceSimulation.Maneuvers.RemoveNodesFromVessel(Utility.ActiveVessel.GlobalId, nodesToDelete);
            _selectedNodeIndex = 0;
        }

        internal override void RefreshData()
        {
            base.RefreshData();
            RefreshManeuverNodes();

            // Add _selectedNodeIndex to entries as well. They will show the correct node's info.
            List<ManeuverEntry> entries = Entries.OfType<ManeuverEntry>().ToList();

            foreach (var entry in entries)
                entry.SelectedNodeIndex = _selectedNodeIndex;
        }

        private void RefreshManeuverNodes()
        {
            ManeuverPlanComponent activeVesselPlan = Utility.ActiveVessel.SimulationObject.FindComponent<ManeuverPlanComponent>();
            if (activeVesselPlan != null)
            {
                _nodes = activeVesselPlan.GetNodes();
            }
        }

        internal void OnManeuverCreatedMessage(MessageCenterMessage message)
        {
            var nodeData = Utility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            _selectedNodeIndex = nodeData != null ? nodeData.Count > 0 ? nodeData.Count - 1 : 0 : 0;
        }

        internal void OnManeuverRemovedMessage(MessageCenterMessage message)
        {
            _selectedNodeIndex = 0;
        }
    }
}