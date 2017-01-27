using System;
using WorldGen.Generator.Data;
using WorldGen.Generator.Interfaces;
using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator.Generators
{
    /// <summary>
    /// A generator that simulates rainfall on the world based on airflows from the coriolis
    /// </summary>
    /// <typeparam name="TWorld">The type of the world that the generator operates on.</typeparam>
    /// <typeparam name="TCell">The type of cells in the world that the generator operates on.</typeparam>
    public class AirflowRainSimulator<TWorld, TCell> : IGenerator<TWorld, TCell> 
        where TWorld: IHeightWorld<TCell>
        where TCell : IHeightCell, IRainfallCell
    {
        /// <summary>
        /// The name of the generator, used for logging.
        /// </summary>
        public string Name
        {
            get { return "Airflow Rain Simulator"; }
        }

        /// <summary>
        /// Run the generator against a world.
        /// </summary>
        /// <param name="world">The world to generate against.</param>
        public void RunGeneration(TWorld world)
        {
            var oceanBiaser = new OceanBiaser<TWorld, TCell>();
            oceanBiaser.CreateBiasPoints(world, 25);

            var rainfallMap = new float[world.Width, world.Height];
            var cloudMap = new float[world.Width, world.Height];

            var iterations = world.Width;

            for (int i = 0; i < iterations; i++)
            {
                PickAndDropRain(rainfallMap, cloudMap, world);
                cloudMap = MoveClouds(cloudMap, world);
            }

            //TODO ocean adjacent bias

            rainfallMap = MapUtil.Normalize(rainfallMap);

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    var oceanBias = oceanBiaser.OceanBiasAt(new CellAddress(x, y));
                    world.GetCell(x, y).Rainfall = 0.8f * rainfallMap[x, y] + 0.2f * oceanBias;
                }
            }
        }

        private void PickAndDropRain(float[,] rainfallMap, float[,] cloudMap, TWorld world)
        {
            var pickupRate = 2f;
            var dropRate = 0.005f;

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    if (IsOcean(x, y, world))
                    {
                        cloudMap[x, y] += pickupRate;
                    }
                    else
                    {
                        var elevation = world.GetCell(x, y).Height / (float)world.MaxElevation;

                        var rainfall = Math.Max(0 , cloudMap[x, y] * dropRate * elevation);
                        rainfallMap[x, y] += rainfall;
                        cloudMap[x, y] -= rainfall;
                    }
                }
            }
        }

        private float[,] MoveClouds(float[,] cloudMap, TWorld world)
        {
            var newClouds = new float[world.Width, world.Height];

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    var easterlyFlow = (float)(-1 * Math.Sin(y * Math.PI / (world.Height / 6)));
                    var westerlyFlow = (float)(-1 * easterlyFlow);

                    easterlyFlow = Math.Max(0, easterlyFlow);
                    //easterlyFlow = 1;
                    westerlyFlow = Math.Max(0, westerlyFlow);
                    var persist = (float)Math.Max(0, 1 - (westerlyFlow + easterlyFlow));

                    var westIndex = MapUtil.WrapIndex(x - 1, world.Width);
                    var eastIndex = MapUtil.WrapIndex(x + 1, world.Width);

                    newClouds[westIndex, y] += cloudMap[x, y] * westerlyFlow;
                    newClouds[eastIndex, y] += cloudMap[x, y] * easterlyFlow;
                    newClouds[x, y] += cloudMap[x, y] * persist;
                }
            }


            return newClouds;
        }

        private float ApplyOceanBias(TWorld world, float[,] rainfallMap, int centerX, int centerY)
        {
            if (IsOcean(centerX, centerY, world))
            {
                return rainfallMap[centerX, centerY];
            }

            var seaInfluenceRadius = Math.Max(world.Width, world.Height) / 100;

            var seaCount = 0;
            var landCount = 0;

            for (int i = -seaInfluenceRadius; i <= seaInfluenceRadius; i++)
            {
                for (int j = -seaInfluenceRadius; j <= seaInfluenceRadius; j++)
                {
                    var addy = new CellAddress(centerX + i, centerY + j);

                    if (Math.Sqrt(i * i + j * j) < seaInfluenceRadius && world.InRange(addy.X,addy.Y))
                    {
                        if (world.GetCell(addy.X,addy.Y).Height < world.SeaLevel)
                        {
                            seaCount += 1;
                        }
                        else
                        {
                            landCount += 1;
                        }
                    }
                }
            }

            var seaPerc = seaCount / (float)(seaCount + landCount);

            return 0.8f * rainfallMap[centerX, centerY] + 0.2f * seaPerc;
        }

        private bool IsOcean(int x, int y, TWorld world)
        {
            return world.GetCell(x, y).Height < world.SeaLevel;
        }
    }
}
