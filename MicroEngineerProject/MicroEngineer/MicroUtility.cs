﻿using System.Reflection;
using KSP.Game;
using KSP.Sim;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;
using KSP.UI.Flight;
using Newtonsoft.Json;
using UnityEngine;
using static KSP.Rendering.Planets.PQSData;
using BepInEx.Logging;
using KSP.Messages;
using KSP.Sim.DeltaV;
using BepInEx.Bootstrap;
using SpaceWarp.API.Mods;

namespace MicroMod
{
    public static class MicroUtility
    {
        public static VesselComponent ActiveVessel;
        public static ManeuverNodeData CurrentManeuver;
        public static string LayoutPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MicroLayout.json");
        private static ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.MicroUtility");
        public static GameStateConfiguration GameState;
        public static MessageCenter MessageCenter;
        public static VesselDeltaVComponent VesselDeltaVComponentOAB;
        public static string InputDisableWindowAbbreviation = "WindowAbbreviation";
        public static string InputDisableWindowName = "WindowName";
        public static double UniversalTime => GameManager.Instance.Game.UniverseModel.UniversalTime;

        /// <summary>
        /// Refreshes the ActiveVessel and CurrentManeuver
        /// </summary>
        public static void RefreshActiveVesselAndCurrentManeuver()
        {
            ActiveVessel = GameManager.Instance?.Game?.ViewController?.GetActiveVehicle(true)?.GetSimVessel(true);
            CurrentManeuver = ActiveVessel != null ? GameManager.Instance?.Game?.SpaceSimulation.Maneuvers.GetNodesForVessel(ActiveVessel.GlobalId).FirstOrDefault(): null;
        }

        public static void RefreshGameManager()
        {
            GameState = GameManager.Instance?.Game?.GlobalGameState?.GetGameState();
            MessageCenter = GameManager.Instance?.Game?.Messages;
        }

        public static void RefreshStagesOAB()
        {
            VesselDeltaVComponentOAB = GameManager.Instance?.Game?.OAB?.Current?.Stats?.MainAssembly?.VesselDeltaV;
        }

        public static string DegreesToDMS(double degreeD)
        {
            var ts = TimeSpan.FromHours(Math.Abs(degreeD));
            int degrees = (int)Math.Floor(ts.TotalHours);
            int minutes = ts.Minutes;
            int seconds = ts.Seconds;

            string result = $"{degrees:N0}<color={Styles.UnitColorHex}>°</color> {minutes:00}<color={Styles.UnitColorHex}>'</color> {seconds:00}<color={Styles.UnitColorHex}>\"</color>";

            return result;
        }

        public static string MetersToDistanceString(double heightInMeters)
        {
            return $"{heightInMeters:N0}";
        }

        public static string SecondsToTimeString(double seconds, bool addSpacing = true, bool returnLastUnit = false)
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
                result += $"{days}{spacing}<color=#{Styles.UnitColorHex}>d</color> ";
            }

            if (hours > 0 || days > 0)
            {
                {
                    result += $"{hours}{spacing}<color=#{Styles.UnitColorHex}>h</color> ";
                }
            }

            if (minutes > 0 || hours > 0 || days > 0)
            {
                if (hours > 0 || days > 0)
                {
                    result += $"{minutes:00.}{spacing}<color=#{Styles.UnitColorHex}>m</color> ";
                }
                else
                {
                    result += $"{minutes}{spacing}<color=#{Styles.UnitColorHex}>m</color> ";
                }
            }

