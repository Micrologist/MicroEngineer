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
using KSP.Messages.PropertyWatchers;

namespace MicroMod
{
	[BepInPlugin("com.micrologist.microengineer", "MicroEngineer", "0.5.0")]
	[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
	public class MicroEngineerMod : BaseSpaceWarpPlugin
	{
		private bool showGUI = false;

		private readonly int windowWidth = 290;
		private readonly int windowHeight = 700;
		public Rect mainGuiRect, settingsGuiRect, vesGuiRect, orbGuiRect, surGuiRect, fltGuiRect, manGuiRect, tgtGuiRect, stgGuiRect;
		private Rect closeBtnRect;

		private GUISkin _spaceWarpUISkin;
		private GUIStyle popoutBtnStyle;
		private GUIStyle mainWindowStyle;
		private GUIStyle popoutWindowStyle;
		private GUIStyle sectionToggleStyle;
		private GUIStyle closeBtnStyle;
		private GUIStyle saveLoadBtnStyle;
		private GUIStyle nameLabelStyle;
		private GUIStyle valueLabelStyle;
		private GUIStyle unitLabelStyle;
		private GUIStyle tableHeaderLabelStyle;

		private string unitColorHex;

		private int spacingAfterHeader = -12;
		private int spacingAfterEntry = -12;
		private int spacingAfterSection = 5;
		private float spacingBelowPopout = 10;

		public bool showSettings = false;
		public bool showVes = true;
		public bool showOrb = true;
		public bool showSur = true;
		public bool showFlt = false;
		public bool showMan = true;
		public bool showTgt = false;
		public bool showStg = true;

		private bool showTestWindow = false; // TEMP - TO DELETE
		private Rect testWindow = new Rect(100, 100, 300, 300); // TEMP - TO DELETE

		public bool popoutSettings, popoutVes, popoutOrb, popoutSur, popoutMan, popoutTgt, popoutFlt, popoutStg;

		private VesselComponent activeVessel;
		private SimulationObjectModel currentTarget;
		private ManeuverNodeData currentManeuver;

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

		public override void OnInitialized()
		{
			_spaceWarpUISkin = Skins.ConsoleSkin;

			mainWindowStyle = new GUIStyle(_spaceWarpUISkin.window)
			{
				padding = new RectOffset(8, 8, 20, 8),
				contentOffset = new Vector2(0, -22),
				fixedWidth = windowWidth
			};

			popoutWindowStyle = new GUIStyle(mainWindowStyle)
			{
				padding = new RectOffset(mainWindowStyle.padding.left, mainWindowStyle.padding.right, 0, mainWindowStyle.padding.bottom - 5),
				fixedWidth = windowWidth
			};

			popoutBtnStyle = new GUIStyle(_spaceWarpUISkin.button)
			{
				alignment = TextAnchor.MiddleCenter,
				contentOffset = new Vector2(0, 2),
				fixedHeight = 15,
				fixedWidth = 15,
				fontSize = 28,
				clipping = TextClipping.Overflow,
				margin = new RectOffset(0, 0, 10, 0)
			};

			sectionToggleStyle = new GUIStyle(_spaceWarpUISkin.toggle)
			{
				padding = new RectOffset(14, 0, 3, 3)
			};

			nameLabelStyle = new GUIStyle(_spaceWarpUISkin.label);
			nameLabelStyle.normal.textColor = new Color(.7f, .75f, .75f, 1);

			valueLabelStyle = new GUIStyle(_spaceWarpUISkin.label)
			{
				alignment = TextAnchor.MiddleRight
			};
			valueLabelStyle.normal.textColor = new Color(.6f, .7f, 1, 1);

			unitLabelStyle = new GUIStyle(valueLabelStyle)
			{
				fixedWidth = 24,
				alignment = TextAnchor.MiddleLeft
			};
			unitLabelStyle.normal.textColor = new Color(.7f, .75f, .75f, 1);

			unitColorHex = ColorUtility.ToHtmlStringRGBA(unitLabelStyle.normal.textColor);

			closeBtnStyle = new GUIStyle(_spaceWarpUISkin.button)
			{
				fontSize = 8
			};

			saveLoadBtnStyle = new GUIStyle(_spaceWarpUISkin.button)
			{
				alignment = TextAnchor.MiddleCenter
			};

			closeBtnRect = new Rect(windowWidth - 23, 6, 16, 16);

			tableHeaderLabelStyle = new GUIStyle(nameLabelStyle) { alignment = TextAnchor.MiddleRight };

			Appbar.RegisterAppButton(
					"Micro Engineer",
					"BTN-MicroEngineerBtn",
					AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"),
					delegate { showGUI = !showGUI; }
			);


			InitializeRects();
			ResetLayout();
			// load window positions and states from disk, if file exists
			LoadLayoutState();
		}

		private void InitializeRects()
		{
			mainGuiRect = settingsGuiRect = vesGuiRect = orbGuiRect = surGuiRect = fltGuiRect = manGuiRect = tgtGuiRect = stgGuiRect = new();
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
			activeVessel = GameManager.Instance?.Game?.ViewController?.GetActiveVehicle(true)?.GetSimVessel(true);
			if (!showGUI || activeVessel == null) return;

			currentTarget = activeVessel.TargetObject;
			currentManeuver = GameManager.Instance?.Game?.SpaceSimulation.Maneuvers.GetNodesForVessel(activeVessel.GlobalId).FirstOrDefault();
			GUI.skin = _spaceWarpUISkin;

			mainGuiRect = GUILayout.Window(
				GUIUtility.GetControlID(FocusType.Passive),
				mainGuiRect,
				FillMainGUI,
				"<color=#696DFF>// MICRO ENGINEER</color>",
				mainWindowStyle,
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

			if (showTgt && popoutTgt && currentTarget != null)
			{
				DrawPopoutWindow(ref tgtGuiRect, FillTarget);
			}

			if (showMan && popoutMan && currentManeuver != null)
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
			//TEMP - END

		}

		private void DrawPopoutWindow(ref Rect guiRect, UnityEngine.GUI.WindowFunction fillAction)
		{
			guiRect = GUILayout.Window(
				GUIUtility.GetControlID(FocusType.Passive),
				guiRect,
				fillAction,
				"",
				popoutWindowStyle,
				GUILayout.Height(0),
				GUILayout.Width(windowWidth)
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
			showVes = GUILayout.Toggle(showVes, "<b>VES</b>", sectionToggleStyle);
			GUILayout.Space(26);
			showStg = GUILayout.Toggle(showStg, "<b>STG</b>", sectionToggleStyle);
			GUILayout.Space(26);
			showOrb = GUILayout.Toggle(showOrb, "<b>ORB</b>", sectionToggleStyle);
			GUILayout.Space(26);
			showSur = GUILayout.Toggle(showSur, "<b>SUR</b>", sectionToggleStyle);
			GUILayout.Space(26);
			showFlt = GUILayout.Toggle(showFlt, "<b>FLT</b>", sectionToggleStyle);
			GUILayout.Space(26);
			showTgt = GUILayout.Toggle(showTgt, "<b>TGT</b>", sectionToggleStyle);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			showMan = GUILayout.Toggle(showMan, "<b>MAN</b>", sectionToggleStyle);
			GUILayout.Space(26);
			showSettings = GUILayout.Toggle(showSettings, "<b>SET</b>", sectionToggleStyle);
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

			if (showTgt && !popoutTgt && currentTarget != null)
			{
				FillTarget();
			}

			if (showMan && !popoutMan && currentManeuver != null)
			{
				FillManeuver();
			}

			GUI.DragWindow(new Rect(0, 0, windowWidth, windowHeight));
		}

		private void FillSettings(int _ = 0)
		{
			DrawSectionHeader("Settings", ref popoutSettings);

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("SAVE LAYOUT", saveLoadBtnStyle))
				SaveLayoutState();
			GUILayout.Space(5);
			if (GUILayout.Button("LOAD LAYOUT", saveLoadBtnStyle))
				LoadLayoutState();
			GUILayout.Space(5);
			if (GUILayout.Button("RESET", saveLoadBtnStyle))
				ResetLayout();
			GUILayout.EndHorizontal();

			// TEMP START - TO BE DELETED
			GUILayout.BeginHorizontal();

			if (GUILayout.Button("EntriesTest", saveLoadBtnStyle))
			{
				showTestWindow = !showTestWindow;
            }

			GUILayout.EndHorizontal();
            // TEMP END


            DrawSectionEnd(popoutSettings);
		}


		// TEMP START - TO DELETE
		public void DrawTestWindow(int windowID)
		{
			InitializeEntries();
			
			foreach (MicroEntry entry in MicroEntries)
			{
				DrawEntry(entry.Name, entry.ValueDisplay, entry.Unit);
                
				//GUILayout.BeginHorizontal();
				//GUILayout.Label(entry.Name);
				//GUILayout.Space(5);
				//GUILayout.Label(entry.ValueDisplay);
				//GUILayout.Space(5);
				//GUILayout.Label(entry.Unit);
				//GUILayout.EndHorizontal();
            }

            GUI.DragWindow(new Rect(0, 0, windowWidth, windowHeight));
        }

		// TEMP END

        private void FillVessel(int _ = 0)
		{
			DrawSectionHeader("Vessel", ref popoutVes, activeVessel.DisplayName);

			DrawEntry("Mass", $"{activeVessel.totalMass * 1000:N0}", "kg");

			VesselDeltaVComponent deltaVComponent = activeVessel.VesselDeltaV;
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

			List<DeltaVStageInfo> stages = activeVessel.VesselDeltaV?.StageInfo;

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

			DrawEntry("Apoapsis", $"{MetersToDistanceString(activeVessel.Orbit.ApoapsisArl)}", "m");
			DrawEntry("Time to Ap.", $"{SecondsToTimeString((activeVessel.Situation == VesselSituations.Landed || activeVessel.Situation == VesselSituations.PreLaunch) ? 0f : activeVessel.Orbit.TimeToAp)}", "s");
			DrawEntry("Periapsis", $"{MetersToDistanceString(activeVessel.Orbit.PeriapsisArl)}", "m");
			DrawEntry("Time to Pe.", $"{SecondsToTimeString(activeVessel.Orbit.TimeToPe)}", "s");
			DrawEntry("Inclination", $"{activeVessel.Orbit.inclination:N3}", "°");
			DrawEntry("Eccentricity", $"{activeVessel.Orbit.eccentricity:N3}");
			DrawEntry("Period", $"{SecondsToTimeString(activeVessel.Orbit.period)}", "s");
			double secondsToSoiTransition = activeVessel.Orbit.UniversalTimeAtSoiEncounter - GameManager.Instance.Game.UniverseModel.UniversalTime;
			if (secondsToSoiTransition >= 0)
			{
				DrawEntry("SOI Trans.", SecondsToTimeString(secondsToSoiTransition), "s");
			}
			DrawSectionEnd(popoutOrb);
		}

		private void FillSurface(int _ = 0)
		{
			DrawSectionHeader("Surface", ref popoutSur, activeVessel.mainBody.bodyName);

			DrawEntry("Situation", SituationToString(activeVessel.Situation));
			DrawEntry("Latitude", $"{DegreesToDMS(activeVessel.Latitude)}", activeVessel.Latitude < 0 ? "S" : "N");
			DrawEntry("Longitude", $"{DegreesToDMS(activeVessel.Longitude)}", activeVessel.Longitude < 0 ? "W" : "E");
			DrawEntry("Biome", BiomeToString(activeVessel.SimulationObject.Telemetry.SurfaceBiome));
			DrawEntry("Alt. MSL", MetersToDistanceString(activeVessel.AltitudeFromSeaLevel), "m");
			DrawEntry("Alt. AGL", MetersToDistanceString(activeVessel.AltitudeFromScenery), "m");
			DrawEntry("Horizontal Vel.", $"{activeVessel.HorizontalSrfSpeed:N1}", "m/s");
			DrawEntry("Vertical Vel.", $"{activeVessel.VerticalSrfSpeed:N1}", "m/s");

			DrawSectionEnd(popoutSur);
		}

		private void FillFlight(int _ = 0)
		{
			DrawSectionHeader("Flight", ref popoutFlt);

			DrawEntry("Speed", $"{activeVessel.SurfaceVelocity.magnitude:N1}", "m/s");
			DrawEntry("Mach Number", $"{activeVessel.SimulationObject.Telemetry.MachNumber:N2}");
			DrawEntry("Atm. Density", $"{activeVessel.SimulationObject.Telemetry.AtmosphericDensity:N3}", "g/L");
			GetAeroStats();

			DrawEntry("Total Lift", $"{totalLift * 1000:N0}", "N");
			DrawEntry("Total Drag", $"{totalDrag * 1000:N0}", "N");

			DrawEntry("Lift / Drag", $"{totalLift / totalDrag:N3}");

			DrawSectionEnd(popoutFlt);
		}

		private void FillTarget(int _ = 0)
		{
			DrawSectionHeader("Target", ref popoutTgt, currentTarget.DisplayName);

			if (currentTarget.Orbit != null)
			{
				DrawEntry("Target Ap.", MetersToDistanceString(currentTarget.Orbit.ApoapsisArl), "m");
				DrawEntry("Target Pe.", MetersToDistanceString(currentTarget.Orbit.PeriapsisArl), "m");

				if (activeVessel.Orbit.referenceBody == currentTarget.Orbit.referenceBody)
				{
					double distanceToTarget = (activeVessel.Orbit.Position - currentTarget.Orbit.Position).magnitude;
					DrawEntry("Distance", MetersToDistanceString(distanceToTarget), "m");
					double relativeVelocity = (activeVessel.Orbit.relativeVelocity - currentTarget.Orbit.relativeVelocity).magnitude;
					DrawEntry("Rel. Speed", $"{relativeVelocity:N1}", "m/s");
					OrbitTargeter targeter = activeVessel.Orbiter.OrbitTargeter;
					DrawEntry("Rel. Incl.", $"{targeter.AscendingNodeTarget.Inclination:N3}", "°");
				}
			}
			DrawSectionEnd(popoutTgt);
		}

		private void FillManeuver(int _ = 0)
		{
			DrawSectionHeader("Maneuver", ref popoutMan);
			PatchedConicsOrbit newOrbit = activeVessel.Orbiter.ManeuverPlanSolver.PatchedConicsList.FirstOrDefault();
			DrawEntry("Projected Ap.", MetersToDistanceString(newOrbit.ApoapsisArl), "m");
			DrawEntry("Projected Pe.", MetersToDistanceString(newOrbit.PeriapsisArl), "m");
			DrawEntry("∆v required", $"{currentManeuver.BurnRequiredDV:N1}", "m/s");
			double timeUntilNode = currentManeuver.Time - GameManager.Instance.Game.UniverseModel.UniversalTime;
			DrawEntry("Time to", SecondsToTimeString(timeUntilNode), "s");
			DrawEntry("Burn Time", SecondsToTimeString(currentManeuver.BurnDuration), "s");

			DrawSectionEnd(popoutMan);
		}

		private void DrawSectionHeader(string sectionName, ref bool isPopout, string value = "")
		{
			GUILayout.BeginHorizontal();
			isPopout = isPopout ? !CloseButton() : GUILayout.Button("⇖", popoutBtnStyle);

			GUILayout.Label($"<b>{sectionName}</b>");
			GUILayout.FlexibleSpace();
			GUILayout.Label(value, valueLabelStyle);
			GUILayout.Space(5);
			GUILayout.Label("", unitLabelStyle);
			GUILayout.EndHorizontal();
			GUILayout.Space(spacingAfterHeader);
		}

		private void DrawStagesHeader(ref bool isPopout)
		{
			GUILayout.BeginHorizontal();
			isPopout = isPopout ? !CloseButton() : GUILayout.Button("⇖", popoutBtnStyle);

			GUILayout.Label("<b>Stage</b>");
			GUILayout.FlexibleSpace();
			GUILayout.Label("∆v", tableHeaderLabelStyle);
			GUILayout.Space(16);
			GUILayout.Label($"TWR", tableHeaderLabelStyle, GUILayout.Width(40));
			GUILayout.Space(16);
			if (isPopout)
			{
				GUILayout.Label($"<color=#{unitColorHex}>Burn</color>", GUILayout.Width(56));
			}
			else
			{
				GUILayout.Label($"Burn", tableHeaderLabelStyle, GUILayout.Width(56));
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(spacingAfterHeader);
		}


		private void DrawEntry(string entryName, string value, string unit = "")
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(entryName, nameLabelStyle);
			GUILayout.FlexibleSpace();
			GUILayout.Label(value, valueLabelStyle);
			GUILayout.Space(5);
			GUILayout.Label(unit, unitLabelStyle);
			GUILayout.EndHorizontal();
			GUILayout.Space(spacingAfterEntry);
		}

		private void DrawStageEntry(int stageID, DeltaVStageInfo stageInfo, string twrFormatString)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label($"{stageID:00.}", nameLabelStyle, GUILayout.Width(24));
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{stageInfo.DeltaVActual:N0} <color=#{unitColorHex}>m/s</color>", valueLabelStyle);
			GUILayout.Space(16);
			GUILayout.Label($"{stageInfo.TWRActual.ToString(twrFormatString)}", valueLabelStyle, GUILayout.Width(40));
			GUILayout.Space(16);
			string burnTime = SecondsToTimeString(stageInfo.StageBurnTime, false);
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

			GUILayout.Label($"{burnTime}<color=#{unitColorHex}>{lastUnit}</color>", valueLabelStyle, GUILayout.Width(56));
			GUILayout.EndHorizontal();
			GUILayout.Space(spacingAfterEntry);
		}

		private void DrawSectionEnd(bool isPopout)
		{
			if (isPopout)
			{
				GUI.DragWindow(new Rect(0, 0, windowWidth, windowHeight));
				GUILayout.Space(spacingBelowPopout);
			}
			else
			{
				GUILayout.Space(spacingAfterSection);
			}
		}

		private bool CloseButton()
		{
			return GUI.Button(closeBtnRect, "x", closeBtnStyle);
		}

		private string SituationToString(VesselSituations situation)
		{
			return situation switch
			{
				VesselSituations.PreLaunch => "Pre-Launch",
				VesselSituations.Landed => "Landed",
				VesselSituations.Splashed => "Splashed down",
				VesselSituations.Flying => "Flying",
				VesselSituations.SubOrbital => "Suborbital",
				VesselSituations.Orbiting => "Orbiting",
				VesselSituations.Escaping => "Escaping",
				_ => "UNKNOWN",
			};
		}

		private string SecondsToTimeString(double seconds, bool addSpacing = true)
		{
			if (seconds == Double.PositiveInfinity)
			{
				return "∞";
			}
			else if (seconds == Double.NegativeInfinity)
			{
				return "-∞";
			}

			seconds = Math.Ceiling(seconds);

			string result = "";
			string spacing = "";
			if (addSpacing)
			{
				spacing = " ";
			}

			if (seconds < 0)
			{
				result += "-";
				seconds = Math.Abs(seconds);
			}

			int days = (int)(seconds / 21600);
			int hours = (int)((seconds - (days * 21600)) / 3600);
			int minutes = (int)((seconds - (hours * 3600) - (days * 21600)) / 60);
			int secs = (int)(seconds - (days * 21600) - (hours * 3600) - (minutes * 60));

			if (days > 0)
			{
				result += $"{days}{spacing}<color=#{unitColorHex}>d</color> ";
			}

			if (hours > 0 || days > 0)
			{
				{
					result += $"{hours}{spacing}<color=#{unitColorHex}>h</color> ";
				}
			}

			if (minutes > 0 || hours > 0 || days > 0)
			{
				if (hours > 0 || days > 0)
				{
					result += $"{minutes:00.}{spacing}<color=#{unitColorHex}>m</color> ";
				}
				else
				{
					result += $"{minutes}{spacing}<color=#{unitColorHex}>m</color> ";
				}
			}

			if (minutes > 0 || hours > 0 || days > 0)
			{
				result += $"{secs:00.}";
			}
			else
			{
				result += secs;
			}

			return result;

		}

		private string MetersToDistanceString(double heightInMeters)
		{
			return $"{heightInMeters:N0}";
		}

		private string BiomeToString(BiomeSurfaceData biome)
		{
			string result = biome.type.ToString().ToLower().Replace('_', ' ');
			return result.Substring(0, 1).ToUpper() + result.Substring(1);
		}

		private string DegreesToDMS(double degreeD)
		{
			var ts = TimeSpan.FromHours(Math.Abs(degreeD));
			int degrees = (int)Math.Floor(ts.TotalHours);
			int minutes = ts.Minutes;
			int seconds = ts.Seconds;

			string result = $"{degrees:N0}<color={unitColorHex}>°</color> {minutes:00}<color={unitColorHex}>'</color> {seconds:00}<color={unitColorHex}>\"</color>";

			return result;
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

			IEnumerable<PartComponent> parts = activeVessel?.SimulationObject?.PartOwner?.Parts;
			if (parts == null || !activeVessel.IsInAtmosphere)
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

		private void InitializeEntries()
		{
            // Refresh ActiveVessel and CurrentManeuver
            MicroUtility.Refresh();
			
			MicroEntries = new List<MicroEntry>();

			InitializeVesselEntries();
			InitializeOrbitalEntries();
			InitializeSurfaceEntries();
			InitializeFlightEntries();
			InitializeTargetEntries();
			InitializeManeuverEntries();
        }

		private void InitializeVesselEntries()
		{
            MicroEntries.Add(new MicroEntry
            {
                Name = "Vessel",
                Description = "Name of the vessel",
                Category = MicroEntryCategory.Vessel,
                EntryValue = MicroUtility.ActiveVessel.DisplayName,
                Unit = null,
                Formatting = null
            });

            MicroEntries.Add(new MicroEntry
            {
                Name = "Mass",
                Description = "Current mass of the vessel",
				Category = MicroEntryCategory.Vessel,
                EntryValue = MicroUtility.ActiveVessel.totalMass * 1000,
                Unit = "kg",
                Formatting = "{0:N0}"
            });

            MicroEntries.Add(new MicroEntry
            {
                Name = "∆v",
                Description = "Current vessel's delta v",
                Category = MicroEntryCategory.Vessel,
                EntryValue = MicroUtility.ActiveVessel.VesselDeltaV?.TotalDeltaVActual,
                Unit = "m/s",
                Formatting = "{0:N0}"
            });

            MicroEntries.Add(new MicroEntry
            {
                Name = "Thrust",
                Description = "Current vessel's thrust in Newtons",
                Category = MicroEntryCategory.Vessel,
                EntryValue = MicroUtility.ActiveVessel.VesselDeltaV?.StageInfo.FirstOrDefault()?.ThrustActual * 1000,
                Unit = "N",
                Formatting = "{0:N0}"
            });

            MicroEntries.Add(new MicroEntry
            {
                Name = "TWR",
                Description = "Current vessel's Thrust to Weight Ratio",
                Category = MicroEntryCategory.Vessel,
                EntryValue = MicroUtility.ActiveVessel.VesselDeltaV?.StageInfo.FirstOrDefault()?.TWRActual,
                Unit = null,
                Formatting = "{0:N2}"
            });
        }

        private void InitializeOrbitalEntries()
        {
            MicroEntries.Add(new Apoapsis
            {
                Name = "Apoapsis",
                Description = "TODO",
                Category = MicroEntryCategory.Orbital,
                EntryValue = MicroUtility.ActiveVessel.Orbit.ApoapsisArl,
                Unit = "m",
                Formatting = null
            });

            MicroEntries.Add(new TimeToApoapsis
            {
                Name = "Time to Ap.",
                Description = "TODO",
                Category = MicroEntryCategory.Orbital,
                EntryValue = (MicroUtility.ActiveVessel.Situation == VesselSituations.Landed || MicroUtility.ActiveVessel.Situation == VesselSituations.PreLaunch) ? 0f : MicroUtility.ActiveVessel.Orbit.TimeToAp,
                Unit = "s",
                Formatting = null
            });

            MicroEntries.Add(new Periapsis
            {
                Name = "Periapsis",
                Description = "TODO",
                Category = MicroEntryCategory.Orbital,
                EntryValue = MicroUtility.ActiveVessel.Orbit.PeriapsisArl,
                Unit = "m",
                Formatting = null
            });

            MicroEntries.Add(new TimeToPeriapsis
            {
                Name = "Time to Pe.",
                Description = "TODO",
                Category = MicroEntryCategory.Orbital,
                EntryValue = MicroUtility.ActiveVessel.Orbit.TimeToPe,
                Unit = "s",
                Formatting = null
            });

            MicroEntries.Add(new MicroEntry
            {
                Name = "Inclination",
                Description = "TODO",
				Category = MicroEntryCategory.Orbital,
                EntryValue = MicroUtility.ActiveVessel.Orbit.inclination,
                Unit = "°",
                Formatting = "{0:N3}"
            });

            MicroEntries.Add(new MicroEntry
            {
                Name = "Eccentricity",
                Description = "TODO",
                Category = MicroEntryCategory.Orbital,
                EntryValue = MicroUtility.ActiveVessel.Orbit.eccentricity,
                Unit = null,
                Formatting = "{0:N3}"
            });

            MicroEntries.Add(new Period
            {
                Name = "Period",
                Description = "TODO",
                Category = MicroEntryCategory.Orbital,
                EntryValue = MicroUtility.ActiveVessel.Orbit.period,
                Unit = "s",
                Formatting = null
            });

            MicroEntries.Add(new SoiTransition
            {
                Name = "SOI Trans.",
                Description = "TODO",
                Category = MicroEntryCategory.Orbital,
                EntryValue = MicroUtility.ActiveVessel.Orbit.UniversalTimeAtSoiEncounter - GameManager.Instance.Game.UniverseModel.UniversalTime,
                Unit = "s",
                Formatting = null
            });
        }

		private void InitializeSurfaceEntries()
		{
            MicroEntries.Add(new Situation
            {
                Name = "Situation",
                Description = "TODO",
                Category = MicroEntryCategory.Surface,
                EntryValue = MicroUtility.ActiveVessel.Situation,
                Unit = null,
                Formatting = null
            });

            
            MicroEntries.Add(new Latitude
            {
                Name = "Latitude",
                Description = "TODO",
                Category = MicroEntryCategory.Surface,
                EntryValue = MicroUtility.ActiveVessel.Latitude,
                Unit = MicroUtility.ActiveVessel.Latitude < 0 ? "S" : "N",
                Formatting = null
            });

            MicroEntries.Add(new Longitude
            {
                Name = "Longitude",
                Description = "TODO",
                Category = MicroEntryCategory.Surface,
                EntryValue = MicroUtility.ActiveVessel.Longitude,
                Unit = MicroUtility.ActiveVessel.Longitude < 0 ? "W" : "E",
                Formatting = null
            });

            MicroEntries.Add(new Biome
            {
                Name = "Biome",
                Description = "TODO",
                Category = MicroEntryCategory.Surface,
                EntryValue = MicroUtility.ActiveVessel.SimulationObject.Telemetry.SurfaceBiome,
                Unit = null,
                Formatting = null
            });

            MicroEntries.Add(new AltitudeAsl
            {
                Name = "Altitude (ASL)",
                Description = "Altitude Above Sea Level",
                Category = MicroEntryCategory.Surface,
                EntryValue = MicroUtility.ActiveVessel.AltitudeFromSeaLevel,
                Unit = "m",
                Formatting = null
            });


            MicroEntries.Add(new AltitudeAgl
            {
                Name = "Altitude (AGL)",
                Description = "Altitude Above Ground Level",
                Category = MicroEntryCategory.Surface,                
                EntryValue = MicroUtility.ActiveVessel.AltitudeFromTerrain,
                Unit = "m",
                Formatting = null
            });

            MicroEntries.Add(new MicroEntry
            {
                Name = "Horizontal Vel.",
                Description = "Horizontal velocity",
                Category = MicroEntryCategory.Surface,
                EntryValue = MicroUtility.ActiveVessel.HorizontalSrfSpeed,
                Unit = "m/s",
                Formatting = "{0:N1}"
            });
            
			MicroEntries.Add(new MicroEntry
            {
                Name = "Vertical Vel.",
                Description = "Vertical velocity",
                Category = MicroEntryCategory.Surface,
                EntryValue = MicroUtility.ActiveVessel.VerticalSrfSpeed,
                Unit = "m/s",
                Formatting = "{0:N1}"
            });
        }

		private void InitializeFlightEntries()
		{
            MicroEntries.Add(new MicroEntry
            {
                Name = "Speed",
                Description = "TODO",
                Category = MicroEntryCategory.Flight,
                EntryValue = MicroUtility.ActiveVessel.SurfaceVelocity.magnitude,
                Unit = "m/s",
                Formatting = "{0:N1}"
            });

            MicroEntries.Add(new MicroEntry
            {
                Name = "Mach Number",
                Description = "TODO",
                Category = MicroEntryCategory.Flight,
                EntryValue = MicroUtility.ActiveVessel.SimulationObject.Telemetry.MachNumber,
                Unit = null,
                Formatting = "{0:N2}"
            });

            MicroEntries.Add(new MicroEntry
            {
                Name = "Atm. Density",
                Description = "TODO",
                Category = MicroEntryCategory.Flight,
                EntryValue = MicroUtility.ActiveVessel.SimulationObject.Telemetry.AtmosphericDensity,
                Unit = "g/L",
                Formatting = "{0:N3}"
            });

            MicroEntries.Add(new TotalLift
            {
                Name = "Total Lift",
                Description = "TODO",
                Category = MicroEntryCategory.Flight,
				// EntryValue => is calculated in the class
                Unit = "N",
                Formatting = "{0:N0}"
            });

            MicroEntries.Add(new TotalDrag
            {
                Name = "Total Drag",
                Description = "TODO",
                Category = MicroEntryCategory.Flight,
                // EntryValue => is calculated in the class
                Unit = "N",
                Formatting = "{0:N0}"
            });

            MicroEntries.Add(new LiftDivDrag
            {
                Name = "Lift / Drag",
                Description = "TODO",
                Category = MicroEntryCategory.Flight,
                // EntryValue => is calculated in the class
                Unit = null,
                Formatting = "{0:N3}"
            });
        }

		private void InitializeTargetEntries()
		{
            MicroEntries.Add(new TargetApoapsis
            {
                Name = "Target Ap.",
                Description = "TODO",
                Category = MicroEntryCategory.Target,
                EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit.ApoapsisArl,
                Unit = "m",
                Formatting = null
            });

            MicroEntries.Add(new TargetPeriapsis
            {
                Name = "Target Pe.",
                Description = "TODO",
                Category = MicroEntryCategory.Target,
                EntryValue = MicroUtility.ActiveVessel.TargetObject?.Orbit.PeriapsisArl,
                Unit = "m",
                Formatting = null
            });


			MicroEntries.Add(new DistanceToTarget
			{
				Name = "Distance to Target",
				Description = "TODO",
				Category = MicroEntryCategory.Target,
				EntryValue = MicroUtility.ActiveVessel.TargetObject != null ? (MicroUtility.ActiveVessel.Orbit.Position - MicroUtility.ActiveVessel.TargetObject.Orbit.Position).magnitude : null,
				Unit = "m",
				Formatting = null
			});

			MicroEntries.Add(new RelativeSpeed
            {
                Name = "Rel. Speed",
                Description = "TODO",
                Category = MicroEntryCategory.Target,
                EntryValue = MicroUtility.ActiveVessel.TargetObject != null ? (MicroUtility.ActiveVessel.Orbit.relativeVelocity - MicroUtility.ActiveVessel.TargetObject.Orbit.relativeVelocity).magnitude : null,
                Unit = "m/s",
                Formatting = "{0:N1}"
            });

            MicroEntries.Add(new RelativeInclination
            {
                Name = "Rel. Inclination",
                Description = "TODO",
                Category = MicroEntryCategory.Target,
                EntryValue = MicroUtility.ActiveVessel.Orbiter.OrbitTargeter.AscendingNodeTarget.Inclination,
                Unit = "°",
                Formatting = "{0:N3}"
            });
        }

		private void InitializeManeuverEntries()
		{
            //MicroUtility.ActiveVessel.Orbiter.HasActiveManeuver

            MicroEntries.Add(new ProjectedAp
            {
                Name = "Projected Ap.",
                Description = "TODO",
                Category = MicroEntryCategory.Maneuver,
                EntryValue = MicroUtility.ActiveVessel.Orbiter.ManeuverPlanSolver.PatchedConicsList.FirstOrDefault().ApoapsisArl,
                Unit = "m",
                Formatting = null
            });

            MicroEntries.Add(new ProjectedPe
            {
                Name = "Projected Pe.",
                Description = "TODO",
                Category = MicroEntryCategory.Maneuver,
                EntryValue = MicroUtility.ActiveVessel.Orbiter.ManeuverPlanSolver.PatchedConicsList.FirstOrDefault().PeriapsisArl,
                Unit = "m",
                Formatting = null
            });

			MicroEntries.Add(new MicroEntry
			{
                Name = "∆v required",
                Description = "TODO",
                Category = MicroEntryCategory.Maneuver,
                EntryValue = MicroUtility.CurrentManeuver?.BurnRequiredDV,
                Unit = "m/s",
                Formatting = "{0:N1}"
            });

			MicroEntries.Add(new TimeToNode
			{
				Name = "Time to Node",
				Description = "TODO",
				Category = MicroEntryCategory.Maneuver,
				EntryValue = MicroUtility.CurrentManeuver != null ? MicroUtility.CurrentManeuver.Time - GameManager.Instance.Game.UniverseModel.UniversalTime: null,
				Unit = "s",
				Formatting = null
			});

            MicroEntries.Add(new BurnTime
            {
                Name = "Burn Time",
                Description = "TODO",
                Category = MicroEntryCategory.Maneuver,
                EntryValue = MicroUtility.CurrentManeuver?.BurnDuration,
                Unit = "s",
                Formatting = null
            });
        }
    }
}