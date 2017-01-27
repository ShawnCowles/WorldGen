using System;
using WorldGen.Generator.Data;
using WorldGen.Generator.Interfaces;
using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator.Generators
{
    /// <summary>
    /// A generator that simulates rainfall on the world based on simplex noise, with a bias 
    /// applied approximating the rain patterns from the coriolis effect. Assumes the world spans
    /// an entire world, pole to pole.
    /// effect.
    /// </summary>
    /// <typeparam name="TWorld">The type of the world that the generator operates on.</typeparam>
    /// <typeparam name="TCell">The type of cells in the world that the generator operates on.</typeparam>
    public class BiasedNoiseRainGenerator<TWorld, TCell> : IGenerator<TWorld, TCell>
        where TWorld : IHeightWorld<TCell>
        where TCell : IHeightCell, IRainfallCell
    {
        /// <summary>
        /// The name of the generator, used for logging.
        /// </summary>
        public string Name
        {
            get { return "Biased Noise Rain Generator"; }
        }

        /// <summary>
        /// Run the generator against a world.
        /// </summary>
        /// <param name="world">The world to generate against.</param>
        public void RunGeneration(TWorld world)
        {
            var oceanBiaser = new OceanBiaser<TWorld, TCell>();
            oceanBiaser.CreateBiasPoints(world, 25);

            var noise = new SimplexNoiseGenerator(world.Seed + "rain".GetHashCode());

            var rainfallMap = new float[world.Width, world.Height];

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    var bias = (float)(-1 * Math.Cos(y / Math.PI / (world.Height / 6f)) / 2 + 0.5f);
                    bias = 0.75f * bias + 0.25f;

                    rainfallMap[x, y] = noise.CoherentNoise(x, y, 0, 3, 75, 0.5f, 2, 0.4f) * bias;
                }
            }

            rainfallMap = MapUtil.Normalize(rainfallMap);

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    if (!IsOcean(x, y, world))
                    {
                        var oceanBias = oceanBiaser.OceanBiasAt(new CellAddress(x, y));

                        world.GetCell(x, y).Rainfall = 0.8f * rainfallMap[x, y] + 0.2f * oceanBias;
                    }
                }
            }
        }

        private bool IsOcean(int x, int y, TWorld world)
        {
            return world.GetCell(x, y).Height < world.SeaLevel;
        }
    }
}
