using BepInEx;
using KSP.Game;
using KSP.Sim.impl;
using UnityEngine;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI;
using SpaceWarp.API.UI.Appbar;
using KSP.Sim.Maneuver;
using KSP.UI.Binding;
using KSP.Sim.DeltaV;
using KSP.Sim;
using KSP.UI.Flight;
using static KSP.Rendering.Planets.PQSData;

namespace MicroMod
{
	[BepInPlugin("com.micrologist.microengineer", "MicroEngineer", "0.5.0")]
	[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
	public class MicroEngineerMod : BaseSpaceWarpPlugin
	{
		private bool showGUI = false;
		
		#region Editing window
		private bool showEditWindow = false;
		private int selectedWindowId = 0;
		private MicroEntryCategory selectedCategory = MicroEntryCategory.Vessel;
		private (bool condition, int index) showTooltip = (false, 0);
		#endregion

		/// <summary>
		/// Holds all entries we can have in any window
		/// </summary>
		private List<MicroEntry> MicroEntries;
		
		/// <summary>
		/// All windows that can be rendered
		/// </summary>
		private List<MicroWindow> MicroWindows;

        public override void OnInitialized()
		{
			Appbar.RegisterAppButton(
					"Micro Engineer",
					"BTN-MicroEngineerBtn",
					AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"),
					delegate { showGUI = !showGUI; }
			);

			MicroStyles.InitializeStyles();
			InitializeEntries();
			InitializeWindows();
			
			// load window positions and states from disk, if file exists
			MicroUtility.LoadLayout(MicroWindows);
		}
		
        public void Update()
        {
            MicroUtility.RefreshActiveVesselAndCurrentManeuver();
            if (MicroUtility.ActiveVessel == null) return;

            // Grab all entries from all active windows and refresh their data
			foreach (MicroEntry entry in MicroWindows
				.Where(w => w.IsFlightActive)
				.SelectMany(w => w.Entries ?? Enumerable.Empty<MicroEntry>()).ToList())
                entry.RefreshData();
        }
		
		private void OnGUI()
		{
            GUI.skin = MicroStyles.SpaceWarpUISkin;

			if (!showGUI || MicroUtility.ActiveVessel == null) return;

            MicroWindow mainGui = MicroWindows.Find(window => window.MainWindow == MainWindow.MainGui);
			
			// Draw main GUI that contains docked windows
            mainGui.FlightRect = GUILayout.Window(
				GUIUtility.GetControlID(FocusType.Passive),
                mainGui.FlightRect,
				FillMainGUI,
				"<color=#696DFF>// MICRO ENGINEER</color>",
                MicroStyles.MainWindowStyle,
				GUILayout.Height(0)
			);
            mainGui.FlightRect.position = MicroUtility.ClampToScreen(mainGui.FlightRect.position, mainGui.FlightRect.size);

            // Draw all other popped out windows
            foreach (var (window, index) in MicroWindows
				.Select((window, index) => (window, index))
				.Where(x => x.window.IsFlightActive && x.window.IsFlightPoppedOut) // must be active & popped out
				.Where(x => x.window.MainWindow != MainWindow.Settings && x.window.MainWindow != MainWindow.Stage && x.window.MainWindow != MainWindow.MainGui)) // MainGUI, Settings and Stage are special, they'll be drawn separately
			{
				// Skip drawing of Target window if there's no active target
				if (window.MainWindow == MainWindow.Target && !MicroUtility.TargetExists())
					continue;

				// Skip drawing of Maneuver window if there's no active maneuver
				if (window.MainWindow == MainWindow.Maneuver && !MicroUtility.ManeuverExists())
					continue;

				// If window is locked set alpha to 80%
				if (window.IsLocked)
					GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.8f);

				window.FlightRect = GUILayout.Window(
					index,
                    window.FlightRect,
					DrawPopoutWindow,
					"",
					MicroStyles.PopoutWindowStyle,
					GUILayout.Height(0),
					GUILayout.Width(MicroStyles.WindowWidth
					));

				// Set alpha back to 100%
                if (window.IsLocked)
                    GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1);

                window.FlightRect.position = MicroUtility.ClampToScreen(window.FlightRect.position, window.FlightRect.size);
            }

			// Draw popped out Settings
            int settingsIndex = MicroWindows.FindIndex(window => window.MainWindow == MainWindow.Settings);
            if (MicroWindows[settingsIndex].IsFlightActive && MicroWindows[settingsIndex].IsFlightPoppedOut)
			{
                MicroWindows[settingsIndex].FlightRect = GUILayout.Window(
					settingsIndex,
                    MicroWindows[settingsIndex].FlightRect,
					DrawSettingsWindow,
					"",
					MicroStyles.PopoutWindowStyle,
					GUILayout.Height(0),
					GUILayout.Width(MicroStyles.WindowWidth)
					);

                MicroWindows[settingsIndex].FlightRect.position = MicroUtility.ClampToScreen(MicroWindows[settingsIndex].FlightRect.position, MicroWindows[settingsIndex].FlightRect.size);
            }

            // Draw popped out Stages
            int stageIndex = MicroWindows.FindIndex(window => window.MainWindow == MainWindow.Stage);
            if (MicroWindows[stageIndex].IsFlightActive && MicroWindows[stageIndex].IsFlightPoppedOut)
            {
                MicroWindows[stageIndex].FlightRect = GUILayout.Window(
                    stageIndex,
                    MicroWindows[stageIndex].FlightRect,
                    DrawStages,
                    "",
                    MicroStyles.PopoutWindowStyle,
                    GUILayout.Height(0),
                    GUILayout.Width(MicroStyles.WindowWidth)
					);

                MicroWindows[stageIndex].FlightRect.position = MicroUtility.ClampToScreen(MicroWindows[stageIndex].FlightRect.position, MicroWindows[stageIndex].FlightRect.size);
            }

			// Draw Edit Window
			if (showEditWindow)
			{
				MicroStyles.EditWindowRect = GUILayout.Window(
					GUIUtility.GetControlID(FocusType.Passive),
					MicroStyles.EditWindowRect,
					DrawEditWindow,
					"",
					MicroStyles.EditWindowStyle
					);
            }
        }

