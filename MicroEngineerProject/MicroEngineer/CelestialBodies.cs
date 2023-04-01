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

        public bool GetBodies()
        {
            if (this.Bodies.Count > 0)
                return true;

            Dictionary<string, CelestialBodyCore> bodies = GameManager.Instance?.Game?.CelestialBodies?.GetAllBodiesData();

            if (bodies == null || bodies.Count == 0)
                return false;

            foreach (var body in bodies)
            {
                Bodies.Add(new CelestialBody
                {
                    Name = body.Value.data.bodyName,
                    GravityASL = body.Value.data.gravityASL,
                    HasAtmosphere = body.Value.data.hasAtmosphere,
                    IsHomeWorld = body.Value.data.isHomeWorld
                });
            }

            return true;
        }

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
