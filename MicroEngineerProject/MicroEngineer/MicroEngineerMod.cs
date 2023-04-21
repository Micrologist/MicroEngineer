using BepInEx;
using UnityEngine;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI.Appbar;
using KSP.UI.Binding;

namespace MicroMod
{
    [BepInPlugin("com.micrologist.microengineer", "MicroEngineer", "1.0.0")]
	[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
	public class MicroEngineerMod : BaseSpaceWarpPlugin
	{
        private Manager _manager;
        private MessageManager _messagesManager;
        private UI _ui;

        public override void OnInitialized()
		{
            Styles.InitializeStyles();

            _manager = new Manager(this);
            _ui = new UI(this, _manager);
            _messagesManager = new MessageManager(this, _manager, _ui);

            BackwardCompatibilityInitializations();            

            // Register Flight and OAB buttons
            Appbar.RegisterAppButton(
                "Micro Engineer",
                "BTN-MicroEngineerBtn",
                AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"),
                isOpen =>
                {
                    _ui.ShowGuiFlight = isOpen;
                    _manager.Windows.Find(w => w.MainWindow == MainWindow.MainGui).IsFlightActive = isOpen;
                    GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(isOpen);
                });

            Appbar.RegisterOABAppButton(
                "Micro Engineer",
                "BTN-MicroEngineerOAB",
                AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"),
                isOpen =>
                {
                    _ui.ShowGuiOAB = isOpen;
                    _manager.Windows.Find(w => w.MainWindow == MainWindow.StageInfoOAB).IsEditorActive = isOpen;
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
            _manager?.Update();
        }

        private void OnGUI()
        {
            _ui?.OnGUI();
        }
    }
}