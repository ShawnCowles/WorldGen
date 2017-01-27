namespace WorldGen.Generator.Data
{
    /// <summary>
    /// An enum of the possible altitude values for biome determination.
    /// </summary>
    public enum AltitudeCategory
    {
        /// <summary>
        /// Ocean, below sea level.
        /// </summary>
        OCEAN,

        /// <summary>
        /// Low altitudes, right near sea level.
        /// </summary>
        LOW,

        /// <summary>
        /// Mid altitudes, foothills and highlands.
        /// </summary>
        MEDIUM,

        /// <summary>
        /// High altitudes, mountains and the like.
        /// </summary>
        HIGH
    }
}
