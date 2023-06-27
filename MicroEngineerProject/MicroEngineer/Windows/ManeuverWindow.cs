using KSP.Game;
using KSP.Messages;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;
using MicroEngineer.UI;

namespace MicroMod
{
    public class ManeuverWindow : EntryWindow
    {
        private List<ManeuverNodeData> _nodes = new();        

        private int _nodeCount;
        public int NodeCount
        {
            get => _nodeCount;
            set
            {
                if (_nodeCount != value)
                {
                    _nodeCount = value;
                    OnNodeCountChanged?.Invoke();
                }                
            }
        }

        public delegate void NodeCountChanged();
        public event NodeCountChanged OnNodeCountChanged;

        private int _selectedNodeIndex = 0;
        public int SelectedNodeIndex
        {
            get => _selectedNodeIndex;
            set
            {
                if (_selectedNodeIndex != value)
                {
                    _selectedNodeIndex = value;
                    OnSelectedNodeIndexChanged?.Invoke();
                }                
            }
        }

        public delegate void SelectedNodeIndexChanged();
        public event SelectedNodeIndexChanged OnSelectedNodeIndexChanged;

        public int SelectPreviousNode()
        {
            if (SelectedNodeIndex > 0)
                SelectedNodeIndex--;

            return SelectedNodeIndex;
        }

        public int SelectNextNode()
        {
            if (SelectedNodeIndex + 1 < _nodes.Count)
                SelectedNodeIndex++;

            return SelectedNodeIndex;
        }

        public int DeleteNodes()
        {
            var nodes = Utility.ActiveVessel.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            List<ManeuverNodeData> nodesToDelete = new List<ManeuverNodeData>();

            // This should never happen, but better be safe
            if (SelectedNodeIndex + 1 > nodes.Count)
                SelectedNodeIndex = Math.Max(0, nodes.Count - 1);

            var nodeToDelete = nodes[SelectedNodeIndex];
            nodesToDelete.Add(nodeToDelete);

            foreach (ManeuverNodeData node in nodes)
            {
                if (!nodesToDelete.Contains(node) && (!nodeToDelete.IsOnManeuverTrajectory || nodeToDelete.Time < node.Time))
                    nodesToDelete.Add(node);
            }
            GameManager.Instance.Game.SpaceSimulation.Maneuvers.RemoveNodesFromVessel(Utility.ActiveVessel.GlobalId, nodesToDelete);
            SelectedNodeIndex = 0;

            return SelectedNodeIndex;
        }

        public override void RefreshData()
        {
            base.RefreshData();
            RefreshManeuverNodes();

            // Toggle showing/hiding UI window depending on whether a maneuver exists
            FlightSceneController.Instance.ManeuverWindowShown = Utility.ManeuverExists();

            // Add _selectedNodeIndex to entries as well. They will show the correct node's info.
            List<ManeuverEntry> entries = Entries.OfType<ManeuverEntry>().ToList();

            foreach (var entry in entries)
                entry.SelectedNodeIndex = SelectedNodeIndex;
        }

        private void RefreshManeuverNodes()
        {
            ManeuverPlanComponent activeVesselPlan = Utility.ActiveVessel.SimulationObject.FindComponent<ManeuverPlanComponent>();
            if (activeVesselPlan != null)
            {
                _nodes = activeVesselPlan.GetNodes();
                NodeCount = _nodes == null ? 0 : _nodes.Count;
            }
        }

        public void OnManeuverCreatedMessage(MessageCenterMessage message)
        {
            var nodeData = Utility.ActiveVessel?.SimulationObject?.FindComponent<ManeuverPlanComponent>()?.GetNodes();
            SelectedNodeIndex = nodeData != null ? nodeData.Count > 0 ? nodeData.Count - 1 : 0 : 0;
        }

        public void OnManeuverRemovedMessage(MessageCenterMessage message)
        {
            SelectedNodeIndex = 0;
        }
    }
}