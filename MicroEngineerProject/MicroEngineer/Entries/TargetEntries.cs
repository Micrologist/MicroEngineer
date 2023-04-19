using KSP.Sim.impl;
using UnityEngine;

namespace MicroMod
{
    public class TargetEntry : MicroEntry
    { }

    public class TargetApoapsis : TargetEntry
    {
        public TargetApoapsis()
        {
            Name = "Target Ap.";
            Description = "Shows the target's apoapsis height relative to the sea level.";
            Category = MicroEntryCategory.Target;
            Unit = "m";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.ApoapsisArl;
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

    public class TargetPeriapsis : TargetEntry
    {
        public TargetPeriapsis()
        {
            Name = "Target Pe.";
            Description = "Shows the target's periapsis height relative to the sea level.";
            Category = MicroEntryCategory.Target;
            Unit = "m";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.PeriapsisArl;
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

    public class DistanceToTarget : TargetEntry
    {
        public DistanceToTarget()
        {
            Name = "Distance to Target";
            Description = "Shows the current distance between the vessel and the target.";
            Category = MicroEntryCategory.Target;
            Unit = "m";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit != null ? (MicroUtility.ActiveVessel.Orbit.Position - MicroUtility.ActiveVessel.TargetObject.Orbit.Position).magnitude : null;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                // return value only if vessel and target are in the same SOI
                return MicroUtility.ActiveVessel.Orbit.referenceBody == MicroUtility.ActiveVessel.TargetObject.Orbit.referenceBody ?
                    MicroUtility.MetersToDistanceString((double)EntryValue) : "-";
            }
        }
    }

    public class RelativeSpeed : TargetEntry
    {
        public RelativeSpeed()
        {
            Name = "Rel. Speed";
            Description = "Shows the relative velocity between the vessel and the target.";
            Category = MicroEntryCategory.Target;
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit != null ? (MicroUtility.ActiveVessel.Orbit.relativeVelocity - MicroUtility.ActiveVessel.TargetObject.Orbit.relativeVelocity).magnitude : null;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                // return value only if vessel and target are in the same SOI
                if (MicroUtility.ActiveVessel.Orbit.referenceBody != MicroUtility.ActiveVessel.TargetObject.Orbit.referenceBody)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(base.Formatting, EntryValue);
            }
        }
    }

    public class RelativeInclination : TargetEntry
    {
        public RelativeInclination()
        {
            Name = "Rel. Inclination";
            Description = "Shows the relative inclination between the vessel and the target.";
            Category = MicroEntryCategory.Target;
            Unit = "°";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbiter?.OrbitTargeter?.AscendingNodeTarget.Inclination;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                // return value only if vessel and target are in the same SOI
                if (MicroUtility.ActiveVessel.Orbit.referenceBody != MicroUtility.ActiveVessel.TargetObject?.Orbit?.referenceBody)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(base.Formatting, EntryValue);
            }
        }
    }





    ////////////////////////        NEW     ENTRIES         ////////////////////////

