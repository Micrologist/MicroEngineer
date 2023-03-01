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
        private Rect guiRect;
        private VesselComponent activeVessel;
        private bool showVes = true;
        private bool showOrb = true;
        private bool showSur = true;
        private bool showMan = true;

        public void Awake() => guiRect = new Rect((Screen.width * 0.8632f) - (windowWidth / 2), (Screen.height / 2) - (windowHeight / 2), 0, 0);

        public override void OnInitialized()
        {
            _spaceWarpUISkin = SpaceWarpManager.Skin;

            SpaceWarpManager.RegisterAppButton(
                "Micro Engineer",
                "BTN-MicroEngineerBtn",
                SpaceWarpManager.LoadIcon(),
                delegate { showGUI = !showGUI; });
        }

        private void OnGUI()
        {
            activeVessel = GameManager.Instance?.Game?.ViewController?.GetActiveVehicle(true)?.GetSimVessel(true);
            if (!showGUI || activeVessel == null) return;
            GUI.skin = _spaceWarpUISkin;
            guiRect = GUILayout.Window(
                GUIUtility.GetControlID(FocusType.Passive),
                guiRect,
                FillGUI,
                "<color=#696DFF>// MICRO ENGINEER</color>",
                GUILayout.Height(0),
                GUILayout.Width(windowWidth)
            );
        }

        private void FillGUI(int windowID)
        {
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            showVes = GUILayout.Toggle(showVes, "<b> VES</b>", GUILayout.Width(windowWidth / 4));
            showOrb = GUILayout.Toggle(showOrb, "<b> ORB</b>", GUILayout.Width(windowWidth / 4));
            showSur = GUILayout.Toggle(showSur, "<b> SUR</b>", GUILayout.Width(windowWidth / 4));
            showMan = GUILayout.Toggle(showMan, "<b> MAN</b>", GUILayout.Width(windowWidth / 4));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();

            if (showVes)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("<b>Vessel</b>");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Name: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{activeVessel.DisplayName}");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"∆v: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{activeVessel.VesselDeltaV.TotalDeltaVActual:0.} m/s");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Mass: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{activeVessel.totalMass:0.000} t");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Thrust: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{activeVessel.VesselDeltaV.StageInfo.FirstOrDefault().ThrustActual:0.} kN");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"TWR: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{activeVessel.VesselDeltaV.StageInfo.FirstOrDefault().TWRActual:0.00}");
                GUILayout.EndHorizontal();
            }

            if (showOrb)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("<b>Orbital</b>");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Apoapsis Height: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{activeVessel.Orbit.ApoapsisArl:N0} m");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Periapsis Height: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{activeVessel.Orbit.PeriapsisArl:N0} m");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Time to Apoapsis: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{SecondsToTimeString(activeVessel.Orbit.TimeToAp)}");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Time to Periapsis: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{SecondsToTimeString(activeVessel.Orbit.TimeToPe)}");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Inclination: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{activeVessel.Orbit.inclination:0.00}°");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Eccentricity: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{activeVessel.Orbit.eccentricity:0.0000}");
                GUILayout.EndHorizontal();
            }

            if (showSur)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("<b>Surface</b>");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Reference Body: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{activeVessel.mainBody.bodyName}");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Situation: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{SituationToString(activeVessel.Situation)}");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Altitude (Terrain): ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{activeVessel.AltitudeFromTerrain:N1} m");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Horizontal Speed: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{activeVessel.HorizontalSrfSpeed:0.0} m/s");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Vertical Speed: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{activeVessel.VerticalSrfSpeed:0.0} m/s");
                GUILayout.EndHorizontal();
            }

            ManeuverNodeData nodeData = GameManager.Instance?.Game?.SpaceSimulation.Maneuvers.GetNodesForVessel(GameManager.Instance.Game.ViewController.GetActiveVehicle(true).Guid).FirstOrDefault();

            if (nodeData != null && showMan)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("<b>Maneuver</b>");
                GUILayout.EndHorizontal();

                double timeUntilNode = nodeData.Time - GameManager.Instance.Game.UniverseModel.UniversalTime;
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Time until Maneuver:");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{SecondsToTimeString(timeUntilNode)}");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Maneuver ∆v:");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{nodeData.BurnRequiredDV:0.0} m/s");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Burn Time:");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{SecondsToTimeString(nodeData.BurnDuration)}");
                GUILayout.EndHorizontal();

            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close"))
            {
                GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
                showGUI = false;
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow(new Rect(0, 0, windowWidth, windowHeight));
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
    }
}