
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
            Unit = null;
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
            Name = "Body Radius";
            Description = "Body's radius.";
            Category = MicroEntryCategory.Body;
            IsDefault = false;
            Unit = "m";
            Formatting = "{0:N0}";
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
            Name = "Std. Grav. Param.";
            Description = "Product of the gravitational constant G and the mass M of the body.";
            Category = MicroEntryCategory.Body;
            IsDefault = false;
            Unit = "μ";
            Formatting = "{0:e4}";
        }

        public override void RefreshData()
        {
            EntryValue = Utility.ActiveVessel.Orbit.ReferenceBodyConstants.StandardGravitationParameter;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }    
}
