using System;
using System.Collections.Generic;
using System.Text;
using KSP.Game;
using KSP.Messages.PropertyWatchers;
using KSP.Sim;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;
using KSP.UI.Flight;
using SpaceWarp.API.UI;
using UnityEngine;
using static KSP.Modules.Data_LiftingSurface;
using static KSP.Rendering.Planets.PQSData;

namespace MicroMod
{
    public static class MicroUtility
    {
        public static VesselComponent ActiveVessel;
        public static ManeuverNodeData CurrentManeuver;

        /// <summary>
        /// Refreshes the ActiveVessel and CurrentManeuver
        /// </summary>
        public static void Refresh()
        {
            ActiveVessel = GameManager.Instance?.Game?.ViewController?.GetActiveVehicle(true)?.GetSimVessel(true);
            CurrentManeuver = GameManager.Instance?.Game?.SpaceSimulation.Maneuvers.GetNodesForVessel(ActiveVessel.GlobalId).FirstOrDefault();
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
    }

    public static class MicroStyles
    {
        public static GUISkin SpaceWarpUISkin = Skins.ConsoleSkin;

        public static GUIStyle NameLabelStyle
        {
            get
            {
                GUIStyle style = SpaceWarpUISkin.label;
                style.normal.textColor = new Color(.7f, .75f, .75f, 1);
                return style;
            }
        }

        public static GUIStyle ValueLabelStyle
        {
            get
            {
                GUIStyle style = new GUIStyle(SpaceWarpUISkin.label);
                style.alignment = TextAnchor.MiddleRight;
                style.normal.textColor = new Color(.6f, .7f, 1, 1);
                return style;
            }
        }

        public static GUIStyle UnitLabelStyle
        {
            get
            {
                GUIStyle style = new GUIStyle(SpaceWarpUISkin.label);
                style.fixedWidth = 24;
                style.alignment = TextAnchor.MiddleLeft;
                style.normal.textColor = new Color(.7f, .75f, .75f, 1);
                return style;
            }
        }

        public static string UnitColorHex { get => ColorUtility.ToHtmlStringRGBA(UnitLabelStyle.normal.textColor); }
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