        /// <summary>
        /// Draws the main GUI with all windows that are toggled and docked
        /// </summary>
        /// <param name="windowID"></param>
        private void FillMainGUI(int windowID)
        {
            try
            {
                if (CloseButton())
                {
                    CloseWindow();
                }

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();

                // Draw toggles for all windows except MainGui
                foreach (var (window, index) in MicroWindows.Select((window, index) => (window, index)).Where(x => x.window.MainWindow != MainWindow.MainGui))
                {
                    // layout can fit 6 toggles, so if all 6 slots are filled then go to a new line. Index == 0 is the MainGUI which isn't rendered
                    if ((index - 1) % 6 == 0 && index > 1)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }
                    window.IsFlightActive = GUILayout.Toggle(window.IsFlightActive, window.Abbreviation, MicroStyles.SectionToggleStyle);
                    GUILayout.Space(26);
                }
                GUILayout.EndHorizontal();

                // Draw Settings window first
                int settingsIndex = MicroWindows.FindIndex(window => window.MainWindow == MainWindow.Settings);
                if (MicroWindows[settingsIndex].IsFlightActive && !MicroWindows[settingsIndex].IsFlightPoppedOut)
                    DrawSettingsWindow(settingsIndex);

                // Draw Stage window next
                int stageIndex = MicroWindows.FindIndex(window => window.MainWindow == MainWindow.Stage);
                if (MicroWindows[stageIndex].IsFlightActive && !MicroWindows[stageIndex].IsFlightPoppedOut)
                    DrawStages(stageIndex);

                // Draw all other windows
                foreach (var (window, index) in MicroWindows
                    .Select((window, index) => (window, index))
                    .Where(x => x.window.IsFlightActive && !x.window.IsFlightPoppedOut) // must be active & docked
                    .Where(x => x.window.MainWindow != MainWindow.Settings && x.window.MainWindow != MainWindow.Stage && x.window.MainWindow != MainWindow.MainGui)) // MainGUI, Settings and Stage are special, they'll be drawn separately

                {
                    // Skip drawing of Target window if there's no active target
                    if (window.MainWindow == MainWindow.Target && !MicroUtility.TargetExists())
                        continue;

                    // Skip drawing of Maneuver window if there's no active maneuver
                    if (window.MainWindow == MainWindow.Maneuver && !MicroUtility.ManeuverExists())
                        continue;

                    DrawSectionHeader(window.Name, ref window.IsFlightPoppedOut, window.IsLocked, "");

                    foreach (MicroEntry entry in window.Entries)
                        DrawEntry(entry.Name, entry.ValueDisplay, entry.Unit);

                    DrawSectionEnd(window);
                }

                GUI.DragWindow(new Rect(0, 0, MicroStyles.WindowWidth, MicroStyles.WindowHeight));

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

		/// <summary>
        /// Draws all windows that are toggled and popped out
        /// </summary>
        /// <param name="windowIndex"></param>
        private void DrawPopoutWindow(int windowIndex)
        {
			MicroWindow windowToDraw = MicroWindows[windowIndex];

            DrawSectionHeader(windowToDraw.Name, ref windowToDraw.IsFlightPoppedOut, windowToDraw.IsLocked, "");

            foreach (MicroEntry entry in windowToDraw.Entries)
				DrawEntry(entry.Name, entry.ValueDisplay, entry.Unit);

            DrawSectionEnd(windowToDraw);
        }

		private void DrawSettingsWindow(int windowIndex)
		{
			MicroWindow windowToDraw = MicroWindows[windowIndex];

            DrawSectionHeader(windowToDraw.Name, ref windowToDraw.IsFlightPoppedOut, windowToDraw.IsLocked, "");

            GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("SAVE LAYOUT", MicroStyles.NormalBtnStyle))
				MicroUtility.SaveLayout(MicroWindows);
			GUILayout.Space(5);
			if (GUILayout.Button("LOAD LAYOUT", MicroStyles.NormalBtnStyle))
				MicroUtility.LoadLayout(MicroWindows);			
			GUILayout.Space(5);
			if (GUILayout.Button("RESET", MicroStyles.NormalBtnStyle))
				ResetLayout();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Edit Windows", MicroStyles.NormalBtnStyle))
			{
				showEditWindow = !showEditWindow;
			}
			GUILayout.EndHorizontal();

            DrawSectionEnd(windowToDraw);
		}

