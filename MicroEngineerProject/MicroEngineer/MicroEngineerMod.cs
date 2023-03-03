using KSP.Game;
using KSP.Sim.impl;
using UnityEngine;
using SpaceWarp.API.Mods;
using SpaceWarp.API;
using KSP.Sim.Maneuver;
using KSP.UI.Binding;
using KSP.Sim.DeltaV;

namespace MicroMod
{
	[MainMod]
	public class MicroEngineerMod : Mod
	{
		private bool showGUI = false;

		private readonly int windowWidth = 290;
		private readonly int windowHeight = 700;
		private Rect mainGuiRect, vesGuiRect, orbGuiRect, surGuiRect, fltGuiRect, manGuiRect, tgtGuiRect;
		private Rect closeBtnRect;

		private GUISkin _spaceWarpUISkin;
		private GUIStyle popoutBtnStyle;
		private GUIStyle mainWindowStyle;
		private GUIStyle popoutWindowStyle;
		private GUIStyle sectionToggleStyle;
		private GUIStyle closeBtnStyle;
		private GUIStyle nameLabelStyle;
		private GUIStyle valueLabelStyle;
		private GUIStyle unitLabelStyle;

		private string unitColorHex;


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
			_spaceWarpUISkin = SpaceWarpManager.Skin;

			mainWindowStyle = new GUIStyle(_spaceWarpUISkin.window)
			{
				padding = new RectOffset(8, 8, 20, 8),
				contentOffset = new Vector2(0, -22)
			};

			popoutWindowStyle = new GUIStyle(mainWindowStyle)
			{
				padding = new RectOffset(mainWindowStyle.padding.left, mainWindowStyle.padding.right, 0, mainWindowStyle.padding.bottom - 5)
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
				padding = new RectOffset(17, 0, 3, 0)
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

			closeBtnRect = new Rect(windowWidth - 23, 6, 16, 16);
			

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

			currentTarget = activeVessel.TargetObject;
			currentManeuver = GameManager.Instance?.Game?.SpaceSimulation.Maneuvers.GetNodesForVessel(activeVessel.GlobalId).FirstOrDefault();
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
			DrawSectionHeader("Vessel", ref popoutVes, activeVessel.DisplayName);
			DrawEntry("Mass", $"{activeVessel.totalMass:N3}", "t");
			VesselDeltaVComponent deltaVComponent = activeVessel.VesselDeltaV;
			if (deltaVComponent != null)
			{
				DrawEntry("∆v", $"{deltaVComponent.TotalDeltaVActual:N3}", "m/s");
				DrawEntry("Thrust", $"{deltaVComponent.StageInfo.FirstOrDefault()?.ThrustActual:N3}", "kN");
				DrawEntry("TWR", $"{deltaVComponent.StageInfo.FirstOrDefault()?.TWRActual:N3}");
			}

			DrawSectionEnd(popoutVes);
		}

		private void FillOrbital(int _ = 0)
		{
			DrawSectionHeader("Orbital", ref popoutOrb);

			DrawEntry("Ap. Height", $"{MetersToDistanceString(activeVessel.Orbit.ApoapsisArl)}", "km");
			DrawEntry("Pe. Height", $"{MetersToDistanceString(activeVessel.Orbit.PeriapsisArl)}", "km");
			DrawEntry("Inclination", $"{activeVessel.Orbit.inclination:N3}", "°");
			DrawEntry("Eccentricity", $"{activeVessel.Orbit.eccentricity:N3}");
			DrawEntry("Time to Ap.", $"{SecondsToTimeString((activeVessel.Situation == VesselSituations.Landed || activeVessel.Situation == VesselSituations.PreLaunch) ? 0f : activeVessel.Orbit.TimeToAp)}", "s");
			DrawEntry("Time to Pe.", $"{SecondsToTimeString(activeVessel.Orbit.TimeToPe)}", "s");
			DrawEntry("Period", $"{SecondsToTimeString(activeVessel.Orbit.period)}", "s");
			DrawSectionEnd(popoutOrb);
		}

		private void FillSurface(int _ = 0)
		{
			DrawSectionHeader("Surface", ref popoutSur, activeVessel.mainBody.bodyName);

			DrawEntry("Situation", SituationToString(activeVessel.Situation));
			DrawEntry("Altitude", MetersToDistanceString(activeVessel.AltitudeFromScenery), "km");
			DrawEntry("Horizontal Vel.", $"{activeVessel.HorizontalSrfSpeed:N3}", "m/s");
			DrawEntry("Vertical Vel.", $"{activeVessel.VerticalSrfSpeed:N3}", "m/s");

			DrawSectionEnd(popoutSur);
		}

