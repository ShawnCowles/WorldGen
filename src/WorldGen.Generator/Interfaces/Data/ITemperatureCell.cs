namespace WorldGen.Generator.Interfaces.Data
{
    /// <summary>
    /// A cell that contains rainfall.
    /// </summary>
    public interface ITemperatureCell : ICell
    {
        /// <summary>
        /// The temperature in the cell, between 0 and 1.
        /// </summary>
        float Temperature { get; set; }
    }
}