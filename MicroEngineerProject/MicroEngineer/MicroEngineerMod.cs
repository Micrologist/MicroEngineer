using BepInEx;
using UnityEngine;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI.Appbar;
using KSP.UI.Binding;
using MicroEngineer.UI;

namespace MicroMod
{
    [BepInPlugin("com.micrologist.microengineer", "MicroEngineer", "1.1.0")]    
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class MicroEngineerMod : BaseSpaceWarpPlugin
	{
        public static MicroEngineerMod Instance { get; set; }
        public string GUID;

        public override void OnInitialized()
		{
            Instance = this;
            
            GUID = Info.Metadata.GUID;

            //BackwardCompatibilityInitializations();

            Styles.Initialize();

            MessageManager.Instance.SubscribeToMessages();

            // Register Flight and OAB buttons
            Appbar.RegisterAppButton(
                "Micro Engineer",
                "BTN-MicroEngineerBtn",
                AssetManager.GetAsset<Texture2D>($"{GUID}/images/icon.png"),
                isOpen =>
                {
                    FlightSceneController.Instance.ShowGui = isOpen;
                    //UI.Instance.ShowGuiFlight = isOpen;
                    Manager.Instance.Windows.Find(w => w.GetType() == typeof(MainGuiWindow)).IsFlightActive = isOpen;
                    //GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(isOpen);
                });

            Appbar.RegisterOABAppButton(
                "Micro Engineer",
                "BTN-MicroEngineerOAB",
                AssetManager.GetAsset<Texture2D>($"{GUID}/images/icon.png"),
                isOpen =>
                {
                    UI.Instance.ShowGuiOAB = isOpen;
                    Manager.Instance.Windows.FindAll(w => w is EntryWindow).Cast<EntryWindow>().ToList().Find(w => w.MainWindow == MainWindow.StageInfoOAB).IsEditorActive = isOpen;
                    GameObject.Find("BTN-MicroEngineerOAB")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(isOpen);
                });
        }

        private void BackwardCompatibilityInitializations()
        {
            // Preserve backward compatibility with SpaceWarp 1.1.x
            if (Utility.IsModOlderThan("SpaceWarp", 1, 2, 0))
            {
                Logger.LogInfo("Older Space Warp version detected. Setting mod GUID to \"micro_engineer\".");
                GUID = "micro_engineer";
            }
            else
                Logger.LogInfo("New Space Warp version detected. No backward compatibility needed.");
        }

        public void Update()
        {
            Manager.Instance.Update();
        }

        private void OnGUI()
        {
            return;
            //UI.Instance.OnGUI();
        }
    }
}