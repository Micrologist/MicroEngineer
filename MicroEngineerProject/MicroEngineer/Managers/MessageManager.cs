using BepInEx.Logging;
using KSP.Game;
using KSP.Messages;
using KSP.UI.Binding;
using MicroEngineer.UI;
using UnityEngine;

namespace MicroMod
{
    public class MessageManager
    {
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.MessageManager");
        private static MessageManager _instance;

        public MessageManager()
        { }

        public static MessageManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MessageManager();

                return _instance;
            }
        }

        /// <summary>
        /// Subscribe to KSP2 messages
        /// </summary>
        public void SubscribeToMessages()
        {
            Utility.RefreshGameManager();

            // While in OAB we use the VesselDeltaVCalculationMessage event to refresh data as it's triggered a lot less frequently than Update()
            Utility.MessageCenter.Subscribe<VesselDeltaVCalculationMessage>(new Action<MessageCenterMessage>(obj => this.RefreshStagingDataOAB((VesselDeltaVCalculationMessage)obj)));

            // We are loading layout state when entering Flight or OAB game state
            Utility.MessageCenter.Subscribe<GameStateEnteredMessage>(new Action<MessageCenterMessage>(this.GameStateEntered));

            // We are saving layout state when exiting from Flight or OAB game state
            Utility.MessageCenter.Subscribe<GameStateLeftMessage>(new Action<MessageCenterMessage>(this.GameStateLeft));

            // Sets the selected node index to the newly created node
            Utility.MessageCenter.Subscribe<ManeuverCreatedMessage>(new Action<MessageCenterMessage>(this.OnManeuverCreatedMessage));

            // Resets node index
            Utility.MessageCenter.Subscribe<ManeuverRemovedMessage>(new Action<MessageCenterMessage>(this.OnManeuverRemovedMessage));

            // Torque update for StageInfoOAB
            Utility.MessageCenter.Subscribe<PartManipulationCompletedMessage>(new Action<MessageCenterMessage>(this.OnPartManipulationCompletedMessage));
        }

        private void OnManeuverCreatedMessage(MessageCenterMessage message)
        {
            var maneuverWindow = Manager.Instance.Windows.Find(w => w.GetType() == typeof(ManeuverWindow)) as ManeuverWindow;
            maneuverWindow.OnManeuverCreatedMessage(message);
        }

        private void OnManeuverRemovedMessage(MessageCenterMessage message)
        {
            var maneuverWindow = Manager.Instance.Windows.Find(w => w.GetType() == typeof(ManeuverWindow)) as ManeuverWindow;
            maneuverWindow.OnManeuverRemovedMessage(message);
        }

        private void OnPartManipulationCompletedMessage(MessageCenterMessage obj)
        {
            var torque = ((StageInfoOabWindow)Manager.Instance.Windows.Find(w => w is StageInfoOabWindow)).Entries.Find(e => e is Torque);
            torque.RefreshData();
        }

        private void GameStateEntered(MessageCenterMessage obj)
        {
            Utility.RefreshGameManager();
            if (Utility.GameState.GameState == GameState.FlightView || Utility.GameState.GameState == GameState.VehicleAssemblyBuilder || Utility.GameState.GameState == GameState.Map3DView)
            {
                Utility.LoadLayout(Manager.Instance.Windows);

                if (Utility.GameState.GameState == GameState.FlightView || Utility.GameState.GameState == GameState.Map3DView)
                {
                    _logger.LogDebug($"Inside GameStateEntered. GameState: {Utility.GameState.GameState}. MainGuiWindow.IsFlightActive: {Manager.Instance.Windows.OfType<MainGuiWindow>().FirstOrDefault().IsFlightActive}");
                    FlightSceneController.Instance.ShowGui = Manager.Instance.Windows.OfType<MainGuiWindow>().FirstOrDefault().IsFlightActive;
                    //UI.Instance.ShowGuiFlight = Manager.Instance.Windows.OfType<MainGuiWindow>().FirstOrDefault().IsFlightActive;
                    //GameObject.Find("BTN-MicroEngineerBtn")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(FlightSceneController.Instance.ShowGui); //UI.Instance.ShowGuiFlight);
                }    

                if (Utility.GameState.GameState == GameState.VehicleAssemblyBuilder)
                {
                    _logger.LogDebug($"Inside GameStateEntered. GameState: {Utility.GameState.GameState}.");
                    
                    // TEMP override till app.bar is fixed
                    //OABSceneController.Instance.ShowGui = Manager.Instance.Windows.OfType<StageInfoOabWindow>().FirstOrDefault().IsEditorActive;
                    OABSceneController.Instance.ShowGui = true;

                    //UI.Instance.ShowGuiOAB = Manager.Instance.Windows.FindAll(w => w is EntryWindow).Cast<EntryWindow>().ToList().Find(w => w.MainWindow == MainWindow.StageInfoOAB).IsEditorActive;
                    //GameObject.Find("BTN-MicroEngineerOAB")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(UI.Instance.ShowGuiOAB);
                    //UI.Instance.CelestialBodies.GetBodies();
                    //UI.Instance.CelestialBodySelectionStageIndex = -1;
                    //Styles.SetActiveTheme(Theme.Gray); // TODO implement other themes in OAB
                }
            }
        }

        private void GameStateLeft(MessageCenterMessage obj)
        {
            Utility.RefreshGameManager();
            if (Utility.GameState.GameState == GameState.FlightView || Utility.GameState.GameState == GameState.VehicleAssemblyBuilder || Utility.GameState.GameState == GameState.Map3DView)
            {
                Utility.SaveLayout(Manager.Instance.Windows);

                if (Utility.GameState.GameState == GameState.FlightView || Utility.GameState.GameState == GameState.Map3DView)
                    FlightSceneController.Instance.ShowGui = false;
                //UI.Instance.ShowGuiFlight = false;

                if (Utility.GameState.GameState == GameState.VehicleAssemblyBuilder)
                    OABSceneController.Instance.ShowGui = false;
                //UI.Instance.ShowGuiOAB = false;
            }
        }

        /// <summary>
        /// Refresh all staging data while in OAB
        /// </summary>
        public void RefreshStagingDataOAB(VesselDeltaVCalculationMessage msg = null)
        {
            // Check if message originated from ships in flight. If yes, return.
            if (msg != null && (msg.DeltaVComponent.Ship == null || !msg.DeltaVComponent.Ship.IsLaunchAssembly())) return;

            Utility.RefreshGameManager();
            if (Utility.GameState.GameState != GameState.VehicleAssemblyBuilder) return;

            Utility.RefreshStagesOAB();

            StageInfoOabWindow stageWindow = Manager.Instance.Windows.OfType<StageInfoOabWindow>().FirstOrDefault();

            if (Utility.VesselDeltaVComponentOAB?.StageInfo == null)
            {
                stageWindow.Entries.Find(e => e.Name == "Stage Info (OAB)").EntryValue = null;
                return;
            }

            stageWindow.RefreshData();
        }
    }
}