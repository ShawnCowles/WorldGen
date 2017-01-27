using System;
using System.Collections.Generic;
using System.Diagnostics;
using WorldGen.Generator.Interfaces;
using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator
{
    /// <summary>
    /// A world generator. Generates worlds based on a series of generators registered with it.
    /// </summary>
    /// <typeparam name="TWorld">The type of the world that the generators operate on.</typeparam>
    /// <typeparam name="TCell">The type of cells in the world that the generators operate on.</typeparam>
    public class WorldGenerator<TWorld, TCell> where TWorld : IWorld<TCell> where TCell : ICell
    {
        private List<IGenerator<TWorld, TCell>> _generators;
        
        /// <summary>
        /// Create a new, empty WorldGenerator.
        /// </summary>
        public WorldGenerator()
        {
            _generators = new List<IGenerator<TWorld, TCell>>();
        }

        /// <summary>
        /// Add a generator to this WorldGenerator.
        /// </summary>
        /// <param name="generator">The generator to add.</param>
        public void AddGenerator(IGenerator<TWorld, TCell> generator)
        {
            _generators.Add(generator);
        }

        /// <summary>
        /// Run all contained generators to build a world.
        /// </summary>
        /// <returns>The generated world.</returns>
        /// <param name="world">The empty world to run generation on.</param>
        /// <param name="logFunction">The function to call to log progress. To disable logging pass null.</param>
        public void RunGeneration(
            TWorld world,
            Action<string> logFunction)
        {
            var stopwatch = new Stopwatch();

            foreach (var generator in _generators)
            {
                stopwatch.Start();
                logFunction?.Invoke($"Running {generator.Name}...");
                generator.RunGeneration(world);
                stopwatch.Stop();
                logFunction?.Invoke(string.Format("Done ({0} elapsed)", stopwatch.Elapsed));
                stopwatch.Reset();
            }
        }
    }
}
