using BepInEx;
using UnityEngine;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI.Appbar;
using KSP.UI.Binding;

namespace MicroMod
{
    //[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
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

            Styles.Initialize();

            MessageManager.Instance.SubscribeToMessages();

            BackwardCompatibilityInitializations();

            // Register Flight and OAB buttons
            Appbar.RegisterAppButton(
                "Micro Engineer",
                "BTN-MicroEngineerBtn",
                AssetManager.GetAsset<Texture2D>($"{GUID}/images/icon.png"),
                isOpen =>
                {
                    UI.Instance.ShowGuiFlight = isOpen;
                    Manager.Instance.Windows.Find(w => w.GetType() == typeof(MainGuiWindow)).IsFlightActive = isOpen;
                    GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(isOpen);
                });

            Appbar.RegisterOABAppButton(
                "Micro Engineer",
                "BTN-MicroEngineerOAB",
                AssetManager.GetAsset<Texture2D>($"{GUID}/images/icon.png"),
                isOpen =>
                {
                    UI.Instance.ShowGuiOAB = isOpen;
                    Manager.Instance.Windows.FindAll(w => w is EntryWindow).Cast<EntryWindow>().ToList().Find(w => w.MainWindow == MainWindow.StageInfoOAB).IsEditorActive = isOpen;
                    GameObject.Find("BTN - MicroEngineerOAB")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(isOpen);
                });
        }

        private void BackwardCompatibilityInitializations()
        {
            // Preserve backward compatibility with SpaceWarp 1.0.1
            if (Utility.IsModOlderThan("SpaceWarp", 1, 1, 0))
            {
                Logger.LogInfo("Space Warp older version detected. Loading old Styles.");
                Styles.SetStylesForOldSpaceWarpSkin();
            }
            else
                Logger.LogInfo("Space Warp new version detected. Loading new Styles.");
        }
        
        public void Update()
        {
            Manager.Instance.Update();
        }

        private void OnGUI()
        {
            UI.Instance.OnGUI();
        }
    }
}