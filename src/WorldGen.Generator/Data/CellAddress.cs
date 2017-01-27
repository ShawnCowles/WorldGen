using System;

namespace WorldGen.Generator.Data
{
    /// <summary>
    /// The address of a cell.
    /// </summary>
    public class CellAddress
    {
        /// <summary>
        /// The x component of the address.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The y component of the address.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Construct a new CellAddress.
        /// </summary>
        /// <param name="x">The x component of the address.</param>
        /// <param name="y">The y component of the address.</param>
        public CellAddress(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Test for equality with another object.
        /// </summary>
        /// <param name="obj">The object to test equality against.</param>
        /// <returns>True if the obj is a CellAddress referring to the same location as this one.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is CellAddress)
            {
                return X == (obj as CellAddress).X
                    && Y == (obj as CellAddress).Y;
            }
            return false;
        }

        /// <summary>
        /// Get the hashcode of this cell address.
        /// </summary>
        /// <returns>The hashcode of this cell address.</returns>
        public override int GetHashCode()
        {
            return (X + "," + Y).GetHashCode();
        }

        /// <summary>
        /// Test for equality between two CellAddresses.
        /// </summary>
        /// <param name="a">The first CellAddress to test.</param>
        /// <param name="b">The second CellAddress to test.</param>
        /// <returns>True if both <paramref name="a"/> and <paramref name="b"/> refer to the same location.</returns>
        public static bool operator ==(CellAddress a, CellAddress b)
        {
            if (((object)a) == null || ((object)b) == null)
            {
                return ((object)a) == ((object)b);
            }
            return a.X == b.X && a.Y == b.Y;
        }

        /// <summary>
        /// Test for inequality between two CellAddresses.
        /// </summary>
        /// <param name="a">The first CellAddress to test.</param>
        /// <param name="b">The second CellAddress to test.</param>
        /// <returns>True if <paramref name="a"/> and <paramref name="b"/> refer different locations.</returns>
        public static bool operator !=(CellAddress a, CellAddress b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Calculate the distance between two CellAddresses.
        /// </summary>
        /// <param name="a">The first CellAddress.</param>
        /// <param name="b">The second CellAddress.</param>
        /// <returns>The distance from <paramref name="a"/> to <paramref name="b"/>.</returns>
        public static double Distance(CellAddress a, CellAddress b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