		private void FillFlight(int _ = 0)
		{
			DrawSectionHeader("Flight", ref popoutFlt);

			DrawEntry("Speed", $"{activeVessel.SurfaceVelocity.magnitude:N3}", "m/s");
			DrawEntry("Mach Number", $"{activeVessel.SimulationObject.Telemetry.MachNumber:N3}");
			DrawEntry("Stat. Pressure", $"{(activeVessel.SimulationObject.Telemetry.StaticPressure_kPa):N3}", "KPa");
			DrawEntry("Dyn. Pressure", $"{(activeVessel.SimulationObject.Telemetry.DynamicPressure_kPa):N3}", "KPa");

			DrawSectionEnd(popoutFlt);
		}

		private void FillTarget(int _ = 0)
		{
			DrawSectionHeader("Target", ref popoutTgt, currentTarget.DisplayName);

			DrawEntry("Target Ap.", MetersToDistanceString(currentTarget.Orbit.ApoapsisArl));
			DrawEntry("Target Pe.", MetersToDistanceString(currentTarget.Orbit.PeriapsisArl));

			if (activeVessel.Orbit.referenceBody == currentTarget.Orbit.referenceBody)
			{
				double distanceToTarget = (activeVessel.Orbit.Position - currentTarget.Orbit.Position).magnitude;
				DrawEntry("Distance", MetersToDistanceString(distanceToTarget), "km");
				double relativeVelocity = (activeVessel.Orbit.relativeVelocity - currentTarget.Orbit.relativeVelocity).magnitude;
				DrawEntry("Rel. Speed", $"{relativeVelocity:N3}", "m/s");
				OrbitTargeter targeter = activeVessel.Orbiter.OrbitTargeter;
				DrawEntry("Rel. Incl.", $"{targeter.AscendingNodeTarget.Inclination:N3}", "°");
			}

			DrawSectionEnd(popoutTgt);
		}

		private void FillManeuver(int _ = 0)
		{
			DrawSectionHeader("Maneuver", ref popoutMan);
			PatchedConicsOrbit newOrbit = activeVessel.Orbiter.ManeuverPlanSolver.PatchedConicsList.FirstOrDefault();
			DrawEntry("Projected Ap.", MetersToDistanceString(newOrbit.ApoapsisArl), "km");
			DrawEntry("Projected Pe.", MetersToDistanceString(newOrbit.PeriapsisArl), "km");
			DrawEntry("∆v required", $"{currentManeuver.BurnRequiredDV:N3}", "m/s");
			double timeUntilNode = currentManeuver.Time - GameManager.Instance.Game.UniverseModel.UniversalTime;
			DrawEntry("Time to", SecondsToTimeString(timeUntilNode), "s");
			DrawEntry("Burn Time", SecondsToTimeString(currentManeuver.BurnDuration), "s");

			DrawSectionEnd(popoutMan);
		}

		private void DrawSectionHeader(string name, ref bool isPopout, string value = "")
		{
			GUILayout.BeginHorizontal();
			if (isPopout)
			{
				isPopout = !CloseButton();
			}
			else
			{
				isPopout = GUILayout.Button("⇖", popoutBtnStyle);
			}

			GUILayout.Label($"<b>{name}</b>");
			GUILayout.FlexibleSpace();
			GUILayout.Label(value, valueLabelStyle);
			GUILayout.Space(5);
			GUILayout.Label("", unitLabelStyle);
			GUILayout.EndHorizontal();
			GUILayout.Space(spacingAfterHeader);
		}


		private void DrawEntry(string name, string value, string unit = "")
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(name, nameLabelStyle);
			GUILayout.FlexibleSpace();
			GUILayout.Label(value, valueLabelStyle);
			GUILayout.Space(5);
			GUILayout.Label(unit, unitLabelStyle);
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
			if (seconds == Double.PositiveInfinity)
			{
				return "∞";
			}
			else if (seconds == Double.NegativeInfinity)
			{
				return "-∞";
			}

			string result = "";
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
				result += $"{days} <color=#{unitColorHex}>d</color> ";
			}

			if (hours > 0 || days > 0)
			{
				{
					result += $"{hours} <color=#{unitColorHex}>h</color> ";
				}
			}

			if (minutes > 0 || hours > 0 || days > 0)
			{
				if (hours > 0 || days > 0)
				{
					result += $"{minutes:00.} <color=#{unitColorHex}>m</color> ";
				}
				else
				{
					result += $"{minutes} <color=#{unitColorHex}>m</color> ";
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
			return $"{heightInMeters / 1000:N3}";
		}

		private void CloseWindow()
		{
			GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
			showGUI = false;
		}
	}
}