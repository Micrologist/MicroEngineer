using BepInEx.Logging;
using MicroMod;
using UitkForKsp2.API;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class EditWindowsController : MonoBehaviour
    {
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.EditWindowsController");
        
        private EditWindowsItemControl _selectedAvailableEntry;
        private EditWindowsItemControl _selectedInstalledEntry;

        public UIDocument EditWindows { get; set; }
        public VisualElement Root { get; set; }
        public Button CloseButton { get; set; }
        public ScrollView AvailableScrollView { get; set; }
        public ScrollView InstalledScrollView { get; set; }
        public DropdownField CategoryDropdown { get; set; }        
        public TextField SelectedWindow { get; set; }
        public Button PreviousWindow { get; set; }
        public Button NextWindow { get; set; }
        public Button NewWindow { get; set; }
        public Button DeleteWindow { get; set; }
        public Toggle LockWindow { get; set; }
        public Button AddEntry { get; set; }
        public Button RemoveEntry { get; set; }
        public Button MoveUp { get; set; }
        public Button MoveDown { get; set; }



        public EditWindowsController()
        { }

        public void OnEnable()
        {
            _logger.LogDebug("Entering OnEnable() of EditWindowsController");
            EditWindows = GetComponent<UIDocument>();
            Root = EditWindows.rootVisualElement;

            CloseButton = Root.Q<Button>("close-button");
            AvailableScrollView = Root.Q<ScrollView>("available-scrollview");
            InstalledScrollView = Root.Q<ScrollView>("installed-scrollview");
            CategoryDropdown = Root.Q<DropdownField>("category__dropdown");
            SelectedWindow = Root.Q<TextField>("selected-window");
            PreviousWindow = Root.Q<Button>("prev-window");
            PreviousWindow.RegisterCallback<PointerUpEvent>(_ => { _logger.LogDebug("PreviousWindow clicked"); });
            NextWindow = Root.Q<Button>("next-window");
            NextWindow.RegisterCallback<PointerUpEvent>(_ => { _logger.LogDebug("NextWindow clicked"); });
            NewWindow = Root.Q<Button>("new-window");
            NewWindow.RegisterCallback<PointerUpEvent>(_ => { _logger.LogDebug("NewWindow clicked"); });
            DeleteWindow = Root.Q<Button>("delete-window");
            LockWindow = Root.Q<Toggle>("lock-window");
            AddEntry = Root.Q<Button>("add-entry");
            RemoveEntry = Root.Q<Button>("remove-entry");
            MoveUp = Root.Q<Button>("move-up");
            MoveDown = Root.Q<Button>("move-down");

            BuildCategoryDropdown();

            //BuildMainGuiHeader();
            //BuildDockedWindows();

            //Root[0].RegisterCallback<PointerUpEvent>(UpdateWindowPosition);

            //MainGuiWindow = (MainGuiWindow)Manager.Instance.Windows.Find(w => w is MainGuiWindow);
            //Root[0].transform.position = MainGuiWindow.FlightRect.position;
        }     
        
        public void Update()
        {
            return;
        }

        private void BuildCategoryDropdown()
        {
            CategoryDropdown.choices = Enum.GetNames(typeof(MicroEntryCategory)).ToList();
            CategoryDropdown.RegisterValueChangedCallback(BuildAvailableEntries);
        }

        private void BuildAvailableEntries(ChangeEvent<string> _)
        {
            AvailableScrollView.Clear();
            _selectedAvailableEntry = null;
            if (CategoryDropdown.value == null)
                return;

            List<BaseEntry> entriesByCategory = Manager.Instance.Entries.FindAll(e => e.Category.ToString() == CategoryDropdown.value); // All entries belong to a category, but they can still be placed in any window
            foreach (var e in entriesByCategory)
            {
                var item = new EditWindowsItemControl(e, true);
                var textField = item.Q<TextField>();
                textField.RegisterCallback<MouseDownEvent>(evt => OnAvailableEntryClicked(evt, item));
                AvailableScrollView.Add(item);
            }
        }

        private void OnAvailableEntryClicked(MouseDownEvent evt, EditWindowsItemControl control)
        {
            if (evt.button == (int)MouseButton.LeftMouse)
            {
                if (control != _selectedAvailableEntry)
                    Select(control);
                else
                    Unselect(control);
            }
        }

        /// <summary>
        /// Implement your logic here when the label is selected
        /// For example, change the label's appearance or perform some action.
        /// </summary>
        public void Select(EditWindowsItemControl control)
        {
            _logger.LogDebug("Select entered");

            if (_selectedAvailableEntry != null && _selectedAvailableEntry != control)
            {
                _selectedAvailableEntry.Unselect();
            }

            _selectedAvailableEntry = control;
            control.Select();
        }

        /// <summary>
        /// Implement your logic here when the label is unselected
        /// For example, revert the label's appearance or perform some action.
        /// </summary>
        public void Unselect(EditWindowsItemControl control)
        {
            _logger.LogDebug("Unselect entered");
            if (control == _selectedAvailableEntry)
            {
                _selectedAvailableEntry = null;
            }
            control.Unselect();
        }        
    }
}
