
namespace MicroMod
{
    public class BodyEntry : MicroEntry
    { }

    public class Body : BodyEntry
    {
        public Body()
        {
            Name = "Body";
            Description = "Shows the body that vessel is currently at.";
            Category = MicroEntryCategory.Body;
            Unit = null;
            Formatting = null;
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.mainBody.bodyName;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ReferenceBodyConstants_Radius : BodyEntry
    {
        public ReferenceBodyConstants_Radius()
        {
            Name = "Body Radius";
            Description = "";
            Category = MicroEntryCategory.Body;
            Unit = "m";
            Formatting = "{0:N0}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.ReferenceBodyConstants.Radius;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }

    public class ReferenceBodyConstants_StandardGravitationParameter : BodyEntry
    {
        public ReferenceBodyConstants_StandardGravitationParameter()
        {
            Name = "Std. Grav. Param.";
            Description = "";
            Category = MicroEntryCategory.Body;
            Unit = "μ";
            Formatting = "{0:e4}";
        }

        public override void RefreshData()
        {
            EntryValue = MicroUtility.ActiveVessel.Orbit.ReferenceBodyConstants.StandardGravitationParameter;
        }

        public override string ValueDisplay => base.ValueDisplay;
    }    
}
