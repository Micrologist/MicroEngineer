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

		public Rect mainGuiRect, settingsGuiRect, vesGuiRect, orbGuiRect, surGuiRect, fltGuiRect, manGuiRect, tgtGuiRect, stgGuiRect;
		private Rect closeBtnRect;		

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
		
		public bool popoutSettings, popoutVes, popoutOrb, popoutSur, popoutMan, popoutTgt, popoutFlt, popoutStg;

		private double totalDrag, totalLift;

		private static readonly List<Type> liftForces = new()
		{
			PhysicsForceDisplaySystem.MODULE_DRAG_BODY_LIFT_TYPE,
			PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_LIFT_TYPE
		};

		private static readonly List<Type> dragForces = new()
		{
			PhysicsForceDisplaySystem.MODULE_DRAG_DRAG_TYPE,
			PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_DRAG_TYPE
		};

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
			InitializeRects();
			ResetLayout();
			// load window positions and states from disk, if file exists
			LoadLayoutState();
		}

		private void InitializeRects()
		{
			mainGuiRect = settingsGuiRect = vesGuiRect = orbGuiRect = surGuiRect = fltGuiRect = manGuiRect = tgtGuiRect = stgGuiRect = new();

            closeBtnRect = new Rect(MicroStyles.WindowWidth - 23, 6, 16, 16);

            foreach (var (window, index) in MicroWindows.Select((window, index) => (window, index)))
            {
                microWindowRects.Add(new Rect(20 + index * 290, index * 100, 290, 100));
            }
        }

		private void ResetLayout()
		{
			popoutVes = popoutStg = popoutOrb = popoutSur = popoutFlt = popoutTgt = popoutMan = popoutSettings = false;
			mainGuiRect.position = new(Screen.width * 0.8f, Screen.height * 0.2f);
			Vector2 popoutWindowPosition = new(Screen.width * 0.6f, Screen.height * 0.2f);
			vesGuiRect.position = popoutWindowPosition;
			stgGuiRect.position = popoutWindowPosition;
			orbGuiRect.position = popoutWindowPosition;
			surGuiRect.position = popoutWindowPosition;
			fltGuiRect.position = popoutWindowPosition;
			tgtGuiRect.position = popoutWindowPosition;
			manGuiRect.position = popoutWindowPosition;
			settingsGuiRect.position = popoutWindowPosition;
		}

		private void OnGUI()
		{
            GUI.skin = MicroStyles.SpaceWarpUISkin;

            MicroUtility.RefreshActiveVesselAndCurrentManeuver();

			if (!showGUI || MicroUtility.ActiveVessel == null) return;			

			mainGuiRect = GUILayout.Window(
				GUIUtility.GetControlID(FocusType.Passive),
				mainGuiRect,
				FillMainGUI,
				"<color=#696DFF>// MICRO ENGINEER</color>",
                MicroStyles.MainWindowStyle,
				GUILayout.Height(0)
			);
			mainGuiRect.position = ClampToScreen(mainGuiRect.position, mainGuiRect.size);

			if (showSettings && popoutSettings)
			{
				DrawPopoutWindow(ref settingsGuiRect, FillSettings);
			}

			if (showVes && popoutVes)
			{
				DrawPopoutWindow(ref vesGuiRect, FillVessel);
			}

			if (showOrb && popoutOrb)
			{
				DrawPopoutWindow(ref orbGuiRect, FillOrbital);
			}

			if (showSur && popoutSur)
			{
				DrawPopoutWindow(ref surGuiRect, FillSurface);
			}

			if (showFlt && popoutFlt)
			{
				DrawPopoutWindow(ref fltGuiRect, FillFlight);
			}

			if (showTgt && popoutTgt && MicroUtility.ActiveVessel.TargetObject != null)
			{
				DrawPopoutWindow(ref tgtGuiRect, FillTarget);
			}

			if (showMan && popoutMan && MicroUtility.CurrentManeuver != null)
			{
				DrawPopoutWindow(ref manGuiRect, FillManeuver);
			}

			if (showStg && popoutStg)
			{
				DrawPopoutWindow(ref stgGuiRect, FillStages);
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

        private void DrawPopoutWindow(ref Rect guiRect, UnityEngine.GUI.WindowFunction fillAction)
		{
			guiRect = GUILayout.Window(
				GUIUtility.GetControlID(FocusType.Passive),
				guiRect,
				fillAction,
				"",
                MicroStyles.PopoutWindowStyle,
				GUILayout.Height(0),
				GUILayout.Width(MicroStyles.WindowWidth)
			);
			guiRect.position = ClampToScreen(guiRect.position, guiRect.size);
		}

		private Vector2 ClampToScreen(Vector2 position, Vector2 size)
		{
			float x = Mathf.Clamp(position.x, 0, Screen.width - size.x);
			float y = Mathf.Clamp(position.y, 0, Screen.height - size.y);
			return new Vector2(x, y);
		}

		private void FillMainGUI(int windowID)
		{
			if (CloseButton())
			{
				CloseWindow();
			}

			GUILayout.Space(10);

			GUILayout.BeginHorizontal();
			showVes = GUILayout.Toggle(showVes, "<b>VES</b>", MicroStyles.SectionToggleStyle);
			GUILayout.Space(26);
			showStg = GUILayout.Toggle(showStg, "<b>STG</b>", MicroStyles.SectionToggleStyle);
			GUILayout.Space(26);
			showOrb = GUILayout.Toggle(showOrb, "<b>ORB</b>", MicroStyles.SectionToggleStyle);
			GUILayout.Space(26);
			showSur = GUILayout.Toggle(showSur, "<b>SUR</b>", MicroStyles.SectionToggleStyle);
			GUILayout.Space(26);
			showFlt = GUILayout.Toggle(showFlt, "<b>FLT</b>", MicroStyles.SectionToggleStyle);
			GUILayout.Space(26);
			showTgt = GUILayout.Toggle(showTgt, "<b>TGT</b>", MicroStyles.SectionToggleStyle);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			showMan = GUILayout.Toggle(showMan, "<b>MAN</b>", MicroStyles.SectionToggleStyle);
			GUILayout.Space(26);
			showSettings = GUILayout.Toggle(showSettings, "<b>SET</b>", MicroStyles.SectionToggleStyle);
			GUILayout.EndHorizontal();


			GUILayout.Space(-3);

			GUILayout.BeginHorizontal();
			GUILayout.EndHorizontal();

			if (showSettings && !popoutSettings)
			{
				FillSettings();
			}

			if (showVes && !popoutVes)
			{
				FillVessel();
			}

			if (showStg && !popoutStg)
			{
				FillStages();
			}

			if (showOrb && !popoutOrb)
			{
				FillOrbital();
			}

			if (showSur && !popoutSur)
			{
				FillSurface();
			}

			if (showFlt && !popoutFlt)
			{
				FillFlight();
			}

			if (showTgt && !popoutTgt && MicroUtility.ActiveVessel.TargetObject != null)
			{
				FillTarget();
			}

			if (showMan && !popoutMan && MicroUtility.CurrentManeuver != null)
			{
				FillManeuver();
			}

			GUI.DragWindow(new Rect(0, 0, MicroStyles.WindowWidth, MicroStyles.WindowHeight));
		}

		private void FillSettings(int _ = 0)
		{
			DrawSectionHeader("Settings", ref popoutSettings);

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("SAVE LAYOUT", MicroStyles.SaveLoadBtnStyle))
				SaveLayoutState();
			GUILayout.Space(5);
			if (GUILayout.Button("LOAD LAYOUT", MicroStyles.SaveLoadBtnStyle))
				LoadLayoutState();
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


            DrawSectionEnd(popoutSettings);
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

        private void FillVessel(int _ = 0)
		{
			DrawSectionHeader("Vessel", ref popoutVes, MicroUtility.ActiveVessel.DisplayName);

			DrawEntry("Mass", $"{MicroUtility.ActiveVessel.totalMass * 1000:N0}", "kg");

			VesselDeltaVComponent deltaVComponent = MicroUtility.ActiveVessel.VesselDeltaV;
			if (deltaVComponent != null)
			{
				DrawEntry("∆v", $"{deltaVComponent.TotalDeltaVActual:N0}", "m/s");
				if (deltaVComponent.StageInfo.FirstOrDefault()?.DeltaVinVac > 0.0001 || deltaVComponent.StageInfo.FirstOrDefault()?.DeltaVatASL > 0.0001)
				{
					DrawEntry("Thrust", $"{deltaVComponent.StageInfo.FirstOrDefault()?.ThrustActual * 1000:N0}", "N");
					DrawEntry("TWR", $"{deltaVComponent.StageInfo.FirstOrDefault()?.TWRActual:N2}");
				}
			}

			DrawSectionEnd(popoutVes);
		}

		private void FillStages(int _ = 0)
		{
			DrawStagesHeader(ref popoutStg);

			List<DeltaVStageInfo> stages = MicroUtility.ActiveVessel.VesselDeltaV?.StageInfo;

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

			DrawSectionEnd(popoutStg);
		}

		private void FillOrbital(int _ = 0)
		{
			DrawSectionHeader("Orbital", ref popoutOrb);

			DrawEntry("Apoapsis", $"{MicroUtility.MetersToDistanceString(MicroUtility.ActiveVessel.Orbit.ApoapsisArl)}", "m");
			DrawEntry("Time to Ap.", $"{MicroUtility.SecondsToTimeString((MicroUtility.ActiveVessel.Situation == VesselSituations.Landed || MicroUtility.ActiveVessel.Situation == VesselSituations.PreLaunch) ? 0f : MicroUtility.ActiveVessel.Orbit.TimeToAp)}", "s");
			DrawEntry("Periapsis", $"{MicroUtility.MetersToDistanceString(MicroUtility.ActiveVessel.Orbit.PeriapsisArl)}", "m");
			DrawEntry("Time to Pe.", $"{MicroUtility.SecondsToTimeString(MicroUtility.ActiveVessel.Orbit.TimeToPe)}", "s");
			DrawEntry("Inclination", $"{MicroUtility.ActiveVessel.Orbit.inclination:N3}", "°");
			DrawEntry("Eccentricity", $"{MicroUtility.ActiveVessel.Orbit.eccentricity:N3}");
			DrawEntry("Period", $"{MicroUtility.SecondsToTimeString(MicroUtility.ActiveVessel.Orbit.period)}", "s");
			double secondsToSoiTransition = MicroUtility.ActiveVessel.Orbit.UniversalTimeAtSoiEncounter - GameManager.Instance.Game.UniverseModel.UniversalTime;
			if (secondsToSoiTransition >= 0)
			{
				DrawEntry("SOI Trans.", MicroUtility.SecondsToTimeString(secondsToSoiTransition), "s");
			}
			DrawSectionEnd(popoutOrb);
		}

		private void FillSurface(int _ = 0)
		{
			DrawSectionHeader("Surface", ref popoutSur, MicroUtility.ActiveVessel.mainBody.bodyName);

			DrawEntry("Situation", MicroUtility.SituationToString(MicroUtility.ActiveVessel.Situation));
			DrawEntry("Latitude", $"{MicroUtility.DegreesToDMS(MicroUtility.ActiveVessel.Latitude)}", MicroUtility.ActiveVessel.Latitude < 0 ? "S" : "N");
			DrawEntry("Longitude", $"{MicroUtility.DegreesToDMS(MicroUtility.ActiveVessel.Longitude)}", MicroUtility.ActiveVessel.Longitude < 0 ? "W" : "E");
			DrawEntry("Biome", MicroUtility.BiomeToString(MicroUtility.ActiveVessel.SimulationObject.Telemetry.SurfaceBiome));
			DrawEntry("Alt. MSL", MicroUtility.MetersToDistanceString(MicroUtility.ActiveVessel.AltitudeFromSeaLevel), "m");
			DrawEntry("Alt. AGL", MicroUtility.MetersToDistanceString(MicroUtility.ActiveVessel.AltitudeFromScenery), "m");
			DrawEntry("Horizontal Vel.", $"{MicroUtility.ActiveVessel.HorizontalSrfSpeed:N1}", "m/s");
			DrawEntry("Vertical Vel.", $"{MicroUtility.ActiveVessel.VerticalSrfSpeed:N1}", "m/s");

			DrawSectionEnd(popoutSur);
		}

		private void FillFlight(int _ = 0)
		{
			DrawSectionHeader("Flight", ref popoutFlt);

			DrawEntry("Speed", $"{MicroUtility.ActiveVessel.SurfaceVelocity.magnitude:N1}", "m/s");
			DrawEntry("Mach Number", $"{MicroUtility.ActiveVessel.SimulationObject.Telemetry.MachNumber:N2}");
			DrawEntry("Atm. Density", $"{MicroUtility.ActiveVessel.SimulationObject.Telemetry.AtmosphericDensity:N3}", "g/L");
			GetAeroStats();

			DrawEntry("Total Lift", $"{totalLift * 1000:N0}", "N");
			DrawEntry("Total Drag", $"{totalDrag * 1000:N0}", "N");

			DrawEntry("Lift / Drag", $"{totalLift / totalDrag:N3}");

			DrawSectionEnd(popoutFlt);
		}

		private void FillTarget(int _ = 0)
		{
			DrawSectionHeader("Target", ref popoutTgt, MicroUtility.ActiveVessel.TargetObject.DisplayName);

			if (MicroUtility.ActiveVessel.TargetObject.Orbit != null)
			{
				DrawEntry("Target Ap.", MicroUtility.MetersToDistanceString(MicroUtility.ActiveVessel.TargetObject.Orbit.ApoapsisArl), "m");
				DrawEntry("Target Pe.", MicroUtility.MetersToDistanceString(MicroUtility.ActiveVessel.TargetObject.Orbit.PeriapsisArl), "m");

				if (MicroUtility.ActiveVessel.Orbit.referenceBody == MicroUtility.ActiveVessel.TargetObject.Orbit.referenceBody)
				{
					double distanceToTarget = (MicroUtility.ActiveVessel.Orbit.Position - MicroUtility.ActiveVessel.TargetObject.Orbit.Position).magnitude;
					DrawEntry("Distance", MicroUtility.MetersToDistanceString(distanceToTarget), "m");
					double relativeVelocity = (MicroUtility.ActiveVessel.Orbit.relativeVelocity - MicroUtility.ActiveVessel.TargetObject.Orbit.relativeVelocity).magnitude;
					DrawEntry("Rel. Speed", $"{relativeVelocity:N1}", "m/s");
					OrbitTargeter targeter = MicroUtility.ActiveVessel.Orbiter.OrbitTargeter;
					DrawEntry("Rel. Incl.", $"{targeter.AscendingNodeTarget.Inclination:N3}", "°");
				}
			}
			DrawSectionEnd(popoutTgt);
		}

		private void FillManeuver(int _ = 0)
		{
			DrawSectionHeader("Maneuver", ref popoutMan);
			PatchedConicsOrbit newOrbit = MicroUtility.ActiveVessel.Orbiter.ManeuverPlanSolver.PatchedConicsList.FirstOrDefault();
			DrawEntry("Projected Ap.", MicroUtility.MetersToDistanceString(newOrbit.ApoapsisArl), "m");
			DrawEntry("Projected Pe.", MicroUtility.MetersToDistanceString(newOrbit.PeriapsisArl), "m");
            double requiredDVremaining = (MicroUtility.ActiveVessel.Orbiter.ManeuverPlanSolver.GetVelocityAfterFirstManeuver(out double ut).vector - MicroUtility.ActiveVessel.Orbit.GetOrbitalVelocityAtUTZup(ut)).magnitude;
            DrawEntry("∆v required", $"{requiredDVremaining:N1}", "m/s");
			double timeUntilNode = MicroUtility.CurrentManeuver.Time - GameManager.Instance.Game.UniverseModel.UniversalTime;
			DrawEntry("Time to", MicroUtility.SecondsToTimeString(timeUntilNode), "s");
			DrawEntry("Burn Time", MicroUtility.SecondsToTimeString(MicroUtility.CurrentManeuver.BurnDuration), "s");

			DrawSectionEnd(popoutMan);
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

		private void DrawStagesHeader(ref bool isPopout)
		{
			GUILayout.BeginHorizontal();
			isPopout = isPopout ? !CloseButton() : GUILayout.Button("⇖", MicroStyles.PopoutBtnStyle);

			GUILayout.Label("<b>Stage</b>");
			GUILayout.FlexibleSpace();
			GUILayout.Label("∆v", MicroStyles.TableHeaderLabelStyle);
			GUILayout.Space(16);
			GUILayout.Label($"TWR", MicroStyles.TableHeaderLabelStyle, GUILayout.Width(40));
			GUILayout.Space(16);
			if (isPopout)
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
			return GUI.Button(closeBtnRect, "x", MicroStyles.CloseBtnStyle);
		}

		private void CloseWindow()
		{
			GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
			showGUI = false;
		}

		private void GetAeroStats()
		{
			totalDrag = 0.0;
			totalLift = 0.0;

			IEnumerable<PartComponent> parts = MicroUtility.ActiveVessel?.SimulationObject?.PartOwner?.Parts;
			if (parts == null || !MicroUtility.ActiveVessel.IsInAtmosphere)
			{
				return;
			}

			foreach (PartComponent part in parts)
			{
				foreach (IForce force in part.SimulationObject.Rigidbody.Forces)
				{
					if (dragForces.Contains(force.GetType()))
					{
						totalDrag += force.RelativeForce.magnitude;
					}
					if (liftForces.Contains(force.GetType()))
					{
						totalLift += force.RelativeForce.magnitude;
					}
				}
			}
		}

		private void SaveLayoutState()
		{
			LayoutState state = new(this);
			state.Save();
		}

		private void LoadLayoutState()
		{
			LayoutState state = LayoutState.Load();

			if (state != null)
			{
				showSettings = false;
				showVes = state.ShowVes;
				showOrb = state.ShowOrb;
				showSur = state.ShowSur;
				showFlt = state.ShowFlt;
				showMan = state.ShowMan;
				showTgt = state.ShowTgt;
				showStg = state.ShowStg;
				popoutSettings = state.IsPopoutSettings;
				popoutVes = state.IsPopoutVes;
				popoutOrb = state.IsPopoutOrb;
				popoutSur = state.IsPopoutSur;
				popoutFlt = state.IsPopOutFlt;
				popoutMan = state.IsPopOutMan;
				popoutTgt = state.IsPopOutTgt;
				popoutStg = state.IsPopOutStg;
				mainGuiRect.position = state.MainGuiPosition;
				settingsGuiRect.position = state.SettingsPosition;
				vesGuiRect.position = state.VesPosition;
				orbGuiRect.position = state.OrbPosition;
				surGuiRect.position = state.SurPosition;
				fltGuiRect.position = state.FltPosition;
				manGuiRect.position = state.ManPosition;
				tgtGuiRect.position = state.TgtPosition;
				stgGuiRect.position = state.StgPosition;
			}
		}

		/// <summary>
		/// Builds the initial list of all Entries
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

		private void InitializeWindows()
		{
			MicroWindows = new List<MicroWindow>();

			try
			{
				MicroWindows.Add(new MicroWindow
				{
					Name = "Vessel",
					Description = "Vessel entries",
					IsEditorActive = false,
					IsFlightActive = true,
					IsMapActive = false,
					IsEditorPoppedOut = false,
					IsFlightPoppedOut = false,
					IsMapPoppedOut = false,
					IsLocked = false,
					IsEditable = true,
					MainWindow = MainWindow.Flight,
					EditorPosition = null,
					FlightPosition = null,
					Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Vessel).ToList()
				});

                MicroWindows.Add(new MicroWindow
                {
                    Name = "Orbital",
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
                    EditorPosition = null,
                    FlightPosition = null,
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Orbital).ToList()
                });

                MicroWindows.Add(new MicroWindow
                {
                    Name = "Surface",
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
                    EditorPosition = null,
                    FlightPosition = null,
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Surface).ToList()
                });

                MicroWindows.Add(new MicroWindow
                {
                    Name = "Flight",
                    Description = "Flight entries",
                    IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    IsEditable = true,
                    MainWindow = MainWindow.Flight,
                    EditorPosition = null,
                    FlightPosition = null,
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Flight).ToList()
                });

                MicroWindows.Add(new MicroWindow
                {
                    Name = "Target",
                    Description = "Flight entries",
                    IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    IsEditable = true,
                    MainWindow = MainWindow.Target,
                    EditorPosition = null,
                    FlightPosition = null,
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Target).ToList()
                });

                MicroWindows.Add(new MicroWindow
                {
                    Name = "Maneuver",
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
                    EditorPosition = null,
                    FlightPosition = null,
                    Entries = Enumerable.Where(MicroEntries, entry => entry.Category == MicroEntryCategory.Maneuver).ToList()
                });

                MicroWindows.Add(new MicroWindow
                {
                    Name = "Stage",
                    Description = "Stage entries",
                    IsEditorActive = false,
                    IsFlightActive = true,
                    IsMapActive = false,
                    IsEditorPoppedOut = false,
                    IsFlightPoppedOut = false,
                    IsMapPoppedOut = false,
                    IsLocked = false,
                    IsEditable = true,
                    MainWindow = MainWindow.Maneuver,
                    EditorPosition = null,
                    FlightPosition = null,
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