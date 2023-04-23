using System.Reflection;
using KSP.Game;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;
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
    public static class Utility
    {
        public static VesselComponent ActiveVessel;
        public static ManeuverNodeData CurrentManeuver;
        public static string LayoutPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MicroLayout.json");
        public static int CurrentLayoutVersion = 5;
        private static ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.Utility");
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
            CurrentManeuver = ActiveVessel != null ? GameManager.Instance?.Game?.SpaceSimulation.Maneuvers.GetNodesForVessel(ActiveVessel.GlobalId).FirstOrDefault() : null;
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

        public static string SecondsToTimeString(double seconds, bool addSpacing = true, bool returnLastUnit = false)
        {
            if (seconds == double.PositiveInfinity)
            {
                return "∞";
            }
            else if (seconds == double.NegativeInfinity)
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
            int hours = (int)((seconds - days * 21600) / 3600);
            int minutes = (int)((seconds - hours * 3600 - days * 21600) / 60);
            int secs = (int)(seconds - days * 21600 - hours * 3600 - minutes * 60);

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
            if (string.IsNullOrEmpty(abbreviation))
                return "CUS";

            return abbreviation.Substring(0, Math.Min(abbreviation.Length, 3)).ToUpperInvariant();
        }

        internal static void SaveLayout(List<BaseWindow> windows)
        {
            try
            {
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

                // Check if user has an old layout version. If it's older, it's not supported, so the default layout will remain active. Once the new layout is saved, it will persist.
                var MainGui = deserializedWindows.OfType<MainGuiWindow>().FirstOrDefault();
                if (MainGui.LayoutVersion < Utility.CurrentLayoutVersion)
                {
                    Logger.LogInfo("Loaded layout version is older than the current supported version. Layout will be reset.");
                    return;
                }

                windows.Clear();
                windows.AddRange(deserializedWindows);

                var settingsWindow = windows.Find(w => w.GetType() == typeof(SettingsWIndow)) as SettingsWIndow;
                settingsWindow.LoadSettings();

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
            try { return ActiveVessel.TargetObject != null; }
            catch { return false; }
        }

        /// <summary>
        /// Checks if current vessel has a maneuver
        /// </summary>
        /// <returns></returns>
        public static bool ManeuverExists()
        {
            try { return GameManager.Instance?.Game?.SpaceSimulation.Maneuvers.GetNodesForVessel(ActiveVessel.GlobalId).FirstOrDefault() != null; }
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
                if (InputDisableWindowAbbreviation == GUI.GetNameOfFocusedControl() || InputDisableWindowName == GUI.GetNameOfFocusedControl())
                {
                    GameManager.Instance.Game.Input.Disable();
                    return false;
                }

                return true;
            }
            else
            {
                if (InputDisableWindowAbbreviation != GUI.GetNameOfFocusedControl() && InputDisableWindowName != GUI.GetNameOfFocusedControl() || !showGuiFlight)
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
        internal static bool IsModOlderThan(string modId, int major, int minor, int patch)
        {
            var modVersion = GetModVersion(modId);

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
}
