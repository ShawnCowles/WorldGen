namespace WorldGen.Generator.Interfaces.Data
{
    /// <summary>
    /// A cell that contains rainfall.
    /// </summary>
    public interface IRainfallCell : ICell
    {
        /// <summary>
        /// The rainfall in the cell, between 0 and 1.
        /// </summary>
        float Rainfall { get; set; }
    }
}