            if (minutes > 0 || hours > 0 || days > 0)
            {
                result += returnLastUnit ? $"{secs:00.}{spacing}<color=#{Styles.UnitColorHex}>s</color>" : $"{secs:00.}";
            }
            else
            {
                result += returnLastUnit ? $"{secs}{spacing}<color=#{Styles.UnitColorHex}>s</color>" : $"{secs}";
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

        public static double RadiansToDegrees(double radians)
        {
            return radians * PatchedConicsOrbit.Rad2Deg;
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

        internal static void SaveLayout(List<BaseWindow> windows)
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

        internal static void LoadLayout(List<BaseWindow> windows)
        {
            try
            {
                List<BaseWindow> deserializedWindows = JsonConvert.DeserializeObject<List<BaseWindow>>(File.ReadAllText(LayoutPath));

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

        /// <summary>
        /// Check if focus is on an editable text field. If it is, disable input controls. If it's not, reenable controls.
        /// </summary>
        /// <param name="gameInputState">If input is currently enabled or disabled</param>
        /// <param name="showGuiFlight">If MainGui window is opened</param>
        /// <returns>True = input is enabled. False = input is disabled</returns>
        internal static bool ToggleGameInputOnControlInFocus(bool gameInputState, bool showGuiFlight)
        {
            if (gameInputState)
            {
                if (MicroUtility.InputDisableWindowAbbreviation == GUI.GetNameOfFocusedControl() || MicroUtility.InputDisableWindowName == GUI.GetNameOfFocusedControl())
                {
                    GameManager.Instance.Game.Input.Disable();
                    return false;
                }

                return true;
            }
            else
            {
                if ((MicroUtility.InputDisableWindowAbbreviation != GUI.GetNameOfFocusedControl() && MicroUtility.InputDisableWindowName != GUI.GetNameOfFocusedControl()) || !showGuiFlight)
                {
                    GameManager.Instance.Game.Input.Enable();
                    return true;
                }

                return false;
            }
        }

        internal static (int major, int minor, int patch)? GetModVersion(string modId)
        {
            var plugin = Chainloader.Plugins?.OfType<BaseSpaceWarpPlugin>().ToList().FirstOrDefault(p => p.SpaceWarpMetadata.ModID.ToLowerInvariant() == modId.ToLowerInvariant());
            string versionString = plugin?.SpaceWarpMetadata?.Version;

            string[] versionNumbers = versionString?.Split(new char[] { '.' }, 3);

            if (versionNumbers != null && versionNumbers.Length >= 1)
            {
                int majorVersion = 0;
                int minorVersion = 0;
                int patchVersion = 0;

                if (versionNumbers.Length >= 1)
                    int.TryParse(versionNumbers[0], out majorVersion);
                if (versionNumbers.Length >= 2)
                    int.TryParse(versionNumbers[1], out minorVersion);
                if (versionNumbers.Length == 3)
                    int.TryParse(versionNumbers[2], out patchVersion);

                Logger.LogInfo($"Space Warp version {majorVersion}.{minorVersion}.{patchVersion} detected.");

                return (majorVersion, minorVersion, patchVersion);
            }
            else return null;
        }

        /// <summary>
        /// Check if installed mod is older than the specified version
        /// </summary>
        /// <param name="modId">SpaceWarp mod ID</param>
        /// <param name="major">Specified major version (X.0.0)</param>
        /// <param name="minor">Specified minor version (0.X.0)</param>
        /// <param name="patch">Specified patch version (0.0.X)</param>
        /// <returns>True = installed mod is older. False = installed mod has the same version or it's newer or version isn't declared or version declared is gibberish that cannot be parsed</returns>
        internal static bool IsModOlderThan (string modId, int major, int minor, int patch)
        {
            var modVersion = MicroUtility.GetModVersion(modId);

            if (!modVersion.HasValue || modVersion.Value == (0, 0, 0))
                return false;

            if (modVersion.Value.Item1 < major)
                return true;
            else if (modVersion.Value.Item1 > major)
                return false;

            if (modVersion.Value.Item2 < minor)
                return true;
            else if (modVersion.Value.Item2 > minor)
                return false;

            if (modVersion.Value.Item3 < patch)
                return true;
            else
                return false;
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

        public static double AngleOfAttack
        {
            get
            {
                double aoe = 0.0;

                ISimulationObjectView simulationViewIfLoaded = GameManager.Instance.Game.ViewController.GetSimulationViewIfLoaded(MicroUtility.ActiveVessel.SimulationObject);
                if (simulationViewIfLoaded != null)
                {
                    Vector3d normalized = GameManager.Instance.Game.UniverseView.PhysicsSpace.VectorToPhysics(MicroUtility.ActiveVessel.SurfaceVelocity).normalized;
                    Vector up = simulationViewIfLoaded.Model.Vessel.ControlTransform.up;
                    Vector3 lhs = GameManager.Instance.Game.UniverseView.PhysicsSpace.VectorToPhysics(up);
                    Vector right = simulationViewIfLoaded.Model.Vessel.ControlTransform.right;
                    Vector3 rhs = GameManager.Instance.Game.UniverseView.PhysicsSpace.VectorToPhysics(right);
                    Vector3 lhs2 = normalized;
                    Vector3 normalized2 = Vector3.Cross(lhs2, rhs).normalized;
                    Vector3 rhs2 = Vector3.Cross(lhs2, normalized2);
                    aoe = Vector3.Dot(lhs, normalized2);
                    aoe = Math.Asin(aoe) * 57.295780181884766;
                    if (double.IsNaN(aoe))
                    {
                        aoe = 0.0;
                    }                    
                }

                return aoe;
            }
        }

        public static double SideSlip
        {
            get
            {
                double sideSlip = 0.0;

                ISimulationObjectView simulationViewIfLoaded = GameManager.Instance.Game.ViewController.GetSimulationViewIfLoaded(MicroUtility.ActiveVessel.SimulationObject);
                if (simulationViewIfLoaded != null)
                {
                    Vector3d normalized = GameManager.Instance.Game.UniverseView.PhysicsSpace.VectorToPhysics(MicroUtility.ActiveVessel.SurfaceVelocity).normalized;
                    Vector up = simulationViewIfLoaded.Model.Vessel.ControlTransform.up;
                    Vector3 lhs = GameManager.Instance.Game.UniverseView.PhysicsSpace.VectorToPhysics(up);
                    Vector right = simulationViewIfLoaded.Model.Vessel.ControlTransform.right;
                    Vector3 rhs = GameManager.Instance.Game.UniverseView.PhysicsSpace.VectorToPhysics(right);
                    Vector3 lhs2 = normalized;
                    Vector3 normalized2 = Vector3.Cross(lhs2, rhs).normalized;
                    Vector3 rhs2 = Vector3.Cross(lhs2, normalized2);

                    sideSlip = Vector3.Dot(lhs, rhs2);
                    sideSlip = Math.Asin(sideSlip) * 57.295780181884766;
                    if (double.IsNaN(sideSlip))
                    {
                        sideSlip = 0.0;
                    }                    
                }

                return sideSlip;
            }
        }


    }
}
