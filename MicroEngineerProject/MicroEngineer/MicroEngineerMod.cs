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

		public bool showSettings = false;
		public bool showVes = true;
		public bool showOrb = true;
		public bool showSur = true;
		public bool showFlt = false;
		public bool showMan = true;
		public bool showTgt = false;
		public bool showStg = true;

		private bool showTestWindow = false; // TEMP - TO DELETE
		private bool showTestWindow2 = false;  // TEMP - TO DELETE
        private Rect testWindow = new Rect(100, 100, 300, 300); // TEMP - TO DELETE
		private List<Rect> microWindowRects = new List<Rect>(); // TEMP - TO DELETE
		
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

		private void ResetLayout()
		{
			InitializeWindows();
		}

		private void OnGUI()
		{
            GUI.skin = MicroStyles.SpaceWarpUISkin;

            MicroUtility.RefreshActiveVesselAndCurrentManeuver(); // TODO: move to Update()			

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
            mainGui.FlightRect.position = ClampToScreen(mainGui.FlightRect.position, mainGui.FlightRect.size);

            // Draw all other popped out windows
            foreach (var (window, index) in MicroWindows
				.Select((window, index) => (window, index))
				.Where(x => x.window.IsFlightActive && x.window.IsFlightPoppedOut) // must be active & popped out
				.Where(x => x.window.MainWindow != MainWindow.Settings && x.window.MainWindow != MainWindow.Stage && x.window.MainWindow != MainWindow.MainGui)) // MainGUI, Settings and Stage are special, they'll be drawn separately
			{
				window.FlightRect = GUILayout.Window(
					index,
                    window.FlightRect,
					DrawPopoutWindow,
					"",
					MicroStyles.PopoutWindowStyle,
					GUILayout.Height(0),
					GUILayout.Width(MicroStyles.WindowWidth
					));

				window.FlightRect.position = ClampToScreen(window.FlightRect.position, window.FlightRect.size);
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
					GUILayout.Width(MicroStyles.WindowWidth
					));

                MicroWindows[settingsIndex].FlightRect.position = ClampToScreen(MicroWindows[settingsIndex].FlightRect.position, MicroWindows[settingsIndex].FlightRect.size);
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
                    GUILayout.Width(MicroStyles.WindowWidth
                    ));

                MicroWindows[stageIndex].FlightRect.position = ClampToScreen(MicroWindows[stageIndex].FlightRect.position, MicroWindows[stageIndex].FlightRect.size);
            }

            //TEMP - TO DELETE
            if (showTestWindow)
			{
                testWindow = GUILayout.Window(0, testWindow, DrawTestWindow, "Test Window");
            }

            if (showTestWindow2)
            {
				MicroUtility.RefreshActiveVesselAndCurrentManeuver();

				foreach (var (window, index) in MicroWindows.Select((window, index) => (window, index)))
				{
					microWindowRects[index] = GUILayout.Window(index, microWindowRects[index], DrawMicroWindow, $"{window.Name}");
				}
			}
            //TEMP - END

        }

		private void DrawPopoutWindow(int windowIndex)
        {
			MicroWindow windowToDraw = MicroWindows[windowIndex];

            DrawSectionHeader(windowToDraw.Name, ref windowToDraw.IsFlightPoppedOut, "");

            foreach (MicroEntry entry in windowToDraw.Entries)
            {
                entry.RefreshData(); //TODO: move refreshing of data to Update()
                DrawEntry(entry.Name, entry.ValueDisplay, entry.Unit);
            }

            DrawSectionEnd(windowToDraw.IsFlightPoppedOut);
        }

		private Vector2 ClampToScreen(Vector2 position, Vector2 size)
		{
			float x = Mathf.Clamp(position.x, 0, Screen.width - size.x);
			float y = Mathf.Clamp(position.y, 0, Screen.height - size.y);
			return new Vector2(x, y);
		}

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
					// layout can fit 6 toggles, so if all 6 slots are filled then go to a new line. Index == 0 will also go to a new line
					if (index % 6 == 0 && index > 0)
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
					DrawSectionHeader(window.Name, ref window.IsFlightPoppedOut, "");

					foreach (MicroEntry entry in window.Entries)
					{
						entry.RefreshData(); //TODO: move refreshing of data to Update()
						DrawEntry(entry.Name, entry.ValueDisplay, entry.Unit);
					}

					DrawSectionEnd(window.IsFlightPoppedOut);
				}

				GUI.DragWindow(new Rect(0, 0, MicroStyles.WindowWidth, MicroStyles.WindowHeight));

			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void DrawSettingsWindow(int windowIndex)
		{
			MicroWindow windowToDraw = MicroWindows[windowIndex];

            DrawSectionHeader(windowToDraw.Name, ref windowToDraw.IsFlightPoppedOut, "");

            GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("SAVE LAYOUT", MicroStyles.SaveLoadBtnStyle))
				MicroUtility.SaveLayout(MicroWindows);
			GUILayout.Space(5);
			if (GUILayout.Button("LOAD LAYOUT", MicroStyles.SaveLoadBtnStyle))
				MicroUtility.LoadLayout(MicroWindows);
			GUILayout.Space(5);
			if (GUILayout.Button("RESET", MicroStyles.SaveLoadBtnStyle))
				ResetLayout();
			GUILayout.EndHorizontal();

			// TEMP START - TO BE DELETED
			GUILayout.BeginHorizontal();

			if (GUILayout.Button("EntriesTest", MicroStyles.SaveLoadBtnStyle))
			{
				showTestWindow = !showTestWindow;
            }

            if (GUILayout.Button("EntriesTest2", MicroStyles.SaveLoadBtnStyle))
            {
                showTestWindow2 = !showTestWindow2;
            }

            GUILayout.EndHorizontal();
            // TEMP END


            DrawSectionEnd(windowToDraw.IsFlightPoppedOut);
		}


		// TEMP START - TO DELETE
		public void DrawTestWindow(int windowID)
		{
			// Refresh ActiveVessel and CurrentManeuver refrences
			MicroUtility.RefreshActiveVesselAndCurrentManeuver();
			
			foreach (MicroEntry entry in MicroEntries)
			{
				// grab new data and draw it
				entry.RefreshData();
				
				DrawEntry(entry.Name, entry.ValueDisplay, entry.Unit);
            }

            MicroWindows[0].Entries.RemoveAt(0); // TEMP for test

            GUI.DragWindow(new Rect(0, 0, MicroStyles.WindowWidth, MicroStyles.WindowHeight));
        }

        public void DrawMicroWindow(int windowIndex)
        {
			MicroWindow windowToDraw = MicroWindows[windowIndex];

            foreach (MicroEntry entry in windowToDraw.Entries)
            {
                // grab new data and draw it
                entry.RefreshData();

                DrawEntry(entry.Name, entry.ValueDisplay, entry.Unit);
            }

            GUI.DragWindow(new Rect(0, 0, MicroStyles.WindowWidth, MicroStyles.WindowHeight));
        }

        // TEMP END

		private void DrawStages(int windowIndex)
		{
            MicroWindow windowToDraw = MicroWindows[windowIndex];

            windowToDraw.RefreshEntryData(); // TODO: move refresh to Update()

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

			DrawSectionEnd(windowToDraw.IsFlightPoppedOut);
		}

		private void DrawSectionHeader(string sectionName, ref bool isPopout, string value = "")
		{
			GUILayout.BeginHorizontal();
			isPopout = isPopout ? !CloseButton() : GUILayout.Button("⇖", MicroStyles.PopoutBtnStyle);

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

		private void DrawSectionEnd(bool isPopout)
		{
			if (isPopout)
			{
				GUI.DragWindow(new Rect(0, 0, MicroStyles.WindowWidth, MicroStyles.WindowHeight));
				GUILayout.Space(MicroStyles.SpacingBelowPopout);
			}
			else
			{
				GUILayout.Space(MicroStyles.SpacingAfterSection);
			}
		}

		private bool CloseButton()
		{
			return GUI.Button(MicroStyles.CloseBtnRect, "x", MicroStyles.CloseBtnStyle);
		}

		private void CloseWindow()
		{
			GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
			showGUI = false;
		}

		/// <summary>
		/// Builds the list of all Entries
		/// </summary>
		private void InitializeEntries()
		{
			MicroEntries = new List<MicroEntry>();

			InitializeVesselEntries();
			InitializeOrbitalEntries();
			InitializeSurfaceEntries();
			InitializeFlightEntries();
			InitializeTargetEntries();
			InitializeManeuverEntries();
			InitializeStageEntries();
        }

		private void InitializeVesselEntries()
		{
			MicroEntries.Add(new Vessel());
			MicroEntries.Add(new Mass());
            MicroEntries.Add(new DeltaV());
            MicroEntries.Add(new Thrust());
            MicroEntries.Add(new TWR());
        }

        private void InitializeOrbitalEntries()
        {
			MicroEntries.Add(new Apoapsis());
			MicroEntries.Add(new TimeToApoapsis());
			MicroEntries.Add(new Periapsis());
			MicroEntries.Add(new TimeToPeriapsis());
			MicroEntries.Add(new Inclination());
			MicroEntries.Add(new Eccentricity());
			MicroEntries.Add(new Period());
			MicroEntries.Add(new SoiTransition());
        }

		private void InitializeSurfaceEntries()
		{
			MicroEntries.Add(new Body());
			MicroEntries.Add(new Situation());
			MicroEntries.Add(new Latitude());
			MicroEntries.Add(new Longitude());
			MicroEntries.Add(new Biome());
			MicroEntries.Add(new AltitudeAsl());
			MicroEntries.Add(new AltitudeAgl());
			MicroEntries.Add(new HorizontalVelocity());
			MicroEntries.Add(new VerticalVelocity());
        }

		private void InitializeFlightEntries()
		{
			MicroEntries.Add(new Speed());
			MicroEntries.Add(new MachNumber());
			MicroEntries.Add(new AtmosphericDensity());
			MicroEntries.Add(new TotalLift());
			MicroEntries.Add(new TotalDrag());
			MicroEntries.Add(new LiftDivDrag());
        }

		private void InitializeTargetEntries()
		{
			MicroEntries.Add(new TargetApoapsis());
			MicroEntries.Add(new TargetPeriapsis());
			MicroEntries.Add(new DistanceToTarget());
			MicroEntries.Add(new RelativeSpeed());
			MicroEntries.Add(new RelativeInclination());
        }

		private void InitializeManeuverEntries()
		{
			MicroEntries.Add(new ProjectedAp());
			MicroEntries.Add(new ProjectedPe());
			MicroEntries.Add(new DeltaVRequired());
			MicroEntries.Add(new TimeToNode());
			MicroEntries.Add(new BurnTime());
        }

		private void InitializeStageEntries()
		{
			MicroEntries.Add(new TotalDeltaVVac());
			MicroEntries.Add(new TotalDeltaVAsl());
			MicroEntries.Add(new TotalDeltaVActual());
			MicroEntries.Add(new StageInfo());
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
					IsEditable = true,
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
                    IsEditable = true,
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
                    IsEditable = true,
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
                    IsEditable = true,
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
                    IsEditable = true,
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
                    IsEditable = true,
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
                    IsEditable = false,
                    MainWindow = MainWindow.Stage,
                    EditorRect = null,
                    FlightRect = new Rect(MicroStyles.PoppedOutX, MicroStyles.PoppedOutY, MicroStyles.WindowWidth, MicroStyles.WindowHeight),
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Stage).ToList()
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
                    IsEditable = false,
                    MainWindow = MainWindow.Settings,
                    EditorRect = null,
                    FlightRect = new Rect(MicroStyles.PoppedOutX, MicroStyles.PoppedOutY, MicroStyles.WindowWidth, MicroStyles.WindowHeight),
                    Entries = null
                });

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
                    IsEditable = false,
                    MainWindow = MainWindow.MainGui,
                    EditorRect = null,
                    FlightRect = new Rect(MicroStyles.MainGuiX, MicroStyles.MainGuiY, MicroStyles.WindowWidth, MicroStyles.WindowHeight),
                    Entries = null
                });
            }
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}
    }
}