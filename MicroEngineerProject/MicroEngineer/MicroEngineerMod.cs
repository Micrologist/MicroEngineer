using BepInEx;
using UnityEngine;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI.Appbar;
using KSP.UI.Binding;
using MicroEngineer.UI;
using KSP.Game;

namespace MicroMod
{
    [BepInPlugin("com.micrologist.microengineer", "MicroEngineer", "1.2.0")]
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
                    FlightSceneController.Instance.ShowGui = isOpen;
                    Manager.Instance.Windows.Find(w => w.GetType() == typeof(MainGuiWindow)).IsFlightActive = isOpen;
                });

            Appbar.RegisterOABAppButton(
                "Micro Engineer",
                "BTN-MicroEngineerOAB",
                AssetManager.GetAsset<Texture2D>($"{GUID}/images/icon.png"),
                isOpen =>
                {
                    Manager.Instance.Windows.FindAll(w => w is EntryWindow).Cast<EntryWindow>().ToList().Find(w => w.MainWindow == MainWindow.StageInfoOAB).IsEditorActive = isOpen;
                    GameObject.Find("BTN-MicroEngineerOAB")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(isOpen);
                });
        }

        public void Update()
        {
            Manager.Instance.Update();

            // Keyboard shortcut for opening UI
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.E))
            {
                if (Utility.GameState.GameState == GameState.FlightView || Utility.GameState.GameState == GameState.Map3DView)
                    FlightSceneController.Instance.ShowGui = !FlightSceneController.Instance.ShowGui;
                else if (Utility.GameState.GameState == GameState.VehicleAssemblyBuilder)
                    OABSceneController.Instance.ShowGui = !OABSceneController.Instance.ShowGui;
            }
        }
    }
}