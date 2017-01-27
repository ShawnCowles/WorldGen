using WorldGen.Generator.Interfaces;
using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator.Generators
{
    /// <summary>
    /// A generator that produces a map of rainfall for a world using simplex noise.
    /// </summary>
    /// <typeparam name="TWorld">The type of the world that the generator operates on.</typeparam>
    /// <typeparam name="TCell">The type of cells in the world that the generator operates on.</typeparam>
    public class SimplexRainGenerator<TWorld, TCell> : IGenerator<TWorld, TCell>
        where TWorld : IWorld<TCell>
        where TCell : IRainfallCell
    {
        private int _octaves;
        private int _scale;
        private float _lacunarity;
        private float _persistence;

        /// <summary>
        /// Create a new SimplexRainGenerator.
        /// </summary>
        /// <param name="octaves">The number of passes to be summed, higher values are slower but add more detail.</param>
        /// <param name="scale">The scale of the noise generated, which results in larger or smaller features.</param>
        /// <param name="lacunarity">The scale multiple factor between each pass. Higher values add detail.</param>
        /// <param name="persistence">How much each additional pass adds to the result. Should be less than 1.</param>
        public SimplexRainGenerator(int octaves = 3, int scale = 75, float lacunarity = 2, float persistence = 0.4f)
        {
            _octaves = octaves;
            _scale = scale;
            _lacunarity = lacunarity;
            _persistence = persistence;
        }

        /// <summary>
        /// The name of the generator, used for logging.
        /// </summary>
        public string Name
        {
            get { return "Simplex Rain Generator"; }
        }

        /// <summary>
        /// Run the generator against a world.
        /// </summary>
        /// <param name="world">The world to generate against.</param>
        public void RunGeneration(TWorld world)
        {
            var noise = new SimplexNoiseGenerator(world.Seed + "rain".GetHashCode());

            var noiseMap = new float[world.Width, world.Height];

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    noiseMap[x, y] = noise.CoherentNoise(x, y, 0, _octaves, _scale, 0.5f, _lacunarity, _persistence);
                }
            }

            noiseMap = MapUtil.Normalize(noiseMap);

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    world.GetCell(x, y).Rainfall = noiseMap[x, y];
                }
            }
        }
    }
}
