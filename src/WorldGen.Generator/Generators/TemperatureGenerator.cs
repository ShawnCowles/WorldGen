using WorldGen.Generator.Data;
using WorldGen.Generator.Interfaces;
using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator.Generators
{
    /// <summary>
    /// A generator that produces a temperature map for a world, based on latitude. Assumes the 
    /// world spans an entire world, with a hot equator and cold poles at the top and bottom.
    /// </summary>
    /// <typeparam name="TWorld">The type of the world that the generator operates on.</typeparam>
    /// <typeparam name="TCell">The type of cells in the world that the generator operates on.</typeparam>
    public class TemperatureGenerator<TWorld, TCell> : IGenerator<TWorld, TCell>
        where TWorld : IHeightWorld<TCell>
        where TCell : ITemperatureCell, IHeightCell
    {
        /// <summary>
        /// The name of the generator, used for logging.
        /// </summary>
        public string Name
        {
            get { return "Temperature Generator"; }
        }

        /// <summary>
        /// Run the generator against a world.
        /// </summary>
        /// <param name="world">The world to generate against.</param>
        public void RunGeneration(TWorld world)
        {
            var oceanBiaser = new OceanBiaser<TWorld, TCell>();
            oceanBiaser.CreateBiasPoints(world, 25);

            var noise = new SimplexNoiseGenerator(world.Seed + "temperature".GetHashCode());

            var noiseMap = new float[world.Width, world.Height];

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    noiseMap[x, y] = noise.CoherentNoise(x, y, 0, 3, 100, 0.5f, 2, 0.4f);
                }
            }

            noiseMap = MapUtil.Normalize(noiseMap);

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    var baseT = y / (float)world.Height * 2;

                    if (y > world.Height / 2)
                    {
                        baseT = (world.Height - y) / (float)world.Height * 2;
                    }

                    var oceanBias = 1f - 0.3f * oceanBiaser.OceanBiasAt(new CellAddress(x, y));

                    world.GetCell(x, y).Temperature = (0.8f * baseT + 0.2f * noiseMap[x, y]) * oceanBias;
                }
            }
        }
    }
}
