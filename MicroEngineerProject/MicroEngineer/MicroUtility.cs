using System.Reflection;
using KSP.Game;
using KSP.Sim;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;
using KSP.UI.Flight;
using Newtonsoft.Json;
using UnityEngine;
using static KSP.Rendering.Planets.PQSData;
using BepInEx.Logging;

namespace MicroMod
{
    public static class MicroUtility
    {
        public static VesselComponent ActiveVessel;
        public static ManeuverNodeData CurrentManeuver;
        public static string LayoutPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MicroLayout.json");
        private static ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.MicroUtility");

        /// <summary>
        /// Refreshes the ActiveVessel and CurrentManeuver
        /// </summary>
        public static void RefreshActiveVesselAndCurrentManeuver()
        {
            ActiveVessel = GameManager.Instance?.Game?.ViewController?.GetActiveVehicle(true)?.GetSimVessel(true);
            CurrentManeuver = ActiveVessel != null ? GameManager.Instance?.Game?.SpaceSimulation.Maneuvers.GetNodesForVessel(ActiveVessel.GlobalId).FirstOrDefault(): null;

        }

        public static string DegreesToDMS(double degreeD)
        {
            var ts = TimeSpan.FromHours(Math.Abs(degreeD));
            int degrees = (int)Math.Floor(ts.TotalHours);
            int minutes = ts.Minutes;
            int seconds = ts.Seconds;

            string result = $"{degrees:N0}<color={MicroStyles.UnitColorHex}>°</color> {minutes:00}<color={MicroStyles.UnitColorHex}>'</color> {seconds:00}<color={MicroStyles.UnitColorHex}>\"</color>";

            return result;
        }

        public static string MetersToDistanceString(double heightInMeters)
        {
            return $"{heightInMeters:N0}";
        }

        public static string SecondsToTimeString(double seconds, bool addSpacing = true)
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
                result += $"{days}{spacing}<color=#{MicroStyles.UnitColorHex}>d</color> ";
            }

            if (hours > 0 || days > 0)
            {
                {
                    result += $"{hours}{spacing}<color=#{MicroStyles.UnitColorHex}>h</color> ";
                }
            }

            if (minutes > 0 || hours > 0 || days > 0)
            {
                if (hours > 0 || days > 0)
                {
                    result += $"{minutes:00.}{spacing}<color=#{MicroStyles.UnitColorHex}>m</color> ";
                }
                else
                {
                    result += $"{minutes}{spacing}<color=#{MicroStyles.UnitColorHex}>m</color> ";
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

        public static string SituationToString(VesselSituations situation)
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

        public static string BiomeToString(BiomeSurfaceData biome)
        {
            string result = biome.type.ToString().ToLower().Replace('_', ' ');
            return result.Substring(0, 1).ToUpper() + result.Substring(1);
        }

        /// <summary>
		/// Validates if user entered a 3 character string
		/// </summary>
		/// <param name="abbreviation">String that will be shortened to 3 characters</param>
		/// <returns>Uppercase string shortened to 3 characters. If abbreviation is empty returns "CUS"</returns>
		public static string ValidateAbbreviation(string abbreviation)
        {
            if (String.IsNullOrEmpty(abbreviation))
                return "CUS";

            return abbreviation.Substring(0, Math.Min(abbreviation.Length, 3)).ToUpperInvariant();
        }

        public static void SaveLayout(List<MicroWindow> windows)
        {
            try
            {
                // Deactivate the Settings window before saving, because it doesn't make sense to save it in an active state since user cannot click the save button without having the Settings window active
                windows.Find(w => w.MainWindow == MainWindow.Settings).IsFlightActive = false;

                File.WriteAllText(LayoutPath, JsonConvert.SerializeObject(windows));
                Logger.LogInfo("SaveLayout successful");
            }
            catch (Exception ex)
            {
                Logger.LogError("Error trying to SaveLayout. Error description: " + ex);
            }
        }

        public static void LoadLayout(List<MicroWindow> windows)
        {
            try
            {
                List<MicroWindow> deserializedWindows = JsonConvert.DeserializeObject<List<MicroWindow>>(File.ReadAllText(LayoutPath));

                windows.Clear();
                windows.AddRange(deserializedWindows);                
                
                Logger.LogInfo("LoadLayout successful");
            }
            catch (FileNotFoundException ex)
            {
                Logger.LogWarning($"Error loading layout. File was not found at the expected location. Full error description:\n" + ex);

            }
            catch (Exception ex)
            {
                Logger.LogError("Error trying to LoadLayout. Full error description:\n" + ex);
            }
        }

        /// <summary>
        /// Check if current vessel has an active target (celestial body or vessel)
        /// </summary>
        /// <returns></returns>
        public static bool TargetExists()
        {
            try { return (MicroUtility.ActiveVessel.TargetObject != null); }
            catch { return false; }
        }

        /// <summary>
        /// Checks if current vessel has a maneuver
        /// </summary>
        /// <returns></returns>
        public static bool ManeuverExists()
        {
            try { return (GameManager.Instance?.Game?.SpaceSimulation.Maneuvers.GetNodesForVessel(MicroUtility.ActiveVessel.GlobalId).FirstOrDefault() != null); }
            catch { return false; }
        }

        public static Vector2 ClampToScreen(Vector2 position, Vector2 size)
        {
            float x = Mathf.Clamp(position.x, 0, Screen.width - size.x);
            float y = Mathf.Clamp(position.y, 0, Screen.height - size.y);
            return new Vector2(x, y);
        }
    }    

    public static class AeroForces
    {
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

        public static double TotalLift
        {
            get
            {
                double toReturn = 0.0;

                IEnumerable<PartComponent> parts = MicroUtility.ActiveVessel?.SimulationObject?.PartOwner?.Parts;
                if (parts == null || !MicroUtility.ActiveVessel.IsInAtmosphere)
                {
                    return toReturn;
                }

                foreach (PartComponent part in parts)
                {
                    foreach (IForce force in part.SimulationObject.Rigidbody.Forces)
                    {
                        if (liftForces.Contains(force.GetType()))
                        {
                            toReturn += force.RelativeForce.magnitude;
                        }
                    }
                }

                return toReturn;
            }
        }

        public static double TotalDrag
        {
            get
            {
                double toReturn = 0.0;

                IEnumerable<PartComponent> parts = MicroUtility.ActiveVessel?.SimulationObject?.PartOwner?.Parts;
                if (parts == null || !MicroUtility.ActiveVessel.IsInAtmosphere)
                    return toReturn;

                foreach (PartComponent part in parts)
                {
                    foreach (IForce force in part.SimulationObject.Rigidbody.Forces)
                    {
                        if (dragForces.Contains(force.GetType()))
                        {
                            toReturn += force.RelativeForce.magnitude;
                        }
                    }
                }

                return toReturn;
            }
        }
    }
}
