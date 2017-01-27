using System;
using WorldGen.Generator.Interfaces;
using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator.Generators
{
    /// <summary>
    /// A generator that produces mountains for a world using a ridged multi fractal.
    /// </summary>
    /// <typeparam name="TWorld">The type of the world that the generator operates on.</typeparam>
    /// <typeparam name="TCell">The type of cells in the world that the generator operates on.</typeparam>
    public class RidgedFractalMountainGenerator<TWorld, TCell> : IGenerator<TWorld, TCell>
        where TWorld : IHeightWorld<TCell>
        where TCell : IHeightCell
    {
        /// <summary>
        /// The name of the generator, used for logging.
        /// </summary>
        public string Name
        {
            get { return "Ridged Fractal Mountain Generator"; }
        }

        /// <summary>
        /// Run the generator against a world.
        /// </summary>
        /// <param name="world">The world to generate against.</param>
        public void RunGeneration(TWorld world)
        {
            var noiseMap = new float[world.Width, world.Height];

            var mountainNoise = new SimplexNoiseGenerator(world.Seed + "mountain_ridge".GetHashCode());

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    noiseMap[x, y] = Math.Abs(mountainNoise.CoherentNoise(x, y, 0, 1, 250, 0.5f, 2, 0.4f));
                }
            }

            noiseMap = MapUtil.Normalize(noiseMap);

            var newHeightMap = new float[world.Width, world.Height];

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    var noiseValue = 1 - (noiseMap[x, y] - 0.2f) * 1.25f;

                    var percHeight = world.GetCell(x, y).Height / (float)world.MaxElevation;

                    newHeightMap[x, y] = 0.7f * percHeight + 0.3f * noiseValue * (0.5f + percHeight / 2);
                }
            }

            newHeightMap = MapUtil.Normalize(newHeightMap);

            for(int x = 0; x < world.Width; x++)
            {
                for(int y = 0; y < world.Height; y++)
                {
                    world.GetCell(x, y).Height = (int)(world.MaxElevation * newHeightMap[x, y]);
                }
            }
        }
    }
}
