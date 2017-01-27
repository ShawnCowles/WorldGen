namespace WorldGen.Generator.Interfaces.Data
{
    /// <summary>
    /// An interface for biomes in the world, representing the climate, flora, and fauna that can
    /// be found in a cell.
    /// </summary>
    public interface IBiome
    {
        /// <summary>
        /// The name of a biome.
        /// </summary>
        string Name { get; }
    }
}