    public class Target_AltitudeFromSeaLevel : TargetEntry
    {
        public Target_AltitudeFromSeaLevel()
        {
            Name = "[T] Altitude (Sea)";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.AltitudeFromSeaLevel;
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

    public class Target_Name : TargetEntry
    {
        public Target_Name()
        {
            Name = "[T] Name";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.DisplayName;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_EccentricAnomaly : TargetEntry
    {
        public Target_EccentricAnomaly()
        {
            Name = "[T] Eccentric Anomaly";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.EccentricAnomaly;
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

    public class Target_MeanAnomaly : TargetEntry
    {
        public Target_MeanAnomaly()
        {
            Name = "[T] Mean Anomaly";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.MeanAnomaly;
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

    public class Target_ObT : TargetEntry
    {
        public Target_ObT()
        {
            Name = "[T] Orbit Time";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "s";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.ObT;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, MicroUtility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class Target_ArgumentOfPeriapsis : TargetEntry
    {
        public Target_ArgumentOfPeriapsis()
        {
            Name = "[T] Arg. of Pe.";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.argumentOfPeriapsis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_Eccentricity : TargetEntry
    {
        public Target_Eccentricity()
        {
            Name = "[T] Eccentricity";
            Description = "Shows the target's orbital eccentricity which is a measure of how much an elliptical orbit is 'squashed'.";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.eccentricity;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_Inclination : TargetEntry
    {
        public Target_Inclination()
        {
            Name = "[T] Inclination";
            Description = "Shows the target's orbital inclination relative to the equator.";
            Category = MicroEntryCategory.Accepted;
            Unit = "°";
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.inclination;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_LongitudeOfAscendingNode : TargetEntry
    {
        public Target_LongitudeOfAscendingNode()
        {
            Name = "[T] LAN";
            Description = "Shows the target's Longitude Of Ascending Node";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.longitudeOfAscendingNode;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_SemiMajorAxis : TargetEntry
    {
        public Target_SemiMajorAxis()
        {
            Name = "[T] Semi Major Axis";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.semiMajorAxis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_ReferenceBodyConstants_Radius : TargetEntry
    {
        public Target_ReferenceBodyConstants_Radius()
        {
            Name = "[T] Ref.Bod.Con.Radius";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.referenceBody?.radius;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_ReferenceBodyConstants_StandardGravitationParameter : TargetEntry
    {
        public Target_ReferenceBodyConstants_StandardGravitationParameter()
        {
            Name = "[T] Ref.Bod.Con.GPar";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.referenceBody.gravParameter;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_SemiLatusRectum : TargetEntry
    {
        public Target_SemiLatusRectum()
        {
            Name = "[T] Semi Latus Rectum";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.SemiLatusRectum;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_SemiMinorAxis : TargetEntry
    {
        public Target_SemiMinorAxis()
        {
            Name = "[T] Semi Minor Axis";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N3}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.SemiMinorAxis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_TrueAnomaly : TargetEntry
    {
        public Target_TrueAnomaly()
        {
            Name = "[T] True Anomaly";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = null;
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.TrueAnomaly;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, MicroUtility.RadiansToDegrees((double)EntryValue));
            }
        }
    }

    public class Target_Period : TargetEntry
    {
        public Target_Period()
        {
            Name = "[T] Period";
            Description = "Shows the amount of time it will take to complete a full orbit.";
            Category = MicroEntryCategory.Accepted;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.period;
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

    public class Target_radius : TargetEntry
    {
        public Target_radius()
        {
            Name = "[T] Orbit Radius";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit.radius;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_Obtvelocity : TargetEntry
    {
        public Target_Obtvelocity()
        {
            Name = "[T] Orbital Speed";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.ObtVelocity.magnitude;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }


    public class DistanceAtCloseApproach1 : TargetEntry
    {
        public DistanceAtCloseApproach1()
        {
            Name = "CloseApp1 Dist";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "m";
            Formatting = null;
        }

        public override void RefreshData()
        {
            bool? isValid = MicroUtility.ActiveVessel.Orbiter?.OrbitTargeter?.Intersect1Target?.IsValid;

            EntryValue = isValid != null && isValid == true ? MicroUtility.ActiveVessel.Orbiter.OrbitTargeter.Intersect1Target.RelativeDistance : null;
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

    public class TimeToCloseApproach1 : TargetEntry
    {
        public TimeToCloseApproach1()
        {
            Name = "CloseApp1 Time";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            bool? isValid = MicroUtility.ActiveVessel.Orbiter?.OrbitTargeter?.Intersect1Target?.IsValid;

            EntryValue = isValid != null && isValid == true ? EntryValue = MicroUtility.ActiveVessel.Orbiter.OrbitTargeter.Intersect1Target.UniversalTime - MicroUtility.UniversalTime : null;
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

    public class RelativeSpeedAtCloseApproach1 : TargetEntry
    {
        public RelativeSpeedAtCloseApproach1()
        {
            Name = "CloseApp1 Speed";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            bool? isValid = MicroUtility.ActiveVessel.Orbiter?.OrbitTargeter?.Intersect1Target?.IsValid;

            EntryValue = isValid != null && isValid == true ? EntryValue = MicroUtility.ActiveVessel.Orbiter.OrbitTargeter.Intersect1Target.RelativeSpeed : null;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class DistanceAtCloseApproach2 : TargetEntry
    {
        public DistanceAtCloseApproach2()
        {
            Name = "CloseApp2 Dist";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "m";
            Formatting = null;
        }

        public override void RefreshData()
        {
            bool? isValid = MicroUtility.ActiveVessel.Orbiter?.OrbitTargeter?.Intersect2Target?.IsValid;

            EntryValue = isValid != null && isValid == true ? EntryValue = MicroUtility.ActiveVessel.Orbiter.OrbitTargeter.Intersect2Target.RelativeDistance : null;
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

    public class TimeToCloseApproach2 : TargetEntry
    {
        public TimeToCloseApproach2()
        {
            Name = "CloseApp2 Time";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            bool? isValid = MicroUtility.ActiveVessel.Orbiter?.OrbitTargeter?.Intersect2Target?.IsValid;

            EntryValue = isValid != null && isValid == true ? EntryValue = MicroUtility.ActiveVessel.Orbiter.OrbitTargeter.Intersect2Target.UniversalTime - MicroUtility.UniversalTime : null;
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

    public class RelativeSpeedAtCloseApproach2 : TargetEntry
    {
        public RelativeSpeedAtCloseApproach2()
        {
            Name = "CloseApp2 Speed";
            Description = "";
            Category = MicroEntryCategory.Accepted;
            Unit = "m/s";
            Formatting = "{0:N1}";
        }

        public override void RefreshData()
        {
            bool? isValid = MicroUtility.ActiveVessel.Orbiter?.OrbitTargeter?.Intersect2Target?.IsValid;

            EntryValue = isValid != null && isValid == true ? EntryValue = MicroUtility.ActiveVessel.Orbiter.OrbitTargeter.Intersect2Target.RelativeSpeed : null;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }    

    public class PhaseAngle : TargetEntry
    {
        public PhaseAngle()
        {
            Name = "Phase angle";
            Description = "";
            Category = MicroEntryCategory.Accepted2;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        private double? GetPhaseAngle()
        {
            // If Target is the body that vessel is orbiting, there is no phase angle
            if (MicroUtility.ActiveVessel.Orbit.referenceBody == MicroUtility.ActiveVessel.TargetObject.CelestialBody)
                return null;

            (CelestialBodyComponent referenceBody, Vector3 localPosition) from = (MicroUtility.ActiveVessel.Orbit.referenceBody, MicroUtility.ActiveVessel.Orbit.Position.localPosition);
            (CelestialBodyComponent referenceBody, Vector3 localPosition) to = (MicroUtility.ActiveVessel.TargetObject.Orbit.referenceBody, MicroUtility.ActiveVessel.TargetObject.Orbit.Position.localPosition);

            // We search for the common celestial body that both ActiveVessel and TargetObject are orbiting and then calculate the phase angle
            bool commonReferenceBodyFound = false;
            
            // Set a limit for common reference body lookups. There should always be a common reference body so this shouldn't be needed, but just to be safe.
            int numberOfLoopTries = 3;

            // Outer loop => TargetObject (to)
            for (int i = 0; i < numberOfLoopTries; i++)
            {
                from.referenceBody = MicroUtility.ActiveVessel.Orbit.referenceBody;
                from.localPosition = MicroUtility.ActiveVessel.Orbit.Position.localPosition;

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
                    from.referenceBody = from.referenceBody.referenceBody;
                }                

                if (commonReferenceBodyFound)
                    break;

                if (to.referenceBody.Orbit == null)
                    break;

                // Set the reference body one level up
                to.localPosition = to.referenceBody.Position.localPosition + to.localPosition;
                to.referenceBody = to.referenceBody.referenceBody;
            }            

            if (commonReferenceBodyFound)
            {
                double phase = Vector3d.SignedAngle(to.localPosition, from.localPosition, Vector3d.up);
                return Math.Round(phase, 1);
            }
            else
                return null;
        }

        public override void RefreshData()
        {
            if (MicroUtility.TargetExists())
            {
                EntryValue = this.GetPhaseAngle();
            }            
        }

        public override string ValueDisplay => base.ValueDisplay;
    }



}
