using KSP.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MicroMod
{
    /// <summary>
    /// Manages saving and loading of window positions and states
    /// </summary>
    public class LayoutState
    {
        public bool ShowSettings { get; set; }
        public bool ShowVes { get; set; }
        public bool ShowOrb { get; set; }
        public bool ShowSur { get; set; }
        public bool ShowFlt { get; set; }
        public bool ShowMan { get; set; }
        public bool ShowTgt { get; set; }
        public bool ShowStg { get; set; }

        public bool IsPopoutSettings { get; set; }
        public bool IsPopoutVes { get; set; }
        public bool IsPopoutOrb { get; set; }
        public bool IsPopoutSur { get; set; }
        public bool IsPopOutMan { get; set; }
        public bool IsPopOutTgt { get; set; }
        public bool IsPopOutFlt { get; set; }
        public bool IsPopOutStg { get; set; }

        public Vector2 MainGuiPosition { get; set; }
        public Vector2 SettingsPosition { get; set; }
        public Vector2 VesPosition { get; set; }
        public Vector2 OrbPosition { get; set; }
        public Vector2 SurPosition { get; set; }
        public Vector2 FltPosition { get; set; }
        public Vector2 ManPosition { get; set; }
        public Vector2 TgtPosition { get; set; }
        public Vector2 StgPosition { get; set; }

        private static string _assemblyFolder;
        public static string AssemblyFolder => _assemblyFolder ?? (_assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        private static string _settingsPath;
        public static string SettingsPath => _settingsPath ?? (_settingsPath = Path.Combine(AssemblyFolder, "LayoutState.json"));

        public LayoutState()
        { }

        public LayoutState(MicroEngineerMod layout)
        {
            ShowSettings = layout.showSettings;
            ShowVes = layout.showVes;
            ShowOrb = layout.showOrb;
            ShowSur = layout.showSur;
            ShowFlt = layout.showFlt;
            ShowMan = layout.showMan;
            ShowTgt = layout.showTgt;
            ShowStg = layout.showStg;
            IsPopoutSettings = layout.popoutSettings;
            IsPopoutVes = layout.popoutVes;
            IsPopoutOrb = layout.popoutOrb;
            IsPopoutSur = layout.popoutSur;
            IsPopOutFlt = layout.popoutFlt;
            IsPopOutMan = layout.popoutMan;
            IsPopOutTgt = layout.popoutTgt;
            IsPopOutStg = layout.popoutStg;
            MainGuiPosition = layout.mainGuiRect.position;
            SettingsPosition = layout.settingsGuiRect.position;
            VesPosition = layout.vesGuiRect.position;
            OrbPosition = layout.orbGuiRect.position;
            SurPosition = layout.surGuiRect.position;
            FltPosition = layout.fltGuiRect.position;
            ManPosition = layout.manGuiRect.position;
            TgtPosition = layout.tgtGuiRect.position;
            StgPosition = layout.stgGuiRect.position;
        }

        /// <summary>
        /// Saves layout to disk
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            try
            {
                File.WriteAllText(LayoutState.SettingsPath, JsonConvert.SerializeObject(this));
            }
            catch (Exception ex)
            {
                // TODO logging
            }

            return true;
        }

        /// <summary>
        /// Loads layout from disk
        /// </summary>
        /// <returns>Loaded LayoutState if file exist, otherwise null</returns>
        public static LayoutState Load()
        {
            try
            {
                return JsonConvert.DeserializeObject<LayoutState>(File.ReadAllText(LayoutState.SettingsPath));

            }
            catch (FileNotFoundException ex)
            {
                // TODO logging
                return null;
            }
            catch (Exception ex)
            {
                // TODO logging
                return null;
            }
        }
    }
}
