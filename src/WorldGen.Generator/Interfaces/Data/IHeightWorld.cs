namespace WorldGen.Generator.Interfaces.Data
{
    /// <summary>
    /// An interface for a world that defines a maximum elevation and a sea level.
    /// </summary>
    public interface IHeightWorld<TCell> : IWorld<TCell> where TCell : ICell
    {
        /// <summary>
        /// The maximum elevation of the world.
        /// </summary>
        float MaxElevation { get; }

        /// <summary>
        /// The elevation of the surface of the sea.
        /// </summary>
        float SeaLevel { get; }
    }
}
