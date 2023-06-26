//using MicroMod;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class EditWindowsItemControl : VisualElement
    {
        public static EditWindowsItemControl AvailableEntrySelected;

        public const string UssClassName = "edit-windows__entry";
        public const string UssEntryTextField = UssClassName + "__text";
        public const string UssSelected = UssEntryTextField + "-selected";
        public const string UssIncreaseDecimal = UssClassName + "__increase-decimal";
        public const string UssIncreaseDecimalBackground = UssIncreaseDecimal + "-background";
        public const string UssDecreaseDecimal = UssClassName + "__decrease-decimal";
        public const string UssDecreaseDecimalBackground = UssDecreaseDecimal + "-background";

        public TextField EntryTextField;
        public Button IncreaseDecimalDigitsButton;
        public Button DecreaseDecimalDigitsButton;

        //public BaseEntry Entry;

        public string EntryName
        {
            get
            {
                return EntryTextField.text;
            }
            set
            {
                EntryTextField.SetValueWithoutNotify(value);
            }
        }

        /*
        public EditWindowsItemControl(BaseEntry entry, bool isAvailableEntry) : this()
        {
            Entry = entry;
            EntryName = entry.Name;

            // If we're instantiating an 'available entry', then we're displaying only the entry's name
            // If we're instantiating an 'installed entry', then we're showing additional controls for renaming and decimal digit manipulation
            if (isAvailableEntry)
            {
                EntryTextField.focusable = false;
                IncreaseDecimalDigitsButton.style.display = DisplayStyle.None;
                DecreaseDecimalDigitsButton.style.display = DisplayStyle.None;
            }
        }
        */

        public EditWindowsItemControl()
        {
            AddToClassList(UssClassName);

            EntryTextField = new TextField()
            {
                name = "entry-name",
            };
            EntryTextField.AddToClassList(UssEntryTextField);
            hierarchy.Add(EntryTextField);

            IncreaseDecimalDigitsButton = new Button()
            {
                name = "increase-decimal",
            };
            // child element that holds the background image
            var increaseDecimalBackground = new VisualElement();
            increaseDecimalBackground.AddToClassList(UssIncreaseDecimalBackground);
            IncreaseDecimalDigitsButton.Add(increaseDecimalBackground);
            IncreaseDecimalDigitsButton.AddToClassList(UssIncreaseDecimal);
            hierarchy.Add(IncreaseDecimalDigitsButton);

            DecreaseDecimalDigitsButton = new Button()
            {
                name = "decrease-decimal",
            };
            // child element that holds the background image
            var decreaseDecimalBackground = new VisualElement();
            decreaseDecimalBackground.AddToClassList(UssDecreaseDecimalBackground);
            DecreaseDecimalDigitsButton.Add(decreaseDecimalBackground);
            DecreaseDecimalDigitsButton.AddToClassList(UssDecreaseDecimal);
            hierarchy.Add(DecreaseDecimalDigitsButton);
        }

        /// <summary>
        /// Implement your logic here when the label is unselected
        /// For example, revert the label's appearance or perform some action.
        /// </summary>
        public void Unselect()
        {
            RemoveFromClassList(UssSelected);
        }

        /// <summary>
        /// Implement your logic here when the label is selected
        /// For example, change the label's appearance or perform some action.
        /// </summary>
        public void Select()
        {
            AddToClassList(UssSelected);
        }

        public new class UxmlFactory : UxmlFactory<EditWindowsItemControl, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription _entryText = new UxmlStringAttributeDescription() { name = "entry_text", defaultValue = "Lorem ipsum" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is EditWindowsItemControl e)
                    e.EntryName = _entryText.GetValueFromBag(bag, cc);
            }
        }
    }
}