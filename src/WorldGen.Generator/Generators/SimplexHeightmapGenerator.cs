using System;
using WorldGen.Generator.Interfaces;
using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator.Generators
{
    /// <summary>
    /// A generator that produces a heightmap for a world using simplex noise, biased by a 
    /// provided <see cref="HeightmapBias"/>.
    /// </summary>
    /// <typeparam name="TWorld">The type of the world that the generator operates on.</typeparam>
    /// <typeparam name="TCell">The type of cells in the world that the generator operates on.</typeparam>
    public class SimplexHeightmapGenerator<TWorld, TCell> : IGenerator<TWorld, TCell>
        where TWorld : IHeightWorld<TCell>
        where TCell : IHeightCell
    {
        private const int CONTINENT_BIAS_SIZE = 10;

        private const int ISLAND_BIAS_SIZE = 20;

        private HeightmapBias _biasType;
        private int _octaves;
        private int _scale;
        private float _lacunarity;
        private float _persistence;
        
        /// <summary>
        /// Create a new SimplexHeightmapGenerator.
        /// </summary>
        /// <param name="bias">The bias to use when generating a heightmap.</param>
        /// <param name="octaves">The number of passes to be summed, higher values are slower but add more detail.</param>
        /// <param name="scale">The scale of the noise generated, which results in larger or smaller features.</param>
        /// <param name="lacunarity">The scale multiple factor between each pass. Higher values add detail.</param>
        /// <param name="persistence">How much each additional pass adds to the result. Should be less than 1.</param>
        public SimplexHeightmapGenerator(
            HeightmapBias bias,
            int octaves = 6, int scale = 120, float lacunarity = 2, float persistence = 0.4f)
        {
            _biasType = bias;
            _octaves = octaves;
            _scale = scale;
            _lacunarity = lacunarity;
            _persistence = persistence;
        }

        /// <summary>
        /// The name of the generator, used for logging.
        /// </summary>
        public string Name
        {
            get { return "Simplex Heightmap Generator"; }
        }

        /// <summary>
        /// Run the generator against a world.
        /// </summary>
        /// <param name="world">The world to generate against.</param>
        public void RunGeneration(TWorld world)
        {
            var biasMap = (float[,]) null;

            switch(_biasType)
            {
                case HeightmapBias.CONTINENTS:
                    biasMap = CreateRandomBias(world, CONTINENT_BIAS_SIZE);
                    break;
                case HeightmapBias.AZEROTH:
                    biasMap = CreateAzerothBias(world);
                    break;
                case HeightmapBias.ISLANDS:
                    biasMap = CreateRandomBias(world, ISLAND_BIAS_SIZE);
                    break;
                case HeightmapBias.NONE:
                    biasMap = CreateNoneBias(world);
                    break;
                case HeightmapBias.LANDMASS:
                    biasMap = CreateLandmassBias(world);
                    break;
                default:
                    throw new Exception("Unknown HeightmassBias: " + _biasType);
            }

            var noise = new SimplexNoiseGenerator(world.Seed + "height".GetHashCode());

            var noiseMap = new float[world.Width, world.Height];

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    noiseMap[x, y] = noise.CoherentNoise(x, y, 0, _octaves, _scale, 0.5f, _lacunarity, _persistence);
                }
            }

            noiseMap = MapUtil.Normalize(noiseMap);

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    world.GetCell(x, y).Height = (int) (world.MaxElevation * noiseMap[x, y] * biasMap[x, y]);
                }
            }
        }

        #region Bias Maps

        private float[,] CreateRandomBias(TWorld world, int biasGridSize)
        {
            var noise = new SimplexNoiseGenerator(world.Seed + "height_bias".GetHashCode());
            
            var biasGrid = new float[biasGridSize, biasGridSize];

            for (int x = 0; x < biasGridSize; x++)
            {
                for (int y = 0; y < biasGridSize; y++)
                {
                    biasGrid[x, y] = noise.CoherentNoise(x, y, 0, 1, 3, 0.5f, 2, 0.9f);
                }
            }

            biasGrid = MapUtil.Normalize(biasGrid);

            for (int x = 0; x < biasGridSize; x++)
            {
                for (int y = 0; y < biasGridSize; y++)
                {
                    if (x <= 0 || x >= biasGridSize - 1 || y <= 0 || y >= biasGridSize - 1)
                    {
                        //Force a bias of zero at the edges.
                        biasGrid[x, y] = 0;
                    }
                    else
                    {
                        //select high points

                        if (biasGrid[x, y] > 0.6)
                        {
                            biasGrid[x, y] = 1;
                        }
                        else
                        {
                            biasGrid[x, y] = 0;
                        }
                    }

                }
            }

            return ExpandBiasGridToMap(world, biasGrid, biasGridSize);
        }

        private float[,] CreateAzerothBias(TWorld world)
        {
            var biasGridSize = 8;
            var biasGrid = new float[biasGridSize, biasGridSize];

            biasGrid[1, 3] = 1;
            biasGrid[1, 4] = 1;
            biasGrid[1, 5] = 1;
            biasGrid[2, 4] = 1;
            biasGrid[2, 5] = 1;
            biasGrid[2, 6] = 1;
            
            biasGrid[3, 1] = 1;
            biasGrid[4, 1] = 1;
            biasGrid[4, 2] = 1;
            
            biasGrid[6, 3] = 1;
            biasGrid[6, 4] = 1;
            biasGrid[6, 5] = 1;
            biasGrid[5, 4] = 1;
            biasGrid[5, 5] = 1;
            biasGrid[5, 6] = 1;

            return ExpandBiasGridToMap(world, biasGrid, biasGridSize);
        }

        private float[,] CreateLandmassBias(TWorld world)
        {
            var biasGridSize = 4;
            var biasGrid = new float[biasGridSize, biasGridSize];

            biasGrid[1, 1] = 1;
            biasGrid[1, 2] = 1;
            biasGrid[2, 1] = 1;
            biasGrid[2, 2] = 1;

            return ExpandBiasGridToMap(world, biasGrid, biasGridSize);
        }

        private float[,] ExpandBiasGridToMap(TWorld world, float[,] biasGrid, int biasGridSize)
        {
            var map = new float[world.Width, world.Height];

            var biasToWorldX = ((biasGridSize - 1) / (double)world.Width);
            var biasToWorldY = ((biasGridSize - 1) / (double)world.Height);

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    var biasX = (int)(x * biasToWorldX);
                    var biasY = (int)(y * biasToWorldY);

                    var ul = MapUtil.ValueAtOrDefault(biasGrid, biasX, biasY, 0);
                    var ur = MapUtil.ValueAtOrDefault(biasGrid, biasX + 1, biasY, 0);
                    var ll = MapUtil.ValueAtOrDefault(biasGrid, biasX, biasY + 1, 0);
                    var lr = MapUtil.ValueAtOrDefault(biasGrid, biasX + 1, biasY + 1, 0);

                    map[x, y] = (float)MapUtil.BilinerInterpolate(ul, ur, ll, lr,
                        (x - (biasX / biasToWorldX)) * biasToWorldX,
                        (y - (biasY / biasToWorldY)) * biasToWorldY);
                }
            }

            return map;
        }

        private float[,] CreateNoneBias(TWorld world)
        {
            var map = new float[world.Width, world.Height];

            for (int i = 0; i < world.Width; i++)
            {
                for (int j = 0; j < world.Height; j++)
                {
                    map[i, j] = 1;
                }
            }

            return map;
        }

        #endregion
    }


    /// <summary>
    /// Enumeration of all possible heightmap biases.
    /// </summary>
    public enum HeightmapBias
    {
        /// <summary>
        /// No bias, heightmap is left unmodified.
        /// </summary>
        NONE,

        /// <summary>
        /// Heightmap is biased to produce a few, large landmasses.
        /// </summary>
        CONTINENTS,

        /// <summary>
        /// Heightmap is biased to produce many small landmasses.
        /// </summary>
        ISLANDS,

        /// <summary>
        /// Heightmap is biased to produce three landmasses, east, west, and north.
        /// </summary>
        AZEROTH,

        /// <summary>
        /// Heightmap is biased to produce a single landmass surrounded by water
        /// </summary>
        LANDMASS
    }
}
