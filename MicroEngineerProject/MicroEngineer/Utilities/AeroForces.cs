using KSP.Game;
using KSP.Sim.impl;
using KSP.Sim;
using KSP.UI.Flight;
using UnityEngine;

namespace MicroMod
{
    public static class AeroForces
    {
        private static readonly List<Type> liftForces = new()
        {
            PhysicsForceDisplaySystem.MODULE_DRAG_BODY_LIFT_TYPE,
            PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_LIFT_TYPE
        };

        private static readonly List<Type> dragForces = new()
        {
            PhysicsForceDisplaySystem.MODULE_DRAG_DRAG_TYPE,
            PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_DRAG_TYPE
        };

        public static double TotalLift
        {
            get
            {
                double toReturn = 0.0;

                IEnumerable<PartComponent> parts = Utility.ActiveVessel?.SimulationObject?.PartOwner?.Parts;
                if (parts == null || !Utility.ActiveVessel.IsInAtmosphere)
                {
                    return toReturn;
                }

                foreach (PartComponent part in parts)
                {
                    foreach (IForce force in part.SimulationObject.Rigidbody.Forces)
                    {
                        if (liftForces.Contains(force.GetType()))
                        {
                            toReturn += force.RelativeForce.magnitude;
                        }
                    }
                }

                return toReturn;
            }
        }

        public static double TotalDrag
        {
            get
            {
                double toReturn = 0.0;

                IEnumerable<PartComponent> parts = Utility.ActiveVessel?.SimulationObject?.PartOwner?.Parts;
                if (parts == null || !Utility.ActiveVessel.IsInAtmosphere)
                    return toReturn;

                foreach (PartComponent part in parts)
                {
                    foreach (IForce force in part.SimulationObject.Rigidbody.Forces)
                    {
                        if (dragForces.Contains(force.GetType()))
                        {
                            toReturn += force.RelativeForce.magnitude;
                        }
                    }
                }

                return toReturn;
            }
        }

        public static double AngleOfAttack
        {
            get
            {
                double aoe = 0.0;

                ISimulationObjectView simulationViewIfLoaded = GameManager.Instance.Game.ViewController.GetSimulationViewIfLoaded(Utility.ActiveVessel.SimulationObject);
                if (simulationViewIfLoaded != null)
                {
                    Vector3d normalized = GameManager.Instance.Game.UniverseView.PhysicsSpace.VectorToPhysics(Utility.ActiveVessel.SurfaceVelocity).normalized;
                    Vector up = simulationViewIfLoaded.Model.Vessel.ControlTransform.up;
                    Vector3 lhs = GameManager.Instance.Game.UniverseView.PhysicsSpace.VectorToPhysics(up);
                    Vector right = simulationViewIfLoaded.Model.Vessel.ControlTransform.right;
                    Vector3 rhs = GameManager.Instance.Game.UniverseView.PhysicsSpace.VectorToPhysics(right);
                    Vector3 lhs2 = normalized;
                    Vector3 normalized2 = Vector3.Cross(lhs2, rhs).normalized;
                    Vector3 rhs2 = Vector3.Cross(lhs2, normalized2);
                    aoe = Vector3.Dot(lhs, normalized2);
                    aoe = Math.Asin(aoe) * 57.295780181884766;
                    if (double.IsNaN(aoe))
                    {
                        aoe = 0.0;
                    }
                }

                return aoe;
            }
        }

        public static double SideSlip
        {
            get
            {
                double sideSlip = 0.0;

                ISimulationObjectView simulationViewIfLoaded = GameManager.Instance.Game.ViewController.GetSimulationViewIfLoaded(Utility.ActiveVessel.SimulationObject);
                if (simulationViewIfLoaded != null)
                {
                    Vector3d normalized = GameManager.Instance.Game.UniverseView.PhysicsSpace.VectorToPhysics(Utility.ActiveVessel.SurfaceVelocity).normalized;
                    Vector up = simulationViewIfLoaded.Model.Vessel.ControlTransform.up;
                    Vector3 lhs = GameManager.Instance.Game.UniverseView.PhysicsSpace.VectorToPhysics(up);
                    Vector right = simulationViewIfLoaded.Model.Vessel.ControlTransform.right;
                    Vector3 rhs = GameManager.Instance.Game.UniverseView.PhysicsSpace.VectorToPhysics(right);
                    Vector3 lhs2 = normalized;
                    Vector3 normalized2 = Vector3.Cross(lhs2, rhs).normalized;
                    Vector3 rhs2 = Vector3.Cross(lhs2, normalized2);

                    sideSlip = Vector3.Dot(lhs, rhs2);
                    sideSlip = Math.Asin(sideSlip) * 57.295780181884766;
                    if (double.IsNaN(sideSlip))
                    {
                        sideSlip = 0.0;
                    }
                }

                return sideSlip;
            }
        }
    }
}
