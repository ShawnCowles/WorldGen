namespace WorldGen.Generator.Interfaces.Data
{
    /// <summary>
    /// A cell that contains height.
    /// </summary>
    public interface IHeightCell : ICell
    {
        /// <summary>
        /// The height of the cell.
        /// </summary>
        float Height { get; set; }
    }
}