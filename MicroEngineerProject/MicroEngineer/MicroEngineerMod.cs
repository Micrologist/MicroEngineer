using KSP.Game;
using KSP.Sim.impl;
using UnityEngine;
using SpaceWarp.API.Mods;
using SpaceWarp.API;
using KSP.Sim.Maneuver;
using KSP.UI.Binding;

namespace MicroMod
{
	[MainMod]
	public class MicroModMod : Mod
	{
		private GUISkin _spaceWarpUISkin;
		private bool showGUI = false;
		private readonly int windowWidth = 350;
		private readonly int windowHeight = 700;
		private Rect mainGuiRect, vesGuiRect, orbGuiRect, surGuiRect, fltGuiRect, manGuiRect, tgtGuiRect;
		
		private VesselComponent activeVessel;
		private bool showVes = true;
		private bool showOrb = true;
		private bool showSur = true;
		private bool showFlt = false;
		private bool showMan = true;
		private bool showTgt = false;

		
		private bool popoutVes, popoutOrb, popoutSur, popoutMan, popoutTgt, popoutFlt;
		private int spacingAfterHeader = -8;
		private int spacingAfterEntry = -10;
		private int spacingAfterSection = -3;

		private GUIStyle popoutBtnStyle;
		private GUIStyle mainWindowStyle;
		private GUIStyle popoutWindowStyle;



		public override void OnInitialized()
		{
			_spaceWarpUISkin = SpaceWarpManager.Skin;
			mainWindowStyle = _spaceWarpUISkin.window;
			popoutWindowStyle = new GUIStyle(mainWindowStyle) { padding = new RectOffset(mainWindowStyle.padding.left, mainWindowStyle.padding.right, 0, mainWindowStyle.padding.bottom - 5) };
			popoutBtnStyle = new GUIStyle(_spaceWarpUISkin.button) { alignment = TextAnchor.UpperLeft, contentOffset = new Vector2(0, -3), fixedHeight = 20, fixedWidth = 20 };

			SpaceWarpManager.RegisterAppButton(
				"Micro Engineer",
				"BTN-MicroEngineerBtn",
				SpaceWarpManager.LoadIcon(),
				delegate { showGUI = !showGUI; }
			);
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
		}

		private void OnGUI()
		{
			activeVessel = GameManager.Instance?.Game?.ViewController?.GetActiveVehicle(true)?.GetSimVessel(true);
			if (!showGUI || activeVessel == null) return;
			GUI.skin = _spaceWarpUISkin;
			
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
			
			if (showTgt && popoutTgt)
			{
				tgtGuiRect = GeneratePopoutWindow(tgtGuiRect, FillTarget);
			}

			if (showMan && popoutMan)
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
			if (GUI.Button(new Rect(windowWidth - 23, 6, 16, 16), "x", new GUIStyle(GUI.skin.button) { fontSize = 8 }))
			{
				CloseWindow();
			}

			GUILayout.BeginHorizontal();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			showVes = GUILayout.Toggle(showVes, "<b>VES</b>");
			GUILayout.Space(2);
			showOrb = GUILayout.Toggle(showOrb, "<b>ORB</b>");
			GUILayout.Space(2);
			showSur = GUILayout.Toggle(showSur, "<b>SUR</b>");
			GUILayout.Space(2);
			showFlt = GUILayout.Toggle(showFlt, "<b>FLT</b>");
			GUILayout.Space(2);
			showTgt = GUILayout.Toggle(showTgt, "<b>TGT</b>");
			GUILayout.Space(2);
			showMan = GUILayout.Toggle(showMan, "<b>MAN</b>");
			GUILayout.EndHorizontal();

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

			if (showTgt && !popoutTgt)
			{
				FillTarget();
			}

			if (showMan && !popoutMan)
			{
				FillManeuver();
			}

			GUI.DragWindow(new Rect(0, 0, windowWidth, windowHeight));
		}

		private void FillVessel(int _ = 0)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Vessel</b>");
			if (popoutVes)
			{
				popoutVes = !GUI.Button(new Rect(windowWidth - 23, 6, 16, 16), "x", new GUIStyle(GUI.skin.button) { fontSize = 8 });
			}
			else
			{
				GUILayout.FlexibleSpace();
				popoutVes = GUILayout.Button("⇗", popoutBtnStyle);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterHeader);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Name: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{activeVessel.DisplayName}");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"∆v: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{activeVessel.VesselDeltaV.TotalDeltaVActual:0.} m/s");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Mass: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{activeVessel.totalMass:0.000} t");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Thrust: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{activeVessel.VesselDeltaV.StageInfo.FirstOrDefault().ThrustActual:0.} kN");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"TWR: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{activeVessel.VesselDeltaV.StageInfo.FirstOrDefault().TWRActual:0.00}");
			GUILayout.EndHorizontal();

