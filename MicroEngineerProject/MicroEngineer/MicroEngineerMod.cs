using BepInEx;
using UnityEngine;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI.Appbar;
using MicroEngineer.UI;
using KSP.Game;

namespace MicroMod
{
    [BepInPlugin("com.micrologist.microengineer", "MicroEngineer", "1.4.0")]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class MicroEngineerMod : BaseSpaceWarpPlugin
	{
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
                    {
                        var mainWindow = Manager.Instance.Windows.Find(w => w is MainGuiWindow) as MainGuiWindow;
                        mainWindow.IsFlightActive = true;
                        mainWindow.IsFlightMinimized = false;
                    }
                    FlightSceneController.Instance.ShowGui = isOpen;
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
                    Utility.SaveLayout();
                });
        }

        public void Update()
        {
             Manager.Instance.Update();

            // Keyboard shortcut for opening UI
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.E))
            {
                if (Utility.GameState.GameState == GameState.FlightView || Utility.GameState.GameState == GameState.Map3DView)
                {
                    var mainWindow = Manager.Instance.Windows.Find(w => w is MainGuiWindow) as MainGuiWindow;

                    // if MainGUI is minimized then treat it like it isn't open at all
                    if (mainWindow.IsFlightMinimized)
                    {
                        mainWindow.IsFlightMinimized = false;
                        mainWindow.IsFlightActive = true;
                        FlightSceneController.Instance.ShowGui = true;                        
                    }
                    else
                    {
                        bool guiState = FlightSceneController.Instance.ShowGui;
                        FlightSceneController.Instance.ShowGui = !guiState;
                        mainWindow.IsFlightActive = !guiState;
                    }
                    Utility.SaveLayout();
                }
                else if (Utility.GameState.GameState == GameState.VehicleAssemblyBuilder)
                {
                    bool guiState = OABSceneController.Instance.ShowGui;
                    OABSceneController.Instance.ShowGui = !guiState;
                    Manager.Instance.Windows.Find(w => w.GetType() == typeof(StageInfoOabWindow)).IsEditorActive = !guiState;
                    Utility.SaveLayout();
                }
            }
        }
    }
}