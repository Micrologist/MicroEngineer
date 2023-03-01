using SpaceWarp.API.Mods;

namespace MicroEngineer
{
    [MainMod]
     public class MicroEngineerMod : Mod
    {
        public override void OnInitialized()
        {
            Logger.Info("Mod is initialized");
        }
    }
}