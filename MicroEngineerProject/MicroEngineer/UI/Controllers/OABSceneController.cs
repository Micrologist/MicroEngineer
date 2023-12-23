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
        private bool _showGui = false;

        public UIDocument StageInfoWindow { get; set; }

        public bool ShowGui
        {
            get => _showGui;
            set
            {
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
            StageInfoWindow = Window.Create(Uxmls.Instance.InstantiateWindowOptions("StageInfoOAB"), Uxmls.Instance.StageInfoOAB);
            StageInfoOABController controller = StageInfoWindow.gameObject.AddComponent<StageInfoOABController>();

            // Manually trigger StageInfo refreshing (regular refreshing is triggered by VesselDeltaVCalculationMessage event)
            MessageManager.Instance.RefreshStagingDataOAB();
        }

        public void RebuildUI()
        {
            DestroyUI();
            if (ShowGui)
                InitializeUI();
        }

        public void DestroyUI()
        {
            if (StageInfoWindow != null && StageInfoWindow.gameObject != null)
                StageInfoWindow.gameObject.DestroyGameObject();
            GameObject.Destroy(StageInfoWindow);
        }
    }
}