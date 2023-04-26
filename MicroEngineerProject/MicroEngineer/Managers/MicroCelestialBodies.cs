using KSP.Game;
using KSP.Sim.impl;

namespace MicroMod
{
    internal class MicroCelestialBodies
    {
        public List<CelestialBody> Bodies = new();

        /// <summary>
        /// Refreshes the list of all CelestialBodies. Does nothing if list is already populated.
        /// </summary>
        /// <returns>True = refresh completed successfully or list is already populated</returns>
        internal bool GetBodies()
        {
            if (this.Bodies.Count > 0)
                return true;

            List<CelestialBodyComponent> bodies = GameManager.Instance?.Game?.UniverseModel?.GetAllCelestialBodies();

            if (bodies == null || bodies.Count == 0)
                return false;

            foreach (var body in bodies)
            {
                this.Bodies.Add(new CelestialBody
                {
                    Name = body.Name,
                    DisplayName = body.Name,
                    GravityASL = body.gravityASL,
                    HasAtmosphere = body.hasAtmosphere,
                    IsHomeWorld = body.isHomeWorld,
                    CelestialBodyComponent = body,
                });
            }

            // Reorder and format all celestial bodies so they form a tree-like structure
            TryReorderBodies();

            return true;
        }

        private void TryReorderBodies()
        {
            // Grab all stars in CelestialBodies
            List<CelestialBodyComponent> stars = Bodies
                .Select(b => b.CelestialBodyComponent)
                .Where(c => c.IsStar)
                .ToList();

            if (stars == null || stars.Count == 0)
                return;

            List<CelestialBody> reorderedBodies = new();

            // Iterate through all stars and grab all planets and moons that are orbiting them
            foreach (var star in stars)
            {
                reorderedBodies.AddRange(InstantiateCelestialBodies(star, 0));
            }

            this.Bodies = reorderedBodies;
        }

        /// <summary>
        /// Instantiates a list of planets and moons in the CelestialBodyComponent + bodies that are orbiting it
        /// </summary>
        /// <param name="cel"></param>
        /// <param name="level">Indicates how much indentation format needs to have</param>
        /// <returns></returns>
        private List<CelestialBody> InstantiateCelestialBodies (CelestialBodyComponent cel, int level)
        {
            List<CelestialBody> instantiatedBodies = new();            
            instantiatedBodies.Add(InstantiateCelestialBody(cel, level));

            foreach (CelestialBodyComponent body in cel.orbitingBodies)
            {
                instantiatedBodies.AddRange(InstantiateCelestialBodies(body, level + 1));
            }

            return instantiatedBodies;
        }


        /// <summary>
        /// Instantiates a single CelestialBody and formats it according to level of indentation
        /// </summary>
        /// <param name="cel"></param>
        /// <param name="level">Indicates how much indentation format needs to have</param>
        /// <returns></returns>
        private CelestialBody InstantiateCelestialBody(CelestialBodyComponent cel, int level)
        {
            CelestialBody body = new ()
            {
                Name = cel.Name,
                DisplayName = cel.Name,
                GravityASL = cel.gravityASL,
                HasAtmosphere = cel.hasAtmosphere,
                IsHomeWorld = cel.isHomeWorld,
                CelestialBodyComponent = cel,
            };

            if (cel.isHomeWorld)
                body.DisplayName = $"{body.DisplayName} *";

            if (level > 0)
            {
                body.DisplayName = $"└ {body.DisplayName}";

                for (int i = 0; i < level; i++)
                    body.DisplayName = $"  {body.DisplayName}";
            }

            return body;
        }

        /// <summary>
        /// Calculates what factor needs to be used for HomeWorld's TWR in order to compensate for gravity of the selected body
        /// </summary>
        /// <param name="bodyName">Name of the CelestialBody for which the TWR factor is calculated</param>
        /// <returns>TWR factor that needs to be multiplied with HomeWorld's TWR to get TWR at the selected body</returns>
        internal double GetTwrFactor(string bodyName)
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
        internal string Name;
        internal string DisplayName;
        internal double GravityASL;
        internal bool HasAtmosphere;
        internal bool IsHomeWorld;
        internal CelestialBodyComponent CelestialBodyComponent;
    }
}