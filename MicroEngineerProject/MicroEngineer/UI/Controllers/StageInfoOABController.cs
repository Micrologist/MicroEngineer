using BepInEx.Logging;
using MicroMod;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace MicroEngineer.UI
{
    public class StageInfoOABController : MonoBehaviour
    {
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.StageInfoOABController");
        public StageInfoOABController()
        { }

        public void OnEnable()
        { }

        public void Update()
        {
            return;
        }

        private void OnCloseButton(ClickEvent evt)
        {
            //MainGuiWindow.IsFlightActive = false;
            //Utility.SaveLayout(Manager.Instance.Windows);
            OABSceneController.Instance.ShowGui = false;
        }
    }
}
