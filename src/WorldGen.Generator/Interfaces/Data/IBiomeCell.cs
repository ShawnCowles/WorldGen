namespace WorldGen.Generator.Interfaces.Data
{
    /// <summary>
    /// A cell that contains a biome.
    /// </summary>
    /// <typeparam name="T">The type of biome this cell contains.</typeparam>
    public interface IBiomeCell<T> : ICell where T : IBiome
    {
        /// <summary>
        /// The biome in the cell.
        /// </summary>
        T Biome { get; set; }
    }
}