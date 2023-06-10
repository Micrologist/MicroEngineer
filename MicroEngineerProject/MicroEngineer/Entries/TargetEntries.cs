using KSP.Sim.impl;

namespace MicroMod
{
    public class TargetEntry : BaseEntry
    { }

    public class Target_Name : TargetEntry
    {
        public Target_Name()
        {
            Name = "Name";
            Description = "Target's name.";
            Category = MicroEntryCategory.Target;
            IsDefault = true;
            BaseUnit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.DisplayName;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TargetApoapsis : TargetEntry
    {
        public TargetApoapsis()
        {
            Name = "Target Ap.";
            Description = "Shows the target's apoapsis height relative to the sea level.";
            Category = MicroEntryCategory.Target;
            IsDefault = true;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.ApoapsisArl;
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
            IsDefault = true;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.PeriapsisArl;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class DistanceToTarget : TargetEntry
    {
        public DistanceToTarget()
        {
            Name = "Distance to Tgt";
            Description = "Shows the current distance between the vessel and the target.";
            Category = MicroEntryCategory.Target;
            IsDefault = true;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit != null ? (Utility.ActiveVessel.Orbit.Position - Utility.ActiveVessel.TargetObject.Orbit.Position).magnitude : null;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                // return value only if vessel and target are in the same SOI
                return Utility.ActiveVessel.Orbit.referenceBody == Utility.ActiveVessel.TargetObject.Orbit.referenceBody ?
                    String.IsNullOrEmpty(this.Formatting) ? EntryValue.ToString() : String.Format(Formatting, EntryValue) : "-";
            }
        }
    }

    public class RelativeSpeed : TargetEntry
    {
        public RelativeSpeed()
        {
            Name = "Rel. speed";
            Description = "Shows the relative velocity between the vessel and the target.";
            Category = MicroEntryCategory.Target;
            IsDefault = true;
            MiliUnit = "mm/s";
            BaseUnit = "m/s";
            KiloUnit = "km/s";
            MegaUnit = "Mm/s";
            GigaUnit = "Gm/s";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit != null ? (Utility.ActiveVessel.Orbit.relativeVelocity - Utility.ActiveVessel.TargetObject.Orbit.relativeVelocity).magnitude : null;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                // return value only if vessel and target are in the same SOI
                if (Utility.ActiveVessel.Orbit.referenceBody != Utility.ActiveVessel.TargetObject.Orbit.referenceBody)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(base.Formatting, EntryValue);
            }
        }
    }

    public class RelativeInclination : TargetEntry
    {
        public RelativeInclination()
        {
            Name = "Rel. inclination";
            Description = "Shows the relative inclination between the vessel and the target.";
            Category = MicroEntryCategory.Target;
            IsDefault = true;
            BaseUnit = "°";
            NumberOfDecimalDigits = 3;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbiter?.OrbitTargeter?.AscendingNodeTarget.Inclination;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                // return value only if vessel and target are in the same SOI
                if (Utility.ActiveVessel.Orbit.referenceBody != Utility.ActiveVessel.TargetObject?.Orbit?.referenceBody)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(base.Formatting, EntryValue);
            }
        }
    }

