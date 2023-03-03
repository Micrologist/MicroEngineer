using KSP.Game;
using KSP.Sim.impl;
using UnityEngine;
using SpaceWarp.API.Mods;
using SpaceWarp.API;
<<<<<<< Updated upstream
using SpaceWarp.API.Assets;
using KSP.UI.Binding;
using SpaceWarp.API.UI;
using SpaceWarp.API.UI.Toolbar;
using KSP.Sim.Maneuver;
using BepInEx;
using SpaceWarp;
=======
using KSP.Sim.Maneuver;
using SpaceWarp.API.Assets;
using KSP.UI.Binding;
using SpaceWarp.API.UI;
using SpaceWarp.UI;
using SpaceWarp;
using SpaceWarp.API.UI.Appbar;
using KSP.Sim.DeltaV;
using KSP.Sim;
using KSP.UI.Flight;
using BepInEx;
>>>>>>> Stashed changes

namespace MicroEngineerMod
{
<<<<<<< Updated upstream
    [BepInPlugin("com.Micro.MicroMod", "MicroMOd", "0.4.0")]
=======
    [BepInPlugin("com.Micro.MicroEngineerMod", "MicroEngineerMod", "0.4.0")]
>>>>>>> Stashed changes
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class MicroEngineerMod : BaseSpaceWarpPlugin
	{
		private bool showGUI = false;
        private static MicroEngineerMod Instance { get; set; }

        private readonly int windowWidth = 290;
		private readonly int windowHeight = 700;
<<<<<<< Updated upstream
		private Rect mainGuiRect, vesGuiRect, orbGuiRect, surGuiRect, fltGuiRect, manGuiRect, tgtGuiRect;
		private Rect closeBtnRect;
=======
		private Rect mainGuiRect, vesGuiRect, orbGuiRect, surGuiRect, fltGuiRect, manGuiRect, tgtGuiRect, stgGuiRect, closeBtnRect;
>>>>>>> Stashed changes

		private GUIStyle popoutBtnStyle;
		private GUIStyle mainWindowStyle;
		private GUIStyle popoutWindowStyle;
		private GUIStyle sectionToggleStyle;
		private GUIStyle closeBtnStyle;
		private GUIStyle nameLabelStyle;
		private GUIStyle valueLabelStyle;

		private int spacingAfterHeader = -12;
		private int spacingAfterEntry = -12;
		private int spacingAfterSection = 5;
		private float spacingBelowPopout = 10;

		private bool showVes = true;
		private bool showOrb = true;
		private bool showSur = true;
		private bool showFlt = false;
		private bool showMan = true;
		private bool showTgt = false;

		private bool popoutVes, popoutOrb, popoutSur, popoutMan, popoutTgt, popoutFlt;

		private VesselComponent activeVessel;
		private SimulationObjectModel currentTarget;
		private ManeuverNodeData currentManeuver;

		public override void OnInitialized()
		{
			base.OnInitialized();
			Instance = this;
			
<<<<<<< Updated upstream
			Toolbar.RegisterAppButton(
				"Micro Engineer",
				"BTN-MicroEngineerBtn",
                AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"),
                ToggleButton
            ); ; ;
        }
        private void ToggleButton(bool toggle)
        {
            showGUI = toggle;
            GameObject.Find("BTN-MircroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(toggle);

        }

        void Awake()
		{
			mainGuiRect = new Rect(Screen.width * 0.8f, Screen.height * 0.3f, 0, 0);
			vesGuiRect = new Rect(Screen.width * 0.6f, Screen.height * 0.3f, 0, 0);
			orbGuiRect = new Rect(Screen.width * 0.6f, Screen.height * 0.3f, 0, 0);
			surGuiRect = new Rect(Screen.width * 0.6f, Screen.height * 0.3f, 0, 0);
			fltGuiRect = new Rect(Screen.width * 0.6f, Screen.height * 0.3f, 0, 0);
			manGuiRect = new Rect(Screen.width * 0.6f, Screen.height * 0.3f, 0, 0);
			tgtGuiRect = new Rect(Screen.width * 0.6f, Screen.height * 0.3f, 0, 0);
=======
			Appbar.RegisterAppButton(
				"Micro Engineer",
				"BTN-MicroEngineerBtn",
                AssetManager.GetAsset<Texture2D>("MicroEngineer/images/icon.png"),
                ToggleButton
            ); ; ;
        }

		private void ToggleButton(bool toggle)
		{
			showGUI = toggle;
			GameObject.Find("BTN-MircroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(toggle);

		}
		void Awake()
		{
				return;
>>>>>>> Stashed changes
		}

		private void OnGUI()
		{
			activeVessel = GameManager.Instance?.Game?.ViewController?.GetActiveVehicle(true)?.GetSimVessel(true);
			if (!showGUI || activeVessel == null) return;

            mainGuiRect = new Rect(windowWidth, windowHeight, 0, 0);
            vesGuiRect = new Rect(Screen.width * 0.6f, Screen.height * 0.3f, 0, 0);
            orbGuiRect = new Rect(Screen.width * 0.6f, Screen.height * 0.3f, 0, 0);
            surGuiRect = new Rect(Screen.width * 0.6f, Screen.height * 0.3f, 0, 0);
            fltGuiRect = new Rect(Screen.width * 0.6f, Screen.height * 0.3f, 0, 0);
            manGuiRect = new Rect(Screen.width * 0.6f, Screen.height * 0.3f, 0, 0);
            tgtGuiRect = new Rect(Screen.width * 0.6f, Screen.height * 0.3f, 0, 0);
<<<<<<< Updated upstream

            GUI.skin = Skins.ConsoleSkin;
            mainWindowStyle = new GUIStyle(GUI.skin.window) { padding = new RectOffset(8, 8, 20, 8), contentOffset = new Vector2(0, -22) };
            popoutWindowStyle = new GUIStyle(mainWindowStyle) { padding = new RectOffset(mainWindowStyle.padding.left, mainWindowStyle.padding.right, 0, mainWindowStyle.padding.bottom - 5) };
            popoutBtnStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.UpperLeft, contentOffset = new Vector2(0, -3), fixedHeight = 20, fixedWidth = 20 };
            sectionToggleStyle = new GUIStyle(GUI.skin.toggle) { padding = new RectOffset(17, 0, 3, 0) };
            nameLabelStyle = new GUIStyle(GUI.skin.label);
            nameLabelStyle.normal.textColor = new Color(.7f, .75f, .75f, 1);
            valueLabelStyle = new GUIStyle(GUI.skin.label);
            closeBtnStyle = new GUIStyle(GUI.skin.button) { fontSize = 8 };
            closeBtnRect = new Rect(windowWidth - 23, 6, 16, 16);

            currentTarget = activeVessel.TargetObject;
			currentManeuver = GameManager.Instance?.Game?.SpaceSimulation.Maneuvers.GetNodesForVessel(activeVessel.GlobalId).FirstOrDefault();
			GUI.skin = GUI.skin;
			
			mainGuiRect = GUILayout.Window(
				GUIUtility.GetControlID(FocusType.Passive),
				mainGuiRect,
				FillMainGUI,
				"<color=#696DFF>// MICRO ENGINEER</color>",
				mainWindowStyle,
				GUILayout.Height(0),
				GUILayout.Width(windowWidth)
			);
			mainGuiRect.position = ClampToScreen(mainGuiRect.position, mainGuiRect.size);

=======
            stgGuiRect = new Rect(Screen.width * 0.6f, Screen.height * 0.3f, 0, 0);

            GUI.skin = Skins.ConsoleSkin;
            nameLabelStyle = new GUIStyle(GUI.skin.label);
            nameLabelStyle.normal.textColor = new Color(.7f, .75f, .75f, 1);


            mainWindowStyle = new GUIStyle(GUI.skin.window)
            {
                padding = new RectOffset(8, 8, 20, 8),
                contentOffset = new Vector2(0, -22)
            };

            popoutWindowStyle = new GUIStyle(mainWindowStyle)
            {
                padding = new RectOffset(mainWindowStyle.padding.left, mainWindowStyle.padding.right, 0, mainWindowStyle.padding.bottom - 5)
            };

            popoutBtnStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                contentOffset = new Vector2(0, 2),
                fixedHeight = 15,
                fixedWidth = 15,
                fontSize = 28,
                clipping = TextClipping.Overflow,
                margin = new RectOffset(0, 0, 10, 0)
            };

            sectionToggleStyle = new GUIStyle(GUI.skin.toggle)
            {
                padding = new RectOffset(17, 0, 3, 0)
            };

            valueLabelStyle = new GUIStyle(GUI.skin.label)
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

            closeBtnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 8
            };

            closeBtnRect = new Rect(windowWidth - 23, 6, 16, 16);


            currentTarget = activeVessel.TargetObject;
			currentManeuver = GameManager.Instance?.Game?.SpaceSimulation.Maneuvers.GetNodesForVessel(activeVessel.GlobalId).FirstOrDefault();
			
			if (showGUI)
			{
                currentTarget = activeVessel.TargetObject;
                currentManeuver = GameManager.Instance?.Game?.SpaceSimulation.Maneuvers.GetNodesForVessel(activeVessel.GlobalId).FirstOrDefault();
                mainGuiRect = GUILayout.Window(
                GUIUtility.GetControlID(FocusType.Passive),
                mainGuiRect,
                FillMainGUI,
                "<color=#696DFF>// MICRO ENGINEER</color>",
                mainWindowStyle,
                GUILayout.Height(0),
                GUILayout.Width(windowWidth)
            );
            }
			
>>>>>>> Stashed changes
			if (showVes && popoutVes)
			{
				vesGuiRect = GeneratePopoutWindow(vesGuiRect, FillVessel);
			}

			if (showOrb && popoutOrb)
			{
				orbGuiRect = GeneratePopoutWindow(orbGuiRect, FillOrbital);
			}

			if (showSur && popoutSur)
			{
				surGuiRect = GeneratePopoutWindow(surGuiRect, FillSurface);
			}

			if (showFlt && popoutFlt)
			{
				fltGuiRect = GeneratePopoutWindow(fltGuiRect, FillFlight);
			}
			
			if (showTgt && popoutTgt && currentTarget != null)
			{
				tgtGuiRect = GeneratePopoutWindow(tgtGuiRect, FillTarget);
			}

			if (showMan && popoutMan && currentManeuver != null)
			{
				manGuiRect = GeneratePopoutWindow(manGuiRect, FillManeuver);
			}
		}

