using BepInEx.Logging;
using KSP.UI.Binding;
using MicroMod;
using UitkForKsp2.API;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class OABSceneController
    {
        private static OABSceneController _instance;
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.OABSceneController");
        private bool _showGui = false;

        public UIDocument StageInfoWindow { get; set; }

        public bool ShowGui
        {
            get => _showGui;
            set
            {
                _logger.LogDebug($"Inside ShowGui SET. Old value: {_showGui}. New value: {value}");
                _showGui = value;

                GameObject.Find("BTN-MicroEngineerOAB")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(value);

                RebuildUI();
            }
        }

        public static OABSceneController Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new OABSceneController();

                return _instance;
            }
        }

        public void InitializeUI()
        {
            _logger.LogDebug("InitializeUI triggered.");
            
            StageInfoWindow = Window.CreateFromUxml(Uxmls.Instance.StageInfoOAB, "StageInfoOAB", null, true);
            StageInfoOABController controller = StageInfoWindow.gameObject.AddComponent<StageInfoOABController>();
            StageInfoWindow.rootVisualElement[0].RegisterCallback<PointerMoveEvent>(evt => Utility.ClampToScreenUitk(StageInfoWindow.rootVisualElement[0]));            
        }

        public void RebuildUI()
        {
            _logger.LogDebug("RebuildUI triggered.");
            DestroyUI();
            if (ShowGui)
                InitializeUI();
        }

        public void DestroyUI()
        {
            _logger.LogDebug("DestroyUI triggered.");
            if (StageInfoWindow != null && StageInfoWindow.gameObject != null)
                StageInfoWindow.gameObject.DestroyGameObject();
            GameObject.Destroy(StageInfoWindow);
        }
    }
}
