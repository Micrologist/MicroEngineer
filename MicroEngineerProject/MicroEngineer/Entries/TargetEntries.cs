using KSP.Sim.impl;

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
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.ApoapsisArl;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TargetPeriapsis : TargetEntry
    {
        public TargetPeriapsis()
        {
            Name = "Target Pe.";
            Description = "Shows the target's periapsis height relative to the sea level.";
            Category = MicroEntryCategory.Target;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.PeriapsisArl;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class DistanceToTarget : TargetEntry
    {
        public DistanceToTarget()
        {
            Name = "Distance to Target";
            Description = "Shows the current distance between the vessel and the target.";
            Category = MicroEntryCategory.Target;
            Unit = "m";
            Formatting = "{0:N0}";
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
                    String.IsNullOrEmpty(this.Formatting) ? EntryValue.ToString() : String.Format(Formatting, EntryValue) : "-";
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
            Name = "Altitude (Sea)";
            Description = "";
            Category = MicroEntryCategory.Target;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.AltitudeFromSeaLevel;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_Name : TargetEntry
    {
        public Target_Name()
        {
            Name = "Name";
            Description = "";
            Category = MicroEntryCategory.Target;
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
            Name = "Eccentric Anomaly";
            Description = "";
            Category = MicroEntryCategory.Target;
            Unit = "°";
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
            Name = "Mean Anomaly";
            Description = "";
            Category = MicroEntryCategory.Target;
            Unit = "°";
            Formatting = "{0:N2}";
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
            Name = "Orbit Time";
            Description = "";
            Category = MicroEntryCategory.Target;
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
            Name = "Argument of Pe.";
            Description = "";
            Category = MicroEntryCategory.Target;
            Unit = "°";
            Formatting = "{0:N2}";
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
            Name = "Eccentricity";
            Description = "Shows the target's orbital eccentricity which is a measure of how much an elliptical orbit is 'squashed'.";
            Category = MicroEntryCategory.Target;
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
            Name = "Inclination";
            Description = "Shows the target's orbital inclination relative to the equator.";
            Category = MicroEntryCategory.Target;
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
            Name = "LAN Ω";
            Description = "Shows the target's Longitude Of Ascending Node";
            Category = MicroEntryCategory.Target;
            Unit = "°";
            Formatting = "{0:N2}";
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
            Name = "Semi Major Axis";
            Description = "";
            Category = MicroEntryCategory.Target;
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
            Name = "Body Radius";
            Description = "";
            Category = MicroEntryCategory.Target;
            Unit = "m";
            Formatting = "{0:N0}";
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
            Name = "Std. Grav. Param.";
            Description = "";
            Category = MicroEntryCategory.Target;
            Unit = "μ";
            Formatting = "{0:e4}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.referenceBody?.gravParameter;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_SemiLatusRectum : TargetEntry
    {
        public Target_SemiLatusRectum()
        {
            Name = "Semi Latus Rectum";
            Description = "";
            Category = MicroEntryCategory.Target;
            Unit = "m";
            Formatting = "{0:N0}";
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
            Name = "Semi Minor Axis";
            Description = "";
            Category = MicroEntryCategory.Target;
            Unit = "m";
            Formatting = "{0:N0}";
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
            Name = "True Anomaly";
            Description = "";
            Category = MicroEntryCategory.Target;
            Unit = "°";
            Formatting = "{0:N1}";
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
            Name = "Period";
            Description = "Shows the amount of time it will take to complete a full orbit.";
            Category = MicroEntryCategory.Target;
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

    public class Target_OrbitRadius : TargetEntry
    {
        public Target_OrbitRadius()
        {
            Name = "Orbit Radius";
            Description = "";
            Category = MicroEntryCategory.Target;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit?.radius;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_Obtvelocity : TargetEntry
    {
        public Target_Obtvelocity()
        {
            Name = "Orbital Speed";
            Description = "";
            Category = MicroEntryCategory.Target;
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
            Name = "CloseAppDist-1";
            Description = "";
            Category = MicroEntryCategory.Target;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            bool? isValid = MicroUtility.ActiveVessel.Orbiter?.OrbitTargeter?.Intersect1Target?.IsValid;

            EntryValue = isValid != null && isValid == true ? MicroUtility.ActiveVessel.Orbiter.OrbitTargeter.Intersect1Target.RelativeDistance : null;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TimeToCloseApproach1 : TargetEntry
    {
        public TimeToCloseApproach1()
        {
            Name = "CloseAppTime-1";
            Description = "";
            Category = MicroEntryCategory.Target;
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
            Name = "CloseAppSpeed-1";
            Description = "";
            Category = MicroEntryCategory.Target;
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
            Name = "CloseAppDist-2";
            Description = "";
            Category = MicroEntryCategory.Target;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            bool? isValid = MicroUtility.ActiveVessel.Orbiter?.OrbitTargeter?.Intersect2Target?.IsValid;

            EntryValue = isValid != null && isValid == true ? EntryValue = MicroUtility.ActiveVessel.Orbiter.OrbitTargeter.Intersect2Target.RelativeDistance : null;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TimeToCloseApproach2 : TargetEntry
    {
        public TimeToCloseApproach2()
        {
            Name = "CloseAppTime-2";
            Description = "";
            Category = MicroEntryCategory.Target;
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
            Name = "CloseAppSpeed-2";
            Description = "";
            Category = MicroEntryCategory.Target;
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
            Name = "Phase Angle";
            Description = "";
            Category = MicroEntryCategory.Target;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.TargetExists() ? TransferInfo.GetPhaseAngle() : null;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TransferAngle : TargetEntry
    {
        public TransferAngle()
        {
            Name = "Transfer Angle";
            Description = "";
            Category = MicroEntryCategory.Target;
            Unit = "°";
            Formatting = "{0:N2}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.TargetExists() ? TransferInfo.GetTransferAngle() : null;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }
}
