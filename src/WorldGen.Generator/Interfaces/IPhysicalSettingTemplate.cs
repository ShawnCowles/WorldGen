using WorldGen.Generator.Data;

namespace WorldGen.Generator.Interfaces
{
    /// <summary>
    /// Interface that provides data to use when generating a setting. This interface defines the 
    /// physical parameters of the world.
    /// </summary>
    /// <typeparam name="TBiome">The type of the biomes in the world.</typeparam>
    public interface IPhysicalSettingTemplate<TBiome>
    {
        /// <summary>
        /// Get the biome matching the specified altitude, temperature, and moisture.
        /// </summary>
        /// <param name="altitude">The altitude for the biome.</param>
        /// <param name="temperature">The temperature for the biome.</param>
        /// <param name="moisture">The moisture level of the biome.</param>
        /// <returns>The biome matching the three parameters.</returns>
        TBiome GetBiomeFor(AltitudeCategory altitude, TemperatureCategory temperature, MoistureCategory moisture);
    }
}
