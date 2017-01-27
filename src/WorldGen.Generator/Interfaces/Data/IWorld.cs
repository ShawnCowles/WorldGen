namespace WorldGen.Generator.Interfaces.Data
{
    /// <summary>
    /// Interface for any world that the WorldGenerator can operate on.
    /// </summary>
    /// <typeparam name="T">The type of cell the world contains.</typeparam>
    public interface IWorld<T> where T : ICell
    {
        /// <summary>
        /// The seed for the world, worlds with identical seeds will themselves be identical.
        /// </summary>
        int Seed { get; }

        /// <summary>
        /// The width of the world in cells.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The height of the world in cells.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Test to see if a cell is within the world.
        /// </summary>
        /// <param name="x">The horizontal position of the cell, in tiles.</param>
        /// <param name="y">The vertical position of the cell, in tiles.</param>
        /// <returns>True if the cell is inside the world, false otherwise.</returns>
        bool InRange(int x, int y);

        /// <summary>
        /// Get the cell at the specified position.
        /// </summary>
        /// <param name="x">The horizontal position of the cell, in tiles.</param>
        /// <param name="y">The vertical position of the cell, in tiles.</param>
        /// <returns>The cell at the coordinates (x,y).</returns>
        T GetCell(int x, int y);
    }
}
