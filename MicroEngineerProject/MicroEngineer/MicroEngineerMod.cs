using BepInEx;
using UnityEngine;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI.Appbar;
using MicroEngineer.UI;
using KSP.Game;
using BepInEx.Configuration;

namespace MicroMod
{
    [BepInPlugin("com.micrologist.microengineer", "MicroEngineer", "1.5.1")]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class MicroEngineerMod : BaseSpaceWarpPlugin
	{
        public static MicroEngineerMod Instance { get; set; }
        public string GUID;
        
        private ConfigEntry<bool> _enableKeybinding;
        private ConfigEntry<KeyCode> _keybind1;
        private ConfigEntry<KeyCode> _keybind2;

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

            _enableKeybinding = Config.Bind("Micro Engineer", "Enable keybinding", true, "Enables or disables keyboard shortcuts to show or hide windows in Flight and OAB scenes.");
            _keybind1 = Config.Bind("Micro Engineer", "Keycode 1", KeyCode.LeftControl, "First keycode.");
            _keybind2 = Config.Bind("Micro Engineer", "Keycode 2", KeyCode.E, "Second keycode.");
        }

        public void Update()
        {
             Manager.Instance.Update();

            // Keyboard shortcut for opening the UI
            if ((_enableKeybinding?.Value ?? false) &&
                (_keybind1.Value != KeyCode.None ? Input.GetKey(_keybind1.Value) : true) &&
                (_keybind2.Value != KeyCode.None ? Input.GetKeyDown(_keybind2.Value) : true))
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