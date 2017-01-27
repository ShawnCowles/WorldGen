using WorldGen.Generator.Interfaces.Data;

namespace WorldGen.Generator.Data
{
    /// <summary>
    /// A simple implementation of <see cref="IBiome"/>.
    /// </summary>
    public class SimpleBiome : IBiome
    {
        /// <summary>
        /// The name of the biome.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Construct a new biome.
        /// </summary>
        /// <param name="name">The name of the biome.</param>
        public SimpleBiome(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Test for equality with another object.
        /// </summary>
        /// <param name="obj">The object to test equality against.</param>
        /// <returns>True if the obj is a Biome with the same name as this one.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is SimpleBiome)
            {
                return Name ==(obj as SimpleBiome).Name;
            }
            return false;
        }

        /// <summary>
        /// Get the hashcode of this Biome.
        /// </summary>
        /// <returns>The hashcode of this Biome.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        /// Test for equality between two Biomes.
        /// </summary>
        /// <param name="a">The first Biome to test.</param>
        /// <param name="b">The second Biome to test.</param>
        /// <returns>True if both <paramref name="a"/> and <paramref name="b"/> have the same name.</returns>
        public static bool operator ==(SimpleBiome a, SimpleBiome b)
        {
            if (((object)a) == null || ((object)b) == null)
            {
                return ((object)a) == ((object)b);
            }
            return a.Name == b.Name;
        }

        /// <summary>
        /// Test for inequality between two Biomes.
        /// </summary>
        /// <param name="a">The first Biome to test.</param>
        /// <param name="b">The second Biome to test.</param>
        /// <returns>True if <paramref name="a"/> and <paramref name="b"/> have different names.</returns>
        public static bool operator !=(SimpleBiome a, SimpleBiome b)
        {
            return !(a == b);
        }
    }
}
