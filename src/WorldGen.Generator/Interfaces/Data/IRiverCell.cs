using WorldGen.Generator.Data;

namespace WorldGen.Generator.Interfaces.Data
{
    /// <summary>
    /// A cell that contains a river.
    /// </summary>
    public interface IRiverCell : ICell
    {
        /// <summary>
        /// The river segment in the cell, if any.
        /// </summary>
        RiverSegment RiverSegmentHere { get; }
    }
}
