using WorldGen.Generator.Data;
using System;
using System.Collections.Generic;
using WorldGen.Generator.Interfaces;
using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator.Generators
{
    /// <summary>
    /// A generator that simulates rainfall based on dominant winds and "point oceans"
    /// </summary>
    /// <typeparam name="TWorld">The type of the world that the generator operates on.</typeparam>
    /// <typeparam name="TCell">The type of cells in the world that the generator operates on.</typeparam>
    public class PointBasedRainfallSimulator<TWorld, TCell> : IGenerator<TWorld, TCell>
        where TWorld : IHeightWorld<TCell>
        where TCell : IHeightCell, IRainfallCell
    {
        private int _pointOceanSize;
        
        /// <summary>
        /// Construct a new PointBasedRainfallSimulator.
        /// </summary>
        /// <param name="pointOceanSize">The edge length of the point oceans, in cells.</param>
        public PointBasedRainfallSimulator(int pointOceanSize = 50)
        {
            _pointOceanSize = pointOceanSize;
        }
        
        /// <summary>
        /// The name of the generator, used for logging.
        /// </summary>
        public string Name
        {
            get { return "Point Based Rainfall Simulator"; }
        }

        /// <summary>
        /// Run the generator against a world.
        /// </summary>
        /// <param name="world">The world to generate against.</param>
        public void RunGeneration(TWorld world)
        {
            var oceans = IdentifyPointOceans(world);

            var rainfallMap = new float[world.Width, world.Height];

            for(int x = 0; x < world.Width; x++)
            {
                for(int y = 0; y < world.Height; y++)
                {
                    var ca = new CellAddress(x, y);

                    if(world.GetCell(ca.X,ca.Y).Height >= world.SeaLevel)
                    {
                        rainfallMap[x, y] = CalculateRainfallAt(ca, world, oceans);
                    }
                }
            }

            rainfallMap = MapUtil.Normalize(rainfallMap);

            for(int x = 0; x < world.Width; x++)
            {
                for(int y = 0; y < world.Height; y++)
                {
                    var ca = new CellAddress(x, y);

                    world.GetCell(ca.X, ca.Y).Rainfall = rainfallMap[x, y];
                }
            }
        }

        private List<CellAddress> IdentifyPointOceans(TWorld world)
        {
            var oceans = new List<CellAddress>();

            var pointsWide = (int)(world.Width / (float)_pointOceanSize);
            var pointsHigh = (int)(world.Height / (float)_pointOceanSize);

            for(int i = 0; i < pointsWide; i++)
            {
                for(int j = 0; j < pointsHigh; j++)
                {
                    var landCount = 0;
                    var oceanCount = 0;

                    for(int u = 0; u < _pointOceanSize; u++)
                    {
                        for(int v = 0; v < _pointOceanSize; v++)
                        {
                            var ca = new CellAddress(
                                i * _pointOceanSize + u,
                                j * _pointOceanSize + v);

                            if(world.InRange(ca.X,ca.Y))
                            {
                                if(world.GetCell(ca.X,ca.Y).Height < world.SeaLevel)
                                {
                                    oceanCount += 1;
                                }
                                else
                                {
                                    landCount += 1;
                                }
                            }
                        }
                    }

                    if(oceanCount > landCount)
                    {
                        oceans.Add(new CellAddress(
                            (int)((i + 0.5) * _pointOceanSize),
                            (int)((j + 0.5) * _pointOceanSize)));
                    }
                }
            }

            return oceans;
        }

        private float CalculateRainfallAt(CellAddress address, TWorld world, List<CellAddress> oceans)
        {
            var horizontalWind = -1 * Math.Sin(address.Y * Math.PI / (world.Height / 6));
            var verticalWind = address.Y > world.Width / 2 ? -0.1 : 0.1;

            var windDir = Math.Atan2(verticalWind, horizontalWind);
            //Result is normalized version of above
            var windDirX = Math.Cos(windDir);
            var windDirY = Math.Sin(windDir);

            var totalRainfall = 0.0;

            foreach(var ocean in oceans)
            {
                var dY = ocean.Y - address.Y;
                var dX = ocean.X - address.X;

                var oceanDir = Math.Atan2(dY, dX);
                var oceanDirX = Math.Cos(oceanDir);
                var oceanDirY = Math.Sin(oceanDir);

                var rainFactor = oceanDirX * windDirX + oceanDirY * windDirY;

                rainFactor = rainFactor / 2 + 0.5; //value between 0 and 1
                rainFactor = rainFactor / Math.Max(_pointOceanSize / 2, Math.Sqrt(dX * dX + dY * dY)); //divide by distance

                totalRainfall += rainFactor;
            }

            return (float)totalRainfall;
        }
    }
}
