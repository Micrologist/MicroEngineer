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
    [BepInPlugin("com.micrologist.microengineer", "MicroEngineer", "1.7.1")]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class MicroEngineerMod : BaseSpaceWarpPlugin
	{
        public static MicroEngineerMod Instance { get; set; }
        public string GUID;
        
        private ConfigEntry<bool> _enableKeybinding;
        private ConfigEntry<KeyCode> _keybind1;
        private ConfigEntry<KeyCode> _keybind2;
        public ConfigEntry<int> MainUpdateLoopUpdateFrequency;
        public ConfigEntry<int> StageInfoUpdateFrequency;

        public Coroutine MainUpdateLoop;

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

            InitializeConfigs();

            MainUpdateLoop = StartCoroutine(DoFlightUpdate());
        }

        public void Update()
        {
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

        private System.Collections.IEnumerator DoFlightUpdate()
        {
            while (true)
            {
                try
                {
                    Manager.Instance.DoFlightUpdate();
                }
                catch (Exception ex)
                {
                    Logger.LogError("Unhandled exception in the DoFlightUpdate loop.\n" +
                                    $"Exception: {ex.Message}\n" +
                                    $"Stack Trace: {ex.StackTrace}");
                }
                 
                yield return new WaitForSeconds((float)MainUpdateLoopUpdateFrequency.Value / 1000);
            }
        }
        
        private void InitializeConfigs()
        {
            _enableKeybinding = Config.Bind(
                "Keybinding",
                "Enable keybinding",
                true,
                "Enables or disables keyboard shortcuts to show or hide windows in Flight and OAB scenes."
                );
            
            _keybind1 = Config.Bind(
                "Keybinding",
                "Keycode 1",
                KeyCode.LeftControl,
                "First keycode."
                );
            
            _keybind2 = Config.Bind(
                "Keybinding",
                "Keycode 2",
                KeyCode.E,
                "Second keycode.");
            
            MainUpdateLoopUpdateFrequency = Config.Bind(
                "Update Frequency (ms)",
                "Main Update",
                100,
                new ConfigDescription(
                    "Time in milliseconds between every entry refresh.\n\nIncrease the value for better performance at the cost of longer time between updates.", 
                    new AcceptableValueRange<int>(0, 1000))
                );
            
            StageInfoUpdateFrequency = Config.Bind(
                "Update Frequency (ms)",
                "Stage Info",
                500,
                new ConfigDescription(
                    "Time in milliseconds between every Stage Info refresh.\n\nIncrease the value for better performance at the cost of longer time between updates.",
                    new AcceptableValueRange<int>(MainUpdateLoopUpdateFrequency.Value, 1000))
                );
        }
    }
}