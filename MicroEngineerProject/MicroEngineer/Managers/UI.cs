using BepInEx.Logging;
using KSP.Game;
using KSP.Sim.impl;
using KSP.UI.Binding;
using UnityEngine;

namespace MicroMod
{
    internal class UI
    {
        private static UI _instance;
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.UI");

        internal bool ShowGuiFlight;
        internal bool ShowGuiOAB;
        private bool _showGuiSettingsFlight;

        // If game input is enabled or disabled (used for locking controls when user is editing a text field
        private bool _gameInputState = true;

        #region Editing window
        private bool _showEditWindow = false;
        private int _selectedWindowId = 0;
        private MicroEntryCategory _selectedCategory = MicroEntryCategory.Vessel;
        private (bool condition, int index) _showTooltip = (false, 0);
        #endregion

        // Index of the stage for which user wants to select a different CelestialBody for different TWR calculations. -1 -> no stage is selected
        internal int CelestialBodySelectionStageIndex = -1;
        private bool _showGuiSettingsOAB;

        Rect settingsFlightRect;

        /// <summary>
        /// Holds data on all bodies for calculating TWR (currently)
        /// </summary>
        internal MicroCelestialBodies CelestialBodies = new();

        internal UI()
        {
        }

        public static UI Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UI();

                return _instance;
            }
        }

        internal void OnGUI()
        {
            GUI.skin = Styles.SpaceWarpUISkin;

            Utility.RefreshGameManager();
            if (Utility.GameState?.GameState == GameState.VehicleAssemblyBuilder)
                OnGUI_OAB();
            else
                OnGUI_Flight();
        }

        private void OnGUI_Flight()
        {
            _gameInputState = Utility.ToggleGameInputOnControlInFocus(_gameInputState, ShowGuiFlight);

            if (!ShowGuiFlight || Utility.ActiveVessel == null) return;

            MainGuiWindow mainGui = (MainGuiWindow)Manager.Instance.Windows.Find(w => w is MainGuiWindow);

            // Draw main GUI that contains docked windows
            mainGui.FlightRect = GUILayout.Window(
                GUIUtility.GetControlID(FocusType.Passive),
                mainGui.FlightRect,
                FillMainGUI,
                "<color=#696DFF>// MICRO ENGINEER</color>",
                Styles.MainWindowStyle,
                GUILayout.Height(0)
            );
            mainGui.FlightRect.position = Utility.ClampToScreen(mainGui.FlightRect.position, mainGui.FlightRect.size);

            List<EntryWindow> entryWindows = Manager.Instance.Windows.FindAll(w => w is EntryWindow).Cast<EntryWindow>().ToList();

            // Draw all other popped out windows
            foreach (var (window, index) in entryWindows
                .Select((window, index) => (window, index))
                .Where(x => x.window.IsFlightActive && x.window.IsFlightPoppedOut) // must be active & popped out
                .Where(x => x.window.MainWindow != MainWindow.Stage)) // Stage is special, it'll be drawn separately
            {
                // Skip drawing of Target window if there's no active target
                if (window.MainWindow == MainWindow.Target && !Utility.TargetExists())
                    continue;

                // Skip drawing of Maneuver window if there's no active maneuver
                if (window.MainWindow == MainWindow.Maneuver && !Utility.ManeuverExists())
                    continue;

                // If window is locked set alpha to 20%
                if (window.IsLocked)
                    GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.2f);
                window.FlightRect = GUILayout.Window(
                    GUIUtility.GetControlID(FocusType.Passive),
                    window.FlightRect,
                    (id) => DrawPopoutWindow(window),
                    "",
                    Styles.PopoutWindowStyle,
                    GUILayout.Height(0),
                    GUILayout.Width(Styles.WindowWidth
                    ));

                // Set alpha back to 100%
                if (window.IsLocked)
                    GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1);

                // Snap popped out windows
                var settings = Manager.Instance.Windows.Find(w => w is SettingsWIndow) as SettingsWIndow;
                if (settings.SnapWindows && window.IsBeingDragged)
                    HandleSnapping(window);

                window.FlightRect.position = Utility.ClampToScreen(window.FlightRect.position, window.FlightRect.size);
            }

            // Draw popped out Stages
            /*
            StageWindow stageWindow= entryWindows.OfType<StageWindow>().FirstOrDefault();
            if (stageWindow.IsFlightActive && stageWindow.IsFlightPoppedOut)
                stageWindow.DrawWindow(this);
            */

            // Draw Edit Window
            if (_showEditWindow)
            {
                Styles.EditWindowRect = GUILayout.Window(
                    GUIUtility.GetControlID(FocusType.Passive),
                    Styles.EditWindowRect,
                    DrawEditWindow,
                    "",
                    Styles.EditWindowStyle,
                    GUILayout.Height(0)
                    );
            }

            // Draw Settings window in Flight 
            if (_showGuiSettingsFlight)
            {
                settingsFlightRect = GUILayout.Window(
                    GUIUtility.GetControlID(FocusType.Passive),
                    settingsFlightRect,
                    DrawSettingsFlightWindow,
                    "",
                    Styles.SettingsFlightStyle,
                    GUILayout.Height(0)
                    );
            }
        }

        private void DrawSettingsFlightWindow(int id)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>// SETTINGS</b>");
            GUILayout.FlexibleSpace();
            _showGuiSettingsFlight = !CloseButton(Styles.CloseBtnStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("<b>Edit window entries</b>");
            if (GUILayout.Button("EDIT WINDOWS", Styles.NormalBtnStyle))
            {
                _showEditWindow = !_showEditWindow;
                Manager.Instance.PupulateTextFieldNames(GetEditableWindows()[_selectedWindowId].Entries);
            }

            // TEMP for UITK rewrite
            GUILayout.Space(10);
            GUILayout.Label("Uitk test bed");
            if (GUILayout.Button("popped out window", Styles.NormalBtnStyle))
            {
                if (UitkControllerTest.Instance.IsInitialized)
                    UitkControllerTest.Instance.Toggle();
                else
                    UitkControllerTest.Instance.Initialize();
            }
            if (GUILayout.Button("create maingui", Styles.NormalBtnStyle))
                UitkControllerTest.Instance.CreateMainGui();
            if (GUILayout.Button("add docked window", Styles.NormalBtnStyle))
                UitkControllerTest.Instance.AddDockedWindow();
            // END OF TEMP

            GUILayout.Space(10);
            GUILayout.Label("<b>Layout control</b>");
            if (GUILayout.Button("SAVE LAYOUT", Styles.NormalBtnStyle))
            {
                Manager.Instance.SaveLayout();
                _showGuiSettingsFlight = false;
            }
            if (GUILayout.Button("LOAD LAYOUT", Styles.NormalBtnStyle))
                Manager.Instance.LoadLayout();
            if (GUILayout.Button("RESET LAYOUT", Styles.NormalBtnStyle))
            {
                Manager.Instance.ResetLayout();
                _selectedWindowId = 0;
            }

            GUILayout.Space(10);
            GUILayout.Label("<b>Theme</b>");
            GUILayout.Space(-10);

            GUILayout.BeginHorizontal();
            var settingsWindow = Manager.Instance.Windows.Find(w => w.GetType() == typeof(SettingsWIndow)) as SettingsWIndow;
            if (GUILayout.Toggle(Styles.ActiveTheme == Theme.munix, "munix", Styles.SectionToggleStyle))
            {
                Styles.SetActiveTheme(Theme.munix);
                settingsWindow.ActiveTheme = Theme.munix;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Toggle(Styles.ActiveTheme == Theme.Gray, "Gray", Styles.SectionToggleStyle))
            {
                Styles.SetActiveTheme(Theme.Gray);
                settingsWindow.ActiveTheme = Theme.Gray;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Toggle(Styles.ActiveTheme == Theme.Black, "Black", Styles.SectionToggleStyle))
            {
                Styles.SetActiveTheme(Theme.Black);
                settingsWindow.ActiveTheme = Theme.Black;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("<b>Other</b>");
            GUILayout.Space(-10);
            settingsWindow.SnapWindows = GUILayout.Toggle(settingsWindow.SnapWindows, "Window snapping", Styles.SectionToggleStyle);

            GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
        }

        private void OnGUI_OAB()
        {
            if (!ShowGuiOAB) return;

            EntryWindow stageInfoOAB = Manager.Instance.Windows.FindAll(w => w is EntryWindow).Cast<EntryWindow>().ToList().Find(w => w.MainWindow == MainWindow.StageInfoOAB);
            if (stageInfoOAB.Entries.Find(e => e.Name == "Stage Info (OAB)").EntryValue == null) return;

            stageInfoOAB.EditorRect = GUILayout.Window(
                GUIUtility.GetControlID(FocusType.Passive),
                stageInfoOAB.EditorRect,
                DrawStageInfoOAB,
                "",
                Styles.StageOABWindowStyle,
                GUILayout.Height(0)
                );
            stageInfoOAB.EditorRect.position = Utility.ClampToScreen(stageInfoOAB.EditorRect.position, stageInfoOAB.EditorRect.size);

            // Draw window for selecting CelestialBody for a stage
            // -1 -> no selection of CelestialBody is taking place
            // any other int -> index represents the stage number for which the selection was clicked
            if (CelestialBodySelectionStageIndex > -1)
            {
                Rect stageInfoOabRect = stageInfoOAB.EditorRect;
                Rect celestialBodyRect = new Rect(stageInfoOabRect.x + stageInfoOabRect.width, stageInfoOabRect.y, 0, 0);

                celestialBodyRect = GUILayout.Window(
                    GUIUtility.GetControlID(FocusType.Passive),
                    celestialBodyRect,
                    DrawCelestialBodySelection,
                    "",
                    Styles.CelestialSelectionStyle,
                    GUILayout.Height(0)
                    );
            }

            // Draw Settings window for the StageInfoOAB
            if (_showGuiSettingsOAB)
            {
                Rect stageInfoOabRect = stageInfoOAB.EditorRect;
                Rect settingsRect = new Rect(stageInfoOabRect.x + stageInfoOabRect.width, stageInfoOabRect.y, 0, 0);

                settingsRect = GUILayout.Window(
                    GUIUtility.GetControlID(FocusType.Passive),
                    settingsRect,
                    DrawSettingsOabWindow,
                    "",
                    Styles.SettingsOabStyle,
                    GUILayout.Height(0)
                    );
            }
        }

        #region Flight scene UI
        /// <summary>
        /// Draws the main GUI with all windows that are toggled and docked
        /// </summary>
        /// <param name="windowID"></param>
        private void FillMainGUI(int windowID)
        {
            List<EntryWindow> entryWindows = Manager.Instance.Windows.FindAll(w => w is EntryWindow).Cast<EntryWindow>().ToList();

            GUILayout.Space(-15);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Styles.Settings20Texture, Styles.SettingsMainGuiBtnStyle))
            {                
                Rect mainGuiRect = Manager.Instance.Windows.OfType<MainGuiWindow>().FirstOrDefault().FlightRect;
                settingsFlightRect = new Rect(mainGuiRect.x - Styles.WindowWidthSettingsFlight, mainGuiRect.y, Styles.WindowWidthSettingsFlight, 0);
                _showGuiSettingsFlight = !_showGuiSettingsFlight;
            }
            GUILayout.FlexibleSpace();
            if (CloseButton(Styles.CloseMainGuiBtnStyle))
                CloseWindow();
            GUILayout.EndHorizontal();

            try
            {
                GUILayout.BeginHorizontal();

                int toggleIndex = -1;
                // Draw toggles for all windows except MainGui and StageInfoOAB
                foreach (EntryWindow window in entryWindows.Where(x => x.MainWindow != MainWindow.MainGui && x.MainWindow != MainWindow.StageInfoOAB))
                {
                    // layout can fit 6 toggles, so if all 6 slots are filled then go to a new line. Index == 0 is the MainGUI which isn't rendered
                    if (++toggleIndex % 6 == 0 && toggleIndex > 0)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }
                    window.IsFlightActive = GUILayout.Toggle(window.IsFlightActive, window.Abbreviation, Styles.SectionToggleStyle);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(-5);

                // Draw Stage window next
                /*
                StageWindow stageWindow = entryWindows.OfType<StageWindow>().FirstOrDefault();
                if (stageWindow.IsFlightActive && !stageWindow.IsFlightPoppedOut)
                    stageWindow.DrawWindow(this);
                */

                // Draw all other windows
                foreach (var (window, index) in entryWindows
                    .Select((window, index) => (window, index))
                    .Where(x => x.window.IsFlightActive && !x.window.IsFlightPoppedOut) // must be active & docked
                    .Where(x => x.window.MainWindow != MainWindow.Stage)) // Stage is special, it'll be drawn separately

                {
                    // Skip drawing of Target window if there's no active target
                    if (window.MainWindow == MainWindow.Target && !Utility.TargetExists())
                        continue;

                    // Skip drawing of Maneuver window if there's no active maneuver
                    if (window.MainWindow == MainWindow.Maneuver && !Utility.ManeuverExists())
                        continue;

                    DrawSectionHeader(window.Name, ref window.IsFlightPoppedOut, window.IsLocked);

                    window.DrawWindowHeader();

                    for (int i = 0; i < window.Entries.Count; i++)
                    {
                        if (window.Entries[i].HideWhenNoData && window.Entries[i].ValueDisplay == "-")
                            continue;
                        GUIStyle s = i == 0 ? Styles.EntryBackground_First : i < window.Entries.Count - 1 ? Styles.EntryBackground_Middle : Styles.EntryBackground_Last;
                        DrawEntry(s, window.Entries[i].Name, window.Entries[i].ValueDisplay, window.Entries[i].UnitDisplay);
                    }

                    window.DrawWindowFooter();

                    DrawSectionEnd(window);
                }

                GUI.DragWindow(new Rect(0, 0, Styles.WindowWidth, Styles.WindowHeight));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        /// <summary>
        /// Draws all windows that are toggled and popped out
        /// </summary>
        /// <param name="windowIndex"></param>
        private void DrawPopoutWindow(EntryWindow w)
        {
            GUILayout.Space(-5);
            DrawSectionHeader(w.Name, ref w.IsFlightPoppedOut, w.IsLocked);
            
            w.DrawWindowHeader();

            for (int i = 0; i < w.Entries.Count; i++)
            {
                if (w.Entries[i].HideWhenNoData && w.Entries[i].ValueDisplay == "-")
                    continue;
                GUIStyle s = i == 0 ? Styles.EntryBackground_First : i < w.Entries.Count - 1 ? Styles.EntryBackground_Middle : Styles.EntryBackground_Last;
                DrawEntry(s, w.Entries[i].Name, w.Entries[i].ValueDisplay, w.Entries[i].UnitDisplay);
            }

            w.DrawWindowFooter();

            DrawSectionEnd(w);
        }

        private void DrawSectionHeader(string sectionName, ref bool isPopout, bool isLocked)
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{sectionName}", Styles.WindowTitleLabelStyle);
            GUILayout.FlexibleSpace();            
            if(GUILayout.Button(Styles.Settings15Texture, Styles.SettingsBtnStyle))
            {
                _selectedWindowId = Manager.Instance.Windows.
                    FindAll(w => w is EntryWindow).Cast<EntryWindow>().ToList().
                    FindAll(w => w.IsEditable).
                    FindIndex(w => w.Name == sectionName);
                _showEditWindow = true;
                Manager.Instance.PupulateTextFieldNames(GetEditableWindows()[_selectedWindowId].Entries);
            }
            isPopout = isPopout && !isLocked ? !CloseButton(Styles.CloseBtnStyle) : !isPopout ? GUILayout.Button(Styles.PopoutTexture, Styles.PopoutBtnStyle) : isPopout;
            GUILayout.EndHorizontal();

            GUILayout.Space(Styles.NegativeSpacingAfterHeader);
        }        

        private void DrawEntry(GUIStyle backgroundTexture, string entryName, string value, string unit = "")
        {
            GUILayout.BeginHorizontal(backgroundTexture);
            GUILayout.Label(entryName, Styles.NameLabelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label(value, Styles.ValueLabelStyle);
            GUILayout.Space(5);
            GUILayout.Label(unit, Styles.UnitLabelStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(Styles.NegativeSpacingAfterEntry);
        }        

        private void DrawSectionEnd(EntryWindow window)
        {
            if (window.IsFlightPoppedOut)
            {
                if (!window.IsLocked)
                    GUI.DragWindow(new Rect(0, 0, Styles.WindowWidth, Styles.WindowHeight));

                GUILayout.Space(Styles.SpacingBelowPopout);
            }
            else
            {
                GUILayout.Space(Styles.SpacingAfterSection);
            }
        }

        /// <summary>
        /// Window for edditing window contents. Add/Remove/Reorder entries.
        /// </summary>
        /// <param name="windowIndex"></param>
        private void DrawEditWindow(int windowIndex)
        {
            List<EntryWindow> editableWindows = GetEditableWindows();
            List<BaseEntry> entriesByCategory = Manager.Instance.Entries.FindAll(e => e.Category == _selectedCategory); // All entries belong to a category, but they can still be placed in any window

            GUILayout.Space(-5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _showEditWindow = !CloseButton(Styles.CloseBtnStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            #region Selection of window to be edited
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>EDITING WINDOW</b>", Styles.TitleLabelStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<", Styles.OneCharacterHighBtnStyle))
            {
                _selectedWindowId = _selectedWindowId > 0 ? _selectedWindowId - 1 : editableWindows.Count - 1;
                Manager.Instance.PupulateTextFieldNames(editableWindows[_selectedWindowId].Entries);
            }
            GUI.SetNextControlName(Utility.InputDisableWindowAbbreviation);
            editableWindows[_selectedWindowId].Abbreviation = GUILayout.TextField(editableWindows[_selectedWindowId].Abbreviation, Styles.WindowSelectionAbbrevitionTextFieldStyle);
            editableWindows[_selectedWindowId].Abbreviation = Utility.ValidateAbbreviation(editableWindows[_selectedWindowId].Abbreviation);
            GUI.SetNextControlName(Utility.InputDisableWindowName);
            editableWindows[_selectedWindowId].Name = GUILayout.TextField(editableWindows[_selectedWindowId].Name, Styles.WindowSelectionTextFieldStyle);
            if (GUILayout.Button(">", Styles.OneCharacterHighBtnStyle))
            {
                _selectedWindowId = _selectedWindowId < editableWindows.Count - 1 ? _selectedWindowId + 1 : 0;
                Manager.Instance.PupulateTextFieldNames(editableWindows[_selectedWindowId].Entries);
            }
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(-10);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            editableWindows[_selectedWindowId].IsLocked = GUILayout.Toggle(editableWindows[_selectedWindowId].IsLocked, "Locked");
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            if (editableWindows[_selectedWindowId].IsDeletable)
            {
                if (GUILayout.Button("DEL WINDOW", Styles.NormalBtnStyle))
                {
                    Manager.Instance.Windows.Remove(editableWindows[_selectedWindowId]);
                    editableWindows.Remove(editableWindows[_selectedWindowId]);
                    _selectedWindowId--;
                }
            }
            if (GUILayout.Button("NEW WINDOW", Styles.NormalBtnStyle))
                _selectedWindowId = Manager.Instance.CreateCustomWindow(editableWindows);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            Styles.DrawHorizontalLine();
            GUILayout.Space(10);

            #region Installed entries in the selected window
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>Installed</b>", Styles.NormalLabelStyle);
            GUILayout.EndHorizontal();

            var entries = editableWindows[_selectedWindowId].Entries.ToList();
            for (int i = 0; i < entries.Count; i++)
            {
                GUIStyle backgroundStyle = i == 0 ? Styles.EntryBackground_First : i < entries.Count - 1 ? Styles.EntryBackground_Middle : Styles.EntryBackground_Last;

                GUILayout.BeginHorizontal(backgroundStyle);

                GUI.SetNextControlName(entries[i].Id.ToString());
                entries[i].Name = GUILayout.TextField(entries[i].Name, Styles.InstalledEntryFieldStyle);
                GUILayout.FlexibleSpace();
                GUI.enabled = entries[i].NumberOfDecimalDigits < 5;
                if (entries[i].Formatting != null && GUILayout.Button(Styles.IncreaseDecimalDigitsTexture, Styles.OneCharacterBtnStyle))
                {
                    entries[i].NumberOfDecimalDigits++;
                }
                GUI.enabled = entries[i].NumberOfDecimalDigits > 0;
                if (entries[i].Formatting != null && GUILayout.Button(Styles.DecreaseDecimalDigitsTexture, Styles.OneCharacterBtnStyle))
                {
                    entries[i].NumberOfDecimalDigits--;
                }
                GUI.enabled = i > 0;
                if (GUILayout.Button("↑", Styles.OneCharacterBtnStyle))
                {
                    editableWindows[_selectedWindowId].MoveEntryUp(i);
                }
                GUI.enabled = i < editableWindows[_selectedWindowId].Entries.Count - 1;
                if (GUILayout.Button("↓", Styles.OneCharacterBtnStyle))
                {
                    editableWindows[_selectedWindowId].MoveEntryDown(i);
                }
                GUI.enabled = true;
                if (GUILayout.Button("X", Styles.OneCharacterBtnStyle))
                    editableWindows[_selectedWindowId].RemoveEntry(i);
                GUILayout.EndHorizontal();
                GUILayout.Space(-4);
            }
            #endregion

            GUILayout.Space(20);
            Styles.DrawHorizontalLine();
            GUILayout.Space(10);

            #region All entries that can be added to any IsEditable window
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>Add</b>", Styles.NormalLabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Category", Styles.NormalLabelStyle);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("<", Styles.OneCharacterHighBtnStyle))
            {
                _selectedCategory = (int)_selectedCategory > 0 ?
                    _selectedCategory - 1 :
                    Enum.GetValues(typeof(MicroEntryCategory)).Cast<MicroEntryCategory>().Last();
            }
            GUILayout.Label(_selectedCategory.ToString(), Styles.NormalCenteredLabelStyle);
            if (GUILayout.Button(">", Styles.OneCharacterHighBtnStyle))
            {
                _selectedCategory = (int)_selectedCategory < (int)Enum.GetValues(typeof(MicroEntryCategory)).Cast<MicroEntryCategory>().Last() ?
                    _selectedCategory + 1 :
                    Enum.GetValues(typeof(MicroEntryCategory)).Cast<MicroEntryCategory>().First();
            }
            GUILayout.EndHorizontal();

            for (int i = 0; i < entriesByCategory.Count; i++)
            {
                GUIStyle backgroundStyle = i == 0 ? Styles.EntryBackground_First : i < entriesByCategory.Count - 1 ? Styles.EntryBackground_Middle : Styles.EntryBackground_Last;

                GUILayout.BeginHorizontal(backgroundStyle);
                GUILayout.Label(entriesByCategory[i].Name, Styles.NameLabelStyle);
                if (GUILayout.Button("?", Styles.OneCharacterBtnStyle))
                {
                    if (!_showTooltip.condition)
                        _showTooltip = (true, i);
                    else
                    {
                        if (_showTooltip.index != i)
                            _showTooltip = (true, i);
                        else
                            _showTooltip = (false, i);
                    }
                }
                if (GUILayout.Button("+", Styles.OneCharacterBtnStyle))
                {                    
                    editableWindows[_selectedWindowId].AddEntry(Activator.CreateInstance(entriesByCategory[i].GetType()) as BaseEntry);
                    Manager.Instance.PupulateTextFieldNames(editableWindows[_selectedWindowId].Entries);
                }
                GUILayout.EndHorizontal();

                if (_showTooltip.condition && _showTooltip.index == i)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(entriesByCategory[i].Description, Styles.BlueLabelStyle);
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(Styles.NegativeSpacingAfterEntry + 7);
            }
            GUILayout.Space(10);
            #endregion

            GUI.DragWindow(new Rect(0, 0, Styles.WindowWidth, Styles.WindowHeight));
        }

        private List<EntryWindow> GetEditableWindows()
        {
            return Manager.Instance.Windows.FindAll(w => w is EntryWindow).Cast<EntryWindow>().ToList().FindAll(w => w.IsEditable); // Editable windows are all except MainGUI, Settings, Stage and StageInfoOAB
        }

        #endregion

        #region OAB scene UI

        private void DrawStageInfoOAB(int windowID)
        {
            EntryWindow stageInfoOabWindow = Manager.Instance.Windows.FindAll(w => w is EntryWindow).Cast<EntryWindow>().ToList().Find(w => w.MainWindow == MainWindow.StageInfoOAB);
            List<BaseEntry> stageInfoOabEntries = stageInfoOabWindow.Entries;

            GUILayout.BeginHorizontal();            
            GUILayout.Label($"<b>Stage Info</b>");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(Styles.Settings15Texture, Styles.SettingsBtnStyle))
                _showGuiSettingsOAB = !_showGuiSettingsOAB;            
            
            if (CloseButton(Styles.CloseBtnStyle))
            {
                GameObject.Find("BTN-MicroEngineerOAB")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
                stageInfoOabWindow.IsEditorActive = false;
                ShowGuiOAB = false;
            }

            GUILayout.EndHorizontal();

            // Draw StageInfo header - Delta V fields
            GUILayout.BeginHorizontal();
            GUILayout.Label("Total ∆v (ASL, vacuum)", Styles.NameLabelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{stageInfoOabEntries.Find(e => e.Name == "Total ∆v Actual (OAB)").ValueDisplay}, {stageInfoOabEntries.Find(e => e.Name == "Total ∆v Vac (OAB)").ValueDisplay}", Styles.ValueLabelStyle);
            GUILayout.Space(5);
            GUILayout.Label("m/s", Styles.UnitLabelStyle);
            GUILayout.EndHorizontal();

            // Draw Torque
            Torque torque = (Torque)stageInfoOabEntries.Find(e => e.Name == "Torque");
            if (torque.IsActive)
            {
                GUILayout.Space(Styles.NegativeSpacingAfterEntry);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Torque", Styles.NameLabelStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(torque.ValueDisplay, Styles.ValueLabelStyle);
                GUILayout.Space(5);
                GUILayout.Label(torque.UnitDisplay, Styles.UnitLabelStyle);
                GUILayout.EndHorizontal();
            }

            // Draw Stage table header
            GUILayout.BeginHorizontal();
            GUILayout.Label("Stage", Styles.NameLabelStyle, GUILayout.Width(40));
            GUILayout.FlexibleSpace();
            GUILayout.Label("TWR", Styles.TableHeaderLabelStyle, GUILayout.Width(65));
            GUILayout.Label("SLT", Styles.TableHeaderLabelStyle, GUILayout.Width(75));
            GUILayout.Label("", Styles.TableHeaderLabelStyle, GUILayout.Width(30));
            GUILayout.Label("ASL ∆v", Styles.TableHeaderLabelStyle, GUILayout.Width(75));
            GUILayout.Label("", Styles.TableHeaderLabelStyle, GUILayout.Width(30));
            GUILayout.Label("Vac ∆v", Styles.TableHeaderLabelStyle, GUILayout.Width(75));
            GUILayout.Label("Burn Time", Styles.TableHeaderLabelStyle, GUILayout.Width(110));
            GUILayout.Space(20);
            GUILayout.Label("Body", Styles.TableHeaderCenteredLabelStyle, GUILayout.Width(80));
            GUILayout.EndHorizontal();
            GUILayout.Space(Styles.NegativeSpacingAfterEntry);

            StageInfo_OAB stageInfoOab = (StageInfo_OAB)stageInfoOabWindow.Entries
                .Find(e => e.Name == "Stage Info (OAB)");

            // Draw each stage that has delta v
            var stages = ((List<DeltaVStageInfo_OAB>)stageInfoOab.EntryValue)
                .FindAll(s => s.DeltaVVac > 0.0001 || s.DeltaVASL > 0.0001);

            int celestialIndex = -1;
            for (int stageIndex = stages.Count - 1; stageIndex >= 0; stageIndex--)
            {
                // Check if this stage has a CelestialBody attached. If not, create a new CelestialBody and assign it to HomeWorld (i.e. Kerbin)
                if (stageInfoOab.CelestialBodyForStage.Count == ++celestialIndex)
                    stageInfoOab.AddNewCelestialBody(CelestialBodies);

                GUILayout.BeginHorizontal();
                GUILayout.Label(String.Format("{0:00}", ((List<DeltaVStageInfo_OAB>)stageInfoOab.EntryValue).Count - stages[stageIndex].Stage), Styles.NameLabelStyle, GUILayout.Width(40));
                GUILayout.FlexibleSpace();

                // We calculate what factor needs to be applied to TWR in order to compensate for different gravity of the selected celestial body                
                double twrFactor = CelestialBodies.GetTwrFactor(stageInfoOab.CelestialBodyForStage[celestialIndex]);
                GUILayout.Label(String.Format("{0:N2}", stages[stageIndex].TWRVac * twrFactor), Styles.ValueLabelStyle, GUILayout.Width(65));

                // Calculate Sea Level TWR and DeltaV
                CelestialBodyComponent cel = CelestialBodies.Bodies.Find(b => b.Name == stageInfoOab.CelestialBodyForStage[celestialIndex]).CelestialBodyComponent;
                GUILayout.Label(String.Format("{0:N2}", stages[stageIndex].GetTWRAtSeaLevel(cel) * twrFactor), Styles.ValueLabelStyle, GUILayout.Width(75));
                GUILayout.Label(String.Format("{0:N0}", stages[stageIndex].GetDeltaVelAtSeaLevel(cel)), Styles.ValueLabelStyle, GUILayout.Width(75));
                GUILayout.Label("m/s", Styles.UnitLabelStyleStageOAB, GUILayout.Width(30));

                GUILayout.Label(String.Format("{0:N0}", stages[stageIndex].DeltaVVac), Styles.ValueLabelStyle, GUILayout.Width(75));
                GUILayout.Label("m/s", Styles.UnitLabelStyleStageOAB, GUILayout.Width(30));
                GUILayout.Label(Utility.SecondsToTimeString(stages[stageIndex].StageBurnTime, true, true), Styles.ValueLabelStyle, GUILayout.Width(110));
                GUILayout.Space(20);
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(stageInfoOab.CelestialBodyForStage[celestialIndex], Styles.CelestialBodyBtnStyle))
                {
                    CelestialBodySelectionStageIndex = celestialIndex;
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(Styles.NegativeSpacingAfterEntry);
            }

            GUILayout.Space(Styles.SpacingBelowPopout);

            GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
        }

        /// <summary>
        /// Opens a window for selecting a CelestialObject for the stage on the given index
        /// </summary>
        private void DrawCelestialBodySelection(int id)
        {
            GUILayout.BeginVertical();

            foreach (var body in CelestialBodies.Bodies)
            {
                if (GUILayout.Button(body.DisplayName, Styles.CelestialSelectionBtnStyle))
                {
                    StageInfo_OAB stageInfoOab = (StageInfo_OAB)Manager.Instance.Windows.FindAll(w => w is EntryWindow).Cast<EntryWindow>().ToList().Find(w => w.MainWindow == MainWindow.StageInfoOAB).Entries.Find(e => e.Name == "Stage Info (OAB)");
                    stageInfoOab.CelestialBodyForStage[CelestialBodySelectionStageIndex] = body.Name;

                    // Hide the selection window
                    CelestialBodySelectionStageIndex = -1;
                }
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Opens a Settings window for OAB
        /// </summary>
        private void DrawSettingsOabWindow(int id)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _showGuiSettingsOAB = !CloseButton(Styles.CloseBtnStyle);
            GUILayout.EndHorizontal();

            EntryWindow stageInfoOabWindow = Manager.Instance.Windows.FindAll(w => w is EntryWindow).Cast<EntryWindow>().ToList().Find(w => w.MainWindow == MainWindow.StageInfoOAB);
            List<BaseEntry> stageInfoOabEntries = stageInfoOabWindow.Entries;
            Torque torqueEntry = (Torque)stageInfoOabEntries.Find(e => e.Name == "Torque");

            torqueEntry.IsActive = GUILayout.Toggle(torqueEntry.IsActive, "Display Torque (experimental)\nTurn on CoT & CoM for this", Styles.SectionToggleStyle);
            GUILayout.Space(15);
        }

        #endregion
        
        private void CloseWindow()
        {
            GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
            ShowGuiFlight = false;
        }

        /// <summary>
        /// Draws a close button (X)
        /// </summary>
        /// <param name="rect">Where to position the close button</param>
        /// <returns></returns>
        internal bool CloseButton(GUIStyle style)//Rect rect)
        {
            //return GUI.Button(rect, Styles.CloseButtonTexture, Styles.CloseBtnStyle);
            return GUILayout.Button(Styles.CloseButtonTexture, style);
        }

        internal void HandleSnapping(EntryWindow draggedWindow)
        {
            List<EntryWindow> poppedOutWindows = Manager.Instance.Windows
                .FindAll(w => typeof(EntryWindow).IsAssignableFrom(w.GetType()))
                .Cast<EntryWindow>()
                .Where(w => w.IsFlightActive && w.IsFlightPoppedOut)
                .ToList();

            var settings = Manager.Instance.Windows.Find(w => w is SettingsWIndow) as SettingsWIndow;

            foreach (var otherWindow in poppedOutWindows)
            {
                // Check if the current window is close to any edge of the other window
                if (otherWindow != draggedWindow && Utility.AreRectsNear(draggedWindow.FlightRect, otherWindow.FlightRect))
                {
                    // Snap to the left edge
                    if (Mathf.Abs(draggedWindow.FlightRect.xMin - otherWindow.FlightRect.xMin) < settings.SnapDistance)
                        draggedWindow.FlightRect.x = otherWindow.FlightRect.xMin;

                    // Snap to the right edge
                    if (Mathf.Abs(draggedWindow.FlightRect.xMax - otherWindow.FlightRect.xMin) < settings.SnapDistance)
                            draggedWindow.FlightRect.x = otherWindow.FlightRect.xMin - draggedWindow.FlightRect.width;

                    // Snap to the left edge
                    if (Mathf.Abs(draggedWindow.FlightRect.xMin - otherWindow.FlightRect.xMax) < settings.SnapDistance)
                            draggedWindow.FlightRect.x = otherWindow.FlightRect.xMax;

                    // Snap to the right edge
                    if (Mathf.Abs(draggedWindow.FlightRect.xMax - otherWindow.FlightRect.xMax) < settings.SnapDistance)
                            draggedWindow.FlightRect.x = otherWindow.FlightRect.xMax - draggedWindow.FlightRect.width;

                    // Snap to the top edge
                    if (Mathf.Abs(draggedWindow.FlightRect.yMin - otherWindow.FlightRect.yMin) < settings.SnapDistance)
                            draggedWindow.FlightRect.y = otherWindow.FlightRect.yMin;

                    // Snap to the bottom edge
                    if (Mathf.Abs(draggedWindow.FlightRect.yMax - otherWindow.FlightRect.yMin) < settings.SnapDistance)
                            draggedWindow.FlightRect.y = otherWindow.FlightRect.yMin - draggedWindow.FlightRect.height;

                    // Snap to the top edge
                    if (Mathf.Abs(draggedWindow.FlightRect.yMin - otherWindow.FlightRect.yMax) < settings.SnapDistance)
                            draggedWindow.FlightRect.y = otherWindow.FlightRect.yMax;

                    // Snap to the bottom edge
                    if (Mathf.Abs(draggedWindow.FlightRect.yMax - otherWindow.FlightRect.yMax) < settings.SnapDistance)
                            draggedWindow.FlightRect.y = otherWindow.FlightRect.yMax - draggedWindow.FlightRect.height;
                }
            }
        }
    }
}