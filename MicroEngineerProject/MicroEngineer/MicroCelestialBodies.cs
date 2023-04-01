using KSP.Game;
using KSP.Sim.Definitions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MicroMod
{
    internal class MicroCelestialBodies
    {
        public List<CelestialBody> Bodies = new();

        /// <summary>
        /// Refreshes the list of all CelestialBodies. Does nothing if list is already populated.
        /// </summary>
        /// <returns>True = refresh completed successfully or list is already populated</returns>
        public bool GetBodies()
        {
            if (this.Bodies.Count > 0)
                return true;

            Dictionary<string, CelestialBodyCore> bodies = GameManager.Instance?.Game?.CelestialBodies?.GetAllBodiesData();

            if (bodies == null || bodies.Count == 0)
                return false;

            foreach (var body in bodies)
            {
                this.Bodies.Add(new CelestialBody
                {
                    Name = body.Value.data.bodyName,
                    GravityASL = body.Value.data.gravityASL,
                    HasAtmosphere = body.Value.data.hasAtmosphere,
                    IsHomeWorld = body.Value.data.isHomeWorld
                });
            }

            return true;
        }

        /// <summary>
        /// Calculates what factor needs to be used for HomeWorld's TWR in order to compensate for gravity of the selected body
        /// </summary>
        /// <param name="bodyName">Name of the CelestialBody for which the TWR factor is calculated</param>
        /// <returns>Factor that needs to be multiplied with HomeWorld's TWR to get TWR at the selected body</returns>
        public double GetTwrFactor(string bodyName)
        {
            if (Bodies.Count == 0) return 0;
            CelestialBody homeWorld = Bodies.Find(b => b.IsHomeWorld);
            CelestialBody targetBody = Bodies.Find(t => t.Name.ToLowerInvariant() == bodyName.ToLowerInvariant()) ?? null;
            if (targetBody == null) return 0;
            
            return homeWorld.GravityASL / targetBody.GravityASL;
        }
    }

    internal class CelestialBody
    {
        public string Name;
        public double GravityASL;
        public bool HasAtmosphere;
        public bool IsHomeWorld;
    }
}