    public class Target_AltitudeFromSeaLevel : TargetEntry
    {
        public Target_AltitudeFromSeaLevel()
        {
            Name = "Altitude (sea)";
            Description = "Shows the target's altitude above sea level.";
            Category = MicroEntryCategory.Target;
            IsDefault = true;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.AltitudeFromSeaLevel;
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
            IsDefault = true;
            BaseUnit = "°";
            NumberOfDecimalDigits = 3;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.inclination;
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
            IsDefault = false;
            BaseUnit = null;
            NumberOfDecimalDigits = 3;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.eccentricity;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_Period : TargetEntry
    {
        public Target_Period()
        {
            Name = "Period";
            Description = "Shows the amount of time it will take the target to complete a full orbit.";
            Category = MicroEntryCategory.Target;
            IsDefault = true;
            BaseUnit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.period;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? Utility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, Utility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class Target_Obtvelocity : TargetEntry
    {
        public Target_Obtvelocity()
        {
            Name = "Orbital speed";
            Description = "Shows the target's orbital speed.";
            Category = MicroEntryCategory.Target;
            IsDefault = true;
            MiliUnit = "mm/s";
            BaseUnit = "m/s";
            KiloUnit = "km/s";
            MegaUnit = "Mm/s";
            GigaUnit = "Gm/s";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.ObtVelocity.magnitude;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_TrueAnomaly : TargetEntry
    {
        public Target_TrueAnomaly()
        {
            Name = "True anomaly";
            Description = "Angle between the direction of periapsis and the current position of the object, as seen from the main focus of the ellipse.";
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            BaseUnit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.TrueAnomaly;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? EntryValue.ToString() : String.Format(Formatting, Utility.RadiansToDegrees((double)EntryValue));
            }
        }
    }

    public class Target_MeanAnomaly : TargetEntry
    {
        public Target_MeanAnomaly()
        {
            Name = "Mean anomaly";
            Description = "Parameter used to describe the position of the target in its orbit around the celestial body.";
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            BaseUnit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.MeanAnomaly;
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

    public class Target_EccentricAnomaly : TargetEntry
    {
        public Target_EccentricAnomaly()
        {
            Name = "Eccentric anomaly";
            Description = "Angle at the center of the orbital ellipse from the semi major axis to the line that passes through the center of the ellipse and the point on the auxiliary circle that is the intersection of the line perpendicular to the semi major axis and passes through the point in the orbit where the target is.";
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            BaseUnit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.EccentricAnomaly;
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

    public class Target_LongitudeOfAscendingNode : TargetEntry
    {
        public Target_LongitudeOfAscendingNode()
        {
            Name = "LAN Ω";
            Description = "Shows the target's Longitude Of Ascending Node.";
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            BaseUnit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.longitudeOfAscendingNode;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_ArgumentOfPeriapsis : TargetEntry
    {
        public Target_ArgumentOfPeriapsis()
        {
            Name = "Argument of Pe.";
            Description = "Angle from the line of the ascending node on the equatorial plane to the point of periapsis passage.";
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            BaseUnit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.argumentOfPeriapsis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_SemiLatusRectum : TargetEntry
    {
        public Target_SemiLatusRectum()
        {
            Name = "Semi latus rect";
            Description = "Half the length of the chord through one focus, perpendicular to the major axis.";
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.SemiLatusRectum;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_SemiMajorAxis : TargetEntry
    {
        public Target_SemiMajorAxis()
        {
            Name = "Semi major axis";
            Description = "Shows the distance from the center of an orbit to the farthest edge.";
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.semiMajorAxis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_SemiMinorAxis : TargetEntry
    {
        public Target_SemiMinorAxis()
        {
            Name = "Semi minor axis";
            Description = "Shows the distance from the center of an orbit to the nearest edge.";
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.SemiMinorAxis;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_ObT : TargetEntry
    {
        public Target_ObT()
        {
            Name = "Orbit time";
            Description = "Shows orbit time in seconds from the Periapsis.";
            EntryType = EntryType.Time;
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            BaseUnit = "s";
            NumberOfDecimalDigits = 3;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.ObT;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? Utility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, Utility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class Target_ReferenceBodyConstants_Radius : TargetEntry
    {
        public Target_ReferenceBodyConstants_Radius()
        {
            Name = "Body radius";
            Description = "Radius of the body that target is orbiting.";
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.referenceBody?.radius;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_ReferenceBodyConstants_StandardGravitationParameter : TargetEntry
    {
        public Target_ReferenceBodyConstants_StandardGravitationParameter()
        {
            Name = "Std. grav. param.";
            Description = "Product of the gravitational constant G and the mass M of the body target is orbiting.";
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            BaseUnit = "μ";
            NumberOfDecimalDigits = 4;
            Formatting = "e";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.referenceBody?.gravParameter;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class Target_OrbitRadius : TargetEntry
    {
        public Target_OrbitRadius()
        {
            Name = "Orbit radius";
            Description = "Length from the center of the ellipse to the object.";
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.TargetObject?.Orbit?.radius;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class PhaseAngle : TargetEntry
    {
        public PhaseAngle()
        {
            Name = "Phase angle";
            Description = "Angle between your vessel, the reference body and the target. How much \"ahead\" or \"behind\" in phase you are with the target.";
            Category = MicroEntryCategory.Target;
            IsDefault = true;
            BaseUnit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.TargetExists() ? TransferInfo.GetPhaseAngle() : null;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TransferAngle : TargetEntry
    {
        public TransferAngle()
        {
            Name = "Transfer angle";
            Description = "Phase angle needed for an optimal Hohmann transfer orbit. Use a circular orbit for a more accurate value.";
            Category = MicroEntryCategory.Target;
            IsDefault = true;
            BaseUnit = "°";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.TargetExists() ? TransferInfo.GetTransferAngle() : null;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class DistanceAtCloseApproach1 : TargetEntry
    {
        public DistanceAtCloseApproach1()
        {
            Name = "C.A.Dist.1";
            Description = "Close approach distance to target (1).";
            Category = MicroEntryCategory.Target;
            IsDefault = true;
            HideWhenNoData = true;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            bool? isValid = Utility.ActiveVessel.Orbiter?.OrbitTargeter?.Intersect1Target?.IsValid;

            EntryValue = isValid != null && isValid == true ? Utility.ActiveVessel.Orbiter.OrbitTargeter.Intersect1Target.RelativeDistance : null;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TimeToCloseApproach1 : TargetEntry
    {
        public TimeToCloseApproach1()
        {
            Name = "C.A.Time1";
            Description = "Close approach time to target (1).";
            EntryType = EntryType.Time;
            Category = MicroEntryCategory.Target;
            IsDefault = true;
            HideWhenNoData = true;
            BaseUnit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            bool? isValid = Utility.ActiveVessel.Orbiter?.OrbitTargeter?.Intersect1Target?.IsValid;

            EntryValue = isValid != null && isValid == true ? EntryValue = Utility.ActiveVessel.Orbiter.OrbitTargeter.Intersect1Target.UniversalTime - Utility.UniversalTime : null;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? Utility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, Utility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class RelativeSpeedAtCloseApproach1 : TargetEntry
    {
        public RelativeSpeedAtCloseApproach1()
        {
            Name = "C.A.Speed1";
            Description = "Close approach relative speed to target (1).";
            Category = MicroEntryCategory.Target;
            IsDefault = true;
            HideWhenNoData = true;
            MiliUnit = "mm/s";
            BaseUnit = "m/s";
            KiloUnit = "km/s";
            MegaUnit = "Mm/s";
            GigaUnit = "Gm/s";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            bool? isValid = Utility.ActiveVessel.Orbiter?.OrbitTargeter?.Intersect1Target?.IsValid;

            EntryValue = isValid != null && isValid == true ? EntryValue = Utility.ActiveVessel.Orbiter.OrbitTargeter.Intersect1Target.RelativeSpeed : null;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class DistanceAtCloseApproach2 : TargetEntry
    {
        public DistanceAtCloseApproach2()
        {
            Name = "C.A.Dist.2";
            Description = "Close approach distance to target (2).";
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            HideWhenNoData = true;
            MiliUnit = "mm";
            BaseUnit = "m";
            KiloUnit = "km";
            MegaUnit = "Mm";
            GigaUnit = "Gm";
            NumberOfDecimalDigits = 2;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            bool? isValid = Utility.ActiveVessel.Orbiter?.OrbitTargeter?.Intersect2Target?.IsValid;

            EntryValue = isValid != null && isValid == true ? EntryValue = Utility.ActiveVessel.Orbiter.OrbitTargeter.Intersect2Target.RelativeDistance : null;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class TimeToCloseApproach2 : TargetEntry
    {
        public TimeToCloseApproach2()
        {
            Name = "C.A.Time2";
            Description = "Close approach time to target (2).";
            EntryType = EntryType.Time;
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            HideWhenNoData = true;
            BaseUnit = "s";
            Formatting = null;
        }

        public override void RefreshData()
        {
            bool? isValid = Utility.ActiveVessel.Orbiter?.OrbitTargeter?.Intersect2Target?.IsValid;

            EntryValue = isValid != null && isValid == true ? EntryValue = Utility.ActiveVessel.Orbiter.OrbitTargeter.Intersect2Target.UniversalTime - Utility.UniversalTime : null;
        }

        public override string ValueDisplay
        {
            get
            {
                if (EntryValue == null)
                    return "-";

                return String.IsNullOrEmpty(base.Formatting) ? Utility.SecondsToTimeString((double)EntryValue, true, false) : String.Format(Formatting, Utility.SecondsToTimeString((double)EntryValue, true, false));
            }
        }
    }

    public class RelativeSpeedAtCloseApproach2 : TargetEntry
    {
        public RelativeSpeedAtCloseApproach2()
        {
            Name = "C.A.Speed2";
            Description = "Close approach relative speed to target (2).";
            Category = MicroEntryCategory.Target;
            IsDefault = false;
            HideWhenNoData = true;
            MiliUnit = "mm/s";
            BaseUnit = "m/s";
            KiloUnit = "km/s";
            MegaUnit = "Mm/s";
            GigaUnit = "Gm/s";
            NumberOfDecimalDigits = 1;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            bool? isValid = Utility.ActiveVessel.Orbiter?.OrbitTargeter?.Intersect2Target?.IsValid;

            EntryValue = isValid != null && isValid == true ? EntryValue = Utility.ActiveVessel.Orbiter.OrbitTargeter.Intersect2Target.RelativeSpeed : null;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }
}