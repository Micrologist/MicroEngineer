using KSP.Sim.impl;
using KSP.Sim;
using UnityEngine;

namespace MicroMod
{
    /// <summary>
    /// Original code by:
    /// https://github.com/ABritInSpace/TransferCalculator-KSP2
    /// License: https://raw.githubusercontent.com/ABritInSpace/TransferCalculator-KSP2/master/LICENSE.md
    /// </summary>
    public static class TransferInfo
    {
        private static Vector3? _fromLocalPosition;
        private static IKeplerOrbit _fromOrbit;
        private static Vector3? _toLocalPosition;
        private static IKeplerOrbit _toOrbit;

        private static bool CalculateParameters()
        {
            // If Target is the body that vessel is orbiting, there is no phase angle
            if (Utility.ActiveVessel.Orbit.referenceBody == Utility.ActiveVessel.TargetObject.CelestialBody)
            {
                _fromLocalPosition = null;
                _toLocalPosition = null;
                _fromOrbit = null;
                _toOrbit = null;
                return false;
            }

            (CelestialBodyComponent referenceBody, Vector3 localPosition, IKeplerOrbit currentOrbit) from = (Utility.ActiveVessel.Orbit.referenceBody, Utility.ActiveVessel.Orbit.Position.localPosition, Utility.ActiveVessel.Orbit);
            (CelestialBodyComponent referenceBody, Vector3 localPosition, IKeplerOrbit currentOrbit) to = (Utility.ActiveVessel.TargetObject.Orbit.referenceBody, Utility.ActiveVessel.TargetObject.Orbit.Position.localPosition, Utility.ActiveVessel.TargetObject.Orbit);

            // We search for the common celestial body that both ActiveVessel and TargetObject are orbiting and then calculate the phase angle
            bool commonReferenceBodyFound = false;

            // Set a limit for common reference body lookups. There should always be a common reference body so this shouldn't be needed, but just to be safe.
            int numberOfLoopTries = 3;

            // Outer loop => TargetObject (to)
            for (int i = 0; i < numberOfLoopTries; i++)
            {
                from.referenceBody = Utility.ActiveVessel.Orbit.referenceBody;
                from.localPosition = Utility.ActiveVessel.Orbit.Position.localPosition;
                from.currentOrbit = Utility.ActiveVessel.Orbit;

                // Inner lookp => ActiveVessel (from)
                for (int j = 0; j < numberOfLoopTries; j++)
                {
                    if (from.referenceBody == to.referenceBody)
                    {
                        commonReferenceBodyFound = true;
                        break;
                    }

                    // referenceBody.Orbit is null when referenceBody is a star (i.e. Kerbol). Lookup should end here since the star isn't orbiting anything (yet!)
                    if (from.referenceBody.Orbit == null)
                        break;

                    // Set the reference body one level up
                    from.localPosition = from.referenceBody.Position.localPosition + from.localPosition;
                    from.currentOrbit = from.referenceBody.Orbit;
                    from.referenceBody = from.referenceBody.referenceBody;
                }

                if (commonReferenceBodyFound)
                    break;

                if (to.referenceBody.Orbit == null)
                    break;

                // Set the reference body one level up
                to.localPosition = to.referenceBody.Position.localPosition + to.localPosition;
                to.currentOrbit = to.referenceBody.Orbit;
                to.referenceBody = to.referenceBody.referenceBody;
            }

            if (commonReferenceBodyFound)
            {
                _fromLocalPosition = from.localPosition;
                _fromOrbit = from.currentOrbit;
                _toLocalPosition = to.localPosition;
                _toOrbit = to.currentOrbit;
                return true;
            }
            else
            {
                _fromLocalPosition = null;
                _fromOrbit = null;
                _toLocalPosition = null;
                _toOrbit = null;
                return false;
            }
        }

        public static double? GetPhaseAngle()
        {
            if (CalculateParameters())
            {
                double phase = Vector3d.SignedAngle((Vector3d)_toLocalPosition, (Vector3d)_fromLocalPosition, Vector3d.up);
                return Math.Round(phase, 1);
            }
            else
                return null;
        }

        public static double? GetTransferAngle()
        {
            if (CalculateParameters())
            {
                double ellipseA = (_toOrbit.semiMajorAxis + _fromOrbit.semiMajorAxis) / 2;
                double time = Mathf.PI * Mathf.Sqrt((float)((ellipseA) * (ellipseA) * (ellipseA)) / ((float)_toOrbit.referenceBody.Mass * 6.67e-11f));
                double transfer = 180 - ((time / _toOrbit.period) * 360);
                while (transfer < -180) { transfer += 360; }
                return Math.Round(transfer, 1);
            }
            else
                return null;
        }
    }
}