		private void DrawStages(int windowIndex)
		{
            MicroWindow windowToDraw = MicroWindows[windowIndex];

            DrawStagesHeader(windowToDraw);

			List<DeltaVStageInfo> stages = (List<DeltaVStageInfo>)windowToDraw.Entries.Find(entry => entry.Name == "Stage Info").EntryValue;

			int stageCount = stages?.Count ?? 0;
			if (stages != null && stageCount > 0)
			{
				float highestTwr = Mathf.Floor(stages.Max(stage => stage.TWRActual));
				int preDecimalDigits = Mathf.FloorToInt(Mathf.Log10(highestTwr)) + 1;
				string twrFormatString = "N2";

				if (preDecimalDigits == 3)
				{
					twrFormatString = "N1";
				}
				else if (preDecimalDigits == 4)
				{
					twrFormatString = "N0";
				}

				for (int i = stages.Count - 1; i >= 0; i--)
				{

					DeltaVStageInfo stageInfo = stages[i];
					if (stageInfo.DeltaVinVac > 0.0001 || stageInfo.DeltaVatASL > 0.0001)
					{
						int stageNum = stageCount - stageInfo.Stage;
						DrawStageEntry(stageNum, stageInfo, twrFormatString);
					}
				}
			}

			DrawSectionEnd(windowToDraw);
		}

		private void DrawSectionHeader(string sectionName, ref bool isPopout, bool isLocked, string value = "")
		{
			GUILayout.BeginHorizontal();
			
			// If window is popped out and it's not locked => show the close button. If it's not popped out => show to popup arrow
			isPopout = isPopout && !isLocked ? !CloseButton() : !isPopout ? GUILayout.Button("⇖", MicroStyles.PopoutBtnStyle) : isPopout;

            GUILayout.Label($"<b>{sectionName}</b>");
			GUILayout.FlexibleSpace();
			GUILayout.Label(value, MicroStyles.ValueLabelStyle);
			GUILayout.Space(5);
			GUILayout.Label("", MicroStyles.UnitLabelStyle);
			GUILayout.EndHorizontal();
			GUILayout.Space(MicroStyles.SpacingAfterHeader);
		}

