namespace MicroMod
{
    public class BodyEntry : BaseEntry
    { }

    public class Body : BodyEntry
    {
        public Body()
        {
            Name = "Body";
            Description = "Shows the body that vessel is currently at.";
            Category = MicroEntryCategory.Body;
            IsDefault = false;
            BaseUnit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.mainBody.bodyName;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ReferenceBodyConstants_Radius : BodyEntry
    {
        public ReferenceBodyConstants_Radius()
        {
            Name = "Body radius";
            Description = "Body's radius.";
            Category = MicroEntryCategory.Body;
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
            EntryValue = Utility.ActiveVessel.Orbit.ReferenceBodyConstants.Radius;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ReferenceBodyConstants_StandardGravitationParameter : BodyEntry
    {
        public ReferenceBodyConstants_StandardGravitationParameter()
        {
            Name = "Std. grav. param.";
            Description = "Product of the gravitational constant G and the mass M of the body.";
            Category = MicroEntryCategory.Body;
            IsDefault = false;
            BaseUnit = "μ";
            NumberOfDecimalDigits = 3;
            Formatting = "e";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.ReferenceBodyConstants.StandardGravitationParameter;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }
    
    public class BodyAtmosphereMaxAltitude : BodyEntry
    {
        public BodyAtmosphereMaxAltitude()
        {
            Name = "Atmosphere Altitude";
            Description = "Altitude at which the atmosphere ends and Low Orbit begins.";
            Category = MicroEntryCategory.Body;
            IsDefault = false;
            MiliUnit = "m";
            BaseUnit = "km";
            KiloUnit = "Mm";
            MegaUnit = "Gm";
            GigaUnit = "Tm";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.GetBodyScienceRegion(Utility.ActiveVessel.mainBody.bodyName).SituationData
                ?.AtmosphereMaxAltutude / 1000;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }
    
    public class BodyLowOrbitMaxAltitude : BodyEntry
    {
        public BodyLowOrbitMaxAltitude()
        {
            Name = "Low Orbit Altitude";
            Description = "Altitude at which the Low Orbit ends and High Orbit begins.";
            Category = MicroEntryCategory.Body;
            IsDefault = false;
            MiliUnit = "m";
            BaseUnit = "km";
            KiloUnit = "Mm";
            MegaUnit = "Gm";
            GigaUnit = "Tm";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.GetBodyScienceRegion(Utility.ActiveVessel.mainBody.bodyName).SituationData
                ?.LowOrbitMaxAltutude / 1000;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }
    
    public class BodyHighOrbitMaxAltitude : BodyEntry
    {
        public BodyHighOrbitMaxAltitude()
        {
            Name = "High Orbit Altitude";
            Description = "Altitude at which the High Orbit ends.";
            Category = MicroEntryCategory.Body;
            IsDefault = false;
            MiliUnit = "m";
            BaseUnit = "km";
            KiloUnit = "Mm";
            MegaUnit = "Gm";
            GigaUnit = "Tm";
            NumberOfDecimalDigits = 0;
            Formatting = "N";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.GetBodyScienceRegion(Utility.ActiveVessel.mainBody.bodyName).SituationData
                ?.HighOrbitMaxAltitude / 1000;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }
}