		private Rect GeneratePopoutWindow(Rect guiRect, UnityEngine.GUI.WindowFunction fillAction)
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
			return guiRect;
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

			GUILayout.BeginHorizontal();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			showVes = GUILayout.Toggle(showVes, "<b>VES</b>", sectionToggleStyle);
			showOrb = GUILayout.Toggle(showOrb, "<b>ORB</b>", sectionToggleStyle);
			showSur = GUILayout.Toggle(showSur, "<b>SUR</b>", sectionToggleStyle);
			showFlt = GUILayout.Toggle(showFlt, "<b>FLT</b>", sectionToggleStyle);
			showTgt = GUILayout.Toggle(showTgt, "<b>TGT</b>", sectionToggleStyle);
			showMan = GUILayout.Toggle(showMan, "<b>MAN</b>", sectionToggleStyle);
			GUILayout.EndHorizontal();
			GUILayout.Space(-5);
			GUILayout.BeginHorizontal();
			GUILayout.EndHorizontal();

			if (showVes && !popoutVes)
			{
				FillVessel();
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

		private void FillVessel(int _ = 0)
		{
			DrawSectionHeader("Vessel", ref popoutVes);

			DrawEntry("Name", activeVessel.DisplayName);
			DrawEntry("∆v", $"{activeVessel.VesselDeltaV.TotalDeltaVActual:0.} m/s");
			DrawEntry("Mass", $"{activeVessel.totalMass:0.000} t");
			DrawEntry("Thrust", $"{activeVessel.VesselDeltaV.StageInfo.FirstOrDefault().ThrustActual:0.} kN");
			DrawEntry("TWR", $"{activeVessel.VesselDeltaV.StageInfo.FirstOrDefault().TWRActual:0.00}");

			DrawSectionEnd(popoutVes);
		}
		
		private void FillOrbital(int _ = 0)
		{
			DrawSectionHeader("Orbital", ref popoutOrb);

			DrawEntry("Ap. Height", $"{MetersToDistanceString(activeVessel.Orbit.ApoapsisArl)}");
			DrawEntry("Pe. Height", $"{MetersToDistanceString(activeVessel.Orbit.PeriapsisArl)}");
			DrawEntry("Time to Ap.", $"{SecondsToTimeString((activeVessel.Situation == VesselSituations.Landed || activeVessel.Situation == VesselSituations.PreLaunch) ? 0f : activeVessel.Orbit.TimeToAp)}");
			DrawEntry("Time to Pe.", $"{SecondsToTimeString(activeVessel.Orbit.TimeToPe)}");
			DrawEntry("Inclination", $"{activeVessel.Orbit.inclination:0.00}°");
			DrawEntry("Eccentricity", $"{activeVessel.Orbit.eccentricity:0.0000}");
			DrawEntry("Period", $"{SecondsToTimeString(activeVessel.Orbit.period)}");

			DrawSectionEnd(popoutOrb);
		}
		
		private void FillSurface(int _ = 0)
		{
			DrawSectionHeader("Surface", ref popoutSur);

			DrawEntry("Ref. Body", activeVessel.mainBody.bodyName);
			DrawEntry("Situation: ", SituationToString(activeVessel.Situation));
			DrawEntry("Altitude", MetersToDistanceString(activeVessel.AltitudeFromScenery));
			DrawEntry("Horizontal Vel.", $"{activeVessel.HorizontalSrfSpeed:0.0} m/s");
			DrawEntry("Vertical Vel.", $"{activeVessel.VerticalSrfSpeed:0.0} m/s");

			DrawSectionEnd(popoutSur);
		}

		private void FillFlight(int _ = 0)
		{
			DrawSectionHeader("Flight", ref popoutFlt);

			DrawEntry("Speed", $"{activeVessel.SurfaceVelocity.magnitude:0.0} m/s");
			DrawEntry("Mach Number", $"{activeVessel.SimulationObject.Telemetry.MachNumber:N2}");
			DrawEntry("Atm. Density", $"{activeVessel.SimulationObject.Telemetry.AtmosphericDensity:N2} kg/m³");
			DrawEntry("Stat. Pressure", $"{(activeVessel.SimulationObject.Telemetry.StaticPressure_kPa * 1000):N1} Pa");
			DrawEntry("Dyn. Pressure", $"{(activeVessel.SimulationObject.Telemetry.DynamicPressure_kPa * 1000):N1} Pa");

			DrawSectionEnd(popoutFlt);
		}

		private void FillTarget(int _ = 0)
		{
			DrawSectionHeader("Target", ref popoutTgt);

			DrawEntry("Name", currentTarget.DisplayName);
			DrawEntry("Target Ap.", MetersToDistanceString(currentTarget.Orbit.ApoapsisArl));
			DrawEntry("Target Pe.", MetersToDistanceString(currentTarget.Orbit.PeriapsisArl));
			
			if (activeVessel.Orbit.referenceBody == currentTarget.Orbit.referenceBody)
			{
				double distanceToTarget = (activeVessel.Orbit.Position - currentTarget.Orbit.Position).magnitude;
				DrawEntry("Distance", MetersToDistanceString(distanceToTarget));
				double relativeVelocity = (activeVessel.Orbit.relativeVelocity - currentTarget.Orbit.relativeVelocity).magnitude;
				DrawEntry("Rel. Speed", $"{relativeVelocity:0.0} m/s");
				OrbitTargeter targeter = activeVessel.Orbiter.OrbitTargeter;
				DrawEntry("Rel. Incl.", $"{targeter.AscendingNodeTarget.Inclination:0.00}°");
			}

			DrawSectionEnd(popoutTgt);
		}
		
		private void FillManeuver(int _ = 0)
		{
			DrawSectionHeader("Maneuver", ref popoutMan);

			double timeUntilNode = currentManeuver.Time - GameManager.Instance.Game.UniverseModel.UniversalTime;
			DrawEntry("Time until:", SecondsToTimeString(timeUntilNode));
			DrawEntry("∆v required", $"{currentManeuver.BurnRequiredDV:0.0} m/s");
			DrawEntry("Burn Time", SecondsToTimeString(currentManeuver.BurnDuration));
			PatchedConicsOrbit newOrbit = activeVessel.Orbiter.ManeuverPlanSolver.PatchedConicsList.FirstOrDefault();
			DrawEntry("Projected Ap.", MetersToDistanceString(newOrbit.ApoapsisArl));
			DrawEntry("Projected Pe.", MetersToDistanceString(newOrbit.PeriapsisArl));

			DrawSectionEnd(popoutMan);
		}

		private void DrawSectionHeader(string name, ref bool isPopout)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label($"<b>{name}</b>");
			if (isPopout)
			{
				isPopout = !CloseButton();
			}
			else
			{
				GUILayout.FlexibleSpace();
				isPopout = GUILayout.Button("⇗", popoutBtnStyle);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(spacingAfterHeader);
		}


		private void DrawEntry(string name, string value)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(name, nameLabelStyle);
			GUILayout.FlexibleSpace();
			GUILayout.Label(value, valueLabelStyle);
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
			switch (situation)
			{
				case VesselSituations.PreLaunch:
					return "Pre-Launch";
				case VesselSituations.Landed:
					return "Landed";
				case VesselSituations.Splashed:
					return "Splashed down";
				case VesselSituations.Flying:
					return "Flying";
				case VesselSituations.SubOrbital:
					return "Suborbital";
				case VesselSituations.Orbiting:
					return "Orbiting";
				case VesselSituations.Escaping:
					return "Escaping";
				default:
					return "UNNOWN";
			}
		}

		private string SecondsToTimeString(double seconds)
		{
			if (seconds > 0 && seconds < TimeSpan.MaxValue.TotalSeconds)
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
				string result = "";
				double h = Math.Floor(timeSpan.TotalHours);
				if (h > 0)
				{
					result += $"{h}h ";
				}
				double m = timeSpan.Minutes;
				if (m > 0)
				{
					result += $"{m}m ";
				}
				double s = timeSpan.Seconds;
				if (s > 0)
				{
					result += $"{s}s";
				}
				return result.Trim();
			}
			else
			{
				return "N/A";
			}
		}

		private string MetersToDistanceString(double heightInMeters)
		{
				return $"{heightInMeters:N1} m";
		}

		private void CloseWindow()
		{
			GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
			showGUI = false;
		}
	}
}