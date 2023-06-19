using BepInEx.Logging;
using MicroMod;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class EditWindowsItemControl : VisualElement
    {
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.EditWindowsItemControl");
        
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

        public BaseEntry Entry;

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

        public EditWindowsItemControl(BaseEntry entry, bool isAvailableEntry) : this()
        {
            Entry = entry;
            EntryName = entry.Name;
            
            // If we're instantiating an 'available entry', then we're displaying only the entry's name
            // If we're instatiating an 'installed entry', then we're showing additional controls for renaming and decimal digit manipulation
            if (isAvailableEntry)
            {
                EntryTextField.focusable = false;
                IncreaseDecimalDigitsButton.style.display = DisplayStyle.None;
                DecreaseDecimalDigitsButton.style.display = DisplayStyle.None;
            }

            //entry.OnEntryValueChanged += HandleEntryValueChanged;
        }


        public EditWindowsItemControl()
        {
            AddToClassList(UssClassName);

            EntryTextField = new TextField()
            {
                name = "entry-name",
            };
            EntryTextField.AddToClassList(UssEntryTextField);
            //EntryTextField.RegisterCallback<MouseDownEvent>(OnMouseDown);
            hierarchy.Add(EntryTextField);

            IncreaseDecimalDigitsButton = new Button()
            {
                name = "increase-decimal",
                //text = "INC"
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
                //text = "DEC"
            };            
            // child element that holds the background image
            var decreaseDecimalBackground = new VisualElement();
            decreaseDecimalBackground.AddToClassList(UssDecreaseDecimalBackground);            
            DecreaseDecimalDigitsButton.Add(decreaseDecimalBackground);
            DecreaseDecimalDigitsButton.AddToClassList(UssDecreaseDecimal);
            hierarchy.Add(DecreaseDecimalDigitsButton);
        }

        // <summary>
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

        /*
        public bool IsSelected(VisualElement selectionContainer)
        {
            Debug.Log("IsSelected entered");
            return this == _currentlySelected;
        }
        
        /// <summary>
        /// Implement your logic here when the label is selected
        /// For example, change the label's appearance or perform some action.
        /// </summary>
        public void Select(VisualElement selectionContainer, bool additive)
        {
            Debug.Log("Select entered");

            if (_currentlySelected != null && _currentlySelected != this)
            {
                _currentlySelected.Unselect(EntryTextField);
            }

            _currentlySelected = this;
            AddToClassList(UssSelected);
        }

        /// <summary>
        /// Implement your logic here when the label is unselected
        /// For example, revert the label's appearance or perform some action.
        /// </summary>
        public void Unselect(VisualElement selectionContainer)
        {
            Debug.Log("Unselect entered");
            if (this == _currentlySelected)
            {
                _currentlySelected = null;
            }
            RemoveFromClassList(UssSelected);
        }
        */

        /// <summary>
        /// Implement your hit test logic here to determine if the given point is within the bounds of the label
        /// For example, you can use the localPoint parameter and the label's visual size to perform the hit test.
        /// </summary>
        //public bool HitTest(Vector2 localPoint) => ContainsPoint(localPoint);

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

        public void HandleEntryValueChanged(string value, string unit)
        {
            /*
            Value = value;
            if (Unit != unit)
                Unit = unit;
            */
        }
        /*
        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button == (int)MouseButton.LeftMouse)
            {
                if (!IsSelected(EntryTextField))
                    Select(EntryTextField, false);
                else
                    Unselect(EntryTextField);
            }
        }
        */
    }
}