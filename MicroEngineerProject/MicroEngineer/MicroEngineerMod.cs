using BepInEx;
using KSP.Game;
using UnityEngine;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI.Appbar;
using KSP.UI.Binding;
using KSP.Sim.DeltaV;
using KSP.Messages;
using KSP.Sim.impl;

namespace MicroMod
{
    [BepInPlugin("com.micrologist.microengineer", "MicroEngineer", "1.0.0")]
	[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
	public class MicroEngineerMod : BaseSpaceWarpPlugin
	{
        private bool _showGuiFlight;
        private bool _showGuiOAB;
        private bool _showGuiSettingsOAB;
		
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
		private List<BaseWindow> MicroWindows;

        /// <summary>
        /// Holds data on all bodies for calculating TWR (currently)
        /// </summary>
        private MicroCelestialBodies _celestialBodies = new();
        
        // Index of the stage for which user wants to select a different CelestialBody for different TWR calculations. -1 -> no stage is selected
        private int _celestialBodySelectionStageIndex = -1;

        // If game input is enabled or disabled (used for locking controls when user is editing a text field
        private bool _gameInputState = true;

        private float _lastUpdate = 0;

        public override void OnInitialized()
		{
            Styles.InitializeStyles();
			InitializeEntries();
			InitializeWindows();
            SubscribeToMessages();
            InitializeCelestialBodies();
			
			// Load window positions and states from disk, if file exists
			MicroUtility.LoadLayout(MicroWindows);

            BackwardCompatibilityInitializations();            

            // Register Flight and OAB buttons
            Appbar.RegisterAppButton(
                "Micro Engineer",
                "BTN-MicroEngineerBtn",
                AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"),
                isOpen =>
                {
                    _showGuiFlight = isOpen;
                    MicroWindows.Find(w => w.MainWindow == MainWindow.MainGui).IsFlightActive = isOpen;
                    GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(isOpen);
                });

            Appbar.RegisterOABAppButton(
                "Micro Engineer",
                "BTN-MicroEngineerOAB",
                AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"),
                isOpen =>
                {
                    _showGuiOAB = isOpen;
                    MicroWindows.Find(w => w.MainWindow == MainWindow.StageInfoOAB).IsEditorActive = isOpen;
                    GameObject.Find("BTN - MicroEngineerOAB")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(isOpen);
                });
        }

        private void BackwardCompatibilityInitializations()
        {
            // Preserve backward compatibility with 0.6.0. If user previously saved the layout and then upgraded without deleting the original folder, then StageInfoOAB window will be wiped by LoadLayout(). So, we add it manually now.
            if (MicroWindows.Find(w => w.MainWindow == MainWindow.StageInfoOAB) == null)
                InitializeStageInfoOABWindow();

            // Preserve backward compatibility with SpaceWarp 1.0.1
            if (MicroUtility.IsModOlderThan("SpaceWarp", 1, 1, 0))
            {
                Logger.LogInfo("Space Warp older version detected. Loading old Styles.");
                Styles.SetStylesForOldSpaceWarpSkin();
            }
            else
                Logger.LogInfo("Space Warp new version detected. Loading new Styles.");

            // Preserve backward compatibility with 0.7.2. If user previously saved the layout and then upgraded without deleting the original folder, then the Torque entry won't be in the loaded StageOAB window. So, we add it manually now.
            BaseWindow stageOabWindow = MicroWindows.Find(w => w.MainWindow == MainWindow.StageInfoOAB);
            if (stageOabWindow.Entries.Find(e => e.Name == "Torque") == null)
            {
                stageOabWindow.Entries.Add(this.MicroEntries.Find(e => e.Name == "Torque"));
                MicroUtility.SaveLayout(this.MicroWindows);
            }
        }

        /// <summary>
        /// Subscribe to Messages KSP2 is using
        /// </summary>
        private void SubscribeToMessages()
        {
            MicroUtility.RefreshGameManager();

            // While in OAB we use the VesselDeltaVCalculationMessage event to refresh data as it's triggered a lot less frequently than Update()
            MicroUtility.MessageCenter.Subscribe<VesselDeltaVCalculationMessage>(new Action<MessageCenterMessage>(this.RefreshStagingDataOAB));
            
            // We are loading layout state when entering Flight or OAB game state
            MicroUtility.MessageCenter.Subscribe<GameStateEnteredMessage>(new Action<MessageCenterMessage>(this.GameStateEntered));
            
            // We are saving layout state when exiting from Flight or OAB game state
            MicroUtility.MessageCenter.Subscribe<GameStateLeftMessage>(new Action<MessageCenterMessage>(this.GameStateLeft));

            // Sets the selected node index to the newly created node
            MicroUtility.MessageCenter.Subscribe<ManeuverCreatedMessage>(new Action<MessageCenterMessage>(this.OnManeuverCreatedMessage));
        }

        private void OnManeuverCreatedMessage(MessageCenterMessage message)
        {
            var maneuverWindow = MicroWindows.Find(w => w.GetType() == typeof(ManeuverWindow)) as ManeuverWindow;
            maneuverWindow.OnManeuverCreatedMessage(message);
        }

        private void GameStateEntered(MessageCenterMessage obj)
        {
            Logger.LogInfo("Message triggered: GameStateEnteredMessage");

            MicroUtility.RefreshGameManager();
            if (MicroUtility.GameState.GameState == GameState.FlightView || MicroUtility.GameState.GameState == GameState.VehicleAssemblyBuilder || MicroUtility.GameState.GameState == GameState.Map3DView)
            {
                MicroUtility.LoadLayout(MicroWindows);

                if(MicroUtility.GameState.GameState == GameState.FlightView || MicroUtility.GameState.GameState == GameState.Map3DView)
                    _showGuiFlight = MicroWindows.Find(w => w.MainWindow == MainWindow.MainGui).IsFlightActive;

                if(MicroUtility.GameState.GameState == GameState.VehicleAssemblyBuilder)
                {
                    _showGuiOAB = MicroWindows.Find(w => w.MainWindow == MainWindow.StageInfoOAB).IsEditorActive;
                    InitializeCelestialBodies();
                    _celestialBodySelectionStageIndex = -1;
                }
            }
        }

        private void GameStateLeft(MessageCenterMessage obj)
        {
            Logger.LogInfo("Message triggered: GameStateLeftMessage");

            MicroUtility.RefreshGameManager();
            if (MicroUtility.GameState.GameState == GameState.FlightView || MicroUtility.GameState.GameState == GameState.VehicleAssemblyBuilder || MicroUtility.GameState.GameState == GameState.Map3DView)
            {
                MicroUtility.SaveLayout(MicroWindows);

                if (MicroUtility.GameState.GameState == GameState.FlightView || MicroUtility.GameState.GameState == GameState.Map3DView)
                    _showGuiFlight = false;

                if (MicroUtility.GameState.GameState == GameState.VehicleAssemblyBuilder)
                    _showGuiOAB = false;
            }
        }

        #region Data refreshing
        /// <summary>
        /// Refresh all staging data while in OAB
        /// </summary>
        private void RefreshStagingDataOAB(MessageCenterMessage obj)
        {
            // Check if message originated from ships in flight. If yes, return.
            VesselDeltaVCalculationMessage msg = (VesselDeltaVCalculationMessage)obj;
            if (msg.DeltaVComponent.Ship == null || !msg.DeltaVComponent.Ship.IsLaunchAssembly()) return;

            MicroUtility.RefreshGameManager();
            if (MicroUtility.GameState.GameState != GameState.VehicleAssemblyBuilder) return;            

            MicroUtility.RefreshStagesOAB();

            BaseWindow stageWindow = MicroWindows.Find(w => w.MainWindow == MainWindow.StageInfoOAB);

            if (MicroUtility.VesselDeltaVComponentOAB?.StageInfo == null)
            {
                stageWindow.Entries.Find(e => e.Name == "Stage Info (OAB)").EntryValue = null;
                return;
            }

            foreach (var entry in stageWindow.Entries)
                entry.RefreshData();
        }

        public void Update()
        {
            MicroUtility.RefreshGameManager();

            // Perform OAB updates only if we're in OAB
            if (MicroUtility.GameState != null && MicroUtility.GameState.IsObjectAssembly)
            {
                // Do updates every 1 sec
                if (Time.time - _lastUpdate > 1)
                {
                    _lastUpdate = Time.time;

                    Torque torque = (Torque)MicroWindows.Find(w => w.MainWindow == MainWindow.StageInfoOAB).Entries.Find(e => e.Name == "Torque");
                    if (torque.IsActive)
                        torque.RefreshData();
                }
            }

            // Perform flight UI updates only if we're in Flight or Map view
            if (MicroUtility.GameState != null && (MicroUtility.GameState.GameState == GameState.FlightView || MicroUtility.GameState.GameState == GameState.Map3DView))
            {
                MicroUtility.RefreshActiveVesselAndCurrentManeuver();

                if (MicroUtility.ActiveVessel == null)
                    return;

                // Refresh all active windows' entries
                foreach (BaseWindow window in MicroWindows.Where(w => w.IsFlightActive))
                    window.RefreshData();
            }
        }
        #endregion

        private void OnGUI()
        {
            GUI.skin = Styles.SpaceWarpUISkin;

            MicroUtility.RefreshGameManager();
            if (MicroUtility.GameState?.GameState == GameState.VehicleAssemblyBuilder)
                OnGUI_OAB();
            else
                OnGUI_Flight();
        }

        #region Flight scene UI and logic
        private void OnGUI_Flight()
		{
            _gameInputState = MicroUtility.ToggleGameInputOnControlInFocus(_gameInputState, _showGuiFlight);

            if (!_showGuiFlight || MicroUtility.ActiveVessel == null) return;

            BaseWindow mainGui = MicroWindows.Find(window => window.MainWindow == MainWindow.MainGui);

			// Draw main GUI that contains docked windows
            mainGui.FlightRect = GUILayout.Window(
				GUIUtility.GetControlID(FocusType.Passive),
                mainGui.FlightRect,
				FillMainGUI,
				"<color=#696DFF>// MICRO ENGINEER</color>",
                Styles.MainWindowStyle,
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
					Styles.PopoutWindowStyle,
					GUILayout.Height(0),
					GUILayout.Width(Styles.WindowWidth
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
					Styles.PopoutWindowStyle,
					GUILayout.Height(0),
					GUILayout.Width(Styles.WindowWidth)
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
                    Styles.PopoutWindowStyle,
                    GUILayout.Height(0),
                    GUILayout.Width(Styles.WindowWidth)
					);

                MicroWindows[stageIndex].FlightRect.position = MicroUtility.ClampToScreen(MicroWindows[stageIndex].FlightRect.position, MicroWindows[stageIndex].FlightRect.size);
            }

			// Draw Edit Window
			if (showEditWindow)
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
        }
        
        /// <summary>
        /// Draws the main GUI with all windows that are toggled and docked
        /// </summary>
        /// <param name="windowID"></param>
        private void FillMainGUI(int windowID)
        {
            try
            {
                if (CloseButton(Styles.CloseBtnRect))
                {
                    CloseWindow();
                }

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();

                int toggleIndex = -1;
                // Draw toggles for all windows except MainGui and StageInfoOAB
                foreach (BaseWindow window in MicroWindows.Where(x => x.MainWindow != MainWindow.MainGui && x.MainWindow != MainWindow.StageInfoOAB))
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
                GUILayout.Space(5);

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

                    window.DrawWindowHeader();

                    foreach (MicroEntry entry in window.Entries)
                        DrawEntry(entry.Name, entry.ValueDisplay, entry.Unit);

                    window.DrawWindowFooter();

                    DrawSectionEnd(window);
                }

                GUI.DragWindow(new Rect(0, 0, Styles.WindowWidth, Styles.WindowHeight));

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        private int _selectedNode = 0;
		/// <summary>
        /// Draws all windows that are toggled and popped out
        /// </summary>
        /// <param name="windowIndex"></param>
        private void DrawPopoutWindow(int windowIndex)
        {
			BaseWindow windowToDraw = MicroWindows[windowIndex];

            DrawSectionHeader(windowToDraw.Name, ref windowToDraw.IsFlightPoppedOut, windowToDraw.IsLocked, "");

            windowToDraw.DrawWindowHeader();
            
            foreach (MicroEntry entry in windowToDraw.Entries)
                DrawEntry(entry.Name, entry.ValueDisplay, entry.Unit);

            windowToDraw.DrawWindowFooter();

            // TEMP - MOVE SOMEWHERE ELSE WHEN DONE
            /*
            if (windowToDraw.Name == "Maneuver")
            {
                if (GUILayout.Button("Delete"))
                {
                    // OLD - DOESN'T WORK
                    var activeVesselPlan2 = GameManager.Instance.Game.SpaceSimulation.Maneuvers.GetNodesForVessel(MicroUtility.ActiveVessel.GlobalId);
                    if (activeVesselPlan2 != null)
                    {
                        var node = activeVesselPlan2.FirstOrDefault();
                        if (node != null)
                        {
                            
                            GameManager.Instance.Game.SpaceSimulation.Maneuvers.RemoveNodesFromVessel(MicroUtility.ActiveVessel.GlobalId, new List<ManeuverNodeData> { node });
                        }
                    }

                    // THIS WORKS!
                    var _activeVesselPlan = MicroUtility.ActiveVessel.SimulationObject.FindComponent<ManeuverPlanComponent>();
                    List<ManeuverNodeData> nodeData = new List<ManeuverNodeData>();
                    nodeData.Add(_activeVesselPlan.ActiveNode);
                    
                    foreach (ManeuverNodeData node in _activeVesselPlan.GetNodes())
                    {
                        if (!nodeData.Contains(node) && (!_activeVesselPlan.ActiveNode.IsOnManeuverTrajectory || _activeVesselPlan.ActiveNode.Time < node.Time))
                            nodeData.Add(node);
                    }
                    GameManager.Instance.Game.SpaceSimulation.Maneuvers.RemoveNodesFromVessel(MicroUtility.ActiveVessel.GlobalId, nodeData);
                    


                    var _activeVesselPlan = MicroUtility.ActiveVessel.SimulationObject.FindComponent<ManeuverPlanComponent>();
                    List<ManeuverNodeData> nodeData = new List<ManeuverNodeData>();

                    var nodeToDelete = _activeVesselPlan.GetNodes()[_selectedNode];
                    nodeData.Add(nodeToDelete);

                    foreach (ManeuverNodeData node in _activeVesselPlan.GetNodes())
                    {
                        if (!nodeData.Contains(node) && (!nodeToDelete.IsOnManeuverTrajectory || nodeToDelete.Time < node.Time))
                            nodeData.Add(node);
                    }
                    GameManager.Instance.Game.SpaceSimulation.Maneuvers.RemoveNodesFromVessel(MicroUtility.ActiveVessel.GlobalId, nodeData);



                    //this.RemoveNodesFromVessel(this._activeVesselPlan.ActiveNode.RelatedSimID, nodeData);
                    //this.NormalizedManeuverTime = ManeuverProvider.NORMALIZED_MANEUVER_BOUND;
                }
            }*/

            // END TEMP
            DrawSectionEnd(windowToDraw);
        }
        private void DrawSettingsWindow(int windowIndex)
		{
			BaseWindow windowToDraw = MicroWindows[windowIndex];

            DrawSectionHeader(windowToDraw.Name, ref windowToDraw.IsFlightPoppedOut, windowToDraw.IsLocked, "");

            GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("SAVE LAYOUT", Styles.NormalBtnStyle))
				MicroUtility.SaveLayout(MicroWindows);
			GUILayout.Space(5);
			if (GUILayout.Button("LOAD LAYOUT", Styles.NormalBtnStyle))
				MicroUtility.LoadLayout(MicroWindows);			
			GUILayout.Space(5);
			if (GUILayout.Button("RESET", Styles.NormalBtnStyle))
				ResetLayout();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Edit Windows", Styles.NormalBtnStyle))
			{
				showEditWindow = !showEditWindow;
			}
			GUILayout.EndHorizontal();

            DrawSectionEnd(windowToDraw);
		}

		private void DrawStages(int windowIndex)
		{
            BaseWindow windowToDraw = MicroWindows[windowIndex];

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
			isPopout = isPopout && !isLocked ? !CloseButton(Styles.CloseBtnRect) : !isPopout ? GUILayout.Button("⇖", Styles.PopoutBtnStyle) : isPopout;

            GUILayout.Label($"<b>{sectionName}</b>");
			GUILayout.FlexibleSpace();
			GUILayout.Label(value, Styles.ValueLabelStyle);
			GUILayout.Space(5);
			GUILayout.Label("", Styles.UnitLabelStyle);
			GUILayout.EndHorizontal();
			GUILayout.Space(Styles.SpacingAfterHeader);
		}

		private void DrawStagesHeader(BaseWindow stageWindow)
		{
            GUILayout.BeginHorizontal();
			stageWindow.IsFlightPoppedOut = stageWindow.IsFlightPoppedOut ? !CloseButton(Styles.CloseBtnRect) : GUILayout.Button("⇖", Styles.PopoutBtnStyle);

			GUILayout.Label($"<b>{stageWindow.Name}</b>");
			GUILayout.FlexibleSpace();
			GUILayout.Label("∆v", Styles.TableHeaderLabelStyle);
			GUILayout.Space(16);
			GUILayout.Label($"TWR", Styles.TableHeaderLabelStyle, GUILayout.Width(40));
			GUILayout.Space(16);
			if (stageWindow.IsFlightPoppedOut)
			{
				GUILayout.Label($"<color=#{Styles.UnitColorHex}>Burn</color>", GUILayout.Width(56));
			}
			else
			{
				GUILayout.Label($"Burn", Styles.TableHeaderLabelStyle, GUILayout.Width(56));
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(Styles.SpacingAfterHeader);
		}

		private void DrawEntry(string entryName, string value, string unit = "")
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(entryName, Styles.NameLabelStyle);
			GUILayout.FlexibleSpace();
			GUILayout.Label(value, Styles.ValueLabelStyle);
			GUILayout.Space(5);
			GUILayout.Label(unit, Styles.UnitLabelStyle);
			GUILayout.EndHorizontal();
			GUILayout.Space(Styles.SpacingAfterEntry);
		}

		private void DrawStageEntry(int stageID, DeltaVStageInfo stageInfo, string twrFormatString)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label($"{stageID:00.}", Styles.NameLabelStyle, GUILayout.Width(24));
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{stageInfo.DeltaVActual:N0} <color=#{Styles.UnitColorHex}>m/s</color>", Styles.ValueLabelStyle);
			GUILayout.Space(16);
			GUILayout.Label($"{stageInfo.TWRActual.ToString(twrFormatString)}", Styles.ValueLabelStyle, GUILayout.Width(40));
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

			GUILayout.Label($"{burnTime}<color=#{Styles.UnitColorHex}>{lastUnit}</color>", Styles.ValueLabelStyle, GUILayout.Width(56));
			GUILayout.EndHorizontal();
			GUILayout.Space(Styles.SpacingAfterEntry);
		}

		private void DrawSectionEnd(BaseWindow window)
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
            List<BaseWindow> editableWindows = MicroWindows.FindAll(w => w.IsEditable); // Editable windows are all except MainGUI, Settings, Stage and StageInfoOAB
            List<MicroEntry> entriesByCategory = MicroEntries.FindAll(e => e.Category == selectedCategory); // All window stageInfoOabEntries belong to a category, but they can still be placed in any window

            showEditWindow = !CloseButton(Styles.CloseBtnRect);

            #region Selection of window to be edited
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>EDITING WINDOW</b>", Styles.TitleLabelStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<", Styles.OneCharacterBtnStyle))
            {
                selectedWindowId = selectedWindowId > 0 ? selectedWindowId - 1 : editableWindows.Count - 1;
            }
            GUI.SetNextControlName(MicroUtility.InputDisableWindowAbbreviation);
            editableWindows[selectedWindowId].Abbreviation = GUILayout.TextField(editableWindows[selectedWindowId].Abbreviation, Styles.WindowSelectionAbbrevitionTextFieldStyle);
            editableWindows[selectedWindowId].Abbreviation = MicroUtility.ValidateAbbreviation(editableWindows[selectedWindowId].Abbreviation);
            GUI.SetNextControlName(MicroUtility.InputDisableWindowName);
            editableWindows[selectedWindowId].Name = GUILayout.TextField(editableWindows[selectedWindowId].Name, Styles.WindowSelectionTextFieldStyle);
            if (GUILayout.Button(">", Styles.OneCharacterBtnStyle))
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
                if (GUILayout.Button("DEL WINDOW", Styles.NormalBtnStyle))
                {
                    MicroWindows.Remove(editableWindows[selectedWindowId]);
                    editableWindows.Remove(editableWindows[selectedWindowId]);
                    selectedWindowId--;
                }
            }
            if (GUILayout.Button("NEW WINDOW", Styles.NormalBtnStyle))
                CreateCustomWindow(editableWindows);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            Styles.DrawHorizontalLine();
            GUILayout.Space(10);

            #region Installed entries in the selected window
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>Installed</b>", Styles.NormalLabelStyle);
            GUILayout.EndHorizontal();

            var entries = editableWindows[selectedWindowId].Entries.ToList();
            foreach (var (entry, index) in entries.Select((entry, index) => (entry, index)))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(entry.Name, Styles.NameLabelStyle);
                if (GUILayout.Button("↑", Styles.OneCharacterBtnStyle))
                {
                    if (index > 0)
                        editableWindows[selectedWindowId].MoveEntryUp(index);
                }
                if (GUILayout.Button("↓", Styles.OneCharacterBtnStyle))
                {
                    if (index < editableWindows[selectedWindowId].Entries.Count - 1)
                        editableWindows[selectedWindowId].MoveEntryDown(index);
                }
                if (GUILayout.Button("X", Styles.OneCharacterBtnStyle))
                    editableWindows[selectedWindowId].RemoveEntry(index);
                GUILayout.EndHorizontal();
            }
            #endregion

            GUILayout.Space(10);
            Styles.DrawHorizontalLine();
            GUILayout.Space(10);

            #region All entries that can be added to any IsEditable window
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>Add</b>", Styles.NormalLabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Category", Styles.NormalLabelStyle);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("<", Styles.OneCharacterBtnStyle))
            {
                selectedCategory = (int)selectedCategory > 0 ? selectedCategory - 1 : Enum.GetValues(typeof(MicroEntryCategory)).Cast<MicroEntryCategory>().Last();
            }
            GUILayout.Label(selectedCategory.ToString(), Styles.NormalCenteredLabelStyle);
            if (GUILayout.Button(">", Styles.OneCharacterBtnStyle))
            {
                selectedCategory = (int)selectedCategory < (int)Enum.GetValues(typeof(MicroEntryCategory)).Cast<MicroEntryCategory>().Last() ? selectedCategory + 1 : Enum.GetValues(typeof(MicroEntryCategory)).Cast<MicroEntryCategory>().First();
            }
            GUILayout.EndHorizontal();

            foreach (var (entry, index) in entriesByCategory.Select((entry, index) => (entry, index)))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(entry.Name, Styles.NameLabelStyle);
                if (GUILayout.Button("?", Styles.OneCharacterBtnStyle))
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
                if (GUILayout.Button("+", Styles.OneCharacterBtnStyle))
                {
                    editableWindows[selectedWindowId].AddEntry(entry);
                }
                GUILayout.EndHorizontal();

                if (showTooltip.condition && showTooltip.index == index)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(entry.Description, Styles.BlueLabelStyle);
                    GUILayout.EndHorizontal();
                }
            }
            #endregion

            GUI.DragWindow(new Rect(0, 0, Styles.WindowWidth, Styles.WindowHeight));
        }

        /// <summary>
        /// Creates a new custom window user can fill with any entry
        /// </summary>
        /// <param name="editableWindows"></param>
        private void CreateCustomWindow(List<BaseWindow> editableWindows)
        {
            // Default window's name will be CustomX where X represents the first not used integer
            int nameID = 1;
            foreach (BaseWindow window in editableWindows)
            {
                if (window.Name == "Custom" + nameID)
                    nameID++;
            }

            BaseWindow newWindow = new BaseWindow()
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
                //EditorRect = null,
                FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                Entries = new List<MicroEntry>()
            };

            MicroWindows.Add(newWindow);
            editableWindows.Add(newWindow);

            selectedWindowId = editableWindows.Count - 1;
        }

        private void ResetLayout()
        {
            InitializeWindows();
        }

        private void CloseWindow()
        {
            GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
            _showGuiFlight = false;
        }

        #endregion
        
        #region OAB scene UI and logic
        private void OnGUI_OAB()
        {
            if (!_showGuiOAB) return;

            BaseWindow stageInfoOAB = MicroWindows.Find(w => w.MainWindow == MainWindow.StageInfoOAB);
            if (stageInfoOAB.Entries.Find(e => e.Name == "Stage Info (OAB)").EntryValue == null) return;

            stageInfoOAB.EditorRect = GUILayout.Window(
                GUIUtility.GetControlID(FocusType.Passive),
                stageInfoOAB.EditorRect,
                DrawStageInfoOAB,
                "",
                Styles.StageOABWindowStyle,
                GUILayout.Height(0)
                );
            stageInfoOAB.EditorRect.position = MicroUtility.ClampToScreen(stageInfoOAB.EditorRect.position, stageInfoOAB.EditorRect.size);

            // Draw window for selecting CelestialBody for a stage
            // -1 -> no selection of CelestialBody is taking place
            // any other int -> index represents the stage number for which the selection was clicked
            if (_celestialBodySelectionStageIndex > -1)
            {
                Rect stageInfoOabRect = MicroWindows.Find(w => w.MainWindow == MainWindow.StageInfoOAB).EditorRect;
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
            if(_showGuiSettingsOAB)
            {
                Rect stageInfoOabRect = MicroWindows.Find(w => w.MainWindow == MainWindow.StageInfoOAB).EditorRect;
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

        private void DrawStageInfoOAB(int windowID)
        {
            BaseWindow stageInfoOabWindow = MicroWindows.Find(w => w.MainWindow == MainWindow.StageInfoOAB);
            List<MicroEntry> stageInfoOabEntries = stageInfoOabWindow.Entries;

            GUILayout.BeginHorizontal();
            if (SettingsButton(Styles.SettingsOABRect))
                _showGuiSettingsOAB = !_showGuiSettingsOAB;

            if (CloseButton(Styles.CloseBtnStagesOABRect))
            {
                stageInfoOabWindow.IsEditorActive = false;
                _showGuiOAB = false;
            }
            GUILayout.Label($"<b>Stage Info</b>");
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
                GUILayout.Space(Styles.SpacingAfterEntry);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Torque", Styles.NameLabelStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(torque.ValueDisplay, Styles.ValueLabelStyle);
                GUILayout.Space(5);
                GUILayout.Label(torque.Unit, Styles.UnitLabelStyle);
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
            GUILayout.Space(Styles.SpacingAfterEntry);

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
                    stageInfoOab.AddNewCelestialBody(_celestialBodies);

                GUILayout.BeginHorizontal();
                GUILayout.Label(String.Format("{0:00}", ((List<DeltaVStageInfo_OAB>)stageInfoOab.EntryValue).Count - stages[stageIndex].Stage), Styles.NameLabelStyle, GUILayout.Width(40));
                GUILayout.FlexibleSpace();

                // We calculate what factor needs to be applied to TWR in order to compensate for different gravity of the selected celestial body                
                double twrFactor = _celestialBodies.GetTwrFactor(stageInfoOab.CelestialBodyForStage[celestialIndex]);
                GUILayout.Label(String.Format("{0:N2}", stages[stageIndex].TWRVac * twrFactor), Styles.ValueLabelStyle, GUILayout.Width(65));

                // Calculate Sea Level TWR and DeltaV
                CelestialBodyComponent cel = _celestialBodies.Bodies.Find(b => b.Name == stageInfoOab.CelestialBodyForStage[celestialIndex]).CelestialBodyComponent;                
                GUILayout.Label(String.Format("{0:N2}", stages[stageIndex].GetTWRAtSeaLevel(cel) * twrFactor), Styles.ValueLabelStyle, GUILayout.Width(75));
                GUILayout.Label(String.Format("{0:N0}", stages[stageIndex].GetDeltaVelAtSeaLevel(cel)), Styles.ValueLabelStyle, GUILayout.Width(75));
                GUILayout.Label("m/s", Styles.UnitLabelStyleStageOAB, GUILayout.Width(30));

                GUILayout.Label(String.Format("{0:N0}", stages[stageIndex].DeltaVVac), Styles.ValueLabelStyle, GUILayout.Width(75));
                GUILayout.Label("m/s", Styles.UnitLabelStyleStageOAB, GUILayout.Width(30));
                GUILayout.Label(MicroUtility.SecondsToTimeString(stages[stageIndex].StageBurnTime, true, true), Styles.ValueLabelStyle, GUILayout.Width(110));
                GUILayout.Space(20);
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(stageInfoOab.CelestialBodyForStage[celestialIndex], Styles.CelestialBodyBtnStyle))
                {
                    _celestialBodySelectionStageIndex = celestialIndex;
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(Styles.SpacingAfterEntry);
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

            foreach (var body in _celestialBodies.Bodies)
            {
                if (GUILayout.Button(body.DisplayName, Styles.CelestialSelectionBtnStyle))
                {
                    StageInfo_OAB stageInfoOab = (StageInfo_OAB)MicroWindows.Find(w => w.MainWindow == MainWindow.StageInfoOAB).Entries.Find(e => e.Name == "Stage Info (OAB)");
                    stageInfoOab.CelestialBodyForStage[_celestialBodySelectionStageIndex] = body.Name;

                    // Hide the selection window
                    _celestialBodySelectionStageIndex = -1;
                }
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Opens a Settings window for OAB
        /// </summary>
        private void DrawSettingsOabWindow(int id)
        {
            if (CloseButton(Styles.CloseBtnSettingsOABRect))
                _showGuiSettingsOAB = false;

            BaseWindow stageInfoOabWindow = MicroWindows.Find(w => w.MainWindow == MainWindow.StageInfoOAB);
            List<MicroEntry> stageInfoOabEntries = stageInfoOabWindow.Entries;
            Torque torqueEntry = (Torque)stageInfoOabEntries.Find(e => e.Name == "Torque");

            torqueEntry.IsActive = GUILayout.Toggle(torqueEntry.IsActive, "Display Torque (experimental)\nTurn on CoT & CoM for this", Styles.SectionToggleStyle);
        }
        #endregion

        /// <summary>
        /// Draws a close button (X)
        /// </summary>
        /// <param name="rect">Where to position the close button</param>
        /// <returns></returns>
        private bool CloseButton(Rect rect)
        {
            return GUI.Button(rect, "X", Styles.CloseBtnStyle);
        }

        /// <summary>
        /// Draws a Settings butoon (≡)
        /// </summary>
        /// <param name="settingsOABRect"></param>
        /// <returns></returns>
        private bool SettingsButton(Rect rect)
        {
            return GUI.Button(rect, "≡", Styles.SettingsBtnStyle);
        }

        #region Window and data initialization
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
            #region OAB entries
            MicroEntries.Add(new TotalBurnTime_OAB());
            MicroEntries.Add(new TotalDeltaVASL_OAB());
            MicroEntries.Add(new TotalDeltaVActual_OAB());
            MicroEntries.Add(new TotalDeltaVVac_OAB());
            MicroEntries.Add(new Torque());
            MicroEntries.Add(new StageInfo_OAB());
            #endregion

            #region New entries
            MicroEntries.Add(new AltitudeFromScenery());
            MicroEntries.Add(new AtmosphericTemperature());
            MicroEntries.Add(new DynamicPressure_kPa());
            MicroEntries.Add(new ExposedArea());
            MicroEntries.Add(new ExternalTemperature());
            MicroEntries.Add(new FuelPercentage());
            MicroEntries.Add(new Heading());
            MicroEntries.Add(new Pitch_HorizonRelative());
            MicroEntries.Add(new Roll_HorizonRelative());
            MicroEntries.Add(new Yaw_HorizonRelative());
            MicroEntries.Add(new DragCoefficient());
            MicroEntries.Add(new OrbitalSpeed());
            MicroEntries.Add(new SoundSpeed());
            MicroEntries.Add(new StageFuelPercentage());
            MicroEntries.Add(new StaticPressure_kPa());
            MicroEntries.Add(new TimeSinceLaunch());
            MicroEntries.Add(new TotalCommandCrewCapacity());
            MicroEntries.Add(new Zenith());
            MicroEntries.Add(new altimeterMode());
            MicroEntries.Add(new geeForce());
            MicroEntries.Add(new gravityForPos());
            MicroEntries.Add(new launchTime());
            MicroEntries.Add(new speedMode());
            MicroEntries.Add(new AutopilotStatus_IsEnabled());
            MicroEntries.Add(new AutopilotStatus_Mode());
            MicroEntries.Add(new EccentricAnomaly());
            MicroEntries.Add(new EndUT());
            MicroEntries.Add(new MeanAnomaly());
            MicroEntries.Add(new ObT());
            MicroEntries.Add(new ArgumentOfPeriapsis());
            MicroEntries.Add(new LongitudeOfAscendingNode());
            MicroEntries.Add(new SemiMajorAxis());
            MicroEntries.Add(new SemiMinorAxis());
            MicroEntries.Add(new OrbitalEnergy());
            MicroEntries.Add(new ReferenceBodyConstants_Radius());
            MicroEntries.Add(new ReferenceBodyConstants_StandardGravitationParameter());
            MicroEntries.Add(new SemiLatusRectum());
            MicroEntries.Add(new StartUT());
            MicroEntries.Add(new TrueAnomaly());
            MicroEntries.Add(new UniversalTimeAtClosestApproach());
            MicroEntries.Add(new UniversalTimeAtSoiEncounter());
            MicroEntries.Add(new orbitPercent());
            MicroEntries.Add(new radius());
            MicroEntries.Add(new ManeuverPrograde());
            MicroEntries.Add(new ManeuverNormal());
            MicroEntries.Add(new ManeuverRadial());




            #endregion
        }

        /// <summary>
        /// Builds the default Windows and fills them with default Entries
        /// </summary>
        private void InitializeWindows()
		{
			MicroWindows = new List<BaseWindow>();

			try
			{
                MicroWindows.Add(new BaseWindow
                {
                    Name = "MainGui",
                    Abbreviation = null,
                    Description = "Main GUI",
                    IsEditorActive = false,
                    IsFlightActive = false,
                    IsMapActive = false,
                    IsEditorPoppedOut = false, // not relevant to Main GUI
                    IsFlightPoppedOut = false, // not relevant to Main GUI
                    IsMapPoppedOut = false, // not relevant to Main GUI
                    IsLocked = false,
                    MainWindow = MainWindow.MainGui,
                    //EditorRect = null,
                    FlightRect = new Rect(Styles.MainGuiX, Styles.MainGuiY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = null
                });

                MicroWindows.Add(new BaseWindow
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
                    //EditorRect = null,
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = null
                });

                MicroWindows.Add(new BaseWindow
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
                    //EditorRect = null,
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
					Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Vessel).ToList()
				});

                MicroWindows.Add(new BaseWindow
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
                    //EditorRect = null,
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Orbital).ToList()
                });

                MicroWindows.Add(new BaseWindow
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
                    //EditorRect = null,
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Surface).ToList()
                });

                MicroWindows.Add(new BaseWindow
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
                    //EditorRect = null,
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Flight).ToList()
                });

                MicroWindows.Add(new BaseWindow
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
                    //EditorRect = null,
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Target).ToList()
                });

                MicroWindows.Add(new ManeuverWindow
                {
                    Name = "Maneuver",
                    Abbreviation = "MAN",
                    Description = "Maneuver entries",
                    WindowType = typeof(ManeuverWindow),
                    IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    MainWindow = MainWindow.Maneuver,
                    //EditorRect = null,
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Maneuver).ToList()
                });

                MicroWindows.Add(new BaseWindow
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
                    //EditorRect = null,
                    FlightRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, Styles.WindowWidth, Styles.WindowHeight),
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Stage).ToList()
                });

                InitializeStageInfoOABWindow();
            }
			catch (Exception ex)
			{
				Logger.LogError("Error creating a BaseWindow. Full exception: " + ex);
			}
		}

        private void InitializeStageInfoOABWindow()
        {
            MicroWindows.Add(new BaseWindow
            {
                Name = "Stage (OAB)",
                Abbreviation = "SOAB",
                Description = "Stage Info window for OAB",
                IsEditorActive = false,
                IsFlightActive = false, // Not used
                IsMapActive = false, // Not used
                IsEditorPoppedOut = true, // Not used
                IsFlightPoppedOut = false, // Not used
                IsMapPoppedOut = false, // Not used
                IsLocked = false, // Not used
                MainWindow = MainWindow.StageInfoOAB,
                EditorRect = new Rect(Styles.PoppedOutX, Styles.PoppedOutY, 0, 0),
                Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.OAB).ToList()
            });
        }

        private void InitializeCelestialBodies()
        {
            if (_celestialBodies.Bodies.Count > 0)
                return;

            _celestialBodies.GetBodies();
        }
        #endregion
    }
}