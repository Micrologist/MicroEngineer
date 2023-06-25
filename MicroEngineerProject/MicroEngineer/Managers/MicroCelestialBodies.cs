using BepInEx.Logging;
using KSP.Game;
using KSP.Sim.impl;

namespace MicroMod
{
    public class MicroCelestialBodies
    {
        private static MicroCelestialBodies _instance;
        private static readonly ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("MicroEngineer.MicroCelestialBodies");

        public List<CelestialBody> Bodies = new();

        public static MicroCelestialBodies Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MicroCelestialBodies();

                return _instance;
            }
        }

        public MicroCelestialBodies()
        {
            _logger.LogDebug("Instantiating singleton.");
            InitializeBodies();
        }

        /// <summary>
        /// Refreshes the list of all CelestialBodies. Does nothing if list is already populated.
        /// </summary>
        /// <returns>True = refresh completed successfully or list is already populated</returns>
        public void InitializeBodies()
        {
            if (this.Bodies.Count > 0)
            {
                _logger.LogInfo("Skipping CelestialBodies build as they're already populated.");
                return;
            }

            List<CelestialBodyComponent> bodies = GameManager.Instance?.Game?.UniverseModel?.GetAllCelestialBodies();

            if (bodies == null || bodies.Count == 0)
            {
                _logger.LogError("Error retrieving CelestialBodies from GameManager.Instance.Game.UniverseModel.");
                return;
            }

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

            _logger.LogDebug("Starting TWR recalculation.");

            // Get TWR for each body
            foreach (var body in Bodies)
            {
                _logger.LogDebug($"Grabbed body: {body.Name}.");
                body.TwrFactor = GetTwrFactor(body.Name);
                _logger.LogDebug($"Recalculated TwrFactor: {body.TwrFactor}.");
            }

            _logger.LogInfo("CelestialBodies successfully built.");
            return;
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
        public double GetTwrFactor(string bodyName)
        {
            if (Bodies.Count == 0) return 0;
            CelestialBody homeWorld = Bodies.Find(b => b.IsHomeWorld);
            CelestialBody targetBody = Bodies.Find(t => t.Name.ToLowerInvariant() == bodyName.ToLowerInvariant()) ?? null;
            if (targetBody == null) return 0;

            _logger.LogDebug($"HomeWorld: {homeWorld.Name}, GravityAsl: {homeWorld.GravityASL}. Target: {targetBody.Name}, GravityAsl: {targetBody.GravityASL}.");

            return homeWorld.GravityASL / targetBody.GravityASL;
        }

        public CelestialBody GetHomeBody()
        {
            if (Bodies == null || Bodies.Count == 0)
                return null;

            return Bodies.Find(b => b.IsHomeWorld);
        }

        public CelestialBody GetBodyByName(string requestedBodyName)
        {
            return Bodies.Find(b => b.DisplayName.ToLowerInvariant() == requestedBodyName.ToLowerInvariant());
        }
    }

    public class CelestialBody
    {
        public string Name;
        public string DisplayName;
        public double GravityASL;
        public bool HasAtmosphere;
        public bool IsHomeWorld;
        public double TwrFactor;
        public CelestialBodyComponent CelestialBodyComponent;
    }
}