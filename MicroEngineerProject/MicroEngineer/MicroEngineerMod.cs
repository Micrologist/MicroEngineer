using BepInEx;
using UnityEngine;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI.Appbar;
using MicroEngineer.UI;
using KSP.Game;
using BepInEx.Logging;

namespace MicroMod
{
    [BepInPlugin("com.micrologist.microengineer", "MicroEngineer", "1.2.0")]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class MicroEngineerMod : BaseSpaceWarpPlugin
	{
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineerMod");
        public static MicroEngineerMod Instance { get; set; }
        public string GUID;

        public override void OnInitialized()
		{
            Instance = this;
            
            GUID = Info.Metadata.GUID;

            MessageManager.Instance.SubscribeToMessages();

            // Register Flight and OAB buttons
            Appbar.RegisterAppButton(
                "Micro Engineer",
                "BTN-MicroEngineerBtn",
                AssetManager.GetAsset<Texture2D>($"{GUID}/images/icon.png"),
                isOpen =>
                {
                    if (isOpen)
                        Manager.Instance.Windows.Find(w => w.GetType() == typeof(MainGuiWindow)).IsFlightActive = isOpen;
                    FlightSceneController.Instance.ShowGui = isOpen;
                    _logger.LogDebug($"Initiating Save from Appbar.RegisterAppButton.");
                    Utility.SaveLayout();
                });

            Appbar.RegisterOABAppButton(
                "Micro Engineer",
                "BTN-MicroEngineerOAB",
                AssetManager.GetAsset<Texture2D>($"{GUID}/images/icon.png"),
                isOpen =>
                {
                    if (isOpen)
                        Manager.Instance.Windows.Find(w => w.GetType() == typeof(StageInfoOabWindow)).IsEditorActive = isOpen;
                    OABSceneController.Instance.ShowGui = isOpen;
                    _logger.LogDebug($"Initiating Save from Appbar.RegisterOABAppButton.");
                    Utility.SaveLayout();
                });
        }

        public void Update()
        {
             Manager.Instance.Update();

            // Keyboard shortcut for opening UI
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.E))
            {
                if (Utility.GameState.GameState == GameState.FlightView || Utility.GameState.GameState == GameState.Map3DView)
                {
                    bool guiState = FlightSceneController.Instance.ShowGui;
                    FlightSceneController.Instance.ShowGui = !guiState;
                    Manager.Instance.Windows.Find(w => w.GetType() == typeof(MainGuiWindow)).IsFlightActive = !guiState;
                }                    
                else if (Utility.GameState.GameState == GameState.VehicleAssemblyBuilder)
                {
                    bool guiState = OABSceneController.Instance.ShowGui;
                    OABSceneController.Instance.ShowGui = !guiState;
                    Manager.Instance.Windows.Find(w => w.GetType() == typeof(StageInfoOabWindow)).IsEditorActive = !guiState;
                }
            }
        }
    }
}