using System;
using WorldGen.Generator.Interfaces;
using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator.Generators
{
    /// <summary>
    /// A generator that adjusts the world's height values to flatten lowlands and sharpen mountains.
    /// </summary>
    /// <typeparam name="TWorld">The type of the world that the generator operates on.</typeparam>
    /// <typeparam name="TCell">The type of cells in the world that the generator operates on.</typeparam>
    public class MountainHeightmapAdjuster<TWorld, TCell> : IGenerator<TWorld, TCell>
        where TWorld : IHeightWorld<TCell>
        where TCell : IHeightCell, IRainfallCell
    {
        /// <summary>
        /// The name of the generator, used for logging.
        /// </summary>
        public string Name
        {
            get { return "Mountain Heightmap Adjuster"; }
        }

        /// <summary>
        /// Run the generator against a world.
        /// </summary>
        /// <param name="world">The world to generate against.</param>
        public void RunGeneration(TWorld world)
        {
            for(int x = 0; x < world.Width; x++)
            {
                for(int y = 0; y < world.Height; y++)
                {
                    var cell = world.GetCell(x, y);
                    var rawHeight = cell.Height / world.MaxElevation;

                    world.GetCell(x, y).Height = world.MaxElevation * ApplyMountainAdjustment(rawHeight, world);
                }
            }
        }

        private float ApplyMountainAdjustment(float rawHeight, TWorld world)
        {
            var seaLevelPercent = world.SeaLevel / world.MaxElevation;

            if (rawHeight < seaLevelPercent)
            {
                return rawHeight;
            }

            return seaLevelPercent + (float)Math.Pow(rawHeight - seaLevelPercent, 3) * 2;
        }
    }
}