		private void DrawStagesHeader(MicroWindow stageWindow)
		{
			GUILayout.BeginHorizontal();
			stageWindow.IsFlightPoppedOut = stageWindow.IsFlightPoppedOut ? !CloseButton() : GUILayout.Button("⇖", MicroStyles.PopoutBtnStyle);

			GUILayout.Label($"<b>{stageWindow.Name}</b>");
			GUILayout.FlexibleSpace();
			GUILayout.Label("∆v", MicroStyles.TableHeaderLabelStyle);
			GUILayout.Space(16);
			GUILayout.Label($"TWR", MicroStyles.TableHeaderLabelStyle, GUILayout.Width(40));
			GUILayout.Space(16);
			if (stageWindow.IsFlightPoppedOut)
			{
				GUILayout.Label($"<color=#{MicroStyles.UnitColorHex}>Burn</color>", GUILayout.Width(56));
			}
			else
			{
				GUILayout.Label($"Burn", MicroStyles.TableHeaderLabelStyle, GUILayout.Width(56));
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(MicroStyles.SpacingAfterHeader);
		}

		private void DrawEntry(string entryName, string value, string unit = "")
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(entryName, MicroStyles.NameLabelStyle);
			GUILayout.FlexibleSpace();
			GUILayout.Label(value, MicroStyles.ValueLabelStyle);
			GUILayout.Space(5);
			GUILayout.Label(unit, MicroStyles.UnitLabelStyle);
			GUILayout.EndHorizontal();
			GUILayout.Space(MicroStyles.SpacingAfterEntry);
		}

		private void DrawStageEntry(int stageID, DeltaVStageInfo stageInfo, string twrFormatString)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label($"{stageID:00.}", MicroStyles.NameLabelStyle, GUILayout.Width(24));
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{stageInfo.DeltaVActual:N0} <color=#{MicroStyles.UnitColorHex}>m/s</color>", MicroStyles.ValueLabelStyle);
			GUILayout.Space(16);
			GUILayout.Label($"{stageInfo.TWRActual.ToString(twrFormatString)}", MicroStyles.ValueLabelStyle, GUILayout.Width(40));
			GUILayout.Space(16);
			string burnTime = MicroUtility.SecondsToTimeString(stageInfo.StageBurnTime, false);
			string lastUnit = "s";
			if (burnTime.Contains('h'))
			{
				burnTime = burnTime.Remove(burnTime.LastIndexOf("<color"));
				lastUnit = "m";
			}
			if (burnTime.Contains('d'))
			{
				burnTime = burnTime.Remove(burnTime.LastIndexOf("<color"));
				lastUnit = "h";
			}