			if (popoutVes)
			{
				GUI.DragWindow(new Rect(0, 0, windowWidth, windowHeight));
			}
			else
			{
				GUILayout.Space(spacingAfterSection);
			}
		}
		
		private void FillOrbital(int _ = 0)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Orbital</b>");
			if (popoutOrb)
			{
				popoutOrb = !GUI.Button(new Rect(windowWidth - 23, 6, 16, 16), "x", new GUIStyle(GUI.skin.button) { fontSize = 8 });
			}
			else
			{
				GUILayout.FlexibleSpace();
				popoutOrb = GUILayout.Button("⇗", popoutBtnStyle);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterHeader);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Apoapsis Height: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{MetersToDistanceString(activeVessel.Orbit.ApoapsisArl)}");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Periapsis Height: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{MetersToDistanceString(activeVessel.Orbit.PeriapsisArl)}");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Time to Apoapsis: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{SecondsToTimeString((activeVessel.Situation == VesselSituations.Landed || activeVessel.Situation == VesselSituations.PreLaunch) ? 0f : activeVessel.Orbit.TimeToAp)}");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Time to Periapsis: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{SecondsToTimeString(activeVessel.Orbit.TimeToPe)}");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Inclination: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{activeVessel.Orbit.inclination:0.00}°");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Eccentricity: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{activeVessel.Orbit.eccentricity:0.0000}");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Orbital Period: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{SecondsToTimeString(activeVessel.Orbit.period)}");
			GUILayout.EndHorizontal();

			if (popoutOrb)
			{
				GUI.DragWindow(new Rect(0, 0, windowWidth, windowHeight));
			}
			else
			{
				GUILayout.Space(spacingAfterSection);
			}
		}
		
		private void FillSurface(int _ = 0)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Surface</b>");
			if (popoutSur)
			{
				popoutSur = !GUI.Button(new Rect(windowWidth - 23, 6, 16, 16), "x", new GUIStyle(GUI.skin.button) { fontSize = 8 });
				GUILayout.Space(-30);
			}
			else
			{
				GUILayout.FlexibleSpace();
				popoutSur = GUILayout.Button("⇗", popoutBtnStyle);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterHeader);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Reference Body: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{activeVessel.mainBody.bodyName}");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Situation: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{SituationToString(activeVessel.Situation)}");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Altitude (Terrain): ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{MetersToDistanceString(activeVessel.AltitudeFromScenery)}");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Horizontal Speed: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{activeVessel.HorizontalSrfSpeed:0.0} m/s");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Vertical Speed: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{activeVessel.VerticalSrfSpeed:0.0} m/s");
			GUILayout.EndHorizontal();

			if (popoutSur)
			{
				GUI.DragWindow(new Rect(0, 0, windowWidth, windowHeight));
			}
			else
			{
				GUILayout.Space(spacingAfterSection);
			}
		}

		private void FillFlight(int _ = 0)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Flight</b>");
			if (popoutFlt)
			{
				popoutFlt = !GUI.Button(new Rect(windowWidth - 23, 6, 16, 16), "x", new GUIStyle(GUI.skin.button) { fontSize = 8 });
				GUILayout.Space(-30);
			}
			else
			{
				GUILayout.FlexibleSpace();
				popoutFlt = GUILayout.Button("⇗", popoutBtnStyle);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterHeader);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Speed: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{activeVessel.SurfaceVelocity.magnitude:0.0} m/s");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Mach Number: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{activeVessel.SimulationObject.Telemetry.MachNumber:N2}");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Atmospheric Density: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{activeVessel.SimulationObject.Telemetry.AtmosphericDensity:N2} kg/m³");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Static Pressure: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{(activeVessel.SimulationObject.Telemetry.StaticPressure_kPa*1000):N1} Pa");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Dynamic Pressure: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{(activeVessel.SimulationObject.Telemetry.DynamicPressure_kPa * 1000):N1} Pa");
			GUILayout.EndHorizontal();

			if (popoutFlt)
			{
				GUI.DragWindow(new Rect(0, 0, windowWidth, windowHeight));
			}
			else
			{
				GUILayout.Space(spacingAfterSection);
			}
		}

		private void FillTarget(int _ = 0)
		{
			SimulationObjectModel tgtObject = activeVessel.TargetObject;

			if (tgtObject == null)
				return;

			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Target</b>");
			if (popoutTgt)
			{
				popoutTgt = !GUI.Button(new Rect(windowWidth - 23, 6, 16, 16), "x", new GUIStyle(GUI.skin.button) { fontSize = 8 });
				GUILayout.Space(-30);
			}
			else
			{
				GUILayout.FlexibleSpace();
				popoutTgt = GUILayout.Button("⇗", popoutBtnStyle);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterHeader);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Name:");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{tgtObject.DisplayName}");
			GUILayout.EndHorizontal();

			OrbitTargeter targeter = activeVessel.Orbiter.OrbitTargeter;

			if (activeVessel.Orbit.referenceBody == tgtObject.Orbit.referenceBody)
			{
				GUILayout.Space(spacingAfterEntry);
				double distanceToTarget = (activeVessel.Orbit.Position - tgtObject.Orbit.Position).magnitude;

				GUILayout.BeginHorizontal();
				GUILayout.Label($"Distance:");
				GUILayout.FlexibleSpace();
				GUILayout.Label($"{MetersToDistanceString(distanceToTarget)}");
				GUILayout.EndHorizontal();

				GUILayout.Space(spacingAfterEntry);

				double relativeVelocity = (activeVessel.Orbit.relativeVelocity - tgtObject.Orbit.relativeVelocity).magnitude;
				GUILayout.BeginHorizontal();
				GUILayout.Label($"Relative Speed:");
				GUILayout.FlexibleSpace();
				GUILayout.Label($"{relativeVelocity:0.0} m/s");
				GUILayout.EndHorizontal();

				GUILayout.Space(spacingAfterEntry);

				GUILayout.BeginHorizontal();
				GUILayout.Label($"Relative Inclination:");
				GUILayout.FlexibleSpace();
				GUILayout.Label($"{targeter.AscendingNodeTarget.Inclination:0.00}°");
				GUILayout.EndHorizontal();

			}

			if (popoutTgt)
			{
				GUI.DragWindow(new Rect(0, 0, windowWidth, windowHeight));
			}
			else
			{
				GUILayout.Space(spacingAfterSection);
			}
		}
		
		private void FillManeuver(int _ = 0)
		{
			ManeuverNodeData nodeData = GameManager.Instance?.Game?.SpaceSimulation.Maneuvers.GetNodesForVessel(GameManager.Instance.Game.ViewController.GetActiveVehicle(true).Guid).FirstOrDefault();

			if (nodeData == null)
				return;
				
			GUILayout.BeginHorizontal();
			GUILayout.Label("<b>Maneuver</b>");
			if (popoutMan)
			{
				popoutMan = !GUI.Button(new Rect(windowWidth - 23, 6, 16, 16), "x", new GUIStyle(GUI.skin.button) { fontSize = 8 });
				GUILayout.Space(-30);
			}
			else
			{
				GUILayout.FlexibleSpace();
				popoutMan = GUILayout.Button("⇗", popoutBtnStyle);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterHeader);

			double timeUntilNode = nodeData.Time - GameManager.Instance.Game.UniverseModel.UniversalTime;
			GUILayout.BeginHorizontal();
			GUILayout.Label($"Time until Maneuver:");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{SecondsToTimeString(timeUntilNode)}");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Maneuver ∆v:");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{nodeData.BurnRequiredDV:0.0} m/s");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Burn Time:");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{SecondsToTimeString(nodeData.BurnDuration)}");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			PatchedConicsOrbit newOrbit = activeVessel.Orbiter.ManeuverPlanSolver.PatchedConicsList.FirstOrDefault();

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Projected Ap:");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{MetersToDistanceString(newOrbit.ApoapsisArl)}");
			GUILayout.EndHorizontal();

			GUILayout.Space(spacingAfterEntry);

			GUILayout.BeginHorizontal();
			GUILayout.Label($"Projected Pe:");
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{MetersToDistanceString(newOrbit.PeriapsisArl)}");
			GUILayout.EndHorizontal();

			if (popoutMan)
			{
				GUI.DragWindow(new Rect(0, 0, windowWidth, windowHeight));

			}
			else
			{
				GUILayout.Space(spacingAfterSection);
			}
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