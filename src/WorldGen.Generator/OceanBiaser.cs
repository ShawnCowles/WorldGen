using System.Collections.Generic;
using WorldGen.Generator.Data;
using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator
{
    /// <summary>
    /// A tool to generate a bias map based on proximity to the ocean.
    /// </summary>
    /// <typeparam name="TWorld">The type of the world that the OceanBiaser operates on.</typeparam>
    /// <typeparam name="TCell">The type of cells in the world that the OceanBiaser operates on.</typeparam>
    public class OceanBiaser<TWorld, TCell> 
        where TWorld : IHeightWorld<TCell>
        where TCell: IHeightCell
    {
        private Dictionary<CellAddress, BiasPoint> _biasPoints;
        private int _cellsPerPoint;
        
        /// <summary>
        /// Construct a new OceanBiaser.
        /// </summary>
        public OceanBiaser()
        {
            _biasPoints = new Dictionary<CellAddress, BiasPoint>();
        }

        /// <summary>
        /// Build the bias map for the world.
        /// </summary>
        /// <param name="world">The world to create bias map for.</param>
        /// <param name="cellsPerPoint">The edge length of the area that each bias point contains, in cells.</param>
        public void CreateBiasPoints(TWorld world, int cellsPerPoint)
        {
            _cellsPerPoint = cellsPerPoint;

            _biasPoints.Clear();

            var pointsWide = (int)(world.Width / (float)cellsPerPoint);
            var pointsHigh = (int)(world.Height / (float)cellsPerPoint);

            for (int i = 0; i < pointsWide; i++)
            {
                for (int j = 0; j < pointsHigh; j++)
                {
                    var landCount = 0;
                    var oceanCount = 0;

                    for (int u = 0; u < cellsPerPoint; u++)
                    {
                        for (int v = 0; v < cellsPerPoint; v++)
                        {
                            var ca = new CellAddress(
                                i * cellsPerPoint + u,
                                j * cellsPerPoint + v);

                            if (world.InRange(ca.X,ca.Y))
                            {
                                if (world.GetCell(ca.X,ca.Y).Height < world.SeaLevel)
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

                    var pointAddress = new CellAddress(i * cellsPerPoint, j * cellsPerPoint);
                    
                    _biasPoints.Add(pointAddress, new BiasPoint(pointAddress, oceanCount / (float)(landCount + oceanCount)));
                }
            }
        }

        private BiasPoint GetBiasPoint(int x, int y)
        {
            var ca = new CellAddress(x, y);

            if (_biasPoints.ContainsKey(ca))
            {
                return _biasPoints[ca];
            }

            return new BiasPoint(ca, 1);
        }

        /// <summary>
        /// Get the ocean bias for a cell.
        /// </summary>
        /// <param name="ca">The cell address</param>
        /// <returns></returns>
        public float OceanBiasAt(CellAddress ca)
        {
            var pX = (ca.X / _cellsPerPoint) * _cellsPerPoint;
            var pY = (ca.Y / _cellsPerPoint) * _cellsPerPoint;

            var ul = GetBiasPoint(pX                 , pY).PercentOcean;
            var ur = GetBiasPoint(pX + _cellsPerPoint, pY).PercentOcean;
            var ll = GetBiasPoint(pX                 , pY + _cellsPerPoint).PercentOcean;
            var lr = GetBiasPoint(pX + _cellsPerPoint, pY + _cellsPerPoint).PercentOcean;

            var xPerc = (ca.X - pX) / (float)_cellsPerPoint;
            var yPerc = (ca.Y - pY) / (float)_cellsPerPoint;

            return (float)MapUtil.BilinerInterpolate(ul, ur, ll, lr, xPerc, yPerc);
        }

        private class BiasPoint
        {
            public CellAddress Location { get; private set; }

            public float PercentOcean { get; private set; }

            public BiasPoint(CellAddress location, float percentOcean)
            {
                Location = location;
                PercentOcean = percentOcean;
            }
        }
    }
}
