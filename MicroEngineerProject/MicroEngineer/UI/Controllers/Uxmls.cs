using BepInEx.Logging;
using MicroMod;
using SpaceWarp.API.Assets;
using UitkForKsp2.API;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class Uxmls
    {
        private static Uxmls _instance;
        private static readonly ManualLogSource _logger = Logger.CreateLogSource("MicroEngineer.Uxmls");

        public const string MAIN_GUI_HEADER_PATH = "/microengineer_flightui/microengineer/mainguiheader.uxml";
        public const string ENTRY_WINDOW_PATH = "/microengineer_flightui/microengineer/entrywindow.uxml";
        public const string STAGE_INFO_HEADER_PATH = "/microengineer_flightui/microengineer/stageinfoheader.uxml";
        public const string BASE_WINDOW_PATH = "/microengineer_flightui/microengineer/basewindow.uxml";
        public const string EDIT_WINDOWS_PATH = "/microengineer_flightui/microengineer/editwindows.uxml";
        public const string OAB_STAGE_INFO_PATH = "/microengineer_oabui/microengineer/stageinfooab.uxml";
        public const string MANEUVER_HEADER_PATH = "/microengineer_flightui/microengineer/maneuverheader.uxml";
        public const string MANEUVER_FOOTER_PATH = "/microengineer_flightui/microengineer/maneuverfooter.uxml";

        public VisualTreeAsset MainGuiHeader;
        public VisualTreeAsset EntryWindow;
        public VisualTreeAsset StageInfoHeader;
        public VisualTreeAsset BaseWindow;
        public VisualTreeAsset EditWindows;
        public VisualTreeAsset StageInfoOAB;
        public VisualTreeAsset ManeuverHeader;
        public VisualTreeAsset ManeuverFooter;

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
            MainGuiHeader = LoadAsset($"{MAIN_GUI_HEADER_PATH}");
            EntryWindow = LoadAsset($"{ENTRY_WINDOW_PATH}");
            StageInfoHeader = LoadAsset($"{STAGE_INFO_HEADER_PATH}");
            BaseWindow = LoadAsset($"{BASE_WINDOW_PATH}");
            EditWindows = LoadAsset($"{EDIT_WINDOWS_PATH}");
            StageInfoOAB = LoadAsset($"{OAB_STAGE_INFO_PATH}");
            ManeuverHeader = LoadAsset($"{MANEUVER_HEADER_PATH}");
            ManeuverFooter = LoadAsset($"{MANEUVER_FOOTER_PATH}");
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

        public WindowOptions InstantiateWindowOptions(string windowId, bool makeDraggable = true)
        {
            return new WindowOptions()
            {
                WindowId = windowId,
                IsHidingEnabled = true,
                MoveOptions = new MoveOptions
                {
                    IsMovingEnabled = makeDraggable,
                    CheckScreenBounds = true
                }
            };
        }
    }
}