			GUILayout.Label($"{burnTime}<color=#{MicroStyles.UnitColorHex}>{lastUnit}</color>", MicroStyles.ValueLabelStyle, GUILayout.Width(56));
			GUILayout.EndHorizontal();
			GUILayout.Space(MicroStyles.SpacingAfterEntry);
		}

		private void DrawSectionEnd(MicroWindow window)
		{
			if (window.IsFlightPoppedOut)
			{
				if (!window.IsLocked)
					GUI.DragWindow(new Rect(0, 0, MicroStyles.WindowWidth, MicroStyles.WindowHeight));
				
				GUILayout.Space(MicroStyles.SpacingBelowPopout);
			}
			else
			{
				GUILayout.Space(MicroStyles.SpacingAfterSection);
			}
		}

        /// <summary>
        /// Window for edditing window contents. Add/Remove/Reorder entries.
        /// </summary>
        /// <param name="windowIndex"></param>
        private void DrawEditWindow(int windowIndex)
        {
            List<MicroWindow> editableWindows = MicroWindows.FindAll(w => w.IsEditable); // Editable windows are all except MainGUI, Settings and Stage
            List<MicroEntry> entriesByCategory = MicroEntries.FindAll(e => e.Category == selectedCategory); // All window entries belong to a category, but they can still be placed in any window

            showEditWindow = !CloseButton();

            #region Selection of window to be edited
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>EDITING WINDOW</b>", MicroStyles.TitleLabelStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<", MicroStyles.OneCharacterBtnStyle))
            {
                selectedWindowId = selectedWindowId > 0 ? selectedWindowId - 1 : editableWindows.Count - 1;
            }
            editableWindows[selectedWindowId].Abbreviation = GUILayout.TextField(editableWindows[selectedWindowId].Abbreviation, MicroStyles.WindowSelectionAbbrevitionTextFieldStyle);
            editableWindows[selectedWindowId].Abbreviation = MicroUtility.ValidateAbbreviation(editableWindows[selectedWindowId].Abbreviation);
            editableWindows[selectedWindowId].Name = GUILayout.TextField(editableWindows[selectedWindowId].Name, MicroStyles.WindowSelectionTextFieldStyle);
            if (GUILayout.Button(">", MicroStyles.OneCharacterBtnStyle))
            {
                selectedWindowId = selectedWindowId < editableWindows.Count - 1 ? selectedWindowId + 1 : 0;
            }
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(-10);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            editableWindows[selectedWindowId].IsLocked = GUILayout.Toggle(editableWindows[selectedWindowId].IsLocked, "Locked");
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            if (editableWindows[selectedWindowId].IsDeletable)
            {
                if (GUILayout.Button("DEL WINDOW", MicroStyles.NormalBtnStyle))
                {
                    MicroWindows.Remove(editableWindows[selectedWindowId]);
                    editableWindows.Remove(editableWindows[selectedWindowId]);
                    selectedWindowId--;
                }
            }
            if (GUILayout.Button("NEW WINDOW", MicroStyles.NormalBtnStyle))
                CreateCustomWindow(editableWindows);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            MicroStyles.DrawHorizontalLine();
            GUILayout.Space(10);

            #region Installed entries in the selected window
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>Installed</b>", MicroStyles.NormalLabelStyle);
            GUILayout.EndHorizontal();

            var entries = editableWindows[selectedWindowId].Entries.ToList();
            foreach (var (entry, index) in entries.Select((entry, index) => (entry, index)))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(entry.Name, MicroStyles.NameLabelStyle);
                if (GUILayout.Button("↑", MicroStyles.OneCharacterBtnStyle))
                {
                    if (index > 0)
                        editableWindows[selectedWindowId].MoveEntryUp(index);
                }
                if (GUILayout.Button("↓", MicroStyles.OneCharacterBtnStyle))
                {
                    if (index < editableWindows[selectedWindowId].Entries.Count - 1)
                        editableWindows[selectedWindowId].MoveEntryDown(index);
                }
                if (GUILayout.Button("X", MicroStyles.OneCharacterBtnStyle))
                    editableWindows[selectedWindowId].RemoveEntry(index);
                GUILayout.EndHorizontal();
            }
            #endregion

            GUILayout.Space(10);
            MicroStyles.DrawHorizontalLine();
            GUILayout.Space(10);

            #region All entries that can be added to any IsEditable window
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>Add</b>", MicroStyles.NormalLabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Category", MicroStyles.NormalLabelStyle);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("<", MicroStyles.OneCharacterBtnStyle))
            {
                selectedCategory = (int)selectedCategory > 0 ? selectedCategory - 1 : Enum.GetValues(typeof(MicroEntryCategory)).Cast<MicroEntryCategory>().Last();
            }
            GUILayout.Label(selectedCategory.ToString(), MicroStyles.NormalCenteredLabelStyle);
            if (GUILayout.Button(">", MicroStyles.OneCharacterBtnStyle))
            {
                selectedCategory = (int)selectedCategory < (int)Enum.GetValues(typeof(MicroEntryCategory)).Cast<MicroEntryCategory>().Last() ? selectedCategory + 1 : Enum.GetValues(typeof(MicroEntryCategory)).Cast<MicroEntryCategory>().First();
            }
            GUILayout.EndHorizontal();

            foreach (var (entry, index) in entriesByCategory.Select((entry, index) => (entry, index)))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(entry.Name, MicroStyles.NameLabelStyle);
                if (GUILayout.Button("?", MicroStyles.OneCharacterBtnStyle))
                {
                    if (!showTooltip.condition)
                        showTooltip = (true, index);
                    else
                    {
                        if (showTooltip.index != index)
                            showTooltip = (true, index);
                        else
                            showTooltip = (false, index);
                    }
                }
                if (GUILayout.Button("+", MicroStyles.OneCharacterBtnStyle))
                {
                    editableWindows[selectedWindowId].AddEntry(entry);
                }
                GUILayout.EndHorizontal();

                if (showTooltip.condition && showTooltip.index == index)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(entry.Description, MicroStyles.BlueLabelStyle);
                    GUILayout.EndHorizontal();
                }
            }
            #endregion

            GUI.DragWindow(new Rect(0, 0, MicroStyles.WindowWidth, MicroStyles.WindowHeight));
        }

        /// <summary>
        /// Creates a new custom window user can fill with any entry
        /// </summary>
        /// <param name="editableWindows"></param>
        private void CreateCustomWindow(List<MicroWindow> editableWindows)
        {
            // Default window's name will be CustomX where X represents the first not used integer
            int nameID = 1;
            foreach (MicroWindow window in editableWindows)
            {
                if (window.Name == "Custom" + nameID)
                    nameID++;
            }

            MicroWindow newWindow = new MicroWindow()
            {
                Name = "Custom" + nameID,
                Abbreviation = nameID.ToString().Length == 1 ? "Cu" + nameID : nameID.ToString().Length == 2 ? "C" + nameID : nameID.ToString(),
                Description = "",
                IsEditorActive = false,
                IsFlightActive = true,
                IsMapActive = false,
                IsEditorPoppedOut = false,
                IsFlightPoppedOut = false,
                IsMapPoppedOut = false,
                IsLocked = false,
                MainWindow = MainWindow.None,
                EditorRect = null,
                FlightRect = new Rect(MicroStyles.PoppedOutX, MicroStyles.PoppedOutY, MicroStyles.WindowWidth, MicroStyles.WindowHeight),
                Entries = new List<MicroEntry>()
            };

            MicroWindows.Add(newWindow);
            editableWindows.Add(newWindow);

            selectedWindowId = editableWindows.Count - 1;
        }

        private bool CloseButton()
		{
			return GUI.Button(MicroStyles.CloseBtnRect, "X", MicroStyles.CloseBtnStyle);
		}

		private void CloseWindow()
		{
			GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
			showGUI = false;
		}

        private void ResetLayout()
        {
            InitializeWindows();
        }

        /// <summary>
        /// Builds the list of all Entries
        /// </summary>
        private void InitializeEntries()
		{
			MicroEntries = new List<MicroEntry>();

            #region Vessel entries
            MicroEntries.Add(new Vessel());
            MicroEntries.Add(new Mass());
            MicroEntries.Add(new DeltaV());
            MicroEntries.Add(new Thrust());
            MicroEntries.Add(new TWR());
			#endregion
            #region Orbital entries
            MicroEntries.Add(new Apoapsis());
            MicroEntries.Add(new TimeToApoapsis());
            MicroEntries.Add(new Periapsis());
            MicroEntries.Add(new TimeToPeriapsis());
            MicroEntries.Add(new Inclination());
            MicroEntries.Add(new Eccentricity());
            MicroEntries.Add(new Period());
            MicroEntries.Add(new SoiTransition());
            #endregion
            #region Surface entries
            MicroEntries.Add(new Body());
            MicroEntries.Add(new Situation());
            MicroEntries.Add(new Latitude());
            MicroEntries.Add(new Longitude());
            MicroEntries.Add(new Biome());
            MicroEntries.Add(new AltitudeAsl());
            MicroEntries.Add(new AltitudeAgl());
            MicroEntries.Add(new HorizontalVelocity());
            MicroEntries.Add(new VerticalVelocity());
            #endregion
            #region Flight entries
            MicroEntries.Add(new Speed());
            MicroEntries.Add(new MachNumber());
            MicroEntries.Add(new AtmosphericDensity());
            MicroEntries.Add(new TotalLift());
            MicroEntries.Add(new TotalDrag());
            MicroEntries.Add(new LiftDivDrag());
            #endregion
            #region Flight entries
            MicroEntries.Add(new TargetApoapsis());
            MicroEntries.Add(new TargetPeriapsis());
            MicroEntries.Add(new DistanceToTarget());
            MicroEntries.Add(new RelativeSpeed());
            MicroEntries.Add(new RelativeInclination());
            #endregion
            #region Maneuver entries
            MicroEntries.Add(new ProjectedAp());
            MicroEntries.Add(new ProjectedPe());
            MicroEntries.Add(new DeltaVRequired());
            MicroEntries.Add(new TimeToNode());
            MicroEntries.Add(new BurnTime());
            #endregion
            #region Stage entries
            MicroEntries.Add(new TotalDeltaVVac());
            MicroEntries.Add(new TotalDeltaVAsl());
            MicroEntries.Add(new TotalDeltaVActual());
            MicroEntries.Add(new StageInfo());
            #endregion
            #region Misc entries
            MicroEntries.Add(new Separator());
            #endregion
        }

		/// <summary>
		/// Builds the default Windows and fills them with default Entries
		/// </summary>
		private void InitializeWindows()
		{
			MicroWindows = new List<MicroWindow>();

			try
			{
                MicroWindows.Add(new MicroWindow
                {
                    Name = "MainGui",
                    Abbreviation = null,
                    Description = "Main GUI",
                    IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false, // not relevant to Main GUI
                    IsFlightPoppedOut = false, // not relevant to Main GUI
                    IsMapPoppedOut = false, // not relevant to Main GUI
                    IsLocked = false,
                    MainWindow = MainWindow.MainGui,
                    EditorRect = null,
                    FlightRect = new Rect(MicroStyles.MainGuiX, MicroStyles.MainGuiY, MicroStyles.WindowWidth, MicroStyles.WindowHeight),
                    Entries = null
                });

                MicroWindows.Add(new MicroWindow
                {
                    Name = "Settings",
                    Abbreviation = "SET",
                    Description = "Settings",
                    IsEditorActive = false,
                    IsFlightActive = false,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Settings,
                    EditorRect = null,
                    FlightRect = new Rect(MicroStyles.PoppedOutX, MicroStyles.PoppedOutY, MicroStyles.WindowWidth, MicroStyles.WindowHeight),
                    Entries = null
                });

                MicroWindows.Add(new MicroWindow
				{
					Name = "Vessel",					
					Abbreviation = "VES",
					Description = "Vessel entries",
					IsEditorActive = false,
					IsFlightActive = true,
					IsMapActive = false,
					IsEditorPoppedOut = false,
					IsFlightPoppedOut = false,
					IsMapPoppedOut = false,
					IsLocked = false,
					MainWindow = MainWindow.Vessel,
					EditorRect = null,
					FlightRect = new Rect(MicroStyles.PoppedOutX, MicroStyles.PoppedOutY, MicroStyles.WindowWidth, MicroStyles.WindowHeight),
					Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Vessel).ToList()
				});

                MicroWindows.Add(new MicroWindow
                {
                    Name = "Orbital",
					Abbreviation = "ORB",
					Description = "Orbital entries",
					IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Orbital,
                    EditorRect = null,
                    FlightRect = new Rect(MicroStyles.PoppedOutX, MicroStyles.PoppedOutY, MicroStyles.WindowWidth, MicroStyles.WindowHeight),
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Orbital).ToList()
                });

                MicroWindows.Add(new MicroWindow
                {
                    Name = "Surface",
                    Abbreviation = "SUR",
                    Description = "Surface entries",
                    IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Surface,
                    EditorRect = null,
                    FlightRect = new Rect(MicroStyles.PoppedOutX, MicroStyles.PoppedOutY, MicroStyles.WindowWidth, MicroStyles.WindowHeight),
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Surface).ToList()
                });

                MicroWindows.Add(new MicroWindow
                {
                    Name = "Flight",
                    Abbreviation = "FLT",
                    Description = "Flight entries",
                    IsEditorActive = false,
                    IsFlightActive = false,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Flight,
                    EditorRect = null,
                    FlightRect = new Rect(MicroStyles.PoppedOutX, MicroStyles.PoppedOutY, MicroStyles.WindowWidth, MicroStyles.WindowHeight),
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Flight).ToList()
                });

                MicroWindows.Add(new MicroWindow
                {
                    Name = "Target",
                    Abbreviation = "TGT",
                    Description = "Flight entries",
                    IsEditorActive = false,
                    IsFlightActive = false,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Target,
                    EditorRect = null,
                    FlightRect = new Rect(MicroStyles.PoppedOutX, MicroStyles.PoppedOutY, MicroStyles.WindowWidth, MicroStyles.WindowHeight),
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Target).ToList()
                });

                MicroWindows.Add(new MicroWindow
                {
                    Name = "Maneuver",
                    Abbreviation = "MAN",
                    Description = "Maneuver entries",
                    IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Maneuver,
                    EditorRect = null,
                    FlightRect = new Rect(MicroStyles.PoppedOutX, MicroStyles.PoppedOutY, MicroStyles.WindowWidth, MicroStyles.WindowHeight),
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Maneuver).ToList()
                });

                MicroWindows.Add(new MicroWindow
                {
                    Name = "Stage",
                    Abbreviation = "STG",
                    Description = "Stage entries",
                    IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Stage,
                    EditorRect = null,
                    FlightRect = new Rect(MicroStyles.PoppedOutX, MicroStyles.PoppedOutY, MicroStyles.WindowWidth, MicroStyles.WindowHeight),
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Stage).ToList()
                });                
            }
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}
    }
}