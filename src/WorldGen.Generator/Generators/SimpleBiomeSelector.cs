using WorldGen.Generator.Interfaces;
using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator.Generators
{
    /// <summary>
    /// A mapper that uses a physical setting template to select a biome for each cell.
    /// </summary>
    /// <typeparam name="TWorld">The type of the world that the generator operates on.</typeparam>
    /// <typeparam name="TCell">The type of cells in the world that the generator operates on.</typeparam>
    /// <typeparam name="TBiome">The type of biome that the generator generates.</typeparam>
    public class SimpleBiomeSelector<TWorld, TCell, TBiome> : IGenerator<TWorld, TCell>
        where TWorld : IHeightWorld<TCell>
        where TCell : IHeightCell, IRainfallCell, ITemperatureCell, IBiomeCell<TBiome>
        where TBiome: IBiome
    {
        private IPhysicalSettingTemplate<TBiome> _physicalTemplate;

        /// <summary>
        /// Construct a new SimpleBiomeSelector.
        /// </summary>
        /// <param name="physicalTemplate">The physical setting template to use.</param>
        public SimpleBiomeSelector(IPhysicalSettingTemplate<TBiome> physicalTemplate)
        {
            _physicalTemplate = physicalTemplate;
        }

        /// <summary>
        /// The name of the generator, used for logging.
        /// </summary>
        public string Name
        {
            get { return "Simple Biome Selector"; }
        }

        /// <summary>
        /// Run the generator against a world.
        /// </summary>
        /// <param name="world">The world to generate against.</param>
        public void RunGeneration(TWorld world)
        {
            for(int x = 0; x < world.Width; x++)
            {
                for(int y = 0; y < world.Height; y++)
                {
                    var cell = world.GetCell(x, y);

                    var altitude = MapUtil.MapAltitudeToCategory(cell.Height, world.MaxElevation, world.SeaLevel);
                    var temperature = MapUtil.MapTemperatureToCategory(cell.Temperature);
                    var moisture = MapUtil.MapMoistureToCategory(cell.Rainfall);

                    cell.Biome = _physicalTemplate.GetBiomeFor(altitude, temperature, moisture);
                }
            }
        }
    }
}
