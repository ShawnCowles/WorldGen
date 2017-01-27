using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator.Interfaces
{
    /// <summary>
    /// A generator that can run against a world.
    /// </summary>
    /// <typeparam name="TW">The type of the world that the generator operates on.</typeparam>
    /// <typeparam name="TC">The type of cells in the world that the generator operates on.</typeparam>
    public interface IGenerator<TW, TC> where TW : IWorld<TC> where TC : ICell
    {
        /// <summary>
        /// The name of the generator, used for logging.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Run the generator against a world.
        /// </summary>
        void RunGeneration(TW world);
    }
}
