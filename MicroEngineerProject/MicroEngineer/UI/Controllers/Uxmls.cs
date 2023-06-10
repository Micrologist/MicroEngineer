using BepInEx.Logging;
using MicroMod;
using SpaceWarp.API.Assets;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class Uxmls
    {
        private static Uxmls _instance;
        private static readonly ManualLogSource _logger = Logger.CreateLogSource("MicroEngineer.Uxmls");

        public const string MAIN_GUI_PATH = "/microengineer_flightui/microengineer/maingui.uxml";
        public const string ENTRY_WINDOW_PATH = "/microengineer_flightui/microengineer/entrywindow.uxml";
        public const string STAGE_INFO_HEADER_PATH = "/microengineer_flightui/microengineer/stageinfoheader.uxml";

        public VisualTreeAsset MainGui;
        public VisualTreeAsset EntryWindow;
        public VisualTreeAsset StageInfoHeader;

        public static Uxmls Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Uxmls();

                return _instance;
            }
        }
        public Uxmls()
        {
            Initialize();
        }

        public void Initialize()
        {
            MainGui = LoadAsset($"{MAIN_GUI_PATH}");
            EntryWindow = LoadAsset($"{ENTRY_WINDOW_PATH}");
            StageInfoHeader = LoadAsset($"{STAGE_INFO_HEADER_PATH}");
        }

        private VisualTreeAsset LoadAsset(string path)
        {
            try
            {
                return AssetManager.GetAsset<VisualTreeAsset>($"{MicroEngineerMod.Instance.GUID}{path}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load VisualTreeAsset at path \"{path}\"\n" + ex.Message);
                return null;
            }
        }
    }
}
