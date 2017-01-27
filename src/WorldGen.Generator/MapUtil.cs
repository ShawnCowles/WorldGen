using System;
using System.Collections.Generic;
using System.Linq;
using WorldGen.Generator.Data;
using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator
{
    /// <summary>
    /// A class holding utility methods for dealing with maps.
    /// </summary>
    public static class MapUtil
    {
        private const float MOUNTAIN_LEVEL_PERC = 0.5f;

        /// <summary>
        /// Normalize a map, scaling all values to between 0 and 1 (inclusive).
        /// </summary>
        /// <param name="input">The input map.</param>
        /// <returns>The input map with all values scaled between 0 and 1, inclusive.</returns>
        public static float[,] Normalize(float[,] input)
        {
            var output = new float[input.GetLength(0), input.GetLength(1)];
            var max = float.MinValue;
            var min = float.MaxValue;

            for (int x = 0; x < input.GetLength(0); x++)
            {
                for (int y = 0; y < input.GetLength(1); y++)
                {
                    max = Math.Max(max, input[x, y]);
                    min = Math.Min(min, input[x, y]);
                }
            }

            if (min >= max)
            {
                throw new Exception("Cannot nomalize map where maximum == minimum.");
            }

            for (int x = 0; x < input.GetLength(0); x++)
            {
                for (int y = 0; y < input.GetLength(1); y++)
                {
                    output[x,y] = (input[x, y] - min) / (max - min);
                }
            }

            return output;
        }

        /// <summary>
        /// Linearly interpolate between two values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="t">The interpolation factor.</param>
        /// <returns>The interpolated value between <paramref name="a"/> and <paramref name="b"/> by
        /// the factor <paramref name="t"/>.</returns>
        public static double Interpolate(double a, double b, double t)
        {
            return (1 - t) * a + t * b;
        }

        /// <summary>
        /// Bilinearly interpolate between four values.
        /// </summary>
        /// <param name="ul">The upper left value.</param>
        /// <param name="ur">The upper right value.</param>
        /// <param name="ll">The lower left value.</param>
        /// <param name="lr">The lower right value.</param>
        /// <param name="x">The horizontal interpolation factor.</param>
        /// <param name="y">The vertical interpolation factor.</param>
        /// <returns>The interpolated value between all four points by the interpolation factors.</returns>
        public static double BilinerInterpolate(double ul, double ur, double ll, double lr, double x,
            double y)
        {
            var u = MapUtil.Interpolate(ul, ur, x);
            var l = MapUtil.Interpolate(ll, lr, x);

            return MapUtil.Interpolate(u, l, y);
        }

        /// <summary>
        /// Return a value from a map, or a default value if the coordinates are out of bounds.
        /// </summary>
        /// <param name="map">The map to retrieve the value from.</param>
        /// <param name="x">The x coordinate of the value to retrieve.</param>
        /// <param name="y">The y coordinate of the value to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the coordinates are out of range.</param>
        /// <returns></returns>
        public static float ValueAtOrDefault(float[,] map, int x, int y, float defaultValue)
        {
            if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
            {
                return defaultValue;
            }

            return map[x, y];
        }


        /// <summary>
        /// Wrap an index to a size.
        /// </summary>
        /// <param name="index">The index to wrap.</param>
        /// <param name="size">The size to wrap into.</param>
        /// <returns>The index wrapped to within the size boundary.</returns>
        public static int WrapIndex(int index, int size)
        {
            if (index < 0)
            {
                index += size;
            }

            return index % size;
        }

        /// <summary>
        /// Map the altitude of a cell to an altitude category.
        /// </summary>
        /// <param name="altitude">The altitude of the cell (in meters).</param>
        /// <param name="maxElevation">The maximum altitude of the world.</param>
        /// <param name="seaLevel">The sea level altitude of the world.</param>
        /// <returns>The altitude category of the cell.</returns>
        public static AltitudeCategory MapAltitudeToCategory(float altitude, float maxElevation, float seaLevel)
        {
            var altPerc = altitude / maxElevation;

            var seaLevelPercent = seaLevel / maxElevation;

            if(altitude < seaLevelPercent)
            {
                return AltitudeCategory.OCEAN;
            }

            var altPercMid = (MOUNTAIN_LEVEL_PERC - seaLevelPercent) / 2 + seaLevelPercent;

            if(altPerc < altPercMid)
            {
                return AltitudeCategory.LOW;
            }
            else if(altPerc < MOUNTAIN_LEVEL_PERC)
            {
                return AltitudeCategory.MEDIUM;
            }

            return AltitudeCategory.HIGH;
        }

        /// <summary>
        /// Map the temperature of a cell to a temperature category.
        /// </summary>
        /// <param name="temperature">The temperature of the cell.</param>
        /// <returns>The temperature category of the cell.</returns>
        public static TemperatureCategory MapTemperatureToCategory(float temperature)
        {
            if(temperature < 0.25)
            {
                return TemperatureCategory.COLD;
            }
            else if(temperature < 0.5)
            {
                return TemperatureCategory.COOL;
            }
            else if(temperature < 0.75)
            {
                return TemperatureCategory.WARM;
            }

            return TemperatureCategory.HOT;
        }

        /// <summary>
        /// Map the moisture of a cell to a moisture category.
        /// </summary>
        /// <param name="moisture">The rainfall in the cell.</param>
        /// <returns>The moisture category of the cell.</returns>
        public static MoistureCategory MapMoistureToCategory(float moisture)
        {
            if(moisture < 0.25)
            {
                return MoistureCategory.ARID;
            }
            else if(moisture < 0.5)
            {
                return MoistureCategory.DRY;
            }
            else if(moisture < 0.75)
            {
                return MoistureCategory.MOIST;
            }

            return MoistureCategory.WET;
        }
        
        /// <summary>
        /// Find the CellAddress closest to the center of a set of CellAddresses.
        /// </summary>
        /// <param name="cellAddresses">The set of CellAddresses.</param>
        /// <returns>The CellAddress closet to the average position of <paramref name="cellAddresses"/>.</returns>
        public static CellAddress FindCenter(IEnumerable<CellAddress> cellAddresses)
        {
            var x = 0;
            var y = 0;

            foreach(var addy in cellAddresses)
            {
                x += addy.X;
                y += addy.Y;
            }

            return new CellAddress(x / cellAddresses.Count(), y / cellAddresses.Count());
        }

        /// <summary>
        /// Determine the dominant biome in a set of cells.
        /// </summary>
        /// <param name="cells">The cells.</param>
        /// <returns>The dominant biome in <paramref name="cells"/>.</returns>
        /// <typeparam name="TBiome">The type of biome the cells contain.</typeparam>
        public static TBiome DominantBiomeIn<TBiome>(IEnumerable<IBiomeCell<TBiome>> cells) where TBiome : IBiome
        {
            var biomeCounts = new Dictionary<IBiome, int>();
            var highBiome = cells.First().Biome;
            biomeCounts.Add(highBiome, 1);

            foreach(var cell in cells)
            {
                if(!biomeCounts.ContainsKey(cell.Biome))
                {
                    biomeCounts.Add(cell.Biome, 0);
                }

                biomeCounts[cell.Biome] += 1;

                if(biomeCounts[cell.Biome] > biomeCounts[highBiome])
                {
                    highBiome = cell.Biome;
                }
            }

            return highBiome;
        }

        /// <summary>
        /// Determine the dominant temperature category in a set of cells.
        /// </summary>
        /// <param name="cells">The cells.</param>
        /// <returns>The dominant temperature category in <paramref name="cells"/>.</returns>
        public static TemperatureCategory DominantTemperature(IEnumerable<ITemperatureCell> cells)
        {
            var tempCounts = new Dictionary<TemperatureCategory, int>();
            var highTemp = MapTemperatureToCategory(cells.First().Temperature);

            tempCounts.Add(highTemp, 1);

            foreach(var cell in cells)
            {
                var temp = MapTemperatureToCategory(cell.Temperature);

                if(!tempCounts.ContainsKey(temp))
                {
                    tempCounts.Add(temp, 0);
                }

                tempCounts[temp] += 1;

                if(tempCounts[temp] > tempCounts[highTemp])
                {
                    highTemp = temp;
                }
            }

            return highTemp;
        }

        /// <summary>
        /// Determine if a cell address is adjacent to the ocean.
        /// </summary>
        /// <param name="addy">The cell address to check.</param>
        /// <param name="world">The world.</param>
        /// <returns>True if one of <paramref name="addy"/>'s orthagonal neighbors is under sea level.</returns>
        public static bool IsOceanAdjacent<TCell>(CellAddress addy, IHeightWorld<TCell> world) where TCell : IHeightCell
        {
            return OrthogonalNeighborsOf(addy).Any(ca => 
                world.GetCell(ca.X, ca.Y).Height < world.SeaLevel);
        }

        /// <summary>
        /// Retrieve the orthogonal (north, south, east, west) neighbors of a cell address.
        /// </summary>
        /// <param name="addy">The cell to find the neighbors of.</param>
        /// <returns>The orthogonal (north, south, east, west) neighbors of a cell address.</returns>
        public static IEnumerable<CellAddress> OrthogonalNeighborsOf(CellAddress addy)
        {
            var neighbors = new List<CellAddress>();

            neighbors.Add(new CellAddress(addy.X + 1, addy.Y));
            neighbors.Add(new CellAddress(addy.X - 1, addy.Y));
            neighbors.Add(new CellAddress(addy.X, addy.Y + 1));
            neighbors.Add(new CellAddress(addy.X, addy.Y - 1));

            return neighbors;
        }
    }